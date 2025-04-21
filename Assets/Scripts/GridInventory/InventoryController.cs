using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance { get; private set; }

    public InventoryGrid SelectedInventory { 
        get => selectedInventory;
        set
        {
            selectedInventory = value;
        }
    }
    [SerializeField] private InventoryGrid selectedInventory;

    public InventoryItem selectedItem { get; private set; }
    InventoryItem overlapItem;
    RectTransform rectTransform;

    private Vector2 screenBounds;

    [SerializeField] GameObject selectedItemContainer;
    [SerializeField] InventoryGrid playerInventory;
    [SerializeField] LootInventory lootInventory;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;

    InventoryHighlight inventoryHighlight;

    public const int baseScreenWidth = 1920;
    public const int baseScreenHeight = 1080;

    public int currentScreenWidth;
    public int currentScreenHeight;

    void Awake()
    {
        Instance = this;
        currentScreenWidth = Screen.width;
        currentScreenHeight = Screen.height;
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        inventoryHighlight = GetComponent<InventoryHighlight>();
    }

    private void Start()
    {
        LoadCachedHeldItem();
    }

    void Update()
    {
        if(GameManager.Instance.gameIsPaused)
            return;

        ItemIconDrag();

        if (Input.GetMouseButtonDown(1))
        {
            RotateItem();
        }

        if (selectedInventory == null)
        {
            inventoryHighlight.Show(false);
            return;
        }

        HandleHighlight();

        if (Input.GetMouseButtonDown(0))
        {
            LeftMouseButtonPress();
        }

        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.W))
        {
            InsertRandomItem();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            EquipItem();
        }
        #endif
    }

    private void LateUpdate()
    {
        
    }

    private void RotateItem()
    {
        if(selectedItem == null) { return; }
        selectedItem.Rotate();
    }

    //inserts a random item into the hovered inventory
    private void InsertRandomItem()
    {
        InventoryItem itemToInsert = CreateRandomItem();
        if (!InsertItem(itemToInsert, selectedInventory))
        {
            Destroy(itemToInsert.gameObject);
            return;
        }
        itemToInsert.InitItemStats();
    }

    //inserts item into the inventory if there is available space
    public bool InsertItem(InventoryItem itemToInsert, InventoryGrid inventoryGrid)
    {
        Vector2Int? posOnGrid = inventoryGrid.FindSpaceForItem(itemToInsert);

        if(posOnGrid == null)
        {
            if(inventoryGrid is LootInventory lootInventory)
            {
                if (lootInventory.TryResizeLootGrid(itemToInsert))
                    posOnGrid = lootInventory.FindSpaceForItem(itemToInsert);
                else
                    return false;
            }
            else
            {
                Debug.Log("no space for item in inventory");
                return false;
            }
        }

        //Debug.Log(itemToInsert);
        Debug.Log(posOnGrid);
        inventoryGrid.InsertItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
        return true;
    }

    public void InsertLoot(ItemDataBase itemData)
    {
        InventoryItem itemToInsert = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        itemToInsert.Set(itemData);
        RectTransform itemTransform = itemToInsert.GetComponent<RectTransform>();
        itemTransform.SetParent(canvasTransform);
        itemTransform.localScale = new Vector3(.5f, .5f, .5f);

        itemToInsert.InitItemStats();

        Vector2Int? posOnGrid = playerInventory.FindSpaceForItem(itemToInsert);
        //Debug.Log(posOnGrid);

        if (posOnGrid != null)
        {
            playerInventory.InsertItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
        }
        else //no space in player inventory
        {
            LootQueue.Instance.AddLootToQueue(itemToInsert);
        }
    }

    public void RemoveHeldItem()
    {
        Destroy(selectedItem.gameObject);
        selectedItem = null;
    }

    /*
     * This highlights items you are hovering if you aren't holding an item
     * and if you are it highlights where the selected item will be placed
    */
    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetTileGridCoords();
        //Debug.Log(positionOnGrid.x + ", " + positionOnGrid.y);
        if (selectedItem == null)
        {
            inventoryHighlight.SetColor(inventoryHighlight.highlightGreen);
            InventoryItem itemToHighlight = selectedInventory.GetItem(positionOnGrid.x, positionOnGrid.y);
            if(itemToHighlight != null)
            {
                inventoryHighlight.Show(true);
                inventoryHighlight.SetSize(itemToHighlight);
                inventoryHighlight.SetParentInventory(selectedInventory);
                inventoryHighlight.SetPosition(selectedInventory, itemToHighlight);
            }
            else
            {
                inventoryHighlight.Show(false);
            }
        }
        else
        {
            bool showHighlight = selectedInventory.PositionCheck(positionOnGrid.x, positionOnGrid.y);
            if (showHighlight == false) { return; }

            if (selectedInventory.BoundaryCheck(positionOnGrid.x, positionOnGrid.y, selectedItem.WIDTH, selectedItem.HEIGHT, out int oobAmountX, out int oobAmountY) == false)
            {
                //Debug.Log("OOB Diff: " + oobAmountX + ", " + oobAmountY);
                positionOnGrid.x += oobAmountX;
                positionOnGrid.y += oobAmountY;
                //Debug.Log("Placed: " + positionOnGrid.x + ", " + positionOnGrid.y);
            }
            else if(!selectedInventory.OverlapCheck(positionOnGrid.x, positionOnGrid.y, selectedItem.WIDTH, selectedItem.HEIGHT))
            {
                Color highlightRed = Color.red;
                highlightRed.a = 0.5f;
                inventoryHighlight.SetColor(highlightRed);
            }
            else
            {
                inventoryHighlight.SetColor(inventoryHighlight.highlightGreen);
            }
            inventoryHighlight.SetParentInventory(selectedInventory);
            inventoryHighlight.Show(true);
            inventoryHighlight.SetSize(selectedItem);
            inventoryHighlight.SetPosition(selectedInventory, selectedItem, positionOnGrid.x, positionOnGrid.y);
        }
    }

    //creates a random item from a list of items and is automatically selected
    private InventoryItem CreateRandomItem()
    {
        InventoryItem item = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        rectTransform = item.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        rectTransform.localScale = new Vector3(.5f, .5f, .5f);

        ItemDataBase randItemData = Items.FindItem<ItemDataBase>();
        item.Set(randItemData);

        return item;
    }

    private void LeftMouseButtonPress()
    {
        Vector2Int tileGridPos = GetTileGridCoords();
        //Debug.Log(tileGridPos.x + ", " + tileGridPos.y);
        if (selectedItem == null)
        {
            PickUpItem(tileGridPos);
        }
        else
        {
            PlaceItem(tileGridPos);
        }
    }

    private void EquipItem()
    {
        Vector2Int tileGridPos = GetTileGridCoords();
        InventoryItem item = selectedInventory.GetItem(tileGridPos.x, tileGridPos.y);
        if (item != null)
        {
            Hero.Instance.EquipItem(item);
            playerInventory.RemoveItem(tileGridPos.x, tileGridPos.y);
        }

    }

    //gets coordinates of the tiles in the inventory from the mouse position
    private Vector2Int GetTileGridCoords()
    {
        Vector2 pos = Input.mousePosition;
        //Debug.Log(currentScreenWidth + "x" + currentScreenHeight);
        pos.x *= (float)baseScreenWidth / currentScreenWidth;
        pos.y *= (float)baseScreenHeight / currentScreenHeight;

        if (selectedItem != null)
        {
            pos.x -= (selectedItem.WIDTH - 1) * InventoryGrid.tileSizeWidthConst / 4;
            pos.y += (selectedItem.HEIGHT - 1) * InventoryGrid.tileSizeHeightConst / 4;
            //Debug.Log(pos);
        }

        Vector2Int inventoryCoord = selectedInventory.GetTileGridCoords(pos);
        inventoryCoord.x = Mathf.Clamp(inventoryCoord.x, 0, selectedInventory.currentGridWidth - 1);
        inventoryCoord.y = Mathf.Clamp(inventoryCoord.y, 0, selectedInventory.currentGridHeight - 1);

        return inventoryCoord;
    }

    //handles logic when placing an item by clicking
    private void PlaceItem(Vector2Int tileGridPos)
    {
        selectedItem.GetComponent<Image>().raycastTarget = true;
        bool itemPlaced = selectedInventory.PlaceItem(selectedItem, tileGridPos.x, tileGridPos.y, ref overlapItem);
        if(itemPlaced)
        {
            selectedItem.ToggleTooltip();
            selectedItem = null;
            if(overlapItem != null)
            {
                selectedItem = overlapItem;
                overlapItem = null;
                rectTransform = selectedItem.GetComponent<RectTransform>();
                selectedItem.transform.SetParent(selectedItemContainer.transform);
                selectedItem.ToggleTooltip();
                
            }
        }
    }

    //handles pick up item logic
    private void PickUpItem(Vector2Int tileGridPos)
    {
        selectedItem = selectedInventory.PickUpItem(tileGridPos.x, tileGridPos.y);
        if (selectedItem != null)
        {
            rectTransform = selectedItem.GetComponent<RectTransform>();
            selectedItem.transform.SetParent(selectedItemContainer.transform);
            selectedItem.ToggleTooltip();
            selectedItem.GetComponent<Image>().raycastTarget = false;
        }
    }

    //attaches selected item to mouse position
    private void ItemIconDrag()
    {
        if (selectedItem != null)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 1;
            ClampSelectedItemPos(ref worldPos);
            selectedItem.transform.position = worldPos;
        }
        else if(selectedItem == null && rectTransform != null)
        {
            return;
        }
    }

    private void ClampSelectedItemPos(ref Vector3 selectedItemPos)
    {
        float itemMidWidth = selectedItem.rotated ? selectedItem.itemData.itemIcon.bounds.extents.y : selectedItem.itemData.itemIcon.bounds.extents.x;
        float itemMidHeight = selectedItem.rotated ? selectedItem.itemData.itemIcon.bounds.extents.x : selectedItem.itemData.itemIcon.bounds.extents.y;
        selectedItemPos.x = Mathf.Clamp(selectedItemPos.x, screenBounds.x * -1 + itemMidWidth, screenBounds.x - itemMidWidth);
        selectedItemPos.y = Mathf.Clamp(selectedItemPos.y, screenBounds.y * -1 + itemMidHeight, screenBounds.y - itemMidHeight);
    }

    private void LoadCachedHeldItem()
    {
        if (DungeonDataCache.Instance.currentFloorNum > 0 && PersistInventoryHandler.Instance.heldItem.itemData != null)
        {
            InventoryItem heldItem = Items.InstantiateCachedItem(PersistInventoryHandler.Instance.heldItem, selectedItemContainer.transform);
            selectedItem = heldItem;
        }
    }

    //Used in editor
    public void PreventSelectedItemSoftlock()
    {
        if(selectedItem != null && PlayerInventory.Instance.FindSpaceForItem(selectedItem) == null && HotbarInventory.Instance.FindSpaceForItem(selectedItem) == null)
        {
            if(InsertItem(selectedItem, LootInventory.Instance))
                selectedItem = null;
        }
    }
}
