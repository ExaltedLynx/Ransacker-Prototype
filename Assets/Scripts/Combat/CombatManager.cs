using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;

    public GameEvent onCombatStart;
    public GameEvent onCombatEnd;
    public GameEvent onHeroAttackEnd;
    public bool inCombat = false;

    public static CombatManager Instance
    {
        get => instance;
    }
    private static CombatManager instance;

    void Start()
    {

    }

    private void Awake()
    {
        instance = this;
    }


    public void HandleCombatStart()
    {
        //CombatUIManager.Instance.toggleActive();
        //Hero.Instance.GetAttackTimer().toggleTimer();
        onCombatStart.TriggerEvent();
        inCombat = true;
        if(GameManager.Instance.battleTutorialEnabled)
            StartCoroutine(EnableHeroTimerAfterTutorial());
    }

    public void HandleCombatEnd()
    {
        //CombatUIManager.Instance.toggleActive();
        //Hero.Instance.GetAttackTimer().toggleTimer();
        //LootInventory.Instance.ChangeVisibility(true);
        onCombatEnd.TriggerEvent();
        inCombat = false;
    }

    public void onHeroAttackEndEvent()
    {
        onHeroAttackEnd.TriggerEvent();
    }

    public void toggleActive()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    private IEnumerator EnableHeroTimerAfterTutorial()
    {
        Hero.Instance.GetAttackTimer().toggleTimer(false);
        yield return new WaitUntil(() => GameManager.Instance.battleTutorialEnabled == false);
        Hero.Instance.GetAttackTimer().toggleTimer(true);
    }
}
