using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonKnight : Boss
{
    [SerializeField] EnemyPartHandler chestPart;
    [SerializeField] EnemyPartHandler shieldPart;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite shieldBrokenBossSprite;
    private bool shieldBroken = false;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
    
    public override void Attack()
    {
        base.Attack();
        Hero.Instance.Damage(damage, this);
    }

    public override void Damage(int amount, Enums.DamageType damageType)
    {
        if (!shieldBroken && !attackTimer.paused)
        {
            //StartCoroutine(SwitchToBlockingSprite());
            shieldPart.Weakpoint.AddDamage(amount, damageType);
            shieldBroken = shieldPart.Weakpoint.CheckWeakpointBreak(this);
            if (targetedPart.gameObject.name.Equals("Shield"))
                amount /= 2;
            else
                amount /= 3;

            amount = Mathf.Max(amount, 1);
            GetHealth().subtract(amount);

            if (shieldBroken)
            {
                spriteRenderer.sprite = shieldBrokenBossSprite;
                Hero.Instance.RemoveTargetedEnemy();
            }
        }
        else
            base.Damage(amount, damageType);
        
    }

    private IEnumerator SwitchToBlockingSprite()
    {
        //Switch to blocking sprite
        yield return new WaitForSecondsRealtime(0.5f);
        //switch back to idle sprite
    }

}
