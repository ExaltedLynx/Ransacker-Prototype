using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugMsgTextBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI debugMsgTextBox;
    public static DebugMsgTextBox Instance { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    public void SetDebugMessage(string msg)
    {
        debugMsgTextBox.SetText(msg);
    }
}
