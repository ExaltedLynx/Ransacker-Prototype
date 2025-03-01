using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorPathFinder
{
    //private Floor floor;
    private PriorityQueue<Room> candidateRooms;

    public FloorPathFinder()
    {
        //this.floor = floor;
        candidateRooms = new PriorityQueue<Room>(new CompareRoomDistance());
    }

    public bool FindPath(Room start, Room end)
    {
       return FindPath(start, end, out _);
    }

    //Uses A* algorithm
    public bool FindPath(Room start, Room end, out List<Room> path)
    {
        path = new List<Room>();
        Dictionary<Room, int> gScore = new Dictionary<Room, int>();
        gScore[start] = 0;

        start.FValue = CalcManhattanDistance(start.roomPos, end.roomPos);
        candidateRooms.Enqueue(start); //adds starting room to priority queue

        while(candidateRooms.Count > 0)
        {
            Room currentRoom = candidateRooms.Dequeue(); //current room becomes next room with the lowest f value
            if (currentRoom.roomPos == end.roomPos)
            {
                candidateRooms.Clear();
                return true;
            }
            foreach(Room neighbor in FloorGenerator.Instance.dungeonFloor.GetConnectedRooms(currentRoom, false))
            {
                int possible_gScore = gScore[currentRoom] + 1; //cost between rooms is 1
                if(!gScore.ContainsKey(neighbor) || possible_gScore < gScore[neighbor])
                {
                    gScore[neighbor] = possible_gScore;
                    neighbor.FValue = possible_gScore + CalcManhattanDistance(neighbor.roomPos, end.roomPos);
                    candidateRooms.Enqueue(neighbor);
                    path.Add(currentRoom);
                }
            }
        }
        candidateRooms.Clear();
        return false;
    }

    private int CalcManhattanDistance(Vector2Int source, Vector2Int target)
    {
        int dist = Mathf.Abs(source.x - target.x) + Mathf.Abs(source.y - target.y);
        return dist;
    }

    internal class CompareRoomDistance : IComparer<Room>
    {
        public int Compare(Room room1, Room room2)
        {

            if (room1.FValue > room2.FValue)
                return 1;

            if (room1.FValue < room2.FValue)
                return -1;

            return 0;
        }
    }
}
