using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    #region Gestion Input Axes Vertical / Horizontal
    // Axe deplacement
    private float _movementX;
    private float _movementZ;

    public Vector3 Movement { get => new Vector3(_movementX, 0, _movementZ); }
    public Vector3 NormalizedMovement { get => Movement.normalized; }
    public Vector3 ClampedMovement { get => Vector3.ClampMagnitude(Movement, 1f); }
    public bool HasMovement { get => Movement != Vector3.zero; }
    #endregion

    #region Gestion Input autre
    private bool _askingRunning;
    private bool _askingJumping;
    private bool _askingSneaking;

    public bool AskingRunning { get => _askingRunning; }
    public bool AskingJumping { get => _askingJumping; }
    public bool AskingSneaking { get => _askingSneaking; }
    #endregion

    private void Update()
    {
        // Stockage Mouvement
        _movementX = Input.GetAxisRaw("Vertical");
        _movementZ = Input.GetAxisRaw("Horizontal");

        // Stockage inputs
        _askingRunning = Input.GetButton("Run");
        _askingJumping = Input.GetButtonDown("Jump");
        _askingSneaking = Input.GetButton("Sneak");
    }
}
