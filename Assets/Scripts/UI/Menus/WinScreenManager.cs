using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject background;
    [SerializeField] private TextMeshProUGUI goldObtainedText;
    [SerializeField] private MenuButtonsController menuButtons;

    private int currentGoldObtainedValue = 0;
    private int incrementAmount = 1;

    public static WinScreenManager Instance;

    private void Awake()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            menuButtons.HideButton(1);
        Instance = this;
    }

    public void EnableWinScreen()
    {
        GameManager.Instance.TogglePauseGame();
        goldObtainedText.SetText("0");
        background.SetActive(true);
        menuButtons.gameObject.SetActive(true);
        StartCoroutine(StartRollingGoldTotalText());
    }

    private IEnumerator StartRollingGoldTotalText()
    {
        while (currentGoldObtainedValue < GameManager.Instance.PlayerMoney)
        {
            currentGoldObtainedValue += incrementAmount;
            yield return null;
            goldObtainedText.SetText(currentGoldObtainedValue.ToString("N0"));
            incrementAmount += 1;
        }
        currentGoldObtainedValue = GameManager.Instance.PlayerMoney;
        goldObtainedText.SetText(currentGoldObtainedValue.ToString("N0"));

    }

}
