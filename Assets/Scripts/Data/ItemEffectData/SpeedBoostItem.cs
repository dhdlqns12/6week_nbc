using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeedBoostItem", menuName = "Item/SpeedBoost")]
public class SpeedBoostEffect : ItemData
{
    public float speedIncrease;
    public float duration;

    public override IEnumerator Use(PlayerController player)
    {
        float originalSpeed = player.walkSpeed;
        float originalRunSpeed = player.runSpeed;

        player.walkSpeed += speedIncrease;
        player.runSpeed += speedIncrease;

        yield return new WaitForSeconds(duration);

        player.walkSpeed = originalSpeed;
        player.runSpeed = originalRunSpeed;
    }
}
