using UnityEngine;
using UnityEngine.InputSystem;

public class DashAction : MonoBehaviour, IPositionDriver {
    public event System.Action<IPositionDriver> OnPositionChangeRequest;

    [SerializeField] private DashSettings settings;
    [SerializeField] private MoveAction motion;

    private float startTimestamp = float.MinValue;
    private Vector3 dashDirection;

    public bool IsLocked => IsDashing;
    public float TimeInDash => Time.time - startTimestamp;
    public bool IsDashing => TimeInDash < settings.Duration;
    public bool IsInCooldown => (startTimestamp + settings.Duration + settings.Cooldown) > Time.time;
    public bool IsReadingDirection => TimeInDash < settings.DirectionReadGraceTime;

    void OnEnable () {
        settings.Input.performed += HandleDash;
    }

    void OnDisable () {
        settings.Input.performed -= HandleDash;
    }

    void Update () {
        if (!IsDashing) return;

        if (IsReadingDirection)
            dashDirection = motion.ImmediateNonZeroDirection;

        OnPositionChangeRequest?.Invoke(this);
    }

    public Vector3 GetPosition(Vector3 position)
        => IsDashing? settings.Speed * Time.deltaTime * dashDirection + position : position;

    private void HandleDash(InputAction.CallbackContext context) {
        if (IsInCooldown)
            return;

        startTimestamp = Time.time;
        dashDirection = motion.ImmediateNonZeroDirection;
    }
}
