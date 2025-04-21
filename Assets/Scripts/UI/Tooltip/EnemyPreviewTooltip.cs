using TMPro;
using UnityEngine;

public class EnemyPreviewTooltip : TooltipBase
{
    [SerializeField] private TextMeshProUGUI enemyName;
    [SerializeField] private TextMeshProUGUI enemyParts;
    public Enemy DisplayedEnemy { get; set; }
    private float padding = 10f;
    public static EnemyPreviewTooltip Instance { get; private set; }

    protected override void Awake()
    {
        Instance = this;
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void ShowTooltip()
    {
        gameObject.SetActive(true);
        SetText();
        enemyName.ForceMeshUpdate();
        enemyParts.ForceMeshUpdate();
        float tooltipWidth = Mathf.Max(enemyName.renderedWidth, enemyParts.renderedWidth);
        float tooltipHeight = enemyName.renderedHeight + enemyParts.renderedHeight;
        toolTipTransform.sizeDelta = new Vector2(tooltipWidth + padding, tooltipHeight + padding);
        UpdatePosition();
    }

    protected override void SetText()
    {
        enemyName.SetText(DisplayedEnemy.GetName());
        stringBuilder.AppendLine("Targetable parts: ");
        foreach(GameObject partsObj in DisplayedEnemy.partsObjects)
            stringBuilder.AppendLine(partsObj.name);

        enemyParts.SetText(stringBuilder.ToString());
        stringBuilder.Clear();
    }
}
