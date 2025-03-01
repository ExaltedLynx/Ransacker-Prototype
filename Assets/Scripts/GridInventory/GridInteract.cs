using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(InventoryGrid))]
public class GridInteract : MonoBehaviour
{
    InventoryController inventoryController;
    InventoryGrid inventory;

    void Awake()
    {
        inventoryController = FindObjectOfType(typeof(InventoryController)) as InventoryController;
        inventory = GetComponent<InventoryGrid>();
    }

    private void OnMouseEnter()
    {
        inventoryController.SelectedInventory = inventory;
    }    

    private void OnMouseExit()
    {
        inventoryController.SelectedInventory = null;
    }

}
