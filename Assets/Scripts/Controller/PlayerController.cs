using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public enum CameraPosition
{
    def, run, crouch, fight
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
    [SerializeField] private Transform _cameraPosFight;

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
    private CapsuleCollider _capsuleCollider;

    private HandsIK _handsIK;

    #endregion

    #region Crouch Parameters

    private float _lastPressedCrouchTime;
    public bool CrouchRequest { get; private set; }
    #endregion

    #region Fight Parameters

    public bool IsFighting;
    public bool PunchRequest { get; set; }
    public bool PunchAnimRequest { get; set; }
    public bool IsPunchActive { get; set; }

    private float _lastPressedPunchTime;
    private bool _isPunchRefilling;
    public bool CanPunch => !_isPunchRefilling && IsPunchActive;
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
    public bool IsMovingHorizontal => Mathf.Abs(new Vector2(Velocity.x, Velocity.z).magnitude) > .05f;
    public bool IsBlockedUp => _raycastInfo.HitGroundInfo.Up;
    public bool IsWallTouching => FaceWallHit && IsGrounded;
    public bool JumpRequest { get; set; }
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
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _handsIK = GetComponent<HandsIK>();

    }

    protected override void Start()
    {
        base.Start();
        SetGravityScale(Data.gravityScale);
        SetHandsIK(false);
        IsPunchActive = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    protected override void Update()
    {
        base.Update();
        
        // manage input buffers time
        ManageJumpBuffer();
        ManageCrouchBuffer();
        ManagePunchBuffer();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        SyncRotationWithCamera();
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
        States.Add(PlayerStates.Crouching, new PlayerCrouchingState(PlayerStates.Crouching, this));
        States.Add(PlayerStates.FightIdle, new PlayerFightIdleState(PlayerStates.FightIdle, this));
        States.Add(PlayerStates.FightPunch, new PlayerPunchState(PlayerStates.FightPunch, this));

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

        _playerInputActions.Player.Crouch.performed += OnCrouchAction;    ////crouch
        _playerInputActions.Player.Crouch.Enable();

        _playerInputActions.Player.Attack.performed += OnPunchAction;    ////fight/block
        _playerInputActions.Player.Attack.Enable();
    }

    private void DisableInput()
    {
        _movementAction.Disable();
        _playerInputActions.Player.Jump.Disable();
        _playerInputActions.Player.Crouch.Disable();   ////crouch
        _playerInputActions.Player.Attack.Disable();   ////fight/block
    }
    #endregion

    #region FightFunctions

    public void SetHandsIK(bool isOn)
    {
        _handsIK.SetHandIK(isOn);
    }

    public void SetFightMode(bool isOn)
    {
        IsFighting = isOn;
    }

    public void Punch()
    {
        
    }

    public void ResetPunchAnimPossibility()
    {
        PunchAnimRequest = false;
    }

    private void OnPunchAction(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            PunchRequest = true;
            _lastPressedPunchTime = Data.punchInputBufferTime; // reset buffer time
        }
    }
    private void ManagePunchBuffer()
    {
        if (!PunchRequest) return;

        _lastPressedPunchTime -= Time.deltaTime;
        if (_lastPressedPunchTime <= 0)
        {
            PunchRequest = false;
        }
    }
    public void RefillPunch()
    {
        StartCoroutine(nameof(PerformRefillPunch));
    }

    private IEnumerator PerformRefillPunch()
    {
        _isPunchRefilling = true;
        yield return new WaitForSeconds(Data.punchRefillTime);
        _isPunchRefilling = false;
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
            case CameraPosition.fight:
                cam.Follow = _cameraPosFight;
                break;
        }
    }

    #endregion

    #region Movement Functions
    public void Run(float lerpAmount, bool canAddBonusJumpApex)
    {
        Vector3 targetDirection = transform.forward.normalized * MovementDirection.normalized.y + transform.right.normalized * MovementDirection.normalized.x;

        Vector3 flatVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        float targetSpeed = MovementDirection.magnitude * Data.runMaxSpeed 
            * (CurrentState==PlayerStates.Crouching? Data.crouchSpeedMultiplier : 1);

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

    #region Crouch Functions
    private void OnCrouchAction(InputAction.CallbackContext context) ////attack/block
    {
        if (context.ReadValueAsButton())
        {
            CrouchRequest = true;
            _lastPressedCrouchTime = Data.crouchInputBufferTime; // reset buffer time
        }
    }

    private void ManageCrouchBuffer() ////
    {
        if (!CrouchRequest) return;

        _lastPressedCrouchTime -= Time.deltaTime;
        if (_lastPressedCrouchTime <= 0)
        {
            CrouchRequest = false;
        }
    }

    public void Crouch()
    {
        SetCameraPosition(CameraPosition.crouch);
        CrouchRequest = false;
        _capsuleCollider.height = 1;
        _capsuleCollider.center = new Vector3(0, -_capsuleCollider.height / 2, 0);

    }
    public void UnCrouch()
    {
        CrouchRequest = false;
        JumpRequest = false;
        _capsuleCollider.center = new Vector3(0, 0, 0);
        _capsuleCollider.height = 2;
        
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
