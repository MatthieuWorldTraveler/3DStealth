using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables / Proprieties
    // Serialized
    [Header("Movement Manager")]
    [Space(10)]
    [SerializeField] float _moveSpeed = 2f;
    [SerializeField] float _sprintSpeed = 5f;
    [SerializeField] float _sneakMultiplierSpeed = .5f;
    [SerializeField] float _groundCheckCoolDownDuration = 0.5f;
    [SerializeField] float _fallSnapMaxHeight = 0.5f;
    [SerializeField] LayerMask _snappingGroundLayer;

    [Header("Jump Manager")]
    [Space(10)]
    [SerializeField] float _jumpHeight = 1.0f;
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
    private float _groundCheckCoolDownEndTime;                                  // Variable de timer pour le cooldown de groundcheck
    private float _landingRecoveryEndTime;                                      // Variable de timer pour la r�cup�ration de l'atterissage
    private bool _isGrounded;
    float _ySpeed;
    Vector3 moveDir = Vector3.zero;
    float _sneakSpeed = 1f;

    // References
    CharacterController _controller;
    PlayerInputs _inputs;
    Transform _transform;
    Camera _camera;
    Animator _animator;

    // Animator transition variables
    Vector2 _currentDir = Vector2.zero;
    float _yVelocity = 0;
    float _xVelocity = 0;

    // Public Proprieties
    public bool IsGrounded { get { return _controller.isGrounded; } }
    public bool IsAnticipationOver { get { return Time.time > _anticipationEndTimer; } }
    public bool IsRecoveryOver { get { return Time.time > _recoveryEndTimer; } }
    public float PlayerVelocityYAxis { get { return _ySpeed; } }

    public bool CanCheckGround { get => Time.time >= _groundCheckCoolDownEndTime; }

    //public float SneakSpeedAnim { get { return _sneakSpeed == 1 ? 1 : .5f; } } 
    #endregion

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
        DetectGround();
    }

    // SmoothDamp the input for fluid transitions
    private void ApplyAnimator()
    {
        if (_inputs.HasMovement)
        {
            Vector2 targetDir = new Vector2(_inputs.Movement.x, _inputs.Movement.z);

            #region Fix SmoothDamp 0 bug
            // Fix The smoothDamp 0 bug
            if (targetDir.x == 0)
                targetDir.x += .1f;
            if (targetDir.y == 0)
                targetDir.y += .1f; 
            #endregion

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
        DetectGround();
        ApplyPlayerRotation();
    }

    // Apply Spint Movement
    public void DoSprint()
    {
        _controller.Move(GetMoveDir(_sprintSpeed));
        DetectGround();
        ApplyPlayerRotation();
    }

    // Apply ySpeed for Jump counter gravity
    public void StartJump()
    {
        _ySpeed = Mathf.Sqrt(_jumpHeight * -3.0f * Physics.gravity.y);
        _groundCheckCoolDownEndTime = Time.time + _groundCheckCoolDownDuration;
        _isGrounded = false;
    }

    // Apply ySpeed to stop gravity
    public void DoGrounded()
    {
        _ySpeed = 0f;
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

    // reset Sneak speed Multiplier
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

    private void DetectGround()
    {
        // Si le cooldown de detection du sol est terminé
        if (CanCheckGround)
        {
            // On gère le snapping
            CheckSnapping();
            // On synchronise notre version locale de variable _isGrounded avec celle du CharacterController
            _isGrounded = _controller.isGrounded;
        }

        // Si le CharacterController est en contact avec le sol
        if (_controller.isGrounded)
        {
            // Alors on r�initialise la velocit� verticale artificielle � 0
            _ySpeed = 0f;
        }
    }
    private void CheckSnapping()
    {
        // Si on on vient de quitter le sol
        if (_isGrounded && !_controller.isGrounded)
        {
            // On tire un raycast sur la distance de chute pour savoir si un sol est detecté
            bool hitGround = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _fallSnapMaxHeight + _controller.skinWidth, _snappingGroundLayer);
            // On dessine le rayon dans la scène pour le Debug
            Debug.DrawRay(transform.position, Vector3.down * (_fallSnapMaxHeight + _controller.skinWidth), hitGround ? Color.green : Color.red, 1f);

            // Si un sol est detecté
            if (hitGround)
            {
                // On déplace le personnage à la même hauteur que le point touché sur le sol par le raycast
                _controller.Move(new Vector3(0f, hit.point.y - transform.position.y, 0f));
            }
        }
    }
}
