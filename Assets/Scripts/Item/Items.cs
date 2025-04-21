using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Items : MonoBehaviour
{
    private static List<ItemDataBase> items;
    private static GameObject itemPrefab;

    public static void LoadItems()
    {
        items = Resources.LoadAll<ItemDataBase>("Items").ToList();
        itemPrefab = Resources.Load<GameObject>("Prefabs/Item");
    }

    //Returns a random item that is of type T and matches the given predicate
    //first filters the list to all items that match the predicate, then returns an item from that list at random
    public static T FindItem<T>(Predicate<T> predicate) where T : ItemDataBase
    {
        List<ItemDataBase> t_items = items.FindAll(item => item is T T_item && predicate(T_item));
        foreach (ItemDataBase item in t_items)
        {
            //Debug.Log(item);
        }
        int randIndex = UnityEngine.Random.Range(0, t_items.Count);
        return t_items[randIndex] as T;
    }

    //returns an random item of type T
    public static T FindItem<T>() where T : ItemDataBase
    {
        List<ItemDataBase> t_items = items.FindAll(item => item is T T_item && T_item.itemIcon != null);
        int randIndex = UnityEngine.Random.Range(0, t_items.Count);
        return t_items[randIndex] as T;
    }

    //Returns all items that match the given predicate and are of type T
    public static List<T> FindItems<T>(Predicate<T> predicate) where T : ItemDataBase
    {
        return items.FindAll(item => item is T T_item && predicate(T_item)).Cast<T>().ToList();
    }

    //Creates a game object in the scene with the given item data
    public static InventoryItem InstantiateItem(ItemDataBase itemData)
    {
        Transform canvasTransform = GameObject.FindWithTag("Canvas").GetComponent<Transform>();
        InventoryItem item = Instantiate(itemPrefab, canvasTransform).GetComponent<InventoryItem>();
        item.transform.localPosition = Vector3.zero;
        item.GetComponent<RectTransform>().localScale = new Vector3(.5f, .5f, .5f);
        item.Set(itemData);
        //item.InitItemStats();
        return item;
    }

    //Creates a game object in the scene with the given item data
    public static InventoryItem InstantiateItem(ItemDataBase itemData, Transform parent)
    {
        InventoryItem item = Instantiate(itemPrefab, parent).GetComponent<InventoryItem>();
        item.transform.localPosition = Vector3.zero;
        item.GetComponent<RectTransform>().localScale = new Vector3(.5f, .5f, .5f);
        item.Set(itemData);
        item.InitItemStats();
        return item;
    }

    public static InventoryItem InstantiateCachedItem(CachedItemData cachedItem, Transform parent)
    {
        InventoryItem item = Instantiate(itemPrefab, parent).GetComponent<InventoryItem>();
        item.transform.localPosition = Vector3.zero;
        item.GetComponent<RectTransform>().localScale = new Vector3(.5f, .5f, .5f);
        item.Set(cachedItem.itemData);
        item.InitItemStats(cachedItem.stats);
        if(cachedItem.isRotated)
            item.Rotate();

        return item;
    }
}

