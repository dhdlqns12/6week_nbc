using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerFSM : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Move,
        Run,
        Jump,
        WallRun,
        Hang,
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

    #region 유니티 callback메서드
    private void Awake()
    {
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
        Debug.Log("핸들러 진입");
        if (playerController.IsGrounded == true)
        {
            Debug.Log(CurState);
            Debug.Log(playerController.IsGrounded);
            if (CurState == PlayerState.Hang)
            {
                playerController.ClimbUp();
                ChangeState(PlayerState.Idle);
                return;
            }

            if (CurState == PlayerState.WallRun)
            {
                playerController.WallJump();
                ChangeState(PlayerState.Jump);
                return;
            }

            if (CurState == PlayerState.Idle ||
                CurState == PlayerState.Move ||
                CurState == PlayerState.Run ||
                CurState == PlayerState.Zoom &&
                playerController.IsGrounded && playerController.Player.curSp >= jumpSpCost)
            {
                ChangeState(PlayerState.Jump);
            }
        }
        if (CurState == PlayerState.Jump && !playerController.IsGrounded)
        {
            if (playerController.CheckHang())
            {
                ChangeState(PlayerState.Hang);
            }
        }
    }

    private void HandleAttackRequest()
    {
        if (CurState != PlayerState.Dead &&
            CurState != PlayerState.Attack &&
            CurState != PlayerState.WallRun &&
            CurState != PlayerState.Hang)
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
            case PlayerState.WallRun:
                UpdateWallRunState();
                break;
            case PlayerState.Hang:
                UpdateHangState();
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
        if (CurState == newState) return;

        ExitState(CurState);
        CurState = newState;
        EnterState(newState);
    }

    private void EnterState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle:
                Debug.Log("Enter Idle");
                break;

            case PlayerState.Move:
                Debug.Log("Enter Move (Walk)");
                break;

            case PlayerState.Run:
                Debug.Log("Enter Run");
                break;

            case PlayerState.Jump:
                Debug.Log("Enter Jump");
                isLanded = false;
                playerController.Jump();
                playerController.Player.ConsumeSp(jumpSpCost);
                EventBus.OnPlayerJumped?.Invoke();
                break;

            case PlayerState.WallRun:
                Debug.Log("Enter WallRun");
                playerController.SetGravity(false);
                break;

            case PlayerState.Hang:
                Debug.Log("Enter Hang");
                playerController.StopMovement();
                playerController.SetGravity(false);
                break;

            case PlayerState.Zoom:
                Debug.Log("Enter Zoom");
                break;

            case PlayerState.Attack:
                Debug.Log("Enter Attack");
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
            case PlayerState.WallRun:
                playerController.SetGravity(true);
                break;

            case PlayerState.Hang:
                playerController.SetGravity(true);
                break;

            case PlayerState.Dead:
                playerController.SetKinematic(false);
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
        if (playerController.IsGrounded && !isLanded)
        {
            isLanded = true;
            EventBus.OnPlayerLanded?.Invoke();

            if (playerController.HasMoveInput())
            {
                ChangeState(playerController.IsSprinting ? PlayerState.Run : PlayerState.Move);
            }
            else
            {
                ChangeState(PlayerState.Idle);
            }
        }

        if (playerController.CanWallRun())
        {
            ChangeState(PlayerState.WallRun);
        }
    }

    private void UpdateWallRunState()
    {
        if (!playerController.IsOnWall())
        {
            ChangeState(PlayerState.Jump);
            return;
        }

        playerController.Player.ConsumeSp(wallRunSpCost * Time.deltaTime);

        if (playerController.Player.curSp <= 0)
        {
            ChangeState(PlayerState.Jump);
        }
    }

    private void UpdateHangState()
    {
        if (playerController.HasDownInput())
        {
            ChangeState(PlayerState.Jump);
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
            case PlayerState.Move:
                playerController.Move(playerController.walkSpeed);
                break;

            case PlayerState.Run:
                playerController.Move(playerController.runSpeed);
                break;

            case PlayerState.Zoom:
                playerController.Move(playerController.walkSpeed * playerController.zoomSpeedMultiplier);
                break;

            case PlayerState.WallRun:
                playerController.WallRun();
                break;
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
}
