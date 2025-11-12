using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeedBoostEffect", menuName = "Item/Effect/SpeedBoost")]
public class SpeedBoostEffect : ItemEffect
{
    public float speedIncrease;
    public float duration;

    public override IEnumerator ApplyEffect(PlayerController player)
    {
        float originalSpeed = player.walkSpeed;
        float originalRunSpeed = player.runSpeed;

        player.walkSpeed += speedIncrease;
        player.runSpeed += speedIncrease;

        Debug.Log($"스피드 부스트! 이동: {player.walkSpeed}, 달리기: {player.runSpeed}");

        //BuffUI.Instance?.ShowBuffTimer(duration);

        yield return new WaitForSeconds(duration);

        player.walkSpeed = originalSpeed;
        player.runSpeed = originalRunSpeed;

        Debug.Log("스피드 부스트 종료!");
    }
}
