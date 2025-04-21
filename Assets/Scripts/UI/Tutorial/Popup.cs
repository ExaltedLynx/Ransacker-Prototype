using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private RectTransform textBoxTransform;
    [SerializeField] private BoxCollider2D popupCollider;
    public IEnumerator<TutorialText> tutorialMsgEnumerator { get; set; }
    public TutorialType tutorialType { get; set; }
    public int tutorialCount { get; private set; } = 0;

    private void OnEnable()
    {  
        tutorialCount++;
        tutorialMsgEnumerator.MoveNext();
        RectTransform textPos = tutorialMsgEnumerator.Current.Position;
        string text = tutorialMsgEnumerator.Current.Text;
        UpdateTutorialText(textPos.position, text);
    }

    private void OnMouseDown()
    {

        if (!GameManager.Instance.gameIsPaused && Input.GetMouseButtonDown(0))
        {
            if(tutorialMsgEnumerator.MoveNext())
            {
                RectTransform textPos = tutorialMsgEnumerator.Current.Position;
                string text = tutorialMsgEnumerator.Current.Text;
                UpdateTutorialText(textPos.position, text);
            }
            else
            {
                switch(tutorialType)
                {
                    case TutorialType.PathSelection:
                        GameManager.Instance.pathSelectionTutEnabled = false;
                        break;
                    case TutorialType.Inventory:
                        GameManager.Instance.inventoryTutorialEnabled = false;
                        break;
                    case TutorialType.Battle:
                        GameManager.Instance.battleTutorialEnabled = false;
                        break ;
                    case TutorialType.Chest:
                        GameManager.Instance.chestTutorialEnabled = false;
                        break;
                    case TutorialType.Loot:
                        GameManager.Instance.lootTutorialEnabled = false;
                        break;
                    case TutorialType.Shop:
                        GameManager.Instance.shopTutorialEnabled = false;
                        break;
                    case TutorialType.Sword:
                        GameManager.Instance.SwordTutorialEnabled = false;
                        break;
                    case TutorialType.Shield:
                        GameManager.Instance.ShieldTutorialEnabled = false;
                        break;
                    case TutorialType.Dagger:
                        GameManager.Instance.DaggerTutorialEnabled = false;
                        break;
                    case TutorialType.Axe:
                        GameManager.Instance.AxeTutorialEnabled = false;
                        break;
                    case TutorialType.Hammer:
                        GameManager.Instance.HammerTutorialEnabled = false;
                        break;
                }
                /*
                if(SceneController.activeScene.buildIndex == 2)
                {
                    PlayerInventory.Instance.ToggleCollider(true);
                    HotbarInventory.Instance.ToggleCollider(true);
                }
                */
                gameObject.SetActive(false);
            }

        }
    }

    public void UpdateTutorialText(Vector2 newTextPos, string newText)
    {
        tutorialText.SetText(newText);
        tutorialText.ForceMeshUpdate();
        textBoxTransform.position = newTextPos;
        Vector2 popupBoxSize = new Vector2(tutorialText.renderedWidth + 10, tutorialText.renderedHeight + 10);
        textBoxTransform.sizeDelta = popupBoxSize;
        popupCollider.size = popupBoxSize;
        popupCollider.offset = new Vector2(popupBoxSize.x / 2, -popupBoxSize.y / 2);
    }
}

public enum TutorialType
{
    PathSelection,
    Inventory,
    Battle,
    Chest,
    Loot,
    Shop,
    Sword,
    Shield,
    Dagger,
    Axe,
    Hammer
}

public struct TutorialText
{
    public string Text { get; init; }
    public RectTransform Position { get; init; }
}
