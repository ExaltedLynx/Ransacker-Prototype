using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TutorialConsts;

public class TutorialController : MonoBehaviour
{
    [SerializeField] Popup tutorialPopup;
    [SerializeField] private List<RectTransform> popupPos = new List<RectTransform>();
    private List<TutorialText> PathSelectTutorial { get; set; } = new();
    private List<TutorialText> InventoryTutorial { get; set; } = new();
    private List<TutorialText> BattleTutorial { get; set; } = new();
    private List<TutorialText> ChestTutorial { get; set; } = new();
    private List<TutorialText> LootInvTutorial { get; set; } = new();
    private List<TutorialText> ShopTutorial { get; set; } = new();
    private List<TutorialText> SwordTutorial { get; set; } = new();
    private List<TutorialText> SwordAndShieldTutorial { get; set; } = new();
    private List<TutorialText> DaggerTutorial { get; set; } = new();
    private List<TutorialText> AxeTutorial { get; set; } = new();
    private List<TutorialText> HammerTutorial { get; set; } = new();

    public bool tutorialActive => tutorialPopup.isActiveAndEnabled;
    public static TutorialController Instance { get; private set; } 

    public void Awake()
    {
        Instance = this;
    }

    public void InitPathSelectionTutorial()
    {
        //this sucks
        PathSelectTutorial.Add(new() { Text = PlanningPhaseText, Position = popupPos[0] });
        PathSelectTutorial.Add(new() { Text = StartRoomText, Position = popupPos[0] });
        PathSelectTutorial.Add(new() { Text = EnemyRoomText, Position = popupPos[0] });
        PathSelectTutorial.Add(new() { Text = EnemyPreviewText, Position = popupPos[1] });
        PathSelectTutorial.Add(new() { Text = TreasureRoomText, Position = popupPos[0] });
        PathSelectTutorial.Add(new() { Text = ShopRoomText, Position = popupPos[0] });
        PathSelectTutorial.Add(new() { Text = PathSelectionText, Position = popupPos[0] });
        PathSelectTutorial.Add(new() { Text = LimitedMovesText, Position = popupPos[0] });
        PathSelectTutorial.Add(new() { Text = BossRoomText, Position = popupPos[0] });
        PathSelectTutorial.Add(new() { Text = RansackButtonText, Position = popupPos[2] });
        tutorialPopup.tutorialMsgEnumerator = PathSelectTutorial.GetEnumerator();
        tutorialPopup.tutorialType = TutorialType.PathSelection;
    }

    public void InitGameplayTutorial()
    {
        //this sucks
        InventoryTutorial.Add(new() { Text = InventoryText, Position = popupPos[0] });
        InventoryTutorial.Add(new() { Text = HotbarText, Position = popupPos[1] });
        InventoryTutorial.Add(new() { Text = EquipmentText, Position = popupPos[2] });

        BattleTutorial.Add(new() { Text = CombatText, Position = popupPos[3] });
        BattleTutorial.Add(new() { Text = AttackTimerText, Position = popupPos[3] });
        BattleTutorial.Add(new() { Text = TargetingText, Position = popupPos[4] });
        BattleTutorial.Add(new() { Text = WeakpointText, Position = popupPos[4] });
        BattleTutorial.Add(new() { Text = WeaknessText, Position = popupPos[4] });
        BattleTutorial.Add(new() { Text = LootQueueText, Position = popupPos[4] });
        BattleTutorial.Add(new() { Text = LootQueueText2, Position = popupPos[4] });

        //ChestTutorial.Add(new() { Text = LootChestText, Position = popupPos[4]});
        ChestTutorial.Add(new() { Text = LootChestText, Position = popupPos[4] });

        //LootInvTutorial.Add(new() { Text = LootInventoryText, Position = popupPos[4] }); //cringe
        LootInvTutorial.Add(new() { Text = LootInventoryText, Position = popupPos[4] });

        ShopTutorial.Add(new() { Text = ShopGameplayText, Position = popupPos[4] });
        ShopTutorial.Add(new() { Text = BarterOrBuyText, Position = popupPos[4] });
        ShopTutorial.Add(new() { Text = ShopItemTooltipText, Position = popupPos[4] });
        ShopTutorial.Add(new() { Text = SellingItemsText, Position = popupPos[4] });
        ShopTutorial.Add(new() { Text = GoldHUDText, Position = popupPos[4] });

        SwordTutorial.Add(new() { Text = SwordText, Position = popupPos[3] });
        SwordAndShieldTutorial.Add(new() { Text = SwordAndShieldText, Position = popupPos[3] });
        SwordAndShieldTutorial.Add(new() { Text = SwordAndShieldText2, Position = popupPos[3] });
        SwordAndShieldTutorial.Add(new() { Text = SwordAndShieldText3, Position = popupPos[3] });
        DaggerTutorial.Add(new() { Text = DaggerText, Position = popupPos[3] });
        AxeTutorial.Add(new() { Text = AxeText, Position = popupPos[3] });
        HammerTutorial.Add(new() { Text = HammerText, Position = popupPos[3] });
    }

