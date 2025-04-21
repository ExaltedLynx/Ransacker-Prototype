using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Stat;
using static EquipmentItem;
using static WeaponItem;
using System.Linq;
using UnityEngine.Jobs;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Member;

public class Hero : MonoBehaviour
{
    public static Hero Instance { get; private set; }

    [SerializeField] private int baseMaxHealth;
    [SerializeField] private float shieldCooldownDuration;
    public Stat Vitality = new Stat(1, StatType.Vitality);
    public Stat Strength = new Stat(1, StatType.Strength);
    public Stat Defense = new Stat(1, StatType.Defense);
    public Stat Dexterity = new Stat(1, StatType.Dexterity);
    public Stat Intelligence = new Stat(1, StatType.Intelligence);
    public Stat Speed = new Stat(1, StatType.Speed);
 
    public static int CachedSpeed = 1;
    public float TotalStats { get => totalStats; internal set => totalStats = value; }
    [SerializeField] private float totalStats;

    public List<StatModifier> equippedStatModifiers { get; private set; }
    public Dictionary<ConsumableItem, TickingEffect> uniqueStatuses { get; private set; }

    [SerializeField] private GameObject[] itemSlots;
    public ItemSlot<EquipmentItem> helmetSlot { get; private set; }
    public ItemSlot<EquipmentItem> chestSlot { get; private set; }
    public ItemSlot<EquipmentItem> pantsSlot { get; private set; }
    public ItemSlot<EquipmentItem> bootsSlot { get; private set; }
    public ItemSlot<WeaponItem> mainHand { get; private set; }
    public ItemSlot<WeaponItem> offHand { get; private set; }
    public Enemy Target { get; private set; }

    private Health heroHealth;
    private AttackTimer attackTimer;
    private bool isBlocking = false;
    private bool blockReleased = false;
    [SerializeField] ShieldCooldownHandler shieldCooldownHandler;
    [SerializeField] SpriteRenderer shieldDownSprite;
    [SerializeField] SpriteRenderer shieldRaisedSprite;

    private AsyncOperationHandle loadSpriteOp; 
    private Sprite stunEffectIcon;
    

    void Awake()
    {
        Instance = this;

        helmetSlot = new ItemSlot<EquipmentItem>(itemSlots[0]);
        chestSlot = new ItemSlot<EquipmentItem>(itemSlots[1]);
        pantsSlot = new ItemSlot<EquipmentItem>(itemSlots[2]);
        bootsSlot = new ItemSlot<EquipmentItem>(itemSlots[3]);
        mainHand = new ItemSlot<WeaponItem>(itemSlots[4]);
        offHand = new ItemSlot<WeaponItem>(itemSlots[5]);

        TotalStats = Vitality.Value + Strength.Value + Defense.Value + Dexterity.Value + Intelligence.Value + Speed.Value;
        equippedStatModifiers = new();
        uniqueStatuses = new();

        attackTimer = GetComponent<AttackTimer>();
        CombatUIManager.Instance.CreateAttackTimerBar(this, transform.position);

        heroHealth = GetComponent<Health>();
        heroHealth.InitHealthBar();

        Addressables.LoadAssetAsync<Sprite>("EffectIconsSheet[StunEffectIcon]").Completed += LoadSprite;

        if(DungeonDataCache.Instance.currentFloorNum == 0)
            GiveStarterWeapon();
        else
            ReloadEquippedItems();
    }

    void Update()
    {
        if(offHand.getEquippedItem() != null && !shieldCooldownHandler.onCooldown)
        {
            if (offHand.getEquippedItemData().weaponType == WeaponType.Shield && Input.GetMouseButton(1))
            {
                isBlocking = true;
                shieldDownSprite.enabled = false;
                shieldRaisedSprite.enabled = true;

            }
            if (Input.GetMouseButtonUp(1))
            {
                isBlocking = false;
                blockReleased = true;
                shieldRaisedSprite.enabled = false;
                shieldDownSprite.enabled = true;
                if (blockReleased) {
                    StopCoroutine(HandleBlockReleasedTiming());
                }
                StartCoroutine(HandleBlockReleasedTiming());
            }
        }
    }

