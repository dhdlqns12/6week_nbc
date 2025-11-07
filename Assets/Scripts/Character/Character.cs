using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("캐릭터 스탯")]
    [SerializeField] protected float maxHp;
    [SerializeField] protected string characterName;

    public float curHp;
    public bool isDead;

    protected virtual void Awake()
    {
        curHp = maxHp;
        isDead = false;
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
        {
            return;
        }

        curHp -= damage;
        curHp = Mathf.Max(curHp, 0);

        if(curHp<=0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        isDead = true;
    }

    public void Init(float _hp,string _name)
    {
        maxHp = _hp;
        characterName = _name;
        curHp = maxHp;
    }
}
