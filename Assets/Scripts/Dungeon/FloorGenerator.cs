using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FloorGenerator : MonoBehaviour
{
    [SerializeField] private GameObject roomPrefab;
    private int floorDimension;
    public OnFloorGeneratedEvent onFloorGenerated = new OnFloorGeneratedEvent();
    [SerializeField] private DungeonGenerator dungeonGenerator;
    private FloorPathFinder pathFinder;
    private int minFilledRooms = 0;
    private int maxRemoveableTiles = 0;
    private int removedTiles = 0;
    public Floor dungeonFloor { get; private set; }
    private int currentFloorNum;
    public static FloorGenerator Instance { get; private set; }

    private void Awake()
    {
        currentFloorNum = 0;
        pathFinder = new FloorPathFinder();
        Instance = this;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.R))
        {
            EditorUtils.ClearLog();
            NewFloor();
        }
        #endif
    }

    //TODO rework to function with new generation method and move to DungeonGenerator
    private void NewFloor()
    {
        removedTiles = 0;
        dungeonFloor.ResetFloor();
        GameManager.Instance.playerSelectedPath.Clear();
        //GenerateFloor();
    }

    public Floor GenerateFloor(int floorDimension)
    {
        this.floorDimension = floorDimension;
        removedTiles = 0;
        maxRemoveableTiles = (floorDimension - 1) * (floorDimension - 1) / 4;
        minFilledRooms = floorDimension * floorDimension / 4;
        dungeonFloor = new Floor(floorDimension);
        bool isFloorValid;
        do
        {
            for (int x = 0; x < floorDimension; x++)
            {
                for (int y = 0; y < floorDimension; y++)
                {
                    Room room = new Room();
                    room.floor = dungeonFloor;
                    room.roomType = Room.RoomType.Empty;
                    dungeonFloor[x, y] = room;
                    onFloorGenerated.AddListener(room.ReconnectDeadEndRoom);
                }
            }

            PickStartAndEndTile();
            //dungeonFloor.startRoom.room.EnableDebugColor(Color.green);
            //dungeonFloor.bossRoom.EnableDebugColor(Color.blue);
            CullTiles();

            isFloorValid = CheckFloorValidity();
            if(isFloorValid == false)
            {
                Debug.Log("generated invalid floor, remaking");
                removedTiles = 0;
                //GameManager.Instance.playerSelectedPath.Clear();
                dungeonFloor.ResetFloor();
            }
        } while (isFloorValid == false);
        onFloorGenerated.Invoke();
        onFloorGenerated.RemoveAllListeners();
        //FixNoExitCorners();
        GenerateRoomTypes();
        //dungeonFloor.PrintFloorToLog();
        currentFloorNum++;
        return new Floor(dungeonFloor);
    }

    //int countRooms = 0;
    public void InstantiateFloor(Floor floor)
    {
        //floor.PrintFloorToLog();
        floorDimension = floor.Dimension;
        float spawnOffset = (floorDimension - 5) * -0.5f;
        transform.localPosition += new Vector3(spawnOffset, spawnOffset, 0);
        for (int x = 0; x < floorDimension; x++)
        {
            for (int y = 0; y < floorDimension; y++)
            {
                //Debug.Log("test");
                if (floor[x, y] != null)
                {                   
                    //countRooms++;
                    //Debug.Log(countRooms);
                    GameObject roomObj = Instantiate(roomPrefab, transform);
                    roomObj.transform.localPosition = new Vector2(x, y) * 1.05f;
                    Room roomData = floor[x, y];
                    //Debug.Log(roomData.roomObjectName);
                    if (roomData.roomObjectName.Equals(""))
                        roomData.roomObjectName = "Room " + floor[x, y].roomPos;

                    RoomObj roomComponent = roomObj.GetComponent<RoomObj>();
                    roomComponent.SetRoomData(ref roomData);
                }
            }
        }
        GameManager.Instance.TotalMovement = (floorDimension * 2) + Mathf.FloorToInt(Hero.CachedSpeed / 10) + 2;
        RoomPreviewController.Instance.InitStepsLeftText();
    }

    private void PickStartAndEndTile()
    {
        int startX = Random.Range(0, floorDimension);
        int startY;
        int endX;
        int endY;

        if (startX > 0 && startX < floorDimension - 1)
        {
            if (Random.Range(0, 2) == 0)
            {
                startY = 0;
                endY = floorDimension - 1;
            }
            else
            {
                startY = floorDimension - 1;
                endY = 0;
            }
            endX = Random.Range(floorDimension - startX - 1, floorDimension - startX - 1);
        }
        else
        {
            if (startX == 0)
            {
                startY = Random.Range(0, floorDimension);
                endX = floorDimension - 1;
                endY = Random.Range(floorDimension - startY - 1, floorDimension - startY - 1);
            }
            else //startX == floorDimension - 1
            {
                startY = Random.Range(0, floorDimension);
                endX = 0;
                endY = Random.Range(floorDimension - startY - 1, floorDimension - startY - 1);
            }
        }

        //Debug.Log(startX +", " + startY);
        //Debug.Log(endX + ", " + endY);
        dungeonFloor.SetStartRoom(dungeonFloor[startX, startY]);
        dungeonFloor.startRoom.roomType = Room.RoomType.Start;
        dungeonFloor.startRoom.roomObjectName = "StartRoom (" + startX + ", " + startY + ")";

        if(currentFloorNum + 1 < dungeonGenerator.GetMaxFloorCount())
        {
            dungeonFloor.SetEndRoom(dungeonFloor[endX, endY]);
            dungeonFloor.bossRoom.roomType = Room.RoomType.Stairs;
            dungeonFloor.bossRoom.roomObjectName = "EndRoom (" + endX + ", " + endY + ")";
        }
        else //current floor num is equal to max floors
        {
            dungeonFloor.SetEndRoom(dungeonFloor[endX, endY]);
            dungeonFloor.bossRoom.roomType = Room.RoomType.Boss;
            dungeonFloor.bossRoom.roomObjectName = "BossRoom (" + endX + ", " + endY + ")";
        }
    }

    //re add min distance between rooms
    private void CullTiles()
    {
        List<Room> rooms = new List<Room>();
        Room randRoom = dungeonFloor.GetRandomRoomFromQuadrant(1, floorDimension - 2);
        rooms.Add(randRoom);
        randRoom = dungeonFloor.GetRandomRoomFromQuadrant(2, floorDimension - 2);
        rooms.Add(randRoom);
        randRoom = dungeonFloor.GetRandomRoomFromQuadrant(3, floorDimension - 2);
        rooms.Add(randRoom);
        randRoom = dungeonFloor.GetRandomRoomFromQuadrant(4, floorDimension - 2);
        rooms.Add(randRoom);

        /*
        List<Room> deniedRooms = new List<Room>();
        int minRoomDist = floorDimension < 7 ? 1 : 2;
        for (int i = 2; i <= 4; i++)
        {
            int flag = 0;
            //Debug.Log("Quadrant: " + i);
            while (flag < rooms.Count)
            {
                flag = 0;
                randRoom = dungeonFloor.GetRandomRoomFromQuadrant(i, floorDimension - 2);
                foreach (Room room in rooms)
                {
                    int dist = Mathf.Abs(randRoom.roomPos.x - room.roomPos.x) + Mathf.Abs(randRoom.roomPos.y - room.roomPos.y);
                    //Debug.Log(dist);
                    if (dist > minRoomDist)
                    {
                        //Debug.Log(randRoom.roomPos);
                        flag++;
                        if (flag == rooms.Count)
                            break;
                    }
                    else
                        deniedRooms.Add(randRoom);
                }
            }
            rooms.Add(randRoom);
        }

        dungeonFloor.roomList.AddRange(deniedRooms);
        */

        foreach (Room room in rooms)
        {
            //room.EnableDebugColor(Color.red);
            StartErodeFromRoom(room);
        }
    }

    private void StartErodeFromRoom(Room room)
    {
        List<Room> connectedRooms = dungeonFloor.GetConnectedRooms(room, floorDimension < 7 ? false : true);

        float erosionChance = 1f;
        while (erosionChance > 0f && removedTiles < maxRemoveableTiles)
        {
            if (connectedRooms.Count == 0)
                break;

            if (Random.Range(0f, 1f) <= erosionChance)
            {
                int randIndex = Random.Range(0, connectedRooms.Count);
                Room neighbor = connectedRooms[Random.Range(0, connectedRooms.Count)];
                if(neighbor != dungeonFloor.startRoom && neighbor != dungeonFloor.bossRoom && dungeonFloor.GetConnectedRooms(neighbor, false).Count >= 3)
                {
                    connectedRooms.Remove(neighbor);
                    dungeonFloor.RemoveRoom(neighbor);
                    removedTiles++;
                    erosionChance -= .2f;
                }
            }
            erosionChance -= .1f;
        }
    }
    
    /*
    private void ClearFloor()
    {
        Room[] rooms = GetComponentsInChildren<Room>();
        foreach (Room room in rooms)
            room.roomObject.Destroy();

        dungeonFloor.ResetFloor();
    }
    */

    private bool CheckFloorValidity()
    {
        List<Room> path;
        bool isFloorValid = pathFinder.FindPath(dungeonFloor.startRoom, dungeonFloor.bossRoom, out path);
        /*
        foreach (Room room in path)
            room.EnableDebugColor(Color.cyan);
        */
        return isFloorValid;
    }

    private void GenerateRoomTypes()
    {
        //spawn treasure and enemy rooms
        bool flag = false;
        int currentFilledRooms = 0;

        List<Room> rooms = new List<Room>();
        List<Room> deniedRooms = new List<Room>();
        Room randRoom = dungeonFloor.GetRandUniqueRoom();
        rooms.Add(randRoom);
        while (dungeonFloor.roomList.Count > 0)
        {
            randRoom = dungeonFloor.GetRandUniqueRoom();
            int validDistanceFlag = 0;
            foreach (Room room in rooms)
            {
                int dist = Mathf.Abs(randRoom.roomPos.x - room.roomPos.x) + Mathf.Abs(randRoom.roomPos.y - room.roomPos.y);
                if (dist > 1)
                    validDistanceFlag++;
            }
            if (validDistanceFlag == rooms.Count)
            {
                if (currentFilledRooms < minFilledRooms)
                {
                    randRoom.roomType = flag ? Room.RoomType.Enemy : Room.RoomType.Treasure;
                    currentFilledRooms++;
                }
                else
                {
                    float roomChance = Random.Range(0f, 1f);
                    if (roomChance >= 0.99f)
                        randRoom.roomType = flag ? Room.RoomType.Enemy : Room.RoomType.Treasure;
                }
                flag = !flag;
                rooms.Add(randRoom);
            }
            else
            {
                deniedRooms.Add(randRoom);
            }
        }
        dungeonFloor.roomList.AddRange(deniedRooms);

        //Debug.Log("treasure/enemy");

        //foreach (Room room in rooms)
        //Debug.Log(room.roomPos);

        //Debug.Log("//");

        //foreach (Room denied in deniedRooms)
        //Debug.Log(denied.roomPos);

        //spawn merchant room

        Room merchantRoom = dungeonFloor.GetRandUniqueRoom(floorDimension - 2);
        if (merchantRoom == null)
            return;
        merchantRoom.roomType = Room.RoomType.Shop;
        /*
        float merchantChance = 1f;
        if (Random.Range(0f, 1f) <= merchantChance)
        {
            Dictionary<Room, int> randRooms = new Dictionary<Room, int>();
            for(int i = 0; i < 3; i++)
            {
                randRoom = dungeonFloor.GetRandUniqueRoom(floorDimension - 4);
                int distToStart = Mathf.Abs(randRoom.roomPos.x - dungeonFloor.startRoom.roomPos.x) + Mathf.Abs(randRoom.roomPos.y - dungeonFloor.startRoom.roomPos.y);
                int distToEnd = Mathf.Abs(randRoom.roomPos.x - dungeonFloor.bossRoom.roomPos.x) + Mathf.Abs(randRoom.roomPos.y - dungeonFloor.bossRoom.roomPos.y);
                //Debug.Log("//");
                //Debug.Log(randRoom.roomPos);
                randRooms.Add(randRoom, distToStart + distToEnd);
            }

            Room merchantRoom = null;
            int maxDist = 0;
            foreach (KeyValuePair<Room, int> kv in randRooms)
            {
                if (kv.Value > maxDist)
                {
                    maxDist = kv.Value;
                    merchantRoom = kv.Key;
                }
            }
            merchantRoom.roomType = Room.RoomType.Shop;
        }
        */
    }

    //rework to check if entrances to weird corners are only on the diagonal to the corner
    private void FixNoExitCorners()
    {
        List<Room> cornerRooms = new List<Room>
        {
            dungeonFloor[0, 0],
            dungeonFloor[0, floorDimension - 1],
            dungeonFloor[floorDimension - 1, 0],
            dungeonFloor[floorDimension - 1, floorDimension - 1]
        };

        foreach(Room room in cornerRooms)
        {
            if(room != null)
            {
                List<Room> neighbors = dungeonFloor.GetConnectedRooms(room, true);

                Vector2Int posOffset1 = room.roomPos.x == 0 ? new Vector2Int(2, 0) : new Vector2Int(-2, 0);
                Vector2Int newRoomPos1 =  room.roomPos + posOffset1;

                Vector2Int posOffset2 = room.roomPos.y == 0 ? new Vector2Int(0, 2) : new Vector2Int(0, -2);
                Vector2Int newRoomPos2 = room.roomPos + posOffset2;

                //if (neighbors.Count == 3 && dungeonFloor.GetConnectedRooms(neighbors[2], false).Count >= 2 && dungeonFloor[newRoomPos1.x, newRoomPos1.y] == null || dungeonFloor[newRoomPos2.x, newRoomPos2.y] == null)
                if (dungeonFloor[newRoomPos1.x, newRoomPos1.y] == null && dungeonFloor[newRoomPos2.x, newRoomPos2.y] == null)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        //Debug.Log(newRoomPos1);
                        ReAddRoom(newRoomPos1);
                    }
                    else
                    {
                        //Debug.Log(newRoomPos2);
                        ReAddRoom(newRoomPos2);
                    }
                }
            }
        }
    }

    public void ReAddRoom(Vector2Int roomPos)
    {
        Room room = new Room();
        room.floor = dungeonFloor;
        dungeonFloor[roomPos.x, roomPos.y] = room;
        room.roomObjectName = "Room " + room.roomPos + " (Re added)";
        //Debug.Log(roomPos, room.roomObject);
    }
}

[System.Serializable]
public class OnFloorGeneratedEvent : UnityEvent { }
