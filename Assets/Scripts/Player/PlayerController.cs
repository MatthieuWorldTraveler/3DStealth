using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Serialized
    [Header("Movement Manager")]
    [Space(10)]
    [SerializeField] float _moveSpeed = 2f;
    [SerializeField] float _sprintSpeed = 5f;
    [SerializeField] float _sneakMultiplierSpeed = .5f;

    [Header("Jump Manager")]
    [Space(10)]
    [SerializeField]  float _jumpHeight = 1.0f;
    [SerializeField] float _anticipationTimer = .2f;
    [SerializeField] float _recoveryTimer = .2f;

    [Header("Camera Manager")]
    [Space(10)]
    [SerializeField] int _degreeRotationSpeed = 25;

    [Header("Animator Manager")]
    [Space(10)]
    [SerializeField] float _smoothTime = 0.3f;

    // variables
    float _anticipationEndTimer;
    float _recoveryEndTimer;
    float _ySpeed;
    Vector3 moveDir = Vector3.zero;
    float _sneakSpeed = 1f;

    // References
    CharacterController _controller;
    PlayerInputs _inputs;
    Transform _transform;
    Camera _camera;
    Animator _animator;
    Vector3 playerVelocity;

    // Animator transition variables
    Vector2 _currentDir = Vector2.zero;
    float _yVelocity = 0;
    float _xVelocity = 0;

    // Public Proprieties
    public bool IsGrounded { get { return _controller.isGrounded; } }
    public bool IsAnticipationOver { get { return Time.time > _anticipationEndTimer; } }
    public bool IsRecoveryOver { get { return Time.time > _recoveryEndTimer; } }
    public float PlayerVelocityYAxis { get { return _controller.velocity.y; } }

    public float SneakSpeedAnim { get { return _sneakSpeed == 1 ? 1 : -1; } }

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _camera = Camera.main;
        _transform = transform;
        _inputs = GetComponent<PlayerInputs>();
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        ApplyGravity();

        ApplyAnimator();
    }

    // Manually set the gravity for Player
    private void ApplyGravity()
    {
        _ySpeed += Physics.gravity.y * Time.deltaTime;
        _controller.Move(ApplyMovement());
    }

    // SmoothDamp the input for fluid transitions
    private void ApplyAnimator()
    {
        if (_inputs.HasMovement)
        {
            Vector2 targetDir = new Vector2(_inputs.Movement.x, _inputs.Movement.z);

            // Fix The smoothDamp 0 bug
            if (targetDir.x == 0)
                targetDir.x += .1f;
            if (targetDir.y == 0)
                targetDir.y += .1f;


            _currentDir.x = Mathf.SmoothDamp(_currentDir.x, targetDir.x, ref _xVelocity, _smoothTime);
            _currentDir.y = Mathf.SmoothDamp(_currentDir.y, targetDir.y, ref _yVelocity, _smoothTime);          
        }

        _animator.SetFloat("XInput", _currentDir.x);
        _animator.SetFloat("ZInput", _currentDir.y);
    }

    // Apply Walk Movement
    public void DoWalk()
    {
        _controller.Move(GetMoveDir(_moveSpeed * _sneakSpeed));
        playerVelocity = new Vector3(_controller.velocity.x, 0, _controller.velocity.z);
        ApplyPlayerRotation();
    }

    // Apply Spint Movement
    public void DoSprint()
    {
        _controller.Move(GetMoveDir(_sprintSpeed));
        playerVelocity = new Vector3(_controller.velocity.x, 0, _controller.velocity.z);
        ApplyPlayerRotation();
    }

    // Apply ySpeed for Jump counter gravity
    public void StartJump()
    {
        _ySpeed = Mathf.Sqrt(_jumpHeight * -3.0f * Physics.gravity.y);
    }

    // Apply ySpeed to stop gravity
    public void DoGrounded()
    {
        _ySpeed = -.5f;
    }

    // Initialize Recovery Timer
    public void StartRecovery()
    {
        _recoveryEndTimer = Time.time + _recoveryTimer;
    }

    // Initialize Anticipating Timer
    public void StartAnticipate()
    {
        _anticipationEndTimer = Time.time + _anticipationTimer;
    }

    // Apply Sneak Multiplier
    public void StartSneak()
    {
        _sneakSpeed = _sneakMultiplierSpeed;
    }
    // reset Sneak Multiplier
    public void ExitSneak()
    {
        _sneakSpeed = 1f;
    }

    /// <summary>
    /// Get the moveDir with playerInputs and Gravity
    /// </summary>
    /// <returns></returns>
    private Vector3 GetMoveDir(float speed)
    {
        moveDir = new Vector3(0, moveDir.y, 0);
        moveDir += _transform.forward * _inputs.ClampedMovement.x * speed * Time.deltaTime;
        moveDir += _transform.right * _inputs.ClampedMovement.z * speed * Time.deltaTime;
        return moveDir;
    }

    // Apply Artificiate gravity on each frame
    private Vector3 ApplyMovement()
    {
        moveDir = _transform.up * _ySpeed * Time.deltaTime;
        return moveDir;
    }

    // Apply rotation only when moving
    private void ApplyPlayerRotation()
    {
        Quaternion cameraRotation = Quaternion.LookRotation(_camera.transform.forward);
        Vector3 rotateToward = Quaternion.RotateTowards(_transform.rotation, cameraRotation, _degreeRotationSpeed * Time.deltaTime).eulerAngles;
        rotateToward.z = rotateToward.x = 0;
        _transform.rotation = Quaternion.Euler(rotateToward);
    }
}