    void FixedUpdate()
    {
        if(uniqueStatuses.Count == 0) 
            return;

        List<TickingEffect> statuses = uniqueStatuses.Values.ToList();
        foreach (TickingEffect status in statuses)
            status.Tick(Time.fixedDeltaTime);
    }

    public Health GetHealth()
    {
        return heroHealth;
    }

    public AttackTimer GetAttackTimer()
    {
        return attackTimer;
    }

    public void Attack()
    {
        if (Target != null)
        {
            WeaponItem weapon = mainHand.getEquippedItemData();
            float finalWeaponDamage = mainHand.getEquippedItemData().baseDamage + Strength.Value;
            int finalDamage;
            switch (weapon.weaponType)
            {
                case WeaponType.Sword:
                    finalDamage = (int)(finalWeaponDamage + (Strength.Value * 0.15f + (Dexterity.Value * 0.10f)));
                    Target.Damage(finalDamage, mainHand.getEquippedItemData().damageType);
                    break;

                case WeaponType.Dagger:
                    finalDamage = (int)(finalWeaponDamage + (Dexterity.Value * 0.25f));
                    Target.Damage(finalDamage, mainHand.getEquippedItemData().damageType);
                    StartCoroutine(OffHandDaggerAttack());
                    break;

                case WeaponType.Axe:
                    finalDamage = (int)(finalWeaponDamage + (Strength.Value * 0.15f + (Dexterity.Value * 0.15f)));
                    Enemy leftEnemy = Target.LeftEnemy;
                    Enemy rightEnemy = Target.RightEnemy;
                    Target.Damage(finalDamage, mainHand.getEquippedItemData().damageType);
                    if(leftEnemy != null)
                    {
                        leftEnemy.Damage((int)(finalDamage / 1.5f), mainHand.getEquippedItemData().damageType);
                    }

                    if (rightEnemy != null)
                    {
                        rightEnemy.Damage((int)(finalDamage / 1.5f), mainHand.getEquippedItemData().damageType);
                    }
                    break;

                case WeaponType.Hammer:
                    finalDamage = (int)(finalWeaponDamage + (Strength.Value * 0.25f));
                    Target.Damage(finalDamage, mainHand.getEquippedItemData().damageType);
                    break;
            }
            weapon.TryTriggerEffect();
        }
    }

    private IEnumerator OffHandDaggerAttack()
    {
        if (!offHand.isEmpty && offHand.getEquippedItemData().weaponType == WeaponType.Dagger)
        {
            float finalWeaponDamage = offHand.getEquippedItemData().baseDamage + Strength.Value;
            int finalDamage = (int)(finalWeaponDamage + (Dexterity.Value * 0.25f));

            yield return attackTimer.PauseAttackTimer(0.4f);

            if (Target != null && !offHand.isEmpty && offHand.getEquippedItemData().weaponType == WeaponType.Dagger)
            {
                Target.Damage(finalDamage, offHand.getEquippedItemData().damageType);
                offHand.getEquippedItemData().TryTriggerEffect();
            }
        }
    }

    public void Damage(int amount, Enemy source)
    {
        if (isBlocking)
        {
            shieldCooldownHandler.AddBlockFatigue(amount, offHand.getEquippedItemData().maxBlockFatigue);
            amount /= 2;
        }

        //Debug.Log(blockReleased);
        if (blockReleased && source != null)
        {
            source.ApplyStatusEffect(new StunEffect(1.5f, stunEffectIcon, source));
            return;
        }

        amount -= (int)(Defense.Value * 0.30f);
        int finalDamageTaken = Math.Clamp(amount, 1, 9999);
        heroHealth.subtract(finalDamageTaken);
    }

    public void Heal(int amount)
    {
        heroHealth.add(amount);
    }

    public void SetTargetEnemy(Enemy enemy)
    {
        Target = enemy;
    }

