using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StatusPanel : MonoBehaviour
{
    [SerializeField] private GameObject statusIconPrefab;
    [SerializeField] private Image panelBG;
    private List<GameObject> statusIcons = new List<GameObject>();

    public void AddStatusToPanel(TickingEffect status)
    {
        GameObject statusIconObj = Instantiate(statusIconPrefab, transform);
        StatusIcon statusIcon = statusIconObj.GetComponent<StatusIcon>();
        statusIcons.Add(statusIconObj);
        statusIcon.InitStatusIcon(status);
        if (status is StackingEffect stackingEffect)
            stackingEffect.statusIcon = statusIcon;

        statusIcons[statusIcons.Count - 1].transform.localPosition += new Vector3(36 * (statusIcons.Count - 1), 0, 0);
        panelBG.enabled = true;
    }

    public void RemoveStatusFromPanel(int index)
    {
        Destroy(statusIcons[index].gameObject);
        statusIcons.RemoveAt(index);
        if (statusIcons.Count > 0)
        {
            for (int i = index; i < statusIcons.Count; i++)
            {
                statusIcons[i].transform.localPosition -= new Vector3(4 + (36 * statusIcons.Count - 1), 0, 0);
            }
        }
        else
            panelBG.enabled = false;
    }
}
