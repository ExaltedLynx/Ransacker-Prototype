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
        //inCombat = false;
        instance = this;
    }

    void Update()
    {
        //COMBAT CYCLE
        /* 
         * 1. Attack Countdown Phase
         * 2. Damage Calc Phase
         * 3. Game State Check Phase (logic for handling game over conditions and battle victory)
         *      3.5 (If battle was won) Loot Phase
         * 4. Hotbar use logic Phase
        */
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
