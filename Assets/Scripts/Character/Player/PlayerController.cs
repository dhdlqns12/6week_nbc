using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("직접 참조")]
    [SerializeField] private Player player;
    private PlayerInputAction inputActions;
    private Rigidbody rb;

    public Player Player => player;

    [Header("스탯")]
    public float walkSpeed;
    public float runSpeed;
    public float zoomSpeedMultiplier;
    public float jumpForce;
    public float spRecoveryRate;

    [Header("카메라 설정")]
    [SerializeField] private Camera playerCamera;
    public Camera PlayerCamera => playerCamera;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float zoomFov;
    [SerializeField] private float normalFov;
    private float xRotation;

    [Header("지면 체크")]
    [SerializeField] Transform groundCheck;
    [SerializeField] private float groundRayDistance;
    [SerializeField] private LayerMask groundLayer;

    [Header("입력")]
    private Vector2 moveInput;
    private Vector2 lookInput;

    public bool IsGrounded { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool IsZoomPressed { get; private set; }

    #region 유니티 callback메서드
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (playerCamera != null)
        {
            normalFov = playerCamera.fieldOfView;
        }

        inputActions = new PlayerInputAction();
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false; // 줌상태일때 커서 숨김? 아니면 crosshair로 대체?
    }

    private void OnEnable()
    {
        inputActions.Enable();
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
        inputActions.Disable();
    }

    private void Update()
    {
        if (player.isDead)
        {
            return;
        }

        CheckGround();
    }

    private void LateUpdate()
    {
        MouseLook();
        Zoom();
    }
    #endregion

    #region 이벤트 구독/해제
    private void SubscribeEvents()
    {
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        inputActions.Player.Look.performed += OnLook;
        inputActions.Player.Look.canceled += OnLook;

        inputActions.Player.Sprint.performed += OnSprint;
        inputActions.Player.Sprint.canceled += OnSprint;

        inputActions.Player.Jump.performed += OnJump;

        inputActions.Player.Zoom.performed += OnZoom;
        inputActions.Player.Zoom.canceled += OnZoom;
    }

    private void UnsubscribeEvents()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;

        inputActions.Player.Look.performed -= OnLook;
        inputActions.Player.Look.canceled -= OnLook;

        inputActions.Player.Sprint.performed -= OnSprint;
        inputActions.Player.Sprint.canceled -= OnSprint;

        inputActions.Player.Jump.performed -= OnJump;

        inputActions.Player.Zoom.performed -= OnZoom;
        inputActions.Player.Zoom.canceled -= OnZoom;
    }
    #endregion

    #region 이벤트 발행
    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        if (player.isDead)
        {
            return;
        }

        IsSprinting = context.performed;

        EventBus.OnPlayerSprintRequested?.Invoke(true);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (player.isDead || !context.performed)
        {
            return;
        }

        EventBus.OnPlayerJumpRequested?.Invoke();
    }

    private void OnZoom(InputAction.CallbackContext context)
    {
        if (player.isDead)
        {
            return;
        }

        IsZoomPressed = context.performed;

        EventBus.OnPlayerZoomRequested?.Invoke(true);
    }
    #endregion


    /// <summary>
    /// FSM이 호출하는 이동 API들: Move/Jump/StopMovement
    /// 이들 메서드는 Rigidbody를 직접 제어하여 물리 기반 움직임을 수행한다.
    /// </summary>
    #region 이동제어
    /// <summary>
    /// 이동 입력값과 속도를 받아 Rigidbody velocity를 설정한다.
    /// - 방향은 transform.forward/right 기준으로 계산
    /// - y 성분은 기존 rb.velocity.y를 유지(중력/점프 영향 유지)
    /// </summary>
    /// <param name="speed">현재 상태에 맞는 이동 속도</param>
    public void Move(float speed)
    {
        Vector3 dir = transform.forward * moveInput.y + transform.right * moveInput.x;

        if (!HasMoveInput())
        {
            StopMovement();
        }

        dir = dir.normalized * speed;
        dir.y = rb.velocity.y;

        rb.velocity = dir;
    }

    /// <summary>
    /// 점프 처리: 수직 속도 초기화 후 Impulse로 힘을 가함.
    /// </summary>
    public void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    /// <summary>
    /// 수평 이동을 정지시킨다(수직 속도는 유지).
    /// FSM이 Dead 상태 진입 시 호출 등에서 사용.
    /// </summary>
    public void StopMovement()
    {
        Vector3 velocity = rb.velocity;
        velocity.x = 0f;
        velocity.z = 0f;
        rb.velocity = velocity;
    }
    #endregion

    #region 레이 체크 메소드
    private void CheckGround()
    {
        if (groundCheck != null)
        {
            IsGrounded = Physics.CheckSphere(groundCheck.position, groundRayDistance, groundLayer);
        }
    }

    public bool HasMoveInput() => moveInput != Vector2.zero;
    public bool HasDownInput() => moveInput.y < -0.5f;
    #endregion

    #region 카메라 제어
    private void MouseLook()
    {
        if (playerCamera == null)
        {
            return;
        }

        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    private void Zoom()
    {
        if (playerCamera == null)
        {
            return;
        }

        // IsZoomPressed로 판단 (FSM 상태 참조 안함)
        float targetFOV = IsZoomPressed ? zoomFov : normalFov;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * 10f);
    }
    #endregion
}
