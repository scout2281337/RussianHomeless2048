using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public enum CameraPosition
{
    def, run, crouch
}

[RequireComponent(typeof(Rigidbody), typeof(RaycastInfo))]
public class PlayerController : BaseStateMachine<PlayerStates>
{
    #region Serialized Fields
    [field: SerializeField] public PlayerMovementData Data { get; private set; }
    [Header("Cameras")]
    [SerializeField] private CinemachinePanTilt panTilt;
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private Transform _cameraPosDef;
    [SerializeField] private Transform _cameraPosRun;
    [SerializeField] private Transform _cameraPosCrouch;

    [Header("VFX points")]
    [SerializeField] private Transform _bottonVFXPoint;
    [SerializeField] private Transform _leftVFXPoint;
    [SerializeField] private Transform _rightVFXPoint;

    [Header("VFX prefabs")]
    [SerializeField] private Transform _jumpDustVFXPrefab;
    [SerializeField] private Transform _dashVFXPrefab;
    [SerializeField] private Transform _flipDirectionVFXPrefab;
    [SerializeField] private Transform _fallDustVFXPrefab;
    #endregion

    public PlayerStates CurrentState => _currentState.StateKey;

    #region Private variables
    private Rigidbody _rb;
    private RaycastInfo _raycastInfo;

    private PlayerInputActions _playerInputActions;
    private InputAction _movementAction;


    #endregion

    #region Dash Parameters
    private float _lastPressedDashTime;
    private bool _isDashRefilling;
    public bool IsDashActive { get; set; } // set to true when grounded, and false when dashing
    public bool CanDash => IsDashActive && !_isDashRefilling;
    public bool DashRequest { get; private set; }
    #endregion

    #region Movement Parameters
    public Vector2 MovementDirection => _movementAction.ReadValue<Vector2>();
    public bool FaceWallHit => _raycastInfo.HitGroundInfo.Forward;
    public Vector3 Velocity
    {
        get => _rb.linearVelocity;
        set => _rb.linearVelocity = value;
    }
    #endregion

    #region Jump Parameters
    private float _lastPressedJumpTime;
    private float _lastPressedJumpTimeE;
    public bool IsGrounded => _raycastInfo.HitGroundInfo.Down;
    public bool IsWallTouching => FaceWallHit && IsGrounded;
    public bool JumpRequest { get; private set; }
    public bool HandleLongJumps { get; private set; }
    public bool IsActiveCoyoteTime { get; set; }

    #endregion

