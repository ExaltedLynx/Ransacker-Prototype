using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(InventoryGrid))]
public class GridInteract : MonoBehaviour
{
    [SerializeField] InventoryController inventoryController;
    InventoryGrid inventory;

    void Awake()
    {
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
