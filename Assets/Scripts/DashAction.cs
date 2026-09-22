using UnityEngine;
using UnityEngine.InputSystem;

public class DashAction : MonoBehaviour, IPositionDriver {
    public event System.Action<IPositionDriver> OnPositionChangeRequest;

    [SerializeField] private DashSettings settings;
    [SerializeField] private MoveAction motion;
    [SerializeField] private GameObject model;

    private float startTimestamp = float.MinValue;
    private Vector3 dashDirection;
    private Vector3 dashStart;

    public bool IsLocked => IsDashing;
    public float TimeInDash => Time.time - startTimestamp;
    public bool IsDashing => TimeInDash < settings.Duration;
    public bool IsInCooldown => (startTimestamp + settings.Duration + settings.Cooldown) > Time.time;
    public bool IsReadingDirection => TimeInDash < settings.DirectionReadGraceTime;
    public Vector3 FinalPosition => dashStart + dashDirection.normalized * settings.Distance;

    public float GraceNormalizedDashTime => Mathf.Clamp01(TimeInDash / settings.DirectionReadGraceTime);
    public float FinalNormalizedDashTime => Mathf.Clamp01((TimeInDash - settings.DirectionReadGraceTime) / (settings.Duration - settings.DirectionReadGraceTime));

    void OnEnable () {
        settings.Input.performed += HandleDash;
    }

    void OnDisable () {
        settings.Input.performed -= HandleDash;
    }

    void Update () {
        model.SetActive(!IsReadingDirection);

        if (!IsDashing) return;

        if (IsReadingDirection)
            dashDirection = motion.ImmediateNonZeroDirection;

        OnPositionChangeRequest?.Invoke(this);
    }

    public Vector3 GetPosition(Vector3 position) {
        if (!IsDashing)
            return position;

        return Vector3.Lerp(dashStart, FinalPosition, IsReadingDirection ? 
            settings.GraceTransition.Evaluate(GraceNormalizedDashTime) : 
            settings.FinalTransition.Evaluate(FinalNormalizedDashTime));
    }

    private void HandleDash(InputAction.CallbackContext context) {
        if (IsInCooldown)
            return;

        startTimestamp = Time.time;
        dashStart = transform.position;
        dashDirection = motion.ImmediateNonZeroDirection;
    }
}
