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

    public AnimationCurve TransitionCurve => IsReadingDirection? settings.GraceTransition : settings.FinalTransition;
    public float NormalizedTime => IsReadingDirection? GraceNormalizedDashTime : FinalNormalizedDashTime;
    public float GraceNormalizedDashTime => Mathf.Clamp01(TimeInDash / settings.DirectionReadGraceTime);
    public float FinalNormalizedDashTime => Mathf.Clamp01((TimeInDash - settings.DirectionReadGraceTime) / (settings.Duration - settings.DirectionReadGraceTime));

    void OnEnable () => settings.Input.performed += HandleDash;
    void OnDisable () => settings.Input.performed -= HandleDash;

    void Update () {
        model.SetActive(!IsReadingDirection);

        if (!IsDashing) return;

        if (IsReadingDirection)
            dashDirection = motion.ImmediateNonZeroDirection;

        OnPositionChangeRequest?.Invoke(this);
    }

    public Vector3 GetPosition (Vector3 position) => !IsDashing? position :
        Vector3.Lerp(dashStart, FinalPosition, TransitionCurve.Evaluate(NormalizedTime));

    // TODO: handle animations :v
    public Vector3 HandleNudgedPosition (Vector3 originalPosition, Vector3 nudgedPosition)
    {
        if (originalPosition != nudgedPosition)
            StopDashing();

        return nudgedPosition;
    }

    private void HandleDash (InputAction.CallbackContext context) {
        if (IsInCooldown)
            return;

        startTimestamp = Time.time;
        dashStart = transform.position;
        dashDirection = motion.ImmediateNonZeroDirection;
    }

    private void StopDashing () => startTimestamp = -1;
}
