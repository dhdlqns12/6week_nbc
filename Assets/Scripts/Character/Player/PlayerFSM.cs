using UnityEngine;

public class PlayerFSM : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Move,
        Run,
        Jump,
        Zoom,
        Attack,
        Interact,
        Dead
    }

    public PlayerState CurState { get; private set; }

    [Header("직접 참조")]
    [SerializeField] private PlayerController playerController;

    [Header("상태 전환 타이머")]
    [SerializeField] private float attackTimer;
    [SerializeField] private float attackDuration;
    [SerializeField] private float interactTimer;
    [SerializeField] private float interactDuration;
    [SerializeField] private bool isLanded;

    [Header("스태미나 소모 설정")]
    [SerializeField] private float runSpCost;
    [SerializeField] private float wallRunSpCost;
    [SerializeField] private float jumpSpCost;

    [Header("애니메이터")]
    private Animator animator;
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int JumpHash = Animator.StringToHash("Jump");

    #region 유니티 callback메서드
    private void Awake()
    {
        animator = GetComponent<Animator>();
        CurState = PlayerState.Idle;
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void Update()
    {
        if (playerController.Player.isDead)
        {
            return;
        }

        UpdateState();
        UpdateAnimator();
        HandleSpRecovery();
    }

    private void FixedUpdate()
    {
        if (playerController.Player.isDead)
        {
            return;
        }

        HandlePhysics();
    }
    #endregion

    #region 이벤트 구독/해제
    private void SubscribeEvents()
    {
        EventBus.OnPlayerJumpRequested += HandleJumpRequest;
        EventBus.OnPlayerAttackRequested += HandleAttackRequest;
        EventBus.OnPlayerInteractRequested += HandleInteractRequest;
        EventBus.OnPlayerSprintRequested += HandleSprintToggle;
        EventBus.OnPlayerZoomRequested += HandleZoomToggle;
    }

    private void UnsubscribeEvents()
    {
        EventBus.OnPlayerJumpRequested -= HandleJumpRequest;
        EventBus.OnPlayerAttackRequested -= HandleAttackRequest;
        EventBus.OnPlayerInteractRequested -= HandleInteractRequest;
        EventBus.OnPlayerSprintRequested -= HandleSprintToggle;
        EventBus.OnPlayerZoomRequested -= HandleZoomToggle;
    }
    #endregion

    #region 이벤트 핸들러
    private void HandleJumpRequest()
    {
        if (playerController.IsGrounded == true)
        {
            Debug.Log(CurState);

            if (CurState == PlayerState.Idle ||
                CurState == PlayerState.Move ||
                CurState == PlayerState.Run ||
                CurState == PlayerState.Zoom &&
                playerController.IsGrounded && playerController.Player.curSp >= jumpSpCost)
            {
                ChangeState(PlayerState.Jump);
            }
        }
    }

    private void HandleAttackRequest()
    {
        if (CurState != PlayerState.Dead && CurState != PlayerState.Attack)
        {
            ChangeState(PlayerState.Attack);
        }
    }

    private void HandleInteractRequest()
    {
        if (CurState == PlayerState.Idle || CurState == PlayerState.Move)
        {
            ChangeState(PlayerState.Interact);
        }
    }

    private void HandleSprintToggle(bool sprinting)
    {
        if (sprinting && CurState == PlayerState.Zoom)
        {
            ChangeState(PlayerState.Move);
            return;
        }

        if (sprinting && CurState == PlayerState.Move)
        {
            ChangeState(PlayerState.Run);
        }
        else if (!sprinting && CurState == PlayerState.Run)
        {
            ChangeState(PlayerState.Move);
        }
    }

    private void HandleZoomToggle(bool zooming)
    {
        if (zooming)
        {
            if (CurState == PlayerState.Idle ||
                CurState == PlayerState.Move ||
                CurState == PlayerState.Run)
            {
                ChangeState(PlayerState.Zoom);
            }
        }
        else
        {
            if (CurState == PlayerState.Zoom)
            {
                ChangeState(playerController.HasMoveInput() ? PlayerState.Move : PlayerState.Idle);
            }
        }
    }
    #endregion

    #region FSM Core
    private void UpdateState()
    {
        switch (CurState)
        {
            case PlayerState.Idle:
                UpdateIdleState();
                break;
            case PlayerState.Move:
                UpdateMoveState();
                break;
            case PlayerState.Run:
                UpdateRunState();
                break;
            case PlayerState.Jump:
                UpdateJumpState();
                break;
            case PlayerState.Zoom:
                UpdateZoomState();
                break;
            case PlayerState.Attack:
                UpdateAttackState();
                break;
            case PlayerState.Interact:
                UpdateInteractState();
                break;
        }
    }

    public void ChangeState(PlayerState newState)
    {
        if (CurState == newState)
        {
            return;
        }

        try
        {
            ExitState(CurState);
            CurState = newState;
            EnterState(newState);
        }
        catch (System.NullReferenceException e)
        {
            Debug.LogError($"상태 전환 실패 ({CurState} → {newState}): {e.Message}\n{e.StackTrace}");
            CurState = PlayerState.Idle;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"({CurState} → {newState}): {e.Message}\n{e.StackTrace}");
            CurState = PlayerState.Idle;
        }
    }

    private void EnterState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle:
                break;

            case PlayerState.Move:
                break;

            case PlayerState.Run:
                break;

            case PlayerState.Jump:
                isLanded = false;
                playerController.Jump();
                playerController.Player.ConsumeSp(jumpSpCost);

                if (animator != null)
                {
                    animator.SetBool(JumpHash, true);
                }

                EventBus.OnPlayerJumped?.Invoke();
                break;

            case PlayerState.Zoom:
                break;

            case PlayerState.Attack:
                attackTimer = 0f;
                EventBus.OnWeaponFired?.Invoke();
                break;

            case PlayerState.Interact:
                Debug.Log("Enter Interact");
                interactTimer = 0f;
                break;

            case PlayerState.Dead:
                Debug.Log("Enter Dead");
                playerController.StopMovement();
                playerController.SetKinematic(true);
                EventBus.OnPlayerDead?.Invoke();
                break;
        }
    }

    private void ExitState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Dead:
                break;
        }
    }
    #endregion

    #region 상태 업데이트
    private void UpdateIdleState()
    {
        if (playerController.HasMoveInput())
        {
            ChangeState(playerController.IsSprinting ? PlayerState.Run : PlayerState.Move);
        }
    }

    private void UpdateMoveState()
    {
        if (!playerController.HasMoveInput())
        {
            ChangeState(PlayerState.Idle);
            return;
        }

        if (playerController.IsSprinting)
        {
            ChangeState(PlayerState.Run);
        }
    }

    private void UpdateRunState()
    {
        if (!playerController.HasMoveInput())
        {
            ChangeState(PlayerState.Idle);
            return;
        }

        if (!playerController.IsSprinting)
        {
            ChangeState(PlayerState.Move);
            return;
        }

        playerController.Player.ConsumeSp(runSpCost * Time.deltaTime);

        if (playerController.Player.curSp <= 0)
        {
            ChangeState(PlayerState.Move);
        }
    }

    private void UpdateJumpState()
    {
        //if (playerController.IsGrounded)
        //{
        //    return;
        //}
        /*
         *Jump 상태에 진입했지만 (Enter Jump), 물리 연산이 아직이라 IsGrounded는 여전히 true
        따라서 IsGrounded 체크를 통과해 착지했다고 오해하고, 점프 애니메이션이 재생되기도 전에 (ChangeState(Idle)) Idle 상태로 바로 돌아가 버리는 것
         */

        if (!playerController.IsGrounded)
        {
            isLanded = true;
            return;
        }

        if (playerController.IsGrounded && isLanded)
        {
            Debug.Log("착지 실행");
            // 착지 로직 실행
            EventBus.OnPlayerLanded?.Invoke();
            animator.SetBool(JumpHash, false);

            if (playerController.HasMoveInput())
            {
                Debug.Log("Landed and Moving");
                ChangeState(playerController.IsSprinting ? PlayerState.Run : PlayerState.Move);
            }
            else
            {
                Debug.Log("Landed and Idle");
                ChangeState(PlayerState.Idle);
            }
        }
    }

    private void UpdateZoomState()
    {
        if (!playerController.IsZoomPressed)
        {
            ChangeState(playerController.HasMoveInput() ? PlayerState.Move : PlayerState.Idle);
        }
    }

    private void UpdateAttackState()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackDuration)
        {
            if (playerController.HasMoveInput())
            {
                ChangeState(playerController.IsSprinting ? PlayerState.Run : PlayerState.Move);
            }
            else
            {
                ChangeState(PlayerState.Idle);
            }
        }
    }

    private void UpdateInteractState()
    {
        interactTimer += Time.deltaTime;

        if (interactTimer >= interactDuration)
        {
            EventBus.OnInteractionCompleted?.Invoke();
            ChangeState(PlayerState.Idle);
        }
    }
    #endregion

    #region 물리 Handler
    public void HandlePhysics()
    {
        switch (CurState)
        {
            case PlayerState.Idle:
                playerController.Move(0f); 
                break;
            case PlayerState.Move:
                playerController.Move(playerController.walkSpeed);
                break;

            case PlayerState.Run:
                playerController.Move(playerController.runSpeed);
                break;

            case PlayerState.Zoom:
                playerController.Move(playerController.walkSpeed * playerController.zoomSpeedMultiplier);
                break;                          
        }
    }
    #endregion

    #region 애니메이션 제어
    private void UpdateAnimator()
    {
        if (animator == null) return;

        if (!animator.isActiveAndEnabled)
        {
            return;
        }

        float targetSpeed = CalculateAnimationSpeed();

        animator.SetFloat(SpeedHash, targetSpeed, 0.1f, Time.deltaTime);

        float currentSpeed = animator.GetFloat(SpeedHash);

        if (currentSpeed < 0.001f) // 1.966e-06 보간때문에 0근처값에서 숫자가 계속 출렁거리는거 보기 싫어서 설정
        {
            animator.SetFloat(SpeedHash, 0f);
        }

        animator.SetBool(IsGroundedHash, playerController.IsGrounded);
    }

    private float CalculateAnimationSpeed()
    {
        if (!playerController.HasMoveInput())
        {
            return 0f;
        }

        switch (CurState)
        {
            case PlayerState.Idle:
                return 0f;

            case PlayerState.Move:
                return 0.5f;

            case PlayerState.Run:
                return 1f;

            case PlayerState.Zoom:
                return 0.3f;

            default:
                return 0f;
        }
    }

    #endregion

    public void Die()
    {
        ChangeState(PlayerState.Dead);
    }

    public void Respawn(Vector3 position)
    {
        playerController.Player.Respawn(position);
        ChangeState(PlayerState.Idle);
    }

    private void HandleSpRecovery()
    {
        if (playerController.Player.isDead)
        {
            return;
        }

        if (playerController.IsSprinting)
        {
            if (playerController.Player.curSp > 0)
            {
                playerController.Player.ConsumeSp(runSpCost * Time.deltaTime);
            }
            return;
        }

        if ( playerController.Player.curSp < playerController.Player.maxSp)
        {
            playerController.Player.RestoreSp(playerController.spRecoveryRate * Time.deltaTime);
        }
    }
}
