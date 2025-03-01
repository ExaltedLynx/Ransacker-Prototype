using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] MenuButtonsController menuButtons;

    void Start()
    {
        //Hides quit game button in webgl builds
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            menuButtons.HideButton(2);
            menuButtons.HideButton(3);
        }

    }
}
