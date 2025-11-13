using System;
using UnityEngine;

public static class EventBus  // 1:1관계인데도 무분별하게 이벤트 사용 남발했음... 강박적으로 eventBus패턴 사용해보자!라고 생각하면서 event사용. 그리고 이벤트 사용할땐 이벤트에 주석 달기, 만약 이벤트가 많아지면 카테코리별로 분리                         
{                                                                                   // 예시로 들어 OnPlayerDead는 여러 시스템이 반응해야함(GameMagner,UIManager,AudioManager 기타 등등에서 반응해야함 1:N관계)
    [Header("플레이어 입력 이벤트")]                                                // 하지만 OnPlayerJumpRequested나 SprintRequest같은 거는 1대1관계로 직접 호출 가능함. 여러곳에서 참조하지않음->PlayerFSM이 PlayerController로 직접 호출 가능한데 이벤트로 구현했음)
    public static Action OnPlayerJumpRequested;                                     // 이벤트는 필요악. 지금 내가 프로젝트에서 겪는것처럼 디버그할때 매우 어려워짐... 그리고 괜히 복잡해서 찾는 것 도 오래걸림...                                                                                                                                                                                                                                                                      
    public static Action<bool> OnPlayerSprintRequested;
    public static Action<bool> OnPlayerZoomRequested;
    public static Action OnInventoryRequested;                                     //예시로 이 이벤트가 호출 되면 인벤토리 로드, 플레이어 골드 업데이트 해서 인벤토리에 골드 표시해주는거에 적용, 추후 확장 가능성 높음 이런 경우 이벤트로?

    [Header("플레이어 상태 이벤트")]
    public static Action OnPlayerJumped; 
    public static Action OnPlayerLanded;
    public static Action OnPlayerRespawned;
    public static Action OnSpChanged;                                           //데이터->UI는 이벤트가 좋긴한것 같은데...

    //예시로
    /// <summary>
    /// 플레이어 사망 시 발생 (UI, 적AI, 사운드, 카메라 등 여러 시스템에서 반응 필요)
    /// 구독자: GameManager, UIManager, EnemyAI, AudioManager, CameraController...
    /// </summary>
    //처럼 작성 필요
    public static Action OnPlayerDead;

}
