using UnityEngine;

#region States

public enum PlayerAir
{
    GROUNDED,
    ANTICIPATING,
    JUMPING,
    FALLING,
    RECOVERY,
}

#endregion

public class PlayerAirSM : MonoBehaviour
{
    private PlayerAir _currentState;
    PlayerInputs _inputs;
    PlayerStealthSM _stealthSM;
    PlayerController _controller;
    Animator _animator;

#region Public properties

    public PlayerAir CurrentState { get => _currentState; private set => _currentState = value; }

#endregion

#region Unity Life Cycles

    private void Start()
    {
        _controller = GetComponent<PlayerController>();
        _inputs = GetComponent<PlayerInputs>();
        _stealthSM = GetComponent<PlayerStealthSM>();
        _animator = GetComponentInChildren<Animator>();
    }
    private void Update()
    {
        OnStateUpdate(CurrentState);
    }
    private void FixedUpdate()
    {
        OnStateFixedUpdate(CurrentState);
    }

#endregion

#region State Machine

    private void OnStateEnter(PlayerAir state)
    {
        switch (state)
        {
            case PlayerAir.GROUNDED:
                OnEnterGrounded();
                break;
            case PlayerAir.ANTICIPATING:
                OnEnterAnticipating();
                break;
            case PlayerAir.JUMPING:
                OnEnterJumping();
                break;
            case PlayerAir.FALLING:
                OnEnterFalling();
                break;
            case PlayerAir.RECOVERY:
                OnEnterRecovery();
                break;
            default:
                Debug.LogError("OnStateEnter: Invalid state " + state.ToString());
                break;
        }
    }
    private void OnStateUpdate(PlayerAir state)
    {
        switch (state)
        {
            case PlayerAir.GROUNDED:
                OnUpdateGrounded();
                break;
            case PlayerAir.ANTICIPATING:
                OnUpdateAnticipating();
                break;
            case PlayerAir.JUMPING:
                OnUpdateJumping();
                break;
            case PlayerAir.FALLING:
                OnUpdateFalling();
                break;
            case PlayerAir.RECOVERY:
                OnUpdateRecovery();
                break;
        }
    }
    private void OnStateFixedUpdate(PlayerAir state)
    {
        switch (state)
        {
            case PlayerAir.GROUNDED:
                OnFixedUpdateGrounded();
                break;
            case PlayerAir.ANTICIPATING:
                OnFixedUpdateAnticipating();
                break;
            case PlayerAir.JUMPING:
                OnFixedUpdateJumping();
                break;
            case PlayerAir.FALLING:
                OnFixedUpdateFalling();
                break;
            case PlayerAir.RECOVERY:
                OnFixedUpdateRecovery();
                break;
            default:
                Debug.LogError("OnStateFixedUpdate: Invalid state " + state.ToString());
                break;
        }
    }
    private void OnStateExit(PlayerAir state)
    {
        switch (state)
        {
            case PlayerAir.GROUNDED:
                OnExitGrounded();
                break;
            case PlayerAir.ANTICIPATING:
                OnExitAnticipating();
                break;
            case PlayerAir.JUMPING:
                OnExitJumping();
                break;
            case PlayerAir.FALLING:
                OnExitFalling();
                break;
            case PlayerAir.RECOVERY:
                OnExitRecovery();
                break;
            default:
                Debug.LogError("OnStateExit: Invalid state " + state.ToString());
                break;
        }
    }
    private void TransitionToState(PlayerAir toState)
    {
        OnStateExit(CurrentState);
        CurrentState = toState;
        OnStateEnter(toState);
    }

#endregion

#region State GROUNDED

    private void OnEnterGrounded()
    {
    }
    private void OnUpdateGrounded()
    {
        // Do Grounded
        _controller.DoGrounded();

        // Transitions
        if (_controller.PlayerVelocityYAxis < -.1f)
            TransitionToState(PlayerAir.FALLING);
        else if (_inputs.AskingJumping && _stealthSM.CurrentState != PlayerStealth.SNEAKING)
            TransitionToState(PlayerAir.ANTICIPATING);
    }
    private void OnFixedUpdateGrounded()
    {
    }
    private void OnExitGrounded()
    {
    }

#endregion

#region State ANTICIPATING

    private void OnEnterAnticipating()
    {
        _animator.SetBool("Anticipating", true);

        // Start Anticipate
        _controller.StartAnticipate();
    }
    private void OnUpdateAnticipating()
    {
        // Do Nothing

        // Transitions
        if (_controller.PlayerVelocityYAxis < -.1f)
            TransitionToState(PlayerAir.FALLING);
        else if (_controller.IsAnticipationOver)
            TransitionToState(PlayerAir.JUMPING);
    }
    private void OnFixedUpdateAnticipating()
    {
    }
    private void OnExitAnticipating()
    {
        _animator.SetBool("Anticipating", false);
    }

#endregion

#region State JUMPING

    private void OnEnterJumping()
    {
        _animator.SetBool("Jumping", true);

        _controller.StartJump();
    }
    private void OnUpdateJumping()
    {
        // Do Jump
        _controller.DoJump();

        // Transitions
        if (_controller.IsGrounded)
            TransitionToState(PlayerAir.RECOVERY);
        else if (_controller.PlayerVelocityYAxis < -.1f)
            TransitionToState(PlayerAir.FALLING);
    }
    private void OnFixedUpdateJumping()
    {
    }
    private void OnExitJumping()
    {
        _animator.SetBool("Jumping", false);
    }

#endregion

#region State FALLING

    private void OnEnterFalling()
    {
        _animator.SetBool("Falling", true);
    }
    private void OnUpdateFalling()
    {
        // Do Fall
        _controller.DoFall();

        // Transitions
        if (_controller.IsGrounded)
            TransitionToState(PlayerAir.RECOVERY);
    }
    private void OnFixedUpdateFalling()
    {
    }
    private void OnExitFalling()
    {
        _animator.SetBool("Falling", false);
    }

#endregion

#region State RECOVERY

    private void OnEnterRecovery()
    {
        _animator.SetBool("Recovering", true);

        // Start Recovery
        _controller.StartRecovery();
    }
    private void OnUpdateRecovery()
    {
        // Do Nothing

        // Transitions
        if (_controller.IsRecoveryOver)
            TransitionToState(PlayerAir.GROUNDED);
    }
    private void OnFixedUpdateRecovery()
    {
    }
    private void OnExitRecovery()
    {
        _animator.SetBool("Recovering", false);
    }

#endregion

}
