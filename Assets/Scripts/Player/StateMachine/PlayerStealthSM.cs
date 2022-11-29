using UnityEngine;

#region States

public enum PlayerStealth
{
    STANDING,
    SNEAKING,
}

#endregion

public class PlayerStealthSM : MonoBehaviour
{
    private PlayerStealth _currentState;
    PlayerAirSM _playerAirSM;
    PlayerInputs _inputs;
    PlayerController _controller;
    Animator _animator;

    #region Public properties

    public PlayerStealth CurrentState { get => _currentState; private set => _currentState = value; }

    #endregion

    #region Unity Life Cycles

    private void Start()
    {
        _playerAirSM = GetComponent<PlayerAirSM>();
        _inputs = GetComponent<PlayerInputs>();
        _controller = GetComponent<PlayerController>();
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

    private void OnStateEnter(PlayerStealth state)
    {
        switch (state)
        {
            case PlayerStealth.STANDING:
                OnEnterStanding();
                break;
            case PlayerStealth.SNEAKING:
                OnEnterSneaking();
                break;
            default:
                Debug.LogError("OnStateEnter: Invalid state " + state.ToString());
                break;
        }
    }
    private void OnStateUpdate(PlayerStealth state)
    {
        switch (state)
        {
            case PlayerStealth.STANDING:
                OnUpdateStanding();
                break;
            case PlayerStealth.SNEAKING:
                OnUpdateSneaking();
                break;
        }
    }
    private void OnStateFixedUpdate(PlayerStealth state)
    {
        switch (state)
        {
            case PlayerStealth.STANDING:
                OnFixedUpdateStanding();
                break;
            case PlayerStealth.SNEAKING:
                OnFixedUpdateSneaking();
                break;
            default:
                Debug.LogError("OnStateFixedUpdate: Invalid state " + state.ToString());
                break;
        }
    }
    private void OnStateExit(PlayerStealth state)
    {
        switch (state)
        {
            case PlayerStealth.STANDING:
                OnExitStanding();
                break;
            case PlayerStealth.SNEAKING:
                OnExitSneaking();
                break;
            default:
                Debug.LogError("OnStateExit: Invalid state " + state.ToString());
                break;
        }
    }
    private void TransitionToState(PlayerStealth toState)
    {
        OnStateExit(CurrentState);
        CurrentState = toState;
        OnStateEnter(toState);
    }

    #endregion

    #region State STANDING

    private void OnEnterStanding()
    {
    }
    private void OnUpdateStanding()
    {
        // Do Nothing

        // Transitions
        if (_inputs.AskingSneaking && _playerAirSM.CurrentState == PlayerAir.GROUNDED)
            TransitionToState(PlayerStealth.SNEAKING);
    }
    private void OnFixedUpdateStanding()
    {

    }
    private void OnExitStanding()
    {

    }

    #endregion

    #region State SNEAKING

    private void OnEnterSneaking()
    {
        _controller.StartSneak();
    }
    private void OnUpdateSneaking()
    {
        // Do Nothing

        // Transitions
        if (!_inputs.AskingSneaking || _playerAirSM.CurrentState == PlayerAir.FALLING)
            TransitionToState(PlayerStealth.STANDING);
    }
    private void OnFixedUpdateSneaking()
    {
    }
    private void OnExitSneaking()
    {
        _controller.ExitSneak();
    }

    #endregion

}
