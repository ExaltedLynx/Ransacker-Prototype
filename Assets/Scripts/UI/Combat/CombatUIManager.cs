using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIManager : MonoBehaviour
{
    [SerializeField] GameObject attackTimerPrefab;
    [SerializeField] GameObject HPBarPrefab;
    [SerializeField] GameObject StatusPanelPrefab;
    [SerializeField] GameObject IndicatorObject;

    private int HPBarCount = 0;
    private int attackTimerCount = 0;
    private int statusPanelCount = 0;

    [SerializeField] public List<GameObject> enemyUI;
    private Image enemyIndicator;
    
    public static CombatUIManager Instance
    {
        get => instance;
    }
    private static CombatUIManager instance;

    // Start is called before the first frame update
    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        enemyIndicator = IndicatorObject.GetComponent<Image>();
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        resetCombatUI();
    }

    private void OnEnable()
    {
        
    }

    public void CreateAttackTimerBar(Enemy owner, Vector3 pos)
    {
        Slider attackTimerBar = Instantiate(attackTimerPrefab, transform, false).GetComponent<Slider>();
        enemyUI.Add(attackTimerBar.gameObject);
        attackTimerBar.name = "Enemy" + (1 + attackTimerCount++) + " AttackTimer";
        RectTransform sliderTransform = attackTimerBar.GetComponent<RectTransform>();
        //pos.y -= .75f;
        sliderTransform.position = pos;
        sliderTransform.localScale = new Vector3(.3f, .3f, .3f);

        AttackTimerController atc = attackTimerBar.GetComponent<AttackTimerController>();
        atc.setTimerInstance(owner.GetAttackTimer());
        owner.GetAttackTimer().attackTimerController = atc;

    }

    public void CreateAttackTimerBar(Hero owner, Vector3 pos)
    {
        
        Slider attackTimerBar = Instantiate(attackTimerPrefab, transform, false).GetComponent<Slider>();
        attackTimerBar.name = "HeroAttackTimer";
        RectTransform sliderTransform = attackTimerBar.GetComponent<RectTransform>();
        pos.y -= .75f;
        sliderTransform.position = pos;
        sliderTransform.localScale = new Vector3(.55f, .55f, .55f);

        AttackTimerController atc = attackTimerBar.GetComponent<AttackTimerController>();
        atc.setTimerInstance(owner.GetAttackTimer());
        owner.GetAttackTimer().attackTimerController = atc;
    }

    public void CreateEnemyHealthBar(Enemy owner, Vector3 pos)
    {
        Slider HealthBar = Instantiate(HPBarPrefab, transform, false).GetComponent<Slider>();
        enemyUI.Add(HealthBar.gameObject);
        HealthBar.name = "Enemy" + (1 + HPBarCount++) + " HealthBar";
        RectTransform HPTransform = (RectTransform)HealthBar.transform;
        //pos.y += .75f;
        HPTransform.position = pos;
        HPTransform.localScale = new Vector3(.5f, .5f, .5f);
        owner.GetHealth().healthBar = HealthBar.GetComponent<HealthBar>();
    }

    public void CreateEnemyStatusPanel(Enemy owner, Vector3 pos, ref StatusPanel panel)
    {
        StatusPanel statusPanel = Instantiate(StatusPanelPrefab, transform, false).GetComponent<StatusPanel>();
        enemyUI.Add(statusPanel.gameObject);
        statusPanel.name = "Enemy" + (1 + statusPanelCount++) + " StatusPanel";
        RectTransform panelTransform = (RectTransform)statusPanel.transform;
        panelTransform.position = pos;
        //panelTransform.localScale = new Vector3(.5f, .5f, .5f);
        panel = statusPanel;
    }

    public void MoveTargetIndicator(Vector2 pos)
    {
        enemyIndicator.enabled = true;
        RectTransform indicatorTransform = (RectTransform)enemyIndicator.transform;
        indicatorTransform.position = pos;
        indicatorTransform.localScale = new Vector3(.5f, .5f, .5f);
    }

    public void HideTargetIndicator()
    {
        enemyIndicator.enabled = false;
    }

    public void DestroyEnemyUI(Enemy enemy)
    {
        GameObject toRemove1 = enemy.GetAttackTimer().attackTimerController.gameObject;
        GameObject toRemove2 = enemy.GetHealth().healthBar.gameObject;
        GameObject toRemove3 = enemy.GetStatusPanel().gameObject;
        enemyUI.Remove(toRemove1);
        enemyUI.Remove(toRemove2);
        enemyUI.Remove(toRemove3);
        Destroy(toRemove1);
        Destroy(toRemove2);
        Destroy(toRemove3);
    }

    public void toggleActive()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    private void resetCombatUI()
    {
        if(enemyUI.Count != 0)
        {
            foreach (GameObject ui in enemyUI)
            {
                Destroy(ui);
            }

            enemyUI.Clear();
            HPBarCount = 0;
            attackTimerCount = 0;
        }
    }
}
