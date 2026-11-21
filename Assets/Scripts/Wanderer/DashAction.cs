using UnityEngine;
using UnityEngine.InputSystem;

public class DashAction : MonoBehaviour, IPositionDriver {
    public event System.Action<IPositionDriver> OnPositionChangeRequest;

    [SerializeField] private DashSettings settings;
    [SerializeField] private MoveAction motion;
    [SerializeField] private GameObject model;

    private float startTimestamp = float.MinValue;
    private float endTimestamp = float.MinValue;
    private Vector3 dashDirection;
    private Vector3 dashStart;

    public bool WasAborted => endTimestamp < startTimestamp + settings.Duration;
    public bool IsLocked => IsDashing;
    public float TimeInDash => Mathf.Max(Time.time - startTimestamp, startTimestamp - endTimestamp);
    public bool IsDashing => Time.time < endTimestamp;
    public bool IsInCooldown => endTimestamp + settings.Cooldown > Time.time;
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
            ForgiveDash(); // TODO: change back to AbortDash after exploring the wall-bonk Gemini proposed

        return nudgedPosition;
    }

    private void HandleDash (InputAction.CallbackContext context) {
        if (IsInCooldown)
            return;

        startTimestamp = Time.time;
        endTimestamp = Time.time + settings.Duration;
        dashStart = transform.position;
        dashDirection = motion.ImmediateNonZeroDirection;
    }

    /// <summary>
    /// actually resets the dash cooldown after stopping it
    /// </summary>
    private void ForgiveDash () => startTimestamp = -1;

    /// <summary>
    /// stops the dash while respecting the cooldown
    /// </summary>
    private void AbortDash () => endTimestamp = Time.time;
}
