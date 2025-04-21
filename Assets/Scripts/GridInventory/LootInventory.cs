using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootInventory : InventoryGrid
{
    public static LootInventory Instance { get; private set; }
    [SerializeField] private GridInteract gridInteract;
    [SerializeField] private Button closeLootInvButton;
    public bool isVisible = false;
    public bool isEmpty = true;
    private Image image;
    private int occupiedWidth = 0;
    private int occupiedHeight = 0;

    void Awake()
    {
        Instance = this;
        image = GetComponent<Image>();
        image.enabled = false;
        invCollider.enabled = false;
    }

    private void Update()
    {
        if(isVisible && Input.GetKeyDown(KeyCode.Escape) && !GameManager.Instance.lootTutorialEnabled)
        {
            ChangeVisibility(false);
            if(ItemTooltip.Instance.isActiveAndEnabled)
                ItemTooltip.Instance.HideTooltip();
        }
    }

    public void ChangeVisibility(bool isVisible)
    {
        if(isEmpty)
            return;

        this.isVisible = isVisible;
        if(isVisible)
        {
            image.enabled = true;
            closeLootInvButton.gameObject.SetActive(true);
            if (GameManager.Instance.lootTutorialEnabled)
            {
                TutorialController.Instance.EnableLootInvTutorial();
                StartCoroutine(EnableGridAfterTutorial());
            }
            else
            {
                closeLootInvButton.interactable = true;
                invCollider.enabled = true;
            }
        }    
        else
        {
            HandleDisableLootInv();
        }
    }

    public bool TryResizeLootGrid(InventoryItem item)
    {
        Debug.Log("Loot Inv Size: " + currentGridWidth + ", " + currentGridHeight);
        if (currentGridWidth == MAX_INVENTORY_WIDTH && currentGridHeight == MAX_INVENTORY_HEIGHT)
            return false;

        int potentialWidth = occupiedWidth + item.WIDTH;
        int potentialHeight = occupiedHeight + item.HEIGHT;

        if (potentialWidth > currentGridWidth)
        {
            ResizeGrid(Mathf.Min(potentialWidth, MAX_INVENTORY_WIDTH), currentGridHeight);
        }
        
        if(potentialHeight > currentGridHeight)
        {
            ResizeGrid(currentGridWidth, Mathf.Min(potentialHeight, MAX_INVENTORY_HEIGHT));
        }
        return true;
    }

    public override void InsertItem(InventoryItem item, int posX, int posY)
    {
        if (posX > occupiedWidth || occupiedWidth == 0)
            occupiedWidth += item.WIDTH;

        if (posY > occupiedHeight || occupiedHeight == 0)
            occupiedHeight += item.HEIGHT;

        isEmpty = false;
        base.InsertItem(item, posX, posY);
    }

    private void HandleDisableLootInv()
    {
        image.enabled = false;
        gridInteract.enabled = false;
        invCollider.enabled = false;
        isEmpty = true;
        closeLootInvButton.gameObject.SetActive(false);
        ClearAllItems();
        ResizeGrid(initialGridWidth, initialGridHeight);
        occupiedHeight = 0;
        occupiedWidth = 0;
    }

    private IEnumerator EnableGridAfterTutorial()
    {
        closeLootInvButton.interactable = false;
        yield return new WaitUntil(() => GameManager.Instance.lootTutorialEnabled == false);
        invCollider.enabled = true;
        closeLootInvButton.interactable = true;
    }

}
