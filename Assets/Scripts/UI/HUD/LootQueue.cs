using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootQueue : MonoBehaviour
{
    public static LootQueue Instance { get; private set; }
    private Queue<InventoryItem> lootQueue = new Queue<InventoryItem>();
    private InventoryItem lootQueuePreview;

    private void Awake()
    {
        Instance = this;
    }

    //used in editor
    public void TryInsertQueueLoot()
    {
        if (lootQueue.TryPeek(out InventoryItem item))
        {
            Vector2Int? posOnGrid = PlayerInventory.Instance.FindSpaceForItem(item);
            if (posOnGrid != null)
            {
                item.gameObject.SetActive(true);
                InventoryController.Instance.InsertItem(item, PlayerInventory.Instance);
                lootQueue.Dequeue();
                UpdatePreview();
            }
        }
    }

    //used in editor
    public void MoveRemainingQueueToLootInv()
    {
        Debug.Log("moving queue to loot inv");
        if (lootQueue.Count > 0)
        {
            while (lootQueue.Count > 0)
            {
                InventoryItem queueItem = lootQueue.Dequeue();
                Debug.Log(queueItem.itemData.itemName);
                queueItem.gameObject.SetActive(true);
                InventoryController.Instance.InsertItem(queueItem, LootInventory.Instance);
            }
            lootQueuePreview = null;
            LootInventory.Instance.ChangeVisibility(true);
        }
    }

    public void AddLootToQueue(InventoryItem item)
    {
        Debug.Log(item.itemData.itemName);
        item.transform.SetParent(transform, true);
        item.transform.position = transform.position;
        item.gameObject.SetActive(false);
        lootQueue.Enqueue(item);
        UpdatePreview();
    }

    private void UpdatePreview()
    {
        if(lootQueue.TryPeek(out InventoryItem item))
        { 
            if (lootQueuePreview != null)
                lootQueuePreview.gameObject.SetActive(false);

            item.gameObject.SetActive(true);
            lootQueuePreview = item;
        }
    }
}
