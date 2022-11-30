using UnityEngine;

#region States

public enum PlayerMovement
{
    IDLE,
    WALKING,
    SPRINTING,
}

#endregion

public class PlayerMovementSM : MonoBehaviour
{
    private PlayerMovement _currentState;
    PlayerController _controller;
    PlayerInputs _inputs;
    PlayerStealthSM _stealthSM;
    Animator _animator;

    float _layerTest;

    #region Public properties

    public PlayerMovement CurrentState { get => _currentState; private set => _currentState = value; }

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

    private void OnStateEnter(PlayerMovement state)
    {
        switch (state)
        {
            case PlayerMovement.IDLE:
                OnEnterIdle();
                break;
            case PlayerMovement.WALKING:
                OnEnterWalking();
                break;
            case PlayerMovement.SPRINTING:
                OnEnterSprinting();
                break;
            default:
                Debug.LogError("OnStateEnter: Invalid state " + state.ToString());
                break;
        }
    }
    private void OnStateUpdate(PlayerMovement state)
    {
        switch (state)
        {
            case PlayerMovement.IDLE:
                OnUpdateIdle();
                break;
            case PlayerMovement.WALKING:
                OnUpdateWalking();
                break;
            case PlayerMovement.SPRINTING:
                OnUpdateSprinting();
                break;
            default:
                Debug.LogError("OnStateUpdate: Invalid state " + state.ToString());
                break;
        }
    }
    private void OnStateFixedUpdate(PlayerMovement state)
    {
        switch (state)
        {
            case PlayerMovement.IDLE:
                OnFixedUpdateIdle();
                break;
            case PlayerMovement.WALKING:
                OnFixedUpdateWalking();
                break;
            case PlayerMovement.SPRINTING:
                OnFixedUpdateSprinting();
                break;
            default:
                Debug.LogError("OnStateFixedUpdate: Invalid state " + state.ToString());
                break;
        }
    }
    private void OnStateExit(PlayerMovement state)
    {
        switch (state)
        {
            case PlayerMovement.IDLE:
                OnExitIdle();
                break;
            case PlayerMovement.WALKING:
                OnExitWalking();
                break;
            case PlayerMovement.SPRINTING:
                OnExitSprinting();
                break;
            default:
                Debug.LogError("OnStateExit: Invalid state " + state.ToString());
                break;
        }
    }
    private void TransitionToState(PlayerMovement toState)
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
                TransitionToState(PlayerMovement.SPRINTING);
            else
                TransitionToState(PlayerMovement.WALKING);
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
            TransitionToState(PlayerMovement.IDLE);
        else if (_inputs.AskingRunning && _stealthSM.CurrentState != PlayerStealth.SNEAKING)
            TransitionToState(PlayerMovement.SPRINTING);
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
            TransitionToState(PlayerMovement.IDLE);
        else if (_inputs.HasMovement && (!_inputs.AskingRunning || _stealthSM.CurrentState == PlayerStealth.SNEAKING))
            TransitionToState(PlayerMovement.WALKING);
    }
    private void OnFixedUpdateSprinting()
    {
    }
    private void OnExitSprinting()
    {

    }

    #endregion

}
