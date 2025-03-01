using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreenController : MonoBehaviour
{
    [SerializeField] private GameObject background;
    [SerializeField] private MenuButtonsController menuButtons;

    private void Awake()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            menuButtons.HideButton(1);

        GameManager.Instance.pauseScreen = this;
    }

    public void ToggleVisibility()
    {
        //string debugStr = "Screen Not Null";
        background.SetActive(!background.activeSelf);
        menuButtons.gameObject.SetActive(!menuButtons.gameObject.activeSelf);
        //debugStr += "\n Background Active: " + background.activeSelf;
        //debugStr += "\n MenuButtons Active: " + menuButtons.gameObject.activeSelf;
        //DebugMsgTextBox.Instance.SetDebugMessage(debugStr);
    }
}
