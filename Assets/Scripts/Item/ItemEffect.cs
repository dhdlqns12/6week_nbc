using System.Collections;
using System;
using UnityEngine;

[Serializable]
public abstract class ItemEffect : ScriptableObject
{
    public abstract IEnumerator ApplyEffect(PlayerController player);
}
