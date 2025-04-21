using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Effect
{
    [SerializeField] protected string effectDesc;
    public abstract bool TryTriggerEffect();
    public abstract string GetEffectDesc();
}

public abstract class TickingEffect : Effect
{
    [field: SerializeField] public Sprite Icon { get; protected set; }
    [field: SerializeField] public float Duration { get; protected set; }
    public float TimeLeft { get; protected set; }
    protected float tickRate;
    protected float timeSinceLastTick;
    public bool isTicking { get; protected set; } = true;
    protected Enemy Target { get; set; }

    public TickingEffect() { }
    public TickingEffect(TickingEffect tickingEffect)
    {
        Icon = tickingEffect.Icon;
        Duration = tickingEffect.Duration;
        TimeLeft = tickingEffect.Duration;
        tickRate = 1f;
        timeSinceLastTick = 0f;
        Target = tickingEffect.Target;
    }

    public TickingEffect(float duration, Sprite icon, Enemy target)
    {
        Icon = icon;
        Duration = duration;
        TimeLeft = duration;
        tickRate = 1f;
        timeSinceLastTick = 0f;
        Target = target;
    }

    public abstract bool Tick(float deltaTime);

    protected virtual void ToggleTicking(bool toggle)
    {
        isTicking = toggle;
    }

    public void RefreshDuration()
    {
        TimeLeft = Duration;
    }
}

public abstract class StackingEffect : TickingEffect
{   [field: SerializeField] public int maxStacks { get; protected set; }
    public int stackCount { get; protected set; }
    public StatusIcon statusIcon { get; set; }
    protected bool stackingEnabled { get; private set; } = true;
    
    public StackingEffect() { }
    public StackingEffect(StackingEffect stackingEffect) : base(stackingEffect)
    {
        isTicking = stackingEffect.isTicking;
        maxStacks = stackingEffect.maxStacks;
        stackCount = stackingEffect.stackCount;
    }

    protected abstract bool OnTick(float deltaTime);

    public sealed override bool Tick(float deltaTime)
    {
        if (!isTicking)
            return false;
        return OnTick(deltaTime);
    }

    public void AddStacks(int stacksToAdd)
    {
        stackCount += stacksToAdd;
        statusIcon.UpdateStackCountText();
    }

    protected sealed override void ToggleTicking(bool toggle)
    {
        base.ToggleTicking(toggle);
        statusIcon.ToggleEffectTimerVisual(toggle);
    }


    protected void DisableStacking()
    {
        stackingEnabled = false;
        stackCount = 0;
        statusIcon.DisableStackCountText();
    }
}

