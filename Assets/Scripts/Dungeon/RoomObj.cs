using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Room;
using static ItemRarity;

public class RoomObj : MonoBehaviour
{
    [SerializeField] private Room roomData;
    [SerializeField] public SpriteRenderer pathRenderer { get; private set; }
    [SerializeField] private RoomPathHandler pathHandler;
    [SerializeField] SpriteRenderer roomRenderer;
    [SerializeField] Sprite EmptyRoomTexture;
    [SerializeField] Sprite EnemyRoomTexture;
    [SerializeField] Sprite BossRoomTexture;
    [SerializeField] Sprite TreasureRoomTexture;
    [SerializeField] Sprite ShopRoomTexture;
    [SerializeField] Sprite StairsRoomTexture;

    private void Awake()
    {
        pathRenderer = GetComponentsInChildren<SpriteRenderer>()[1];
    }

    void Start()
    {

    }

    private void OnMouseDown()
    {
        if(GameManager.Instance.gameIsPaused)
            return;

        if(roomData.roomType == RoomType.Enemy || roomData.roomType == RoomType.Boss && GameManager.Instance.pathSelectionTutEnabled == false)
        {
            if (roomData.roomType is RoomType.Enemy)
                RoomPreviewController.Instance.SetEnemyPreviewSprites(roomData.roomEnemies);
            else //boss room
                RoomPreviewController.Instance.SetBossPreviewSprite(roomData.roomBoss);
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void InitRoom(RoomType roomType)
    {
        switch (roomType)
        {
            case RoomType.Empty:
                roomRenderer.sprite = EmptyRoomTexture;
                break;
            case RoomType.Start:
                roomRenderer.sprite = EmptyRoomTexture;
                //name = "StartRoom";
                pathRenderer.enabled = true;
                pathHandler.SetStartRoomPathTexture();
                break;
            case RoomType.Stairs:
                roomRenderer.sprite = StairsRoomTexture;
                //name = "EndRoom";
                break;
            case RoomType.Boss:
                roomRenderer.sprite = BossRoomTexture;
                InitBossRoom();
                //name = "BossRoom";
                break;
            case RoomType.Enemy:
                roomRenderer.sprite = EnemyRoomTexture;
                InitEnemyRoom();
                break;
            case RoomType.Treasure:
                roomRenderer.sprite = TreasureRoomTexture;
                InitTreasureRoom();
                break;
            case RoomType.Shop:
                roomRenderer.sprite = ShopRoomTexture;
                InitShopRoom();
                break;
        }
    }

    private void InitEnemyRoom()
    {
        int roomEnemyNum = UnityEngine.Random.Range(1, 5);
        roomData.roomEnemies = new List<Enemy>();
        for (int i = 0; i < roomEnemyNum; i++)
        {
            roomData.roomEnemies.Add(Enemies.GetRandomEnemy());
        }
    }

    private void InitTreasureRoom()
    {
        int chestItemNum = UnityEngine.Random.Range(1, 5);
        Rarity maxLootRarity = (Rarity)(4 - chestItemNum);
        roomData.containedItems = new List<ItemDataBase>();
        for(int i = 0; i < chestItemNum; i++)
        {
            int randNum = UnityEngine.Random.Range(0, 3);
            Rarity randRarity = GetDropRarity();
            Rarity minLootRarity = (Rarity)Math.Min((int)randRarity, (int)maxLootRarity);
            switch (randNum)
            {
                case 0:
                    roomData.containedItems.Add(Items.FindItem<WeaponItem>(item => item.itemRarity >= minLootRarity && item.itemRarity <= maxLootRarity));
                    break;
                case 1:
                    roomData.containedItems.Add(Items.FindItem<EquipmentItem>(item => item.itemRarity <= maxLootRarity && item.itemRarity <= maxLootRarity));
                    break;
                case 2:
                    roomData.containedItems.Add(Items.FindItem<ConsumableItem>(item => item.itemRarity <= maxLootRarity && item.itemRarity <= maxLootRarity));
                    break;
            }
        }
    }

    private void InitShopRoom()
    {
        roomData.containedItems = new List<ItemDataBase>();
        for (int i = 0; i < 3; i++)
        {
            int randNum = UnityEngine.Random.Range(0, 2);
            //Remove once legendary equipment is added
            Rarity itemRarity = GetDropRarity();
            itemRarity = (Rarity)Math.Clamp((int)itemRarity, (int)Rarity.Common, (int)Rarity.Epic);

            switch (randNum)
            {
                case 0:
                    roomData.containedItems.Add(Items.FindItem<WeaponItem>(item => item.itemRarity == itemRarity));
                    break;
                case 1:
                    roomData.containedItems.Add(Items.FindItem<EquipmentItem>(item => item.itemRarity == itemRarity));
                    break;
            }
        }
    }

    private void InitBossRoom()
    {
        roomData.roomBoss = Enemies.GetRandomBoss(DungeonType.dungeon0);
    }

    public bool IsRoomAdjacent(Room room)
    {
        List<Room> adjacentRooms = roomData.floor.GetConnectedRooms(roomData, false);

        if(adjacentRooms.Contains(room))
            return true;

        return false;
    }

    public ref Room GetRoomData() { return ref roomData; }
    public void SetRoomData(ref Room roomData)
    {
        this.roomData = roomData;
        this.roomData.roomObject = this;
        
    }
}

[Serializable]
public class Room : IEquatable<Room>
{
    public string roomObjectName { get; set; } = "";
    public RoomObj roomObject
    {
        get => _roomObject;
        set
        {
            _roomObject = value;
            value.name = roomObjectName;
            value.InitRoom(_roomType);
            if(debugEnabled)
                value.GetComponent<SpriteRenderer>().color = debugColor;
        }
    }
    private RoomObj _roomObject;

    public RoomType roomType { get => _roomType; set => _roomType = value; }
    [SerializeField] private RoomType _roomType;

    public Vector2Int roomPos;
    public int FValue;
    public Boss roomBoss;
    public List<Enemy> roomEnemies;
    public List<ItemDataBase> containedItems;
    public Floor floor { get; set; }
    [NonSerialized] public List<Room> adjacentRooms;

    private Color debugColor;
    private bool debugEnabled;

    public Room()
    {
        containedItems = new List<ItemDataBase>();
        adjacentRooms = new List<Room>();
    }

    public void ReconnectDeadEndRoom()
    {
        if (this != null && this != floor.startRoom && this != floor.bossRoom)
        {
            adjacentRooms = floor.GetConnectedRooms(this, false);
            if (adjacentRooms.Count == 0)
            {
                floor.RemoveRoom(this);
            }
            else if (adjacentRooms.Count == 1)
            {
                AddNeighborInRandDirection();
            }
            else if((roomPos.x == floor.Dimension - 1 || roomPos.x == 0 && roomPos.y == floor.Dimension - 1 || roomPos.y == 0) && roomPos.x != roomPos.y && roomPos != new Vector2Int(floor.Dimension - 1, 0) && roomPos != new Vector2Int(0, floor.Dimension - 1))
            {
                adjacentRooms = floor.GetConnectedRooms(this, true);
                if (adjacentRooms.Count == 3)
                {
                    AddNeighborToEdgeRoom();
                }
            }
        }
    }

    private void AddNeighborInRandDirection()
    {
        Vector2Int newRoomPos = new Vector2Int();
        do
        {
            int randNeighbor = UnityEngine.Random.Range(0, 4);
            switch (randNeighbor)
            {
                case 0:
                    newRoomPos = roomPos + Vector2Int.right;
                    break;
                case 1:
                    newRoomPos = roomPos + Vector2Int.left;
                    break;
                case 2:
                    newRoomPos = roomPos + Vector2Int.up;
                    break;
                case 3:
                    newRoomPos = roomPos + Vector2Int.down;
                    break;
            }
        } while (!floor.RoomIsWithinBounds(newRoomPos) || newRoomPos == adjacentRooms[0].roomPos);
        FloorGenerator.Instance.ReAddRoom(newRoomPos);
    }

    private void AddNeighborToEdgeRoom()
    {
        //Debug.Log(roomPos);
        Vector2Int? newRoomPos = DetermineNewRoomPos();
        if (newRoomPos != null)
        {
            FloorGenerator.Instance.ReAddRoom((Vector2Int)newRoomPos);
            //Debug.Log("Parent Room: " + roomPos + " Added Room: " + newRoomPos + " Floor Size: " + floor.Dimension);
        }
    }

    private Vector2Int? DetermineNewRoomPos()
    {
        Vector2Int newRoomPos = roomPos;
        Vector2Int testRoom1 = new Vector2Int();
        Vector2Int testRoom2 = new Vector2Int();
        int maxPos = floor.Dimension - 1;

        if (roomPos.x == 0)
        {
            testRoom1 = roomPos + Vector2Int.down;
            testRoom2 = roomPos + Vector2Int.up;
            //Debug.Log("Neighbor 1: " + testRoom1 + " Neighbor 2: " + testRoom2);
            if (floor[testRoom1.x, testRoom1.y] != null && floor[testRoom2.x, testRoom2.y] != null)
                return null;

            if (floor[testRoom1.x, testRoom1.y] == null)
            {
                newRoomPos += Vector2Int.down + Vector2Int.right;
            }
            else
            {
                newRoomPos += Vector2Int.up + Vector2Int.right;
            }
        }
        else if (roomPos.x == maxPos)
        {
            testRoom1 = roomPos + Vector2Int.down;
            testRoom2 = roomPos + Vector2Int.up;
            //Debug.Log("Neighbor 1: " + testRoom1 + " Neighbor 2: " + testRoom2);
            if (floor[testRoom1.x, testRoom1.y] != null && floor[testRoom2.x, testRoom2.y] != null)
                return null;

            if (floor[testRoom1.x, testRoom1.y] == null)
            {
                newRoomPos += Vector2Int.down + Vector2Int.left;
            }
            else
            {
                newRoomPos += Vector2Int.up + Vector2Int.left;
            }
        }
        else if (roomPos.y == 0)
        {
            testRoom1 = roomPos + Vector2Int.left;
            testRoom2 = roomPos + Vector2Int.right;
            //Debug.Log("Neighbor 1: " + testRoom1 + " Neighbor 2: " + testRoom2);
            if (floor[testRoom1.x, testRoom1.y] != null && floor[testRoom2.x, testRoom2.y] != null)
                return null;

            if (floor[testRoom1.x, testRoom1.y] == null)
            {
                newRoomPos += Vector2Int.left + Vector2Int.up;
            }
            else
            {
                newRoomPos += Vector2Int.right + Vector2Int.up;
            }
        }
        else if (roomPos.y == maxPos)
        {
            testRoom1 = roomPos + Vector2Int.left;
            testRoom2 = roomPos + Vector2Int.right;
            //Debug.Log("Neighbor 1: " + testRoom1 + " Neighbor 2: " + testRoom2);
            if (floor[testRoom1.x, testRoom1.y] != null && floor[testRoom2.x, testRoom2.y] != null)
                return null;

            if (floor[testRoom1.x, testRoom1.y] == null)
            {
                newRoomPos += Vector2Int.left + Vector2Int.down;
            }
            else
            {
                newRoomPos += Vector2Int.right + Vector2Int.down;
            }
        }
        return newRoomPos;
    }

    public bool Equals(Room other)
    {
        if (roomPos == other.roomPos)
            return true;

        return false;
    }

    public void EnableDebugColor(Color color)
    {
        debugEnabled = true;
        debugColor = color;
    }

    public enum RoomType
    {
        Empty,
        Start,
        Stairs,
        Boss,
        Enemy,
        Treasure,
        Shop
    }
}
