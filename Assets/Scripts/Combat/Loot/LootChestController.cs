using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class LootChestController : MonoBehaviour
{
    [SerializeField] SpriteRenderer chestRenderer;
    [SerializeField] Sprite chestClosed;
    [SerializeField] Sprite chestOpen;
    [SerializeField] Collider2D chestCollider;
    public bool chestWasOpened = false;
    public static LootChestController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.chestTutorialEnabled)
            return;

        chestRenderer.sprite = chestOpen;
        foreach (ItemDataBase itemData in GameManager.Instance.currentRoom.containedItems)
        {
            InventoryController.Instance.InsertLoot(itemData);
            //InventoryItem item = Items.InstantiateItem(itemData);
            //ItemStatsGenerator.AddStatsToItem(item);
            
        }
        LootQueue.Instance.MoveRemainingQueueToLootInv();
        chestCollider.enabled = false;
        StartCoroutine(DelayDisableChest());
    }

    public void EnableTreasureRoomChest()
    {
        chestRenderer.sprite = chestClosed;
        chestWasOpened = false;
        chestRenderer.enabled = true;
        chestCollider.enabled = true;
    }

    private IEnumerator DelayDisableChest()
    {
        yield return new WaitForSeconds(1);
        chestWasOpened = true;
        chestRenderer.enabled = false;
    }
}
