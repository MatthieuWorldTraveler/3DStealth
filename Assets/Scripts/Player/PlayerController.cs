using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Serialized
    [Header("Movement Manager")]
    [Space(10)]
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] float _sprintSpeed = 8f;
    [SerializeField] float _sneakSpeed = .5f;

    [Header("Jump Manager")]
    [Space(10)]
    [SerializeField]  float _jumpHeight = 1.0f;
    [SerializeField] float _anticipationTimer = .2f;
    [SerializeField] float _recoveryTimer = .2f;

    [Header("Camera Manager")]
    [Space(10)]
    [SerializeField] int _degreeRotationSpeed = 25;

    // variables
    float _anticipationEndTimer;
    float _recoveryEndTimer;
    float _ySpeed;
    Vector3 moveDir = Vector3.zero;

    // References
    CharacterController _controller;
    PlayerInputs _inputs;
    Transform _transform;
    Camera _camera;
    Animator _animator;
    Vector3 playerVelocity;

    // Public Proprieties
    public bool IsGrounded { get { return _controller.isGrounded; } }
    public bool IsAnticipationOver { get { return Time.time > _anticipationEndTimer; } }
    public bool IsRecoveryOver { get { return Time.time > _recoveryEndTimer; } }
    public float PlayerVelocityYAxis { get { return _controller.velocity.y; } }

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
        _ySpeed += Physics.gravity.y * Time.deltaTime;
        _controller.Move(ApplyMovement());
        _animator.SetFloat("VerticalDegrees", GetVerticalDirection());
        _animator.SetFloat("HorizontalInput", Input.GetAxisRaw("Mouse Y"));
    }

    public void DoWalk()
    {
        _controller.Move(GetMoveDir(_moveSpeed));
        ApplyPlayerRotation();
    }
    public void DoSprint()
    {
        _controller.Move(GetMoveDir(_sprintSpeed));
        ApplyPlayerRotation();
    }

    public void StartJump()
    {
        _ySpeed = Mathf.Sqrt(_jumpHeight * -3.0f * Physics.gravity.y);
    }

    public void DoJump()
    {

    }
    public void DoFall()
    {

    }
    public void DoGrounded()
    {
        _ySpeed = -.5f;
    }
    public void StartRecovery()
    {
        _recoveryEndTimer = Time.time + _recoveryTimer;
    }
    public void StartAnticipate()
    {
        _anticipationEndTimer = Time.time + _anticipationTimer;
    }
    public void DoSneak()
    {
        _controller.Move(GetMoveDir(_sneakSpeed));
        ApplyPlayerRotation();
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

    private Vector3 ApplyMovement()
    {
        moveDir = _transform.up * _ySpeed * Time.deltaTime;
        return moveDir;
    }

    private void ApplyPlayerRotation()
    {
        Quaternion cameraRotation = Quaternion.LookRotation(_camera.transform.forward);
        Vector3 rotateToward = Quaternion.RotateTowards(_transform.rotation, cameraRotation, _degreeRotationSpeed * Time.deltaTime).eulerAngles;
        rotateToward.z = rotateToward.x = 0;
        _transform.rotation = Quaternion.Euler(rotateToward);
    }

    private float GetVerticalDirection()
    {
        //float verticalDir = (180 / Mathf.PI) * Mathf.Atan2(_transform.forward.y - _camera.transform.forward.y, _transform.forward.x - _camera.transform.forward.x);
        float verticalDir = Vector3.Angle(transform.forward, _camera.transform.forward);
        return verticalDir;
    }
}
