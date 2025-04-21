using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Room;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public LinkedList<Room> playerSelectedPath = new LinkedList<Room>();
    public Room currentRoom { get; private set; }

    public int TotalMovement { get => totalMovement; set => totalMovement = value; }
    private int totalMovement;
    public int PlayerMoney { get => playerMoney; private set => playerMoney = value; }
    [SerializeField] private int playerMoney;
    public int currHeroHealthCache;

    public PauseScreenController pauseScreen { get; set; }
    public bool gameIsPaused { get; private set; }
    [SerializeField] private DifficultyMultiplier difficultyMultiplier;
    public SceneController sceneController { get; set; }
    public DungeonGenerator DungeonGenerator { get; set; }

    public bool tutorialEnabled => inventoryTutorialEnabled || battleTutorialEnabled || chestTutorialEnabled || lootTutorialEnabled || shopTutorialEnabled
                                   || SwordTutorialEnabled || ShieldTutorialEnabled || DaggerTutorialEnabled || AxeTutorialEnabled || HammerTutorialEnabled;
    public bool pathSelectionTutEnabled { get; set; }
    public bool inventoryTutorialEnabled { get; set; }
    public bool battleTutorialEnabled { get; set; }
    public bool chestTutorialEnabled { get; set; }
    public bool lootTutorialEnabled { get; set; }
    public bool shopTutorialEnabled { get; set; }
    public bool SwordTutorialEnabled { get; set; }
    public bool ShieldTutorialEnabled { get; set; }
    public bool DaggerTutorialEnabled { get; set; }
    public bool AxeTutorialEnabled { get; set; }
    public bool HammerTutorialEnabled { get; set; }

    private DungeonType currentDungeonType = DungeonType.dungeon0;

    private void Awake()
    {
        gameIsPaused = false;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(Instance != this)
        {
            Destroy(Instance.gameObject);
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        Items.LoadItems();
        Enemies.LoadEnemies();
    }

    void Start() 
    {

    }

    void Update()
    {
        if (SceneController.activeScene.buildIndex != 0)
        {
            switch (SceneController.activeScene.buildIndex)
            {
                case 1: //Path Selection
                    if (SceneController.activeScene.isLoaded && Input.GetKeyDown(KeyCode.Escape))
                    {
                        TogglePauseGame();
                        pauseScreen.ToggleVisibility();
                        EnemyPreviewTooltip.Instance.HideTooltip();
                    }
                    break;
                case 2: //Gameplay
                    if (SceneController.activeScene.isLoaded && Input.GetKeyDown(KeyCode.Escape) && LootInventory.Instance.isVisible == false)
                    {
                        TogglePauseGame();
                        pauseScreen.ToggleVisibility();
                        PlayerInventory.Instance.ToggleCollider();
                        HotbarInventory.Instance.ToggleCollider();
                        ItemTooltip.Instance.HideTooltip();
                        BarterTooltip.Instance.HideTooltip();
                    }
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.isLoaded)
        {
            switch(scene.buildIndex)
            {
                case 0: //Main Menu
                    if (gameIsPaused)
                        TogglePauseGame();

                    Hero.CachedSpeed = 1;
                    DungeonDataCache.Instance.ResetDungeonData();

                    playerSelectedPath.Clear();
                    difficultyMultiplier.ToggleDiffScaling(false);
                break;

                case 1: //Path Selection
                    difficultyMultiplier.ToggleDiffScaling(false);
                    if(tutorialEnabled)
                    {
                        DungeonGenerator.currentDungeon = DungeonGenerator.TutorialDungeon;
                    }
                    else
                    {
                        switch(currentDungeonType)
                        {
                            case DungeonType.dungeon0:
                                DungeonGenerator.currentDungeon = DungeonGenerator.Dungeon0;
                                break;
                            case DungeonType.dungeon1:
                                DungeonGenerator.currentDungeon = DungeonGenerator.Dungeon1;
                                break;
                        }
                    }

                    if (pathSelectionTutEnabled)
                    {
                        TutorialController.Instance.InitPathSelectionTutorial();
                        TutorialController.Instance.EnableTutorialPopup();
                    }
                    break;

                case 2: //Gameplay

                    if (tutorialEnabled)
                    {
                        TutorialController.Instance.InitGameplayTutorial();
                    }

                    if (inventoryTutorialEnabled)
                    {
                        TutorialController.Instance.EnableInventoryTutorial();
                        StartCoroutine(WaitForInvTutorialToFinish());
                    }
                    else
                        StartRoomMovement();

                    if (DungeonDataCache.Instance.currentFloorNum > 0)
                        Hero.Instance.GetHealth().setCurrentHealth(currHeroHealthCache);
                break;
            }
        }
    }


    public void StartRoomMovement()
    {
        StartCoroutine(MoveToNextRoom());
    }

    //TODO should switch to using UniTasks
    //main game loop
    public IEnumerator MoveToNextRoom()
    {
        playerSelectedPath.RemoveFirst();
        foreach (Room room in playerSelectedPath)
        {
            BackgroundAnimController.StartBackgroundAnim();
            yield return new WaitUntil(() => BackgroundAnimController.animInProgress == false);
            Debug.Log(room.roomPos + ", " + room.roomType);
            currentRoom = room;
            HandleEnterRoom();

            switch (currentRoom.roomType)
            {
                case RoomType.Enemy:
                case RoomType.Boss:
                    yield return new WaitWhile(() => CombatManager.Instance.inCombat == true);
                    difficultyMultiplier.ToggleDiffScaling(false);
                    if (currentRoom.roomType == RoomType.Boss && DungeonDataCache.Instance.currentFloorNum + 1 == DungeonGenerator.currentDungeon.MaxFloorCount)
                    {
                        WinScreenManager.Instance.EnableWinScreen();
                    }
                    else
                    {
                        //currentDungeonType += 1;
                        //sceneController.LoadScene("PathSelection");
                    }
                break;

                case RoomType.Treasure:
                    yield return new WaitWhile(() => LootChestController.Instance.chestWasOpened == false);
                break;

                case RoomType.Shop:
                    yield return new WaitWhile(() => MerchantRoomController.Instance.shopIsOpen == true);
                break;
            }
            if(LootInventory.Instance.isVisible == true)
                yield return new WaitUntil(() => LootInventory.Instance.isVisible == false);
        }
    }

    public void HandleEnterRoom()
    {
        switch(currentRoom.roomType)
        {
            case RoomType.Enemy:
            case RoomType.Boss:
                difficultyMultiplier.ToggleDiffScaling(true);
                if (currentRoom.roomType == RoomType.Boss)
                    EnemyController.Instance.SpawnBoss(currentRoom.roomBoss);
                else
                    EnemyController.Instance.SpawnEnemies(currentRoom.roomEnemies);

                if (battleTutorialEnabled)
                {
                    TutorialController.Instance.EnableBattleTutorial();
                    EnemyController.Instance.toggleAllTimers(false);
                    StartCoroutine(WaitForBattleTutorialToFinish());
                }
                CombatManager.Instance.HandleCombatStart();
            break;

            case RoomType.Treasure:
                LootChestController.Instance.EnableTreasureRoomChest();
                if (chestTutorialEnabled)
                    TutorialController.Instance.EnableChestTutorial();
                break;

            case RoomType.Shop:
                MerchantRoomController.Instance.EnableMerchantRoom(currentRoom.containedItems);

                if (shopTutorialEnabled)
                {
                    TutorialController.Instance.EnableShopTutorial();
                }
                break;

            case RoomType.Stairs:
                currHeroHealthCache = Hero.Instance.GetHealth().getCurrentHealth();
                Hero.Instance.CachePlayerSpeed();
                Hero.Instance.ReleaseAddressableAssets();
                //Debug.Log(DungeonDataCache.Instance.currentFloorNum);
                DungeonDataCache.Instance.currentFloorNum += 1;
                //Debug.Log(DungeonDataCache.Instance.currentFloorNum);
                sceneController.LoadScene("PathSelection");
            break;
        }
    }

    private IEnumerator WaitForInvTutorialToFinish()
    {
        yield return new WaitUntil(() => inventoryTutorialEnabled == false);
        StartRoomMovement();
    }

    private IEnumerator WaitForBattleTutorialToFinish()
    {
        yield return new WaitUntil(() => battleTutorialEnabled == false);
        EnemyController.Instance.toggleAllTimers(true);
    }

    public void AddPlayerMoney(int amount)
    {
        playerMoney += amount;
        PlayerGoldText.Instance.UpdateText();
    }

    public void SubtractPlayerMoney(int amount)
    {
        playerMoney -= amount;
        PlayerGoldText.Instance.UpdateText();
    }

    public void TogglePauseGame()
    {
        gameIsPaused = !gameIsPaused;
        if (gameIsPaused)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    //used in editor
    public void ToggleTutorial(bool toggle)
    {
        pathSelectionTutEnabled = toggle;
        inventoryTutorialEnabled = toggle;
        battleTutorialEnabled = toggle;
        chestTutorialEnabled = toggle;
        lootTutorialEnabled = toggle;
        shopTutorialEnabled = toggle;
        SwordTutorialEnabled = toggle;
        ShieldTutorialEnabled = toggle;
        DaggerTutorialEnabled = toggle;
        AxeTutorialEnabled = toggle;
        HammerTutorialEnabled = toggle;
    }
}
