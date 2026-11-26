using UnityEngine;
using UnityEngine.AI;

public partial class DashAction : MonoBehaviour, IPositionDriver {
    public event System.Action<IPositionDriver> OnPositionChangeRequest;
    public event System.Func<Vector3, NavMeshHit, Vector3> OnNudgedPosition;

    private int dashId;
    private float startTimestamp = float.MinValue;
    private float endTimestamp = float.MinValue;
    private float endCooldown = float.MinValue;
    private Vector3 dashStart;

    [field: SerializeField] public DashSettings Settings { get; private set; }

    public int CurrentDashId => dashId;
    public bool WasAborted => endTimestamp < startTimestamp + Settings.Duration;
    public bool IsLocked => IsDashing;
    public float TimeInDash => Mathf.Min(Time.time - startTimestamp, endTimestamp - startTimestamp);
    public bool IsDashing => Time.time < endTimestamp;
    public bool IsInCooldown => endCooldown > Time.time;
    public Vector3 FinalPosition => dashStart + DashDirection * Settings.Distance;
    public Vector3 DashDirection { get; private set; }
    public float NormalizedTime => Mathf.Clamp01(TimeInDash / Settings.Duration);

    void Update () {
        if (IsDashing)
            OnPositionChangeRequest?.Invoke(this);
    }

    public void SetDirection (Vector3 direction) => DashDirection = direction.normalized;

    public Vector3 GetPosition (Vector3 position) => !IsDashing? position :
        Vector3.Lerp(dashStart, FinalPosition, Settings.TimeVsMotion.Evaluate(NormalizedTime));

    // TODO: handle animations :v
    public Vector3 HandleNudgedPosition (Vector3 originalPosition, NavMeshHit hit)
        => OnNudgedPosition == null? hit.position : OnNudgedPosition.Invoke(originalPosition, hit);

    public int Dash (Vector3 direction)
    {
        startTimestamp = Time.time;
        endTimestamp = Time.time + Settings.Duration;
        endCooldown = endTimestamp + Settings.Cooldown;
        dashStart = transform.position;
        DashDirection = direction.normalized;
        return ++dashId;
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
