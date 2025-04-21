using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LootHandler;
using static Enums;

public abstract class Enemy : MonoBehaviour
{
    protected Health enemyHealth;
    protected AttackTimer attackTimer;
    protected StatusPanel statusPanel;

    [SerializeField] private string enemyName; 

    public EnemyState State { 
        get => _state; 
        private set 
        {
            _state = value;
            HandleNewState(_state);
        }
    }
    [SerializeField] private EnemyState _state;

    [SerializeField] private EnemyAnimHandler animator;
    [SerializeField] private Transform healthBarWorldPos;
    [SerializeField] private Transform attackTimerWorldPos;
    [SerializeField] private Transform statusPanelPos;
    //[SerializeField] protected int maxDrops = 1;
    [SerializeField] protected int damage;
    [SerializeField] public List<GameObject> partsObjects = new List<GameObject>();
    [SerializeField][Range(0, 1)] protected float weaponDropChance;
    [SerializeField][Range(0, 1)] protected float equipDropChance;
    [SerializeField][Range(0, 1)] protected float itemDropChance;
    [SerializeField] protected WeaponLootTable weaponLootTable = new();
    [SerializeField] protected EquipmentLootTable equipmentLootTable = new();
    [SerializeField] protected ItemLootTable itemLootTable = new();
    //public bool isStunned { get; set; }

    public Enemy LeftEnemy { get; set; } = null;
    public Enemy RightEnemy { get; set; } = null;
    public EnemyPartHandler targetedPart { get; protected set; }
    public List<TickingEffect> statuses { get; private set; }

    public enum EnemyState
    {
        Spawning,
        Idle,
        Attacking,
        Dying
    }
    
    protected virtual void Awake()
    {
        attackTimer = GetComponent<AttackTimer>();
        CombatUIManager.Instance.CreateAttackTimerBar(this, attackTimerWorldPos.position);

        enemyHealth = gameObject.GetComponent<Health>();
        CombatUIManager.Instance.CreateEnemyHealthBar(this, healthBarWorldPos.position);
        enemyHealth.InitHealthBar();

        CombatUIManager.Instance.CreateEnemyStatusPanel(this, statusPanelPos.position, ref statusPanel);
        statuses = new();

        State = EnemyState.Spawning;
        //Debug.Log(animator.enabled);
        if (!animator.enabled)
            State = EnemyState.Idle;

        enemyHealth.onDeathEvent.AddListener(DestroyEnemy);
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {

    }

    private void FixedUpdate()
    {
        if(statuses.Count == 0)
            return;

        for(int i = 0; i < statuses.Count; i++)
        {
            if (statuses[i] == null)
                statuses.RemoveAt(i);

            bool isFinished = statuses[i].Tick(Time.fixedDeltaTime);
            if (isFinished)
            {
                statuses.RemoveAt(i);
                statusPanel.RemoveStatusFromPanel(i);
            }
        }
    }

    public virtual void Attack()
    {
        State = EnemyState.Attacking;
    }

    protected virtual void DestroyEnemy()
    {
        DropLoot();
        //HandleEnemyDeath();
        CombatUIManager.Instance.DestroyEnemyUI(this);
        CombatUIManager.Instance.HideTargetIndicator();
        if (Hero.Instance.Target == this)
        {
            Hero.Instance.SetTargetEnemy(null);
        }

        if (statusPanel.gameObject != null)
            Destroy(statusPanel.gameObject);

        statuses.Clear();

        State = EnemyState.Dying;
    }

    private void DropLoot()
    {
        
        ItemDataBase weaponDrop = weaponLootTable.TryGetDrop(weaponDropChance);
        if (weaponDrop != null)
        {
            InventoryController.Instance.InsertLoot(weaponDrop);
        }

        ItemDataBase equipDrop = equipmentLootTable.TryGetDrop(equipDropChance);
        if (equipDrop != null)
        {
            InventoryController.Instance.InsertLoot(equipDrop);
        }

        ItemDataBase itemDrop = itemLootTable.TryGetDrop(itemDropChance);
        if (itemDrop != null)
        {
            InventoryController.Instance.InsertLoot(itemDrop);
        }
    }

    public void HandleEnemyDeath()
    {
        EnemyController.Instance.Enemies.Remove(this);
        Destroy(gameObject);
        if (EnemyController.Instance.Enemies.Count == 0)
        {
            Debug.Log("ending combat");
            CombatManager.Instance.HandleCombatEnd();
        }
    }

    public void HandleMouseDownOnPart(EnemyPartHandler enemyPart)
    {
        Hero.Instance.SetTargetEnemy(this);
        targetedPart = enemyPart;
        CombatUIManager.Instance.MoveTargetIndicator(enemyPart.transform.position);
    }

    public virtual void Damage(int amount, DamageType damageType)
    {
        enemyHealth.subtract(amount);
        if(targetedPart != null && targetedPart.Weakpoint != null)
        {
            targetedPart.Weakpoint.AddDamage(amount, damageType);
            targetedPart.Weakpoint.CheckWeakpointBreak(this);
        }
    }

    private void HandleNewState(EnemyState state)
    {
        switch(state)
        {
            case EnemyState.Spawning:

            break;

            case EnemyState.Idle:
                if(!GameManager.Instance.battleTutorialEnabled)
                    attackTimer.toggleTimer(true);
            break;

            case EnemyState.Attacking:

                if (animator.enabled)
                {
                    GetAttackTimer().toggleTimer(false); //off
                    animator.HandleNewState(state);
                }
                else
                    State = EnemyState.Idle;
            break;

            case EnemyState.Dying:
                GetAttackTimer().toggleTimer(false); //off
                if (animator.enabled)
                    animator.HandleNewState(state);
                else
                    HandleEnemyDeath();

            break;
        }
    }

    public void ForceKillEnemy()
    {
        enemyHealth.setCurrentHealth(0);
    }

    public void SetIdle()
    {
        State = EnemyState.Idle;
    }
    public Health GetHealth() => enemyHealth;
    public AttackTimer GetAttackTimer() => attackTimer;

    public void ApplyStatusEffect(TickingEffect status)
    {
        //Debug.Log(status);
        statusPanel.AddStatusToPanel(status);
        statuses.Add(status);

    }

    public StatusPanel GetStatusPanel() => statusPanel;
    public string GetName() => name;
}
