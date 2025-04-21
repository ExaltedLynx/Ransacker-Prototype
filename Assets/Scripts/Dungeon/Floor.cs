using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Floor
{
    public int Dimension { get; private set; }
    public Room startRoom { get; private set; }
    public Room bossRoom { get; private set; }
    public List<Room> roomList = new List<Room>();
    private Room[,] floor;

    public Floor(int dimension)
    {
        Dimension = dimension;
        floor = new Room[dimension, dimension];
    }

    public Floor(Floor existingFloor)
    {
        Dimension = existingFloor.Dimension;
        startRoom = existingFloor.startRoom;
        bossRoom = existingFloor.bossRoom;
        roomList = existingFloor.roomList;
        floor = existingFloor.floor;
    }

    public Room this[int x, int y]
    {
        get => floor[x, y];
        set
        {
            floor[x, y] = value;
            value.roomPos = new Vector2Int(x, y);
            roomList.Add(value);
        }
    }

    public List<Room> GetConnectedRooms(Room room, bool includeDiagonals)
    {
        Vector2Int parentRoomCoord = room.roomPos;
        List<Room> result = new List<Room>();
        Vector2Int neighborPos = new Vector2Int();
        int maxNeighbors = includeDiagonals ? 8 : 4;
        for(int i = 0; i < maxNeighbors; i++)
        {
            switch(i)
            {
                case 0: neighborPos = parentRoomCoord + Vector2Int.right;
                    break;
                case 1: neighborPos = parentRoomCoord + Vector2Int.left;
                    break;
                case 2: neighborPos = parentRoomCoord + Vector2Int.up;
                    break;
                case 3: neighborPos = parentRoomCoord + Vector2Int.down;
                    break;
                case 4: neighborPos = parentRoomCoord + (Vector2Int.down + Vector2Int.right);
                    break;
                case 5: neighborPos = parentRoomCoord + (Vector2Int.down + Vector2Int.left);
                    break;
                case 6: neighborPos = parentRoomCoord + (Vector2Int.up + Vector2Int.right);
                    break;
                case 7: neighborPos = parentRoomCoord + (Vector2Int.up + Vector2Int.left);
                    break;
            }
            //checks if room is within floor bounds
            if(RoomIsWithinBounds(neighborPos) && floor[neighborPos.x, neighborPos.y] != null)   
                result.Add(floor[neighborPos.x, neighborPos.y]);
        }
        return result;
    }

    public void ResetFloor()
    {
        for(int x = 0; x < Dimension; x++)
            for (int y = 0; y < Dimension; y++)
                floor[x, y] = null;

        //startRoom.roomObject.Destroy();
        //bossRoom.roomObject.Destroy();
        startRoom = null;
        bossRoom = null;
        roomList.Clear();
        FloorGenerator.Instance.onFloorGenerated.RemoveAllListeners();
    }

    public void RemoveRoom(Room room)
    {
        //if (room.roomObject != null && room != startRoom && room != bossRoom)
        if (room != startRoom && room != bossRoom)
        {
            if (roomList.Contains(room))
                roomList.Remove(room);

            
            floor[room.roomPos.x, room.roomPos.y] = null;
            //room.roomObject.Destroy();
        }
    }

    public Room GetRandUniqueRoom()
    {
        int randIndex = UnityEngine.Random.Range(0, roomList.Count);
        //Debug.Log(randIndex + " " + roomList.Count);
        Room randRoom = roomList[randIndex];
        roomList.Remove(randRoom);
        return randRoom;
    }

    public Room GetRandUniqueRoom(int maxDimension)
    {
        int minX = (Dimension - maxDimension) / 2;
        int minY = minX;
        int maxX = Dimension - minX - 1;
        int maxY = maxX;

        List<Room> filtered = roomList.Where(room =>
        room.roomPos.x >= minX
        && room.roomPos.x <= maxX
        && room.roomPos.y >= minY
        && room.roomPos.y <= maxY).ToList();

        int randIndex = UnityEngine.Random.Range(0, filtered.Count);
        if (filtered.Count == 0)
            return null;
        //Debug.Log(randIndex);
        Room randRoom = filtered[randIndex];
        roomList.Remove(randRoom);
        return randRoom;
    }

    /// <summary>
    /// Returns a random unique empty room from the floor within a specified quadrant.
    /// bottom left = 1, bottom right = 2, top left = 3, top right = 4
    /// </summary>
    /// <param name="quadrant"></param>
    public Room GetRandomRoomFromQuadrant(int quadrant, int maxDimension)
    {
        int minX = 0, minY = 0, maxX = 0, maxY = 0;
        SetRoomPosLimits(quadrant, maxDimension, ref minX, ref maxX, ref minY, ref maxY);

        List<Room> filtered = roomList.Where(room =>
        room.roomPos.x >= minX
        && room.roomPos.x <= maxX
        && room.roomPos.y >= minY
        && room.roomPos.y <= maxY).ToList();

        //Debug.Log(quadrant);
        //foreach (Room room in filtered)
            //Debug.Log(room.roomPos);
        int randIndex = UnityEngine.Random.Range(0, filtered.Count);
        //Debug.Log(randIndex);
        Room randRoom = filtered[randIndex];
        //Debug.Log(randRoom.roomPos + " Index: " + randIndex);
        roomList.Remove(randRoom);
        return randRoom;
    }

    private void SetRoomPosLimits(int quadrant, int maxDimension, ref int minX, ref int maxX, ref int minY, ref int maxY)
    {
        int _minX = (Dimension - maxDimension) / 2;
        int floorMidpoint = Dimension / 2;
        int floorEndpoint = Dimension - _minX - 1;

        //Debug.Log(maxDimension);
        //Debug.Log(_minX + ", " + floorMidpoint + ", " + floorEndpoint);

        if (maxDimension % 2 == 0) //floor size is even
        {
            switch (quadrant)
            {
                case 1:
                    minX = _minX; maxX = floorMidpoint - 1;
                    minY = _minX; maxY = floorMidpoint - 1;
                    break;
                case 2:
                    minX = floorMidpoint; maxX = floorEndpoint;
                    minY = _minX; maxY = floorMidpoint - 1;
                    break;
                case 3:
                    minX = _minX; maxX = floorMidpoint - 1;
                    minY = floorMidpoint; maxY = floorEndpoint;
                    break;
                case 4:
                    minX = floorMidpoint; maxX = floorEndpoint;
                    minY = floorMidpoint; maxY = floorEndpoint;
                    break;
            }
        }
        else //floor size is odd
        {
            switch (quadrant)
            {
                case 1:
                    minX = _minX; maxX = floorMidpoint;
                    minY = _minX; maxY = floorMidpoint - 1;
                    break;
                case 2:
                    minX = floorMidpoint + 1; maxX = floorEndpoint;
                    minY = _minX; maxY = floorMidpoint;
                    break;
                case 3:
                    minX = _minX; maxX = floorMidpoint - 1;
                    minY = floorMidpoint; maxY = floorEndpoint;
                    break;
                case 4:
                    minX = floorMidpoint; maxX = floorEndpoint;
                    minY = floorMidpoint + 1; maxY = floorEndpoint;
                    break;
            }
        }
       //Debug.Log("Min: " + minX + ", " + minY + " Max: " + maxX + ", " + maxY);
    }

    public void SetStartRoom(Room room)
    {
        startRoom = room;
        roomList.Remove(startRoom);
    }

    public void SetEndRoom(Room room)
    {
        bossRoom = room;
        roomList.Remove(bossRoom);
    }

    public bool RoomIsWithinBounds(Vector2Int roomPos)
    {
        return roomPos.x > -1 && roomPos.x < Dimension && roomPos.y > -1 && roomPos.y < Dimension;
    }

    public void PrintFloorToLog()
    {
        for (int x = 0; x < Dimension; x++)
        {
            for (int y = 0; y < Dimension; y++)
            {
                if(floor[x, y] != null)
                    Debug.Log(floor[x, y]);
            }
        }
    }
}