    public void RemoveTargetedEnemy()
    {
        Target = null;
        CombatUIManager.Instance.HideTargetIndicator();
    }

    public bool EquipItem(InventoryItem item)
    {
        bool wasItemEquipped = false;
        if (item.itemData is EquipmentItem)
        {
            EquipmentItem toEquip = (EquipmentItem)item.itemData;
            EquipmentType itemType = toEquip.equipmentType;
            switch (itemType)
            {
                case EquipmentType.Helmet:
                    wasItemEquipped = helmetSlot.setEquippedItem(item);
                    break;
                case EquipmentType.Chestplate:
                    wasItemEquipped = chestSlot.setEquippedItem(item);
                    break;
                case EquipmentType.Leggings:
                    wasItemEquipped = pantsSlot.setEquippedItem(item);
                    break;
                case EquipmentType.Boots:
                    wasItemEquipped = bootsSlot.setEquippedItem(item);
                    break;
            }
        }
        else
        {
            WeaponItem toEquip = (WeaponItem)item.itemData;
            WeaponType weaponType = toEquip.weaponType;
            switch (weaponType)
            {
                case WeaponType.Dagger:
                    if (!mainHand.isEmpty)
                    {
                        if (mainHand.getEquippedItemData().weaponType == WeaponType.Dagger)
                        {
                            if (!offHand.isEmpty) //Replace weaker dagger
                            {
                                //TODO change this when weapons actually have stats to compare with
                                wasItemEquipped = offHand.setEquippedItem(item);
                            }
                            else //equip to offhand if only a dagger in main hand
                                wasItemEquipped = offHand.setEquippedItem(item);
                        }
                        else //not a dagger in the main hand
                        {
                            if (!offHand.isEmpty)
                                UnequipItem(offHand);

                            wasItemEquipped = mainHand.setEquippedItem(item);
                        }
                    }
                    else
                        wasItemEquipped = mainHand.setEquippedItem(item);
               break;

                case WeaponType.Shield:
                    if(!mainHand.isEmpty && mainHand.getEquippedItemData().weaponType == WeaponType.Sword)
                    {
                        wasItemEquipped = offHand.setEquippedItem(item);
                        shieldCooldownHandler.ResetBlockFatigue();
                        shieldDownSprite.enabled = true;
                    }
                break;

                case WeaponType.Sword:
                    wasItemEquipped = mainHand.setEquippedItem(item);
                    if (!offHand.isEmpty && offHand.getEquippedItemData().weaponType != WeaponType.Shield)
                        UnequipItem(offHand);
                break;

                case WeaponType.Hammer:
                    wasItemEquipped = mainHand.setEquippedItem(item);
                    if (!offHand.isEmpty)
                        UnequipItem(offHand);
                break;

                case WeaponType.Axe:
                    wasItemEquipped = mainHand.setEquippedItem(item);
                    if(!offHand.isEmpty) 
                        UnequipItem(offHand);
                break;
            }
            SetAttackTimerDuration(mainHand.getEquippedItemData().attackSpeed);
        }

        if(wasItemEquipped)
        {
            equippedStatModifiers.AddRange(item.GetItemStats().statList);
            equippedStatModifiers.Sort(ItemStats.SortModifiers);
            if (item.itemData is WeaponItem weapon)
                StartCoroutine(HandleWeaponTutorial(weapon.weaponType));
        }
        UpdateStats();
        return wasItemEquipped;
    }

    public void ReloadEquippedItems()
    {
        InventoryItem cachedItem;
        for (int i = 0; i < PersistInventoryHandler.Instance.playerEquipment.Length; i++)
        {
            if(PersistInventoryHandler.Instance.playerEquipment[i].itemData != null)
            {
                cachedItem = Items.InstantiateCachedItem(PersistInventoryHandler.Instance.playerEquipment[i], itemSlots[0].transform);
                switch(i)
                {
                    case 0:
                        helmetSlot.setEquippedItem(cachedItem);
                        break;

                    case 1:
                        chestSlot.setEquippedItem(cachedItem);
                        break;

                    case 2:
                        pantsSlot.setEquippedItem(cachedItem);
                        break;

                    case 3:
                        bootsSlot.setEquippedItem(cachedItem);
                        break;

                    case 4:
                        mainHand.setEquippedItem(cachedItem);
                        break;

                    case 5:
                        offHand.setEquippedItem(cachedItem);
                        break;
                }
            }
        }
        SetAttackTimerDuration(mainHand.getEquippedItemData().attackSpeed);
        UpdateStats();
    }

