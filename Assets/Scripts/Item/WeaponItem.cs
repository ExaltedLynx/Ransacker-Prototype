using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

[CreateAssetMenu(fileName = "WeaponItem", menuName = "Item Data/Weapon")]
public class WeaponItem : ItemDataBase
{
    public enum WeaponType
    {
        Sword,
        Dagger,
        Axe,
        Hammer,
        Shield
    }

    [Header("Weapon Info")]
    public WeaponType weaponType;
    [SerializeReference] private ItemFields itemFields;
    [field: SerializeField][field: HideInInspector] public int baseDamage { get; private set; }
    [field: SerializeField][field: HideInInspector] public float attackSpeed { get; private set; }
    [field: SerializeField][field: HideInInspector] public DamageType damageType { get; private set; }
    [field: SerializeField][field: HideInInspector] public int maxBlockFatigue { get; private set; }

    [Header("Weapon Effect")]
    [SerializeField] private EffectSO weaponEffectSO;
    [SerializeReference] private Effect weaponEffect;
    [SerializeField][HideInInspector] private EffectSO currWeaponEffectSO;

    public void TryTriggerEffect()
    {
        if (weaponEffect != null)
        {
            weaponEffect.TryTriggerEffect();
        }
    }
    public string GetEffectDesc()
    {
        if (weaponEffect == null)
            return "";
        
        return weaponEffect.GetEffectDesc();
    }
    private void OnValidate()
    {
        if (weaponEffectSO != null)
        {
            //Debug.Log("EffectSO changed");
            if (currWeaponEffectSO == null || currWeaponEffectSO != weaponEffectSO)
            {
                //Debug.Log("Updating Effect");
                currWeaponEffectSO = weaponEffectSO;
                weaponEffect = weaponEffectSO.GetEffectInstance();
            }
        }
        else
        {
            //Debug.Log("Removed EffectSO");
            weaponEffect = null;
            currWeaponEffectSO = null;
        }

        switch(weaponType)
        {
            case WeaponType.Shield:
                if (itemFields != null)
                {
                    ShieldFields shieldFields = (ShieldFields)itemFields;
                    maxBlockFatigue = shieldFields.maxBlockFatigue;
                }
                else if(itemFields == null)
                    itemFields = new ShieldFields();
                break;

            default:
                if (itemFields != null)
                {
                    WeaponFields weaponFields = (WeaponFields)itemFields;
                    baseDamage = weaponFields.baseDamage;
                    attackSpeed = weaponFields.attackSpeed;
                    damageType = weaponFields.damageType;
                }
                else if(itemFields == null)
                    itemFields = new WeaponFields();
                break;
        }
    }

    public bool HasEffect()
    {
        return weaponEffect != null;
    }

    [Serializable]
    public abstract class ItemFields { }

    [Serializable]
    private class WeaponFields : ItemFields 
    {
        public int baseDamage;
        public float attackSpeed;
        public DamageType damageType;
    }

    [Serializable]
    private class ShieldFields : ItemFields
    {
        public int maxBlockFatigue;
    }
}
