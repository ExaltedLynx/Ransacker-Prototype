using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryHighlight : MonoBehaviour
{
    [SerializeField] RectTransform highlighter;
    [SerializeField] Image highlighterImage;
    public Color highlightGreen { get; private set; }

    private void Awake()
    {
        //Color temp = Color.HSVToRGB(0.347f, 0.85f, 1f);
        Color temp = Color.green;
        temp.a = 0.5f;
        highlightGreen = temp;
        highlighter.localScale = new Vector3(.5f, .5f, .5f);
    }
    public void Show(bool b)
    {
        highlighter.gameObject.SetActive(b);
    }

    //changes size of the highlight tiles based on the size of targetItem
    public void SetSize(InventoryItem targetItem)
    {
        Vector2 size = new Vector2();
        size.x = targetItem.WIDTH * InventoryGrid.tileSizeWidthConst;
        size.y = targetItem.HEIGHT * InventoryGrid.tileSizeHeightConst;
        highlighter.sizeDelta = size;
    }

    //sets the position of the highlighted tiles relative the grid 
    public void SetPosition(InventoryGrid targetGrid, InventoryItem targetItem)
    {
        SetPosition(targetGrid, targetItem, targetItem.gridPositionX, targetItem.gridPositionY);
    }

    //sets the position of the highlighted tiles relative the grid
    public void SetPosition(InventoryGrid targetGrid, InventoryItem targetItem, int posX, int posY)
    {
        Vector2 pos = targetGrid.CalculateLocalPosOnGrid(targetItem, posX, posY);
        highlighter.localPosition = pos;
    }

    public void SetParentInventory(InventoryGrid targetGrid)
    {
        highlighter.SetParent(targetGrid.transform);
        highlighter.SetAsFirstSibling();
    }

    public void SetColor(Color color)
    {
        highlighterImage.color = color;
    }
}
