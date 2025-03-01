using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DungeonFloorText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI floorNumText;
    public static DungeonFloorText Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateText(int dungeonNum, int floorNum)
    {
        floorNumText.SetText(dungeonNum + "-" + floorNum);
    }
}
