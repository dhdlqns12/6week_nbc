using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StaminaRegenEffect", menuName = "Item/Effect/StaminaRegen")]
public class StaminaRegenEffect : ItemEffect
{
    public float regenIncrease;
    public float duration;

    public override IEnumerator ApplyEffect(PlayerController player)
    {
        float originalRegenRate = player.spRecoveryRate;

        player.spRecoveryRate += regenIncrease;

        yield return new WaitForSeconds(duration);

        player.spRecoveryRate = originalRegenRate;
    }
}   
