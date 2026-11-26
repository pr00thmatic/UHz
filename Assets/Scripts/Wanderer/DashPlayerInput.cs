using UnityEngine;
using UnityEngine.InputSystem;

public class DashPlayerInput : MonoBehaviour
{
    [SerializeField] private DashAction dash;
    [SerializeField] private MoveAction motion;

    private int latestCommandedDash = 0;

    private void OnEnable () => dash.Settings.Input.performed += HandleInput;
    private void OnDisable () => dash.Settings.Input.performed -= HandleInput;

    private void Update () {
        if (dash.CurrentDashId != latestCommandedDash || !dash.IsDashing)
            return;

        if (dash.TimeInDash > dash.Settings.DirectionReadGraceTime)
            return;

        dash.SetDirection(motion.ImmediateNonZeroDirection);
    }

    private void HandleInput (InputAction.CallbackContext context)
    {
        if (dash.IsInCooldown)
            return;

        latestCommandedDash = dash.Dash(motion.ImmediateNonZeroDirection);
    }
}
