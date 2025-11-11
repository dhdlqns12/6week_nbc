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

    [Header("카메라 설정")]
    [SerializeField] private Camera playerCamera;
    public Camera PlayerCamera => playerCamera;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float zoomFov;
    private float xRotation;
    public float normalFov;

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
        inputActions.Player.Attack.performed += OnAttack;
        inputActions.Player.Interact.performed += OnInteract;

        inputActions.Player.Zoom.performed += OnZoom;
        inputActions.Player.Zoom.canceled += OnZoom;

        inputActions.Player.Inventory.performed += OnInventory;
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
        inputActions.Player.Attack.performed -= OnAttack;
        inputActions.Player.Interact.performed -= OnInteract;

        inputActions.Player.Zoom.performed -= OnZoom;
        inputActions.Player.Zoom.canceled -= OnZoom;

        inputActions.Player.Inventory.performed -= OnInventory;
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

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (player.isDead || !context.performed)
        {
            return;
        }

        EventBus.OnPlayerAttackRequested?.Invoke();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (player.isDead || !context.performed)
        {
            return;
        }

        EventBus.OnPlayerInteractRequested?.Invoke();
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

    private void OnInventory(InputAction.CallbackContext context)
    {
        if (player.isDead || !context.performed)
        {
            return;
        }

        EventBus.OnInventoryRequested?.Invoke();
    }
    #endregion

    /// <summary>
    /// FSM이 호출
    /// </summary>
    #region 이동제어
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

    public void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }


    public void StopMovement()
    {
        Vector3 velocity = rb.velocity;
        velocity.x = 0f;
        velocity.z = 0f;
        rb.velocity = velocity;
    }

    public void SetGravity(bool enabled)
    {
        rb.useGravity = enabled;
    }

    public void SetKinematic(bool kinematic)
    {
        rb.isKinematic = kinematic;
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
        if (playerCamera == null) return;

        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    private void Zoom()
    {
        if (playerCamera == null) return;

        // IsZoomPressed로 판단 (FSM 상태 참조 안함)
        float targetFOV = IsZoomPressed ? zoomFov : normalFov;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * 10f);
    }
    #endregion

    #region 디버그용
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundLayer);
        }

        Gizmos.color = Color.blue;

        Vector3 hangOrigin = transform.position + Vector3.up * 1.5f + transform.forward * 0.3f;
        Gizmos.color = Color.yellow;
    }
    #endregion
}
