using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class EffectSO : ScriptableObject
{
    public abstract Effect GetEffectInstance();
}
