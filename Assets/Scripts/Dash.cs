using UnityEngine;
using UnityEngine.InputSystem;

public class Dash : MonoBehaviour {
    [SerializeField] private DashSettings settings;
    [SerializeField] private PlayerMotion motion;

    private float startTimestamp = float.MinValue;
    private Vector3 dashDirection;

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

        transform.position += settings.Speed * Time.deltaTime * dashDirection;
    }

    private void HandleDash(InputAction.CallbackContext context) {
        if (IsInCooldown)
            return;

        startTimestamp = Time.time;
        dashDirection = motion.ImmediateNonZeroDirection;
    }
}
