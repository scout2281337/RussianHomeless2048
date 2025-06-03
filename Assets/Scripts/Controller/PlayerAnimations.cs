using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private PlayerController _player;

    private Animator _animator;

    private int _xSpeedHash;
    private int _ySpeedHash;
    private int _isGroundedHash;
    private int _jumpHash;
    private int _isCrouchHash;
    private int _isFightHash;
    private int _punchHash;

    private void Awake()
    {
        _player = GetComponent<PlayerController>();
        _animator = GetComponentInChildren<Animator>();

        // set animations hashes
        _xSpeedHash = Animator.StringToHash("xSpeed");
        _ySpeedHash = Animator.StringToHash("ySpeed");
        _isGroundedHash = Animator.StringToHash("isGrounded");
        _jumpHash = Animator.StringToHash("jump");
        _isCrouchHash = Animator.StringToHash("isCrouching");
        _isFightHash = Animator.StringToHash("isFight");
        _punchHash = Animator.StringToHash("punch");
    }

    private void Update()
    {
        if (_player.IsGrounded && _player.JumpRequest && _player.CurrentState != PlayerStates.Crouching)
            _animator.SetTrigger(_jumpHash);
        if (_player.PunchAnimRequest)
        {
            _animator.SetTrigger(_punchHash);
            _player.ResetPunchAnimPossibility();
        }
    }

    private void LateUpdate()
    {
        _animator.SetFloat(_xSpeedHash, Mathf.Abs(new Vector2(_player.Velocity.x, _player.Velocity.z).magnitude));
        _animator.SetFloat(_ySpeedHash, _player.Velocity.y);
        _animator.SetBool(_isGroundedHash, _player.IsGrounded);
        _animator.SetBool(_isCrouchHash, _player.CurrentState == PlayerStates.Crouching);
        _animator.SetBool(_isFightHash, _player.IsFighting);
    }

}
