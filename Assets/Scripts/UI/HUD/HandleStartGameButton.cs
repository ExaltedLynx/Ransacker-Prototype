using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleStartGameButton : MonoBehaviour
{
    public static Button button;
    
    void Start()
    {
        button = GetComponent<Button>();
        button.interactable = false;
    }  
}
