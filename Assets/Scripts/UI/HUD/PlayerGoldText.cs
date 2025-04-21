using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerGoldText : MonoBehaviour
{
    public static PlayerGoldText Instance;
    [SerializeField] TextMeshProUGUI playerGoldText;

    void Start()
    {
        Instance = this;
        playerGoldText.SetText(GameManager.Instance.PlayerMoney.ToString("N0"));
    }

    public void UpdateText()
    {
        playerGoldText.SetText(GameManager.Instance.PlayerMoney.ToString("N0"));
    }
}
