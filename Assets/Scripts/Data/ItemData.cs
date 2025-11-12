using System.Collections;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public enum ItemType
    {
        Buff,
        Equip,
        Potion
    }

    [Header("아이템 정보")]
    public string itemName;
    public string itemDescirption;

    [Header("아이템 타입")]
    public ItemType itmeType;

    [Header("리스폰 타임(Buff만)")]
    public float respawnTime;

    public abstract IEnumerator Use(PlayerController player);
}