    private void UnequipItem(ItemSlot itemSlot)
    {
        if(itemSlot.getEquippedItem().itemData is WeaponItem wpn && wpn.weaponType is WeaponType.Shield)
        {
            shieldDownSprite.enabled = false;
            shieldRaisedSprite.enabled = false;
            shieldCooldownHandler.HideShieldCooldown();
        }
        itemSlot.UnequipItem();
    }

    public void ApplyStatusEffect(ConsumableItem source, TickingEffect effect)
    {
        if (uniqueStatuses.ContainsKey(source))
            uniqueStatuses[source].RefreshDuration();
        else
        {
            uniqueStatuses.Add(source, effect);
            if (uniqueStatuses[source] is StatBuffEffect buffEffect)
                AddStatusStatMod(buffEffect.statModifier);
        }
    }

    public void RemoveStatusEffect(ConsumableItem source)
    {
        if (uniqueStatuses[source] is StatBuffEffect buffEffect)
            RemoveStatusStatMod(buffEffect.statModifier);

        uniqueStatuses.Remove(source);
    }

    private void UpdateStats()
    {
        equippedStatModifiers.ForEach(statMod =>
        {
            switch (statMod.statType)
            {
                case StatType.Vitality: Vitality.AddModifier(statMod);
                    break;
                case StatType.Strength: Strength.AddModifier(statMod);
                    break;
                case StatType.Defense: Defense.AddModifier(statMod);
                    break;
                case StatType.Dexterity: Dexterity.AddModifier(statMod);
                    break;
                case StatType.Intelligence: Intelligence.AddModifier(statMod);
                    break;
                case StatType.Speed: Speed.AddModifier(statMod);
                    break;
            }
        });

        Vitality.ApplyModifiers();
        Strength.ApplyModifiers();
        Defense.ApplyModifiers();
        Dexterity.ApplyModifiers();
        Intelligence.ApplyModifiers();
        Speed.ApplyModifiers();
        TotalStats = Vitality.Value + Strength.Value + Defense.Value + Dexterity.Value + Intelligence.Value + Speed.Value;

        heroHealth.setMaxHealth(baseMaxHealth + (int)Vitality.Value);
    }

    private void AddStatusStatMod(StatModifier statModifier)
    {
        switch (statModifier.statType)
        {
            case StatType.Vitality: Vitality.AddStatusModifier(statModifier);
                 Vitality.ApplyStatusModifiers();
                 heroHealth.setMaxHealth(baseMaxHealth + (int)Vitality.Value);
            break;
            case StatType.Strength: Strength.AddStatusModifier(statModifier);
                 Strength.ApplyStatusModifiers();
            break;
            case StatType.Defense: Defense.AddStatusModifier(statModifier);
                 Defense.ApplyStatusModifiers();
            break;
            case StatType.Dexterity: Dexterity.AddStatusModifier(statModifier);
                 Dexterity.ApplyStatusModifiers();
            break;
            case StatType.Intelligence: Intelligence.AddStatusModifier(statModifier);
                 Intelligence.ApplyStatusModifiers();
            break;
            case StatType.Speed: Speed.AddStatusModifier(statModifier);
                 Speed.ApplyStatusModifiers();
            break;
        }
        TotalStats = Vitality.Value + Strength.Value + Defense.Value + Dexterity.Value + Intelligence.Value + Speed.Value;
    }

