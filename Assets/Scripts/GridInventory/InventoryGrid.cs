using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryGrid : MonoBehaviour
{
    public const float tileSizeWidthConst = 128;
    public const float tileSizeHeightConst = 128;

    public float tileSizeWidth = 128;
    public float tileSizeHeight = 128;

    [SerializeField] public int initialGridWidth;
    [SerializeField] public int initialGridHeight;
    [HideInInspector] public int currentGridWidth;
    [HideInInspector] public int currentGridHeight;

    public const int MAX_INVENTORY_WIDTH = 9;
    public const int MAX_INVENTORY_HEIGHT = 9;

    RectTransform rectTransform;
    [SerializeField] protected BoxCollider2D invCollider;

    public InventoryItem[,] inventoryItemSlots { get; private set; }

    Vector2 positionOnGrid = new Vector2();
    Vector2Int tileGridCoords = new Vector2Int();

    protected virtual void Start()
    {
        Image image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        float ppum = image.pixelsPerUnitMultiplier;
        tileSizeWidth /= ppum;
        tileSizeHeight = tileSizeWidth;
        currentGridWidth = initialGridWidth;
        currentGridHeight = initialGridHeight;
        InitGridInventory(initialGridWidth, initialGridHeight);
    }

    private void InitGridInventory(int gridWidth, int gridHeight)
    {
        Vector2 gridSize = new Vector2(tileSizeWidth * gridWidth, tileSizeHeight * gridHeight);
        rectTransform.sizeDelta = gridSize;
        invCollider.size = gridSize;
        Vector2 colliderOffset = new Vector2(gridSize.x / 2, -gridSize.y / 2);
        invCollider.offset = colliderOffset;
        inventoryItemSlots = new InventoryItem[MAX_INVENTORY_WIDTH, MAX_INVENTORY_HEIGHT];
    }

    protected void ResizeGrid(int width, int height)
    {
        currentGridWidth = width;
        currentGridHeight = height;
        Vector2 gridSize = new Vector2(tileSizeWidth * currentGridWidth, tileSizeHeight * currentGridHeight);
        rectTransform.sizeDelta = gridSize;
        invCollider.size = gridSize;
        Vector2 colliderOffset = new Vector2(gridSize.x / 2, -gridSize.y / 2);
        invCollider.offset = colliderOffset;
    }

    public InventoryItem PickUpItem(int x, int y)
    {
        InventoryItem toReturn = inventoryItemSlots[x, y];

        if (toReturn != null)
            CleanGridReference(toReturn);

        return toReturn;
    }

    public void RemoveItem(int x, int y)
    {
        InventoryItem toRemove = inventoryItemSlots[x, y];
        CleanGridReference(toRemove);
    }

    public void RemoveAndDestroyItem(int x, int y)
    {
        InventoryItem toRemove = inventoryItemSlots[x, y];
        if(toRemove != null)
        {
            CleanGridReference(toRemove);
            Destroy(toRemove.gameObject);
        }
    }

    //Removes the stored item from the inventory 2d array
    private void CleanGridReference(InventoryItem item)
    {
        for (int ix = 0; ix < item.WIDTH; ix++)
        {
            for (int iy = 0; iy < item.HEIGHT; iy++)
            {
                inventoryItemSlots[item.gridPositionX + ix, item.gridPositionY + iy] = null;
            }
        }
    }

    public void ClearAllItems()
    {
        for(int x = 0; x < currentGridWidth; x++)
        {
            for(int y = 0; y < currentGridHeight; y++)
            {
                RemoveAndDestroyItem(x, y);
            }
        }
    }

    public InventoryItem GetItem(int x, int y)
    {
        return inventoryItemSlots[x, y];
    }

    //using some math gets the coordinates of an inventory slot
    public Vector2Int GetTileGridCoords(Vector2 mousePos)
    {

        //Debug.Log(mousePos);
        //Debug.Log(rectTransform);
        positionOnGrid.x = mousePos.x - rectTransform.anchoredPosition.x;
        positionOnGrid.y = rectTransform.anchoredPosition.y - mousePos.y;

        //Debug.Log(positionOnGrid);

        tileGridCoords.x = (int)(positionOnGrid.x / tileSizeWidth);
        tileGridCoords.y = (int)(positionOnGrid.y / tileSizeHeight);

        return tileGridCoords;
    }

    //returns a slot in the inventory where there is space to place the passed item
    public virtual Vector2Int? FindSpaceForItem(InventoryItem itemToInsert)
    {
        int height = currentGridHeight - itemToInsert.HEIGHT + 1;
        int width = currentGridWidth - itemToInsert.WIDTH + 1;

        for (int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
               if(CheckAvailableSpace(x, y, itemToInsert.WIDTH, itemToInsert.HEIGHT) == true)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return null;
    }

    //ONLY FOR WHEN THE PLAYER MANUALLY PLACES ITEMS
    //Places item in inventory but checks if it will be overlapping another item or if its out of bounds
    public bool PlaceItem(InventoryItem item, int posX, int posY, ref InventoryItem overlapItem)
    {
        //if mouse is not within inventory
        if(PositionCheck(posX, posY) == false) { return false; }

        //if the item itself is not within inventory
        if (BoundaryCheck(posX, posY, item.WIDTH, item.HEIGHT, out int oobAmountX, out int oobAmountY) == false)
        {
            Debug.Log("OOB Diff: " + oobAmountX + ", " + oobAmountY);
            posX += oobAmountX;
            posY += oobAmountY;
            Debug.Log("Placed: " + posX + ", " + posY);
        }

        if(posX < 0 || posY < 0) { 
            return false; 
        }

        if (OverlapCheck(posX, posY, item.WIDTH, item.HEIGHT, ref overlapItem) == false)
        {
            overlapItem = null;
            return false;
        }

        if (overlapItem != null)
        {
            CleanGridReference(overlapItem);
        }

        InsertItem(item, posX, posY);

        return true;
        
    }

    //places item in the appropriate slots
    public virtual void InsertItem(InventoryItem item, int posX, int posY)
    {
        RectTransform itemTransform = item.GetComponent<RectTransform>();
        itemTransform.SetParent(rectTransform);

        for (int x = 0; x < item.WIDTH; x++)
        {
            for (int y = 0; y < item.HEIGHT; y++)
            {
                inventoryItemSlots[posX + x, posY + y] = item;
            }
        }

        item.gridPositionX = posX;
        item.gridPositionY = posY;

        Vector2 position = CalculateLocalPosOnGrid(item, posX, posY);

        itemTransform.localPosition = position;
    }

    //calculates coordinate positions on the grid in local world space
    public Vector2 CalculateLocalPosOnGrid(InventoryItem item, int posX, int posY)
    {
        Vector2 position = new Vector2();
        position.x = posX * tileSizeWidth + tileSizeWidth * item.WIDTH / 2;
        position.y = -(posY * tileSizeHeight + tileSizeHeight * item.HEIGHT / 2);
        return position;
    }

    //checks if an item in the inventory is going to overlap with another item
    private bool OverlapCheck(int posX, int posY, int width, int height, ref InventoryItem overlapItem)
    {
        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                
                if(inventoryItemSlots[posX+x, posY+y] != null)
                {
                    if (overlapItem == null)
                    {
                        overlapItem = inventoryItemSlots[posX + x, posY + y];
                    }
                    else
                    {
                        if(overlapItem != inventoryItemSlots[posX + x, posY + y])
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    public bool OverlapCheck(int posX, int posY, int width, int height)
    {
        InventoryItem overlapItem = null;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inventoryItemSlots[posX + x, posY + y] != null)
                {
                    if (overlapItem == null)
                    {
                        overlapItem = inventoryItemSlots[posX + x, posY + y];
                    }
                    else
                    {
                        if (overlapItem != inventoryItemSlots[posX + x, posY + y])
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    //checks a part of the grid for available space
    protected bool CheckAvailableSpace(int posX, int posY, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inventoryItemSlots[posX + x, posY + y] != null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    //checks if posX and PosY are inside an inventory
    public bool PositionCheck(int posX, int posY)
    {
        if(posX < 0 || posY < 0)
        {
            return false;
        }

        if(posX >= currentGridWidth || posY >= currentGridHeight)
        {
            return false;
        }

        return true;
    }

    //returns true if the area between two points is within an inventory
    public bool BoundaryCheck(int posX, int posY, int width, int height)
    {
        if(PositionCheck(posX, posY) == false) { return false; }

        //Debug.Log("Start: " + posX + ", " + posY);
        posX += width-1;
        posY += height-1;
        //Debug.Log("End: " + posX + ", " + posY);

        if (PositionCheck(posX, posY) == false) { return false; }

        return true;
    }

    public bool BoundaryCheck(int posX, int posY, int width, int height , out int oobAmountX, out int oobAmountY)
    {
        oobAmountX = 0;
        oobAmountY = 0;
        if (PositionCheck(posX, posY) == false) { return false; }

        //Debug.Log("Start: " + posX + ", " + posY);
        posX += width - 1;
        posY += height - 1;
        //Debug.Log("End: " + posX + ", " + posY);

        if (PositionCheck(posX, posY) == false) 
        {
            if(posX >= currentGridWidth)
                oobAmountX = -(posX - (currentGridWidth - 1)); //negated to only have to use addition later
            if(posY >= currentGridHeight)
                oobAmountY = -(posY - (currentGridHeight - 1)); //negated to only have to use addition later
            return false;
        }

        return true;
    }

    public void ToggleCollider()
    {
        invCollider.enabled = !invCollider.enabled;
    }
}
