using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataCache : MonoBehaviour
{
    [SerializeField] private PersistInventoryHandler persistInventoryHandler;
    [SerializeField] private DungeonDataCache dungeonDataCache;
    private static GameDataCache Instance { get; set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            persistInventoryHandler.InitInstance();
            dungeonDataCache.InitInstance();
            DontDestroyOnLoad(gameObject);
        }

    }
}
