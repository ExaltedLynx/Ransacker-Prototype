using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public Dungeon TutorialDungeon = new() { DungeonType = DungeonType.dungeonTut, MaxFloorCount = 2, StartSize = 5, FloorSizeExpands = false };
    public Dungeon Dungeon0 = new() { DungeonType = DungeonType.dungeon0, MaxFloorCount = 3, StartSize = 6, FloorSizeExpands = true };
    public Dungeon Dungeon1 = new() { DungeonType = DungeonType.dungeon1, MaxFloorCount = 5, StartSize = 6, FloorSizeExpands = true };

    public bool areFloorsGenerated { get; set; } = false;
    [SerializeField] public Dungeon currentDungeon;
    [SerializeField] public int currentFloorNum = 0;
    [SerializeField] public int maxFloors => currentDungeon.MaxFloorCount;

    private List<Floor> floors;
    public Floor currentFloor { get; private set; }

    public static DungeonGenerator Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        GameManager.Instance.DungeonGenerator = this;
    }

    void Start()
    {
        LoadDungeonData();

        if (!areFloorsGenerated)
            GenerateAllFloors();

        GameManager.Instance.playerSelectedPath.Clear();
        GameManager.Instance.playerSelectedPath.AddLast(floors[currentFloorNum].startRoom);
        SpawnNextFloor();
        Debug.Log(currentFloorNum);
        DungeonFloorText.Instance.UpdateText((int)currentDungeon.DungeonType + 1, currentFloorNum);
    }

    void Update()
    {

    }

    public void GenerateAllFloors()
    {
        floors = new List<Floor>();
        DungeonDataCache.Instance.areFloorsGenerated = true;
        int floorSize = currentDungeon.StartSize;
        for (int i = 0; i < maxFloors; i++)
        {
            if (currentDungeon.FloorSizeExpands)
                floorSize = Mathf.Min(currentDungeon.StartSize + i, 9);

            //Debug.LogWarning("Floor " + (i + 1) + ", Size: " + floorSize);
            floors.Add(FloorGenerator.Instance.GenerateFloor(floorSize));
        }
        DungeonDataCache.Instance.floors = floors;
    }

    public void SpawnNextFloor()
    {
        if (currentFloorNum < maxFloors)
        {
            FloorGenerator.Instance.InstantiateFloor(floors[currentFloorNum]);
            currentFloor = floors[currentFloorNum];
            currentFloorNum++;
        }
    }
    //public int GetCurrentFloorNum() => currentFloorNum;
    public int GetMaxFloorCount() => maxFloors;
    

    private void LoadDungeonData()
    {
        areFloorsGenerated = DungeonDataCache.Instance.areFloorsGenerated;
        floors = DungeonDataCache.Instance.floors;
        Debug.Log(DungeonDataCache.Instance.currentFloorNum);
        currentFloorNum = DungeonDataCache.Instance.currentFloorNum;
    }
}

public struct Dungeon
{
    public DungeonType DungeonType { get; init; }
    public int MaxFloorCount { get; init; }
    public int StartSize { get; init; }
    public bool FloorSizeExpands { get; init; }
}

public enum DungeonType
{
    dungeonTut = -1,
    dungeon0 = 0,
    dungeon1 = 1
}