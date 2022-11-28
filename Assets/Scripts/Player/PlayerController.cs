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
    Vector3 playerVelocity;
    
    // Const
    const float gravityValue = -9.81f;

    // Public Proprieties
    public bool IsGrounded { get { return _controller.isGrounded; } }
    public bool IsAnticipationOver { get { return Time.time > _anticipationEndTimer; } }
    public bool IsRecoveryOver { get { return Time.time > _recoveryEndTimer; } }
    public float PlayerVelocityYAxis { get { return _controller.velocity.y; } }

    private void Awake()
    {
        _camera = Camera.main;
        _transform = transform;
        _inputs = GetComponent<PlayerInputs>();
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        _ySpeed += Physics.gravity.y * Time.deltaTime;
        _controller.Move(ApplyMovement());
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
        _ySpeed = Mathf.Sqrt(_jumpHeight * -3.0f * gravityValue);
    }

    public void DoJump()
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
    public void DoFall()
    {

    }

    /// <summary>
    /// Get the moveDir with playerInputs and Gravity
    /// </summary>
    /// <returns></returns>
    private Vector3 GetMoveDir(float speed)
    {
        moveDir = new Vector3(0, moveDir.y, 0);
        moveDir += _transform.forward * _inputs.Movement.x * speed * Time.deltaTime;
        moveDir += _transform.right * _inputs.Movement.z * speed * Time.deltaTime;
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
        Quaternion rotateToward = Quaternion.RotateTowards(_transform.rotation., cameraRotation, _degreeRotationSpeed * Time.deltaTime);
        //_transform.rotation = new Vector3(rotateToward, _transform.rotation.y, _transform.rotation.z);
    }
}
