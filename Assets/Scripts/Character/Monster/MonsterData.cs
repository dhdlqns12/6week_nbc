using UnityEngine;

[CreateAssetMenu(fileName ="MonsterData",menuName ="Character/MonsterData")]
public class MonsterData : ScriptableObject
{
    public enum AttackType
    {
        Melee,
        Ranged
    }

    [Header("기본 설정")]
    public string monsterName;
    public GameObject monsterPrefab;

    [Header("스탯")]
    public float maxHp;
    public float moveSpeed;
    public float detectionRange;
    public float attackDamage;
    public float attackRange;
    public float attackCooldown;

    [Header("원거리 전용")]
    public GameObject projectilePrefab;
    public float projectileSpeed;
}
