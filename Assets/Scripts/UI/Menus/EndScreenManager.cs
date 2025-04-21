using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject background;
    [SerializeField] private MenuButtonsController menuButtons;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            menuButtons.HideButton(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableGameOverScreen()
    {
        Debug.Log("Test");
        GameManager.Instance.TogglePauseGame();
        background.SetActive(true);
        menuButtons.gameObject.SetActive(true);
    }
}