    #region Unity Functions
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = true;
        _raycastInfo = GetComponent<RaycastInfo>();
        _playerInputActions = new PlayerInputActions();
    }

    protected override void Start()
    {
        base.Start();
        SetGravityScale(Data.gravityScale);
    }

    protected override void Update()
    {
        base.Update();
        SyncRotationWithCamera();
        // manage input buffers time
        ManageJumpBuffer();
        ManageDashBuffer();///crouch?
    }

    private void OnEnable()
    {
        EnableInput();
    }

    private void OnDisable()
    {
        DisableInput();
    }
    #endregion

    #region State Machine Functions
    protected override void SetStates()
    {
        States.Add(PlayerStates.Grounded, new PlayerGroundedState(PlayerStates.Grounded, this));
        States.Add(PlayerStates.Jumping, new PlayerJumpingState(PlayerStates.Jumping, this));
        States.Add(PlayerStates.Falling, new PlayerFallingState(PlayerStates.Falling, this));
        //States.Add(PlayerStates.WallSliding, new PlayerWallSlidingState(PlayerStates.WallSliding, this));
        //States.Add(PlayerStates.WallJumping, new PlayerWallJumpingState(PlayerStates.WallJumping, this));
        //States.Add(PlayerStates.Dashing, new PlayerDashingState(PlayerStates.Dashing, this));

        // set the player's initial state
        _currentState = States[PlayerStates.Grounded];
    }
    #endregion

    #region Input
    private void EnableInput()
    {
        _movementAction = _playerInputActions.Player.Move;
        _movementAction.Enable();

        _playerInputActions.Player.Jump.started += OnJumpAction;
        _playerInputActions.Player.Jump.canceled += OnJumpAction;
        _playerInputActions.Player.Jump.Enable();

        //_playerInputActions.Player.Dash.performed += OnDashAction;    ////crouch/fight/block
        //_playerInputActions.Player.Dash.Enable();
    }

    private void DisableInput()
    {
        _movementAction.Disable();
        _playerInputActions.Player.Jump.Disable();
        //_playerInputActions.Player.Dash.Disable();   ////crouch/fight/block
    }
    #endregion

    #region CameraFunctions
    void SyncRotationWithCamera()
    {
        if (panTilt != null)
        {
            transform.rotation = Quaternion.Euler(0f, panTilt.PanAxis.Value, 0f);
        }
    }
    public void SetCameraPosition(CameraPosition pos)
    {
        switch (pos)
        {
            case CameraPosition.def:
                cam.Follow = _cameraPosDef;
                break;
            case CameraPosition.run:
                cam.Follow = _cameraPosRun;
                break;
            case CameraPosition.crouch:
                cam.Follow = _cameraPosCrouch;
                break;
        }
    }
    #endregion

    #region Movement Functions
    public void Run(float lerpAmount, bool canAddBonusJumpApex)
    {
        {//Vector3 cameraForward = Camera.main.transform.forward;
        //Vector3 cameraRight = Camera.main.transform.right;
        //cameraForward.y = 0f;
        //cameraRight.y = 0f;
        //cameraForward.Normalize();
        //cameraRight.Normalize();

        //Vector3 targetDirection = cameraForward * MovementDirection.normalized.y + cameraRight * MovementDirection.normalized.x;
        //targetDirection.y = 0f;
        //targetDirection.Normalize();
        }

        Vector3 targetDirection = transform.forward.normalized * MovementDirection.normalized.y + transform.right.normalized * MovementDirection.normalized.x;

        Vector3 flatVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        float targetSpeed = MovementDirection.magnitude * Data.runMaxSpeed;

        targetSpeed = Mathf.Lerp(flatVelocity.magnitude, targetSpeed, lerpAmount);

        float accelRate;
        if (IsGrounded)
        {
            accelRate = (targetSpeed > 0.01f)
                ? Data.runAccelAmount
                : Data.runDecelAmount;
        }
        else
        {
            accelRate = (targetSpeed > 0.01f)
                ? Data.runAccelAmount * Data.accelInAirMult
                : Data.runDecelAmount * Data.decelInAirMult;
        }

        if (canAddBonusJumpApex && Mathf.Abs(_rb.linearVelocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }

        Vector3 targetVelocity = targetDirection * targetSpeed;
        Vector3 velocityDiff = targetVelocity - flatVelocity;
        Vector3 movement = velocityDiff * accelRate;

        _rb.AddForce(movement, ForceMode.Force);

        // _rb.velocity = new Vector3(
        //     Mathf.Lerp(_rb.velocity.x, targetVelocity.x, accelRate * Time.fixedDeltaTime),
        //     _rb.velocity.y,
        //     Mathf.Lerp(_rb.velocity.z, targetVelocity.z, accelRate * Time.fixedDeltaTime)
        // );
    }

    #endregion

    #region Jump Functions
    public void Jump()
    {
        JumpRequest = false;

        float force = Data.jumpForce;

        // avoid shorter jumps when falling and jumping with coyote time
        if (_rb.linearVelocity.y < 0)
            force -= _rb.linearVelocity.y;
        _rb.AddForce(Vector3.up * force, ForceMode.Impulse);

        //InstantiateJumpDustVFX();
    }

    private void OnJumpAction(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            JumpRequest = true;
            _lastPressedJumpTime = Data.jumpInputBufferTime; // reset buffer time
        }

        // if still pressing jump button, perform long jump
        HandleLongJumps = context.ReadValueAsButton();
    }
    private void ManageJumpBuffer()
    {
        if (!JumpRequest) return;

        _lastPressedJumpTime -= Time.deltaTime;
        if (_lastPressedJumpTime <= 0)
        {
            JumpRequest = false;
        }
    }
    #endregion

    #region Dash Functions
    public void RefillDash() ////refill attack/block
    {
        StartCoroutine(nameof(PerformRefillDash));
    }

    private IEnumerator PerformRefillDash()
    {
        _isDashRefilling = true;
        yield return new WaitForSeconds(Data.dashRefillTime);
        _isDashRefilling = false;
    }

    private void OnDashAction(InputAction.CallbackContext context) ////attack/block
    {
        if (context.ReadValueAsButton())
        {
            DashRequest = true;
            _lastPressedDashTime = Data.dashInputBufferTime; // reset buffer time
        }
    }

    private void ManageDashBuffer() ////
    {
        if (!DashRequest) return;

        _lastPressedDashTime -= Time.deltaTime;
        if (_lastPressedDashTime <= 0)
        {
            DashRequest = false;
        }
    }
    #endregion

    #region General Methods
    public void SetGravityScale(float scale)
    {
        _rb.mass = scale;
    }

    public void Sleep(float duration)
    {
        StartCoroutine(nameof(PerformSleep), duration);
    }

    private IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }


    #endregion

    #region VFX Methods
    public void InstantiateJumpDustVFX()////for smth VFX
    {
        Instantiate(_jumpDustVFXPrefab, _bottonVFXPoint.position, _jumpDustVFXPrefab.rotation);
    }

    #endregion

    #region Debug
#if UNITY_EDITOR
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        string rootStateName = _currentState.Name;
        GUILayout.Label($"<color=black><size=20>State: {rootStateName}</size></color>");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label($"<color=black><size=20>Input: {MovementDirection}</size></color>");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label($"<color=black><size=20>Speed: {Velocity}</size></color>");
        GUILayout.EndHorizontal();
    }
#endif
    #endregion
}
