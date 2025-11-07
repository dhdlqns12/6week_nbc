using System;
using UnityEngine;

public static class EventBus 
{
    [Header("플레이어 입력 이벤트")]
    public static Action OnPlayerJumpRequested;
    public static Action OnPlayerAttackRequested;
    public static Action OnPlayerInteractRequested;
    public static Action<bool> OnPlayerSprintRequested;
    public static Action<bool> OnPlayerZoomRequested;
    public static Action OnInventoryRequested;
    public static Action OnInteractionCompleted;

    [Header("플레이어 상태 이벤트")]
    public static Action OnPlayerJumped;
    public static Action OnPlayerLanded;
    public static Action OnPlayerDead;
    public static Action OnWeaponFired;
    public static Action OnPlayerRespawned;
    public static Action OnSpChanged;
}
