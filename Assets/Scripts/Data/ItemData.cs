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
    public Sprite ItemImage;
    public GameObject itemPrefab;
    public string itemName;
    public string itemDescirption;

    [Header("아이템 타입")]
    public ItemType itmeType;
}
