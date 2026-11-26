using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public partial class DashAction : MonoBehaviour, IPositionDriver {
    public event System.Action<IPositionDriver> OnPositionChangeRequest;
    public event System.Func<Vector3, NavMeshHit, Vector3> OnNudgedPosition;

    [SerializeField] private DashSettings settings;
    [SerializeField] private MoveAction motion;
    [SerializeField] private GameObject model;

    private float startTimestamp = float.MinValue;
    private float endTimestamp = float.MinValue;
    private float endCooldown = float.MinValue;
    private Vector3 dashDirection;
    private Vector3 dashStart;
    private bool malleableDirection = false;

    public bool WasAborted => endTimestamp < startTimestamp + settings.Duration;
    public bool IsLocked => IsDashing;
    public float TimeInDash => Mathf.Min(Time.time - startTimestamp, endTimestamp - startTimestamp);
    public bool IsDashing => Time.time < endTimestamp;
    public bool IsInCooldown => endCooldown > Time.time;
    public bool IsBlinking => IsDashing && TimeInDash < settings.DirectionReadGraceTime;
    public bool IsReadingDirection => malleableDirection && IsBlinking;
    public Vector3 FinalPosition => dashStart + dashDirection.normalized * settings.Distance;
    public Vector3 DashDirection => dashDirection;

    public AnimationCurve TransitionCurve => IsBlinking? settings.GraceTransition : settings.FinalTransition;
    public float NormalizedTime => IsBlinking? GraceNormalizedDashTime : FinalNormalizedDashTime;
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
    public Vector3 HandleNudgedPosition (Vector3 originalPosition, NavMeshHit hit)
        => OnNudgedPosition == null? hit.position : OnNudgedPosition.Invoke(originalPosition, hit);

    private void HandleDash (InputAction.CallbackContext context) {
        if (IsInCooldown)
            return;

        Dash(motion.ImmediateNonZeroDirection, malleableDirection: true);
    }

    public void Dash (Vector3 direction, bool malleableDirection)
    {
        this.malleableDirection = malleableDirection;
        startTimestamp = Time.time;
        endTimestamp = Time.time + settings.Duration;
        endCooldown = endTimestamp + settings.Cooldown;
        dashStart = transform.position;
        dashDirection = direction.normalized;
    }

    /// <summary>
    /// actually resets the dash cooldown after stopping it
    /// </summary>
    public void ForgiveDash ()
    {
        AbortDash();
        endCooldown = Time.time;
    }

    /// <summary>
    /// stops the dash while respecting the cooldown
    /// </summary>
    public void AbortDash () => endTimestamp = Time.time;
}
