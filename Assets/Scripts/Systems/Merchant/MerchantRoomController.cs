using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MerchantRoomController : MonoBehaviour
{
    [SerializeField] GameObject merchantItemPrefab;
    [SerializeField] Image shopRenderer;
    [SerializeField] Button leaveShopButton;

    public MerchantItem stockedItem1 { get; set; }
    public MerchantItem stockedItem2 { get; set; }
    public MerchantItem stockedItem3 { get; set; }
    public bool shopIsOpen { get; private set; } = false;
    public static MerchantRoomController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {

    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.shopTutorialEnabled)
            return;

        if (shopIsOpen && Input.GetMouseButtonDown(0) && InventoryController.Instance.selectedItem != null)
        {
            GameManager.Instance.AddPlayerMoney(InventoryController.Instance.selectedItem.moneyValue);
            InventoryController.Instance.RemoveHeldItem();
        }
    }

    public void EnableMerchantRoom(List<ItemDataBase> containedItems)
    {
        for(int i = 0; i < containedItems.Count; i++)
        {
            switch(i)
            {
                case 0:
                    stockedItem1 = Instantiate(merchantItemPrefab, transform, false).GetComponent<MerchantItem>();
                    stockedItem1.transform.localPosition = new Vector3(200, 0, 0);
                    stockedItem1.SetItem(Items.InstantiateItem(containedItems[i], stockedItem1.transform));
                    stockedItem1.AddBarterCriteria();
                    break;
                case 1:
                    stockedItem2 = Instantiate(merchantItemPrefab, transform, false).GetComponent<MerchantItem>();
                    stockedItem2.transform.localPosition = new Vector3(400, 0, 0);
                    stockedItem2.SetItem(Items.InstantiateItem(containedItems[i], stockedItem2.transform));
                    stockedItem2.AddBarterCriteria();
                    break;
                case 2:
                    stockedItem3 = Instantiate(merchantItemPrefab, transform, false).GetComponent<MerchantItem>();
                    stockedItem3.transform.localPosition = new Vector3(600, 0, 0);
                    stockedItem3.SetItem(Items.InstantiateItem(containedItems[i], stockedItem3.transform));
                    stockedItem3.AddBarterCriteria();
                    break;
            }
        }
        shopRenderer.enabled = true;
        shopRenderer.raycastTarget = true;
        leaveShopButton.gameObject.SetActive(true);
        shopIsOpen = true;
    }

    //used in editor by LeaveShopButton
    public void DisableMerchantRoom()
    {
        Destroy(stockedItem1.gameObject);
        Destroy(stockedItem2.gameObject);
        Destroy(stockedItem3.gameObject);
        shopRenderer.enabled = false;
        shopRenderer.raycastTarget = false;
        leaveShopButton.gameObject.SetActive(false);
        shopIsOpen = false;
    }
}
