using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableItem", menuName = "Item Data/Consumable")]
public class ConsumableItem : ItemDataBase
{
    [Header("Consumable Effect")]
    [SerializeField] private EffectSO effectSO;
    [SerializeReference] private Effect effect;
    [SerializeField][HideInInspector] private EffectSO currEffectSO;

    public bool OnUse()
    {
        bool usedEffect = false;
        if(effect != null)
            usedEffect = effect.TryTriggerEffect();
        return usedEffect;
    }

    public string GetEffectDesc() => effect.GetEffectDesc();

    public void OnValidate()
    {
        if (effectSO != null)
        {
            //Debug.Log("EffectSO changed");
            if (currEffectSO == null || currEffectSO != effectSO)
            {
                //Debug.Log("Updating Effect");
                currEffectSO = effectSO;
                effect = effectSO.GetEffectInstance();
            }
        }
        else
        {
            //Debug.Log("Removed EffectSO");
            effect = null;
            currEffectSO = null;
        }
    }

    public bool HasEffect()
    {
        return effect != null;
    }
}
