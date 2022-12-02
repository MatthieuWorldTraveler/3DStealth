using UnityEngine;

#region States

public enum PlayerMovementState
{
    IDLE,
    WALKING,
    SPRINTING,
}

#endregion

public class PlayerMovementSM : MonoBehaviour
{
    private PlayerMovementState _currentState;
    PlayerController _controller;
    PlayerInputs _inputs;
    PlayerStealthSM _stealthSM;
    Animator _animator;

    float _layerTest;

    #region Public properties

    public PlayerMovementState CurrentState { get => _currentState; private set => _currentState = value; }

    #endregion

    #region Unity Life Cycles

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _stealthSM = GetComponent<PlayerStealthSM>();
        _inputs = GetComponent<PlayerInputs>();
        _controller = GetComponent<PlayerController>();
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

    private void OnStateEnter(PlayerMovementState state)
    {
        switch (state)
        {
            case PlayerMovementState.IDLE:
                OnEnterIdle();
                break;
            case PlayerMovementState.WALKING:
                OnEnterWalking();
                break;
            case PlayerMovementState.SPRINTING:
                OnEnterSprinting();
                break;
            default:
                Debug.LogError("OnStateEnter: Invalid state " + state.ToString());
                break;
        }
    }
    private void OnStateUpdate(PlayerMovementState state)
    {
        switch (state)
        {
            case PlayerMovementState.IDLE:
                OnUpdateIdle();
                break;
            case PlayerMovementState.WALKING:
                OnUpdateWalking();
                break;
            case PlayerMovementState.SPRINTING:
                OnUpdateSprinting();
                break;
            default:
                Debug.LogError("OnStateUpdate: Invalid state " + state.ToString());
                break;
        }
    }
    private void OnStateFixedUpdate(PlayerMovementState state)
    {
        switch (state)
        {
            case PlayerMovementState.IDLE:
                OnFixedUpdateIdle();
                break;
            case PlayerMovementState.WALKING:
                OnFixedUpdateWalking();
                break;
            case PlayerMovementState.SPRINTING:
                OnFixedUpdateSprinting();
                break;
            default:
                Debug.LogError("OnStateFixedUpdate: Invalid state " + state.ToString());
                break;
        }
    }
    private void OnStateExit(PlayerMovementState state)
    {
        switch (state)
        {
            case PlayerMovementState.IDLE:
                OnExitIdle();
                break;
            case PlayerMovementState.WALKING:
                OnExitWalking();
                break;
            case PlayerMovementState.SPRINTING:
                OnExitSprinting();
                break;
            default:
                Debug.LogError("OnStateExit: Invalid state " + state.ToString());
                break;
        }
    }
    private void TransitionToState(PlayerMovementState toState)
    {
        OnStateExit(CurrentState);
        CurrentState = toState;
        OnStateEnter(toState);
    }

    #endregion

    #region State IDLE

    private void OnEnterIdle()
    {
        _animator.SetBool("HasMovement", false);
    }
    private void OnUpdateIdle()
    {
        ////_animator.SetFloat("Speed", 0.1f, .1f, Time.deltaTime);
        float i = Mathf.SmoothDamp(_animator.GetLayerWeight(2), 0, ref _layerTest, .2f);
        _animator.SetLayerWeight(2, i);

        // Do Nothing on idle
        
        // Transitions
        if (_inputs.HasMovement)
        {
            if (_inputs.AskingRunning && _stealthSM.CurrentState != PlayerStealth.SNEAKING)
                TransitionToState(PlayerMovementState.SPRINTING);
            else
                TransitionToState(PlayerMovementState.WALKING);
        }
    }
    private void OnFixedUpdateIdle()
    {
    }
    private void OnExitIdle()
    {
        _animator.SetBool("HasMovement", true);
    }

    #endregion

    #region State WALKING
    private void OnEnterWalking()
    {

    }
    private void OnUpdateWalking()
    {
        ////_animator.SetFloat("Speed", 1 * _controller.SneakSpeedAnim, .1f, Time.deltaTime);
        float i = Mathf.SmoothDamp(_animator.GetLayerWeight(2), 0, ref _layerTest, .2f);
        _animator.SetLayerWeight(2, i);

        // Do walk
        _controller.DoWalk();

        // Transitions
        if (!_inputs.HasMovement)
            TransitionToState(PlayerMovementState.IDLE);
        else if (_inputs.AskingRunning && _stealthSM.CurrentState != PlayerStealth.SNEAKING)
            TransitionToState(PlayerMovementState.SPRINTING);
    }
    private void OnFixedUpdateWalking()
    {
    }
    private void OnExitWalking()
    {
    }

    #endregion

    #region State SPRINTING

    private void OnEnterSprinting()
    {

    }
    private void OnUpdateSprinting()
    {
        ////_animator.SetFloat("Speed", 2, .1f, Time.deltaTime);
        float i = Mathf.SmoothDamp(_animator.GetLayerWeight(2), 1, ref _layerTest, .2f);
        _animator.SetLayerWeight(2, i);

        // Do Sprint
        _controller.DoSprint();

        // Transitions
        if (!_inputs.HasMovement || _stealthSM.CurrentState == PlayerStealth.SNEAKING)
            TransitionToState(PlayerMovementState.IDLE);
        else if (_inputs.HasMovement && (!_inputs.AskingRunning || _stealthSM.CurrentState == PlayerStealth.SNEAKING))
            TransitionToState(PlayerMovementState.WALKING);
    }
    private void OnFixedUpdateSprinting()
    {
    }
    private void OnExitSprinting()
    {

    }

    #endregion

}
