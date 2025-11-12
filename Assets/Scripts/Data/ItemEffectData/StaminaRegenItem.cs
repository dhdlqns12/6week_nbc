using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StaminaRegenItem", menuName = "Item/StaminaRegen")]
public class StaminaRegenItem : ItemData
{
    public float regenIncrease;
    public float duration;

    public override IEnumerator Use(PlayerController player)
    {
        float originalRegenRate = player.spRecoveryRate;

        player.spRecoveryRate += regenIncrease;

        yield return new WaitForSeconds(duration);

        player.spRecoveryRate = originalRegenRate;
    }
}   
