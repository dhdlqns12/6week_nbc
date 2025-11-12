using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData",menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
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
    public float value;

    [Header("아이템 타입")]
    public ItemType itmeType;

    [Header("아이템 효과")]
    public ItemEffect itemEffect;

    [Header("리스폰 타임(Buff만")]
    public float respawnTime;
}
