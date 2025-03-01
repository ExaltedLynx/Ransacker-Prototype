using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DungeonDataCache : MonoBehaviour
{
    public List<Floor> floors { get; set; } = new();
    [SerializeField] public int currentFloorNum = 0;
    public bool areFloorsGenerated { get; set; } = false;
    public static DungeonDataCache Instance { get; private set; }

    public void InitInstance()
    {
        Instance = this;
    }

    public void ResetDungeonData()
    {
        currentFloorNum = 0;
        areFloorsGenerated = false;
        floors = new List<Floor>();
    }
}