    private void RemoveStatusStatMod(StatModifier statModifier)
    {
        switch (statModifier.statType)
        {
            case StatType.Vitality:
                Vitality.RemoveStatusModifier(statModifier);
                heroHealth.setMaxHealth(baseMaxHealth + (int)Vitality.Value);
                break;
            case StatType.Strength:
                Strength.RemoveStatusModifier(statModifier);
                break;
            case StatType.Defense:
                Defense.RemoveStatusModifier(statModifier);
                break;
            case StatType.Dexterity:
                Dexterity.RemoveStatusModifier(statModifier);
                break;
            case StatType.Intelligence:
                Intelligence.RemoveStatusModifier(statModifier);
                break;
            case StatType.Speed:
                Speed.RemoveStatusModifier(statModifier);
                break;
        }
        TotalStats = Vitality.Value + Strength.Value + Defense.Value + Dexterity.Value + Intelligence.Value + Speed.Value;
    }

    private void GiveStarterWeapon()
    {
        WeaponItem startingWeapon = Items.FindItem<WeaponItem>(item => item.itemName == "Iron Sword");
        mainHand.setEquippedItem(Items.InstantiateItem(startingWeapon));
        SetAttackTimerDuration(mainHand.getEquippedItemData().attackSpeed);
        heroHealth.setCurrentHealth(100);
        UpdateStats();
    }

    private void SetAttackTimerDuration(float duration)
    {
        attackTimer.SetStartTime(duration);
    }

    private IEnumerator HandleBlockReleasedTiming()
    {
        yield return new WaitForSeconds(.5f);
        blockReleased = false;
    }

    private void LoadSprite(AsyncOperationHandle<Sprite> op)
    {
        loadSpriteOp = op;
        if (op.Result == null)
        {
            Debug.LogError("no sprite in sheet here.");
            return;
        }

        stunEffectIcon = op.Result;
        Debug.Log(stunEffectIcon);
    }

    public void ReleaseAddressableAssets()
    {
        Addressables.Release(loadSpriteOp);
    }

    public void CachePlayerSpeed()
    {
        CachedSpeed = (int)Speed.Value;
    }

    public float GetShieldCDDuration() => shieldCooldownDuration;

    public void DisableShieldBlocking()
    {
        isBlocking = false;
        blockReleased = false;
        shieldRaisedSprite.enabled = false;
        shieldDownSprite.enabled = true;
    }

    private IEnumerator HandleWeaponTutorial(WeaponType weaponType)
    {
        EnemyController.Instance.toggleAllTimers(false);
        attackTimer.toggleTimer(false);
        switch (weaponType)
        {
            case WeaponType.Sword:
                if (GameManager.Instance.SwordTutorialEnabled)
                {
                    TutorialController.Instance.EnableSwordTutorial();
                    yield return new WaitUntil(() => GameManager.Instance.SwordTutorialEnabled == false);
                }
                break;

            case WeaponType.Shield:
                if(GameManager.Instance.ShieldTutorialEnabled)
                {
                    TutorialController.Instance.EnableShieldTutorial();
                    yield return new WaitUntil(() => GameManager.Instance.ShieldTutorialEnabled == false);
                }
                break;
            
            case WeaponType.Dagger:
                if (GameManager.Instance.DaggerTutorialEnabled)
                {
                    TutorialController.Instance.EnableDaggerTutorial();
                    yield return new WaitUntil(() => GameManager.Instance.DaggerTutorialEnabled == false);
                }
                break;
            
            case WeaponType.Axe:
                if (GameManager.Instance.AxeTutorialEnabled)
                {
                    TutorialController.Instance.EnableAxeTutorial();
                    yield return new WaitUntil(() => GameManager.Instance.AxeTutorialEnabled == false);
                }
                break;
            
            case WeaponType.Hammer:
                if (GameManager.Instance.HammerTutorialEnabled)
                {
                    TutorialController.Instance.EnableHammerTutorial();
                    yield return new WaitUntil(() => GameManager.Instance.HammerTutorialEnabled == false);
                }
                break;
        }
        EnemyController.Instance.toggleAllTimers(true);
        attackTimer.toggleTimer(true);
    }
}