    public void EnableInventoryTutorial()
    {
        tutorialPopup.tutorialMsgEnumerator = InventoryTutorial.GetEnumerator();
        tutorialPopup.tutorialType = TutorialType.Inventory;
        EnableTutorialPopup();
    }

    public void EnableBattleTutorial()
    {
        tutorialPopup.tutorialMsgEnumerator = BattleTutorial.GetEnumerator();
        tutorialPopup.tutorialType = TutorialType.Battle;
        EnableTutorialPopup();   
    }

    public void EnableChestTutorial()
    {
        tutorialPopup.tutorialMsgEnumerator = ChestTutorial.GetEnumerator();
        tutorialPopup.tutorialType = TutorialType.Chest;
        EnableTutorialPopup();
    }

    public void EnableLootInvTutorial()
    {
        tutorialPopup.tutorialMsgEnumerator = LootInvTutorial.GetEnumerator();
        tutorialPopup.tutorialType = TutorialType.Loot;
        EnableTutorialPopup();
    }

    public void EnableShopTutorial()
    {
        tutorialPopup.tutorialMsgEnumerator = ShopTutorial.GetEnumerator();
        tutorialPopup.tutorialType = TutorialType.Shop;
        EnableTutorialPopup();
    }

    public void EnableSwordTutorial()
    {
        tutorialPopup.tutorialMsgEnumerator = SwordTutorial.GetEnumerator();
        tutorialPopup.tutorialType = TutorialType.Sword;
        EnableTutorialPopup();
    }

    public void EnableShieldTutorial()
    {
        tutorialPopup.tutorialMsgEnumerator = SwordAndShieldTutorial.GetEnumerator();
        tutorialPopup.tutorialType = TutorialType.Shield;
        EnableTutorialPopup();
    }

    public void EnableDaggerTutorial()
    {
        tutorialPopup.tutorialMsgEnumerator = DaggerTutorial.GetEnumerator();
        tutorialPopup.tutorialType = TutorialType.Dagger;
        EnableTutorialPopup();
    }

    public void EnableAxeTutorial()
    {
        tutorialPopup.tutorialMsgEnumerator = AxeTutorial.GetEnumerator();
        tutorialPopup.tutorialType = TutorialType.Axe;
        EnableTutorialPopup();
    }

    public void EnableHammerTutorial()
    {
        tutorialPopup.tutorialMsgEnumerator = HammerTutorial.GetEnumerator();
        tutorialPopup.tutorialType = TutorialType.Hammer;
        EnableTutorialPopup();
    }

    public void EnableTutorialPopup()
    {
        tutorialPopup.gameObject.SetActive(true);
        /*
        if(SceneController.activeScene.buildIndex == 2) //Gameplay
        {
            PlayerInventory.Instance.ToggleCollider(false);
            HotbarInventory.Instance.ToggleCollider(false);
        }
        */
    }
}
