using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtonsController : MonoBehaviour
{
    [SerializeField] Button[] menuButtons;

    //used in editor
    public void QuitGame()
    {
        Application.Quit();
    }

    //used in editor
    public void ResetInventoryCache()
    {
        PersistInventoryHandler.Instance.ResetInventoryCache();
    }

    //used in editor
    public void DisableButtons()
    {
        foreach (var button in menuButtons) 
            button.interactable = false;
    }

    public void HideButton(int buttonIndex)
    {
        menuButtons[buttonIndex].gameObject.SetActive(false);
    }
}
