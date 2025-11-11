using UnityEngine;

public class Player : Character
{
    [Header("플레이어 스탯")]
    public float maxSp = 100f;

    public float curSp;

    protected override void Awake()
    {
        base.Awake();
        curSp = maxSp;
    }

    public void ConsumeSp(float amount)
    {
        if (isDead) return;
        curSp = Mathf.Max(curSp - amount, 0);
        EventBus.OnSpChanged?.Invoke();
    }

    public void RestoreSp(float amount)
    {
        if (isDead) return;
        curSp = Mathf.Min(curSp + amount, maxSp);
        EventBus.OnSpChanged?.Invoke();
    }

    public void Respawn(Vector3 position)
    {
        transform.position = position;
        curHp = maxHp;
        curSp = maxSp;
        isDead = false;

        EventBus.OnPlayerRespawned?.Invoke();
    }
}
