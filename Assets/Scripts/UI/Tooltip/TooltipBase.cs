using System.Text;
using UnityEngine;

public abstract class TooltipBase : MonoBehaviour
{
    [SerializeField] protected RectTransform toolTipTransform;
    //move this to item and merchant item tooltips
    protected InventoryItem displayedItem;
    protected StringBuilder stringBuilder = new StringBuilder();

    protected virtual void Awake()
    {
        gameObject.SetActive(false);
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        UpdatePosition();
    }

    public abstract void ShowTooltip();

    protected abstract void SetText();

    public virtual void HideTooltip()
    {
        if (this != null)
            gameObject.SetActive(false);
    }

    //TODO clamp position within camera
    protected void UpdatePosition()
    {
        
        Vector3 tooltipPos = new Vector3();
        Vector3 mousePos = Input.mousePosition;
        float tooltipWidth = toolTipTransform.rect.width;
        float tooltipHeight = toolTipTransform.rect.height;
        tooltipPos.x = Mathf.Clamp(mousePos.x, 0, Screen.width - tooltipWidth);
        tooltipPos.y = Mathf.Clamp(mousePos.y, 0 + tooltipHeight, Screen.height);
        tooltipPos = Camera.main.ScreenToWorldPoint(tooltipPos);
        tooltipPos.z = 0;
        toolTipTransform.position = tooltipPos;
    }
}
