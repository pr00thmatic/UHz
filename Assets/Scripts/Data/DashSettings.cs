using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "DashSettings", menuName = "UHz/Actions/DashSettings")]
public class DashSettings : ScriptableObject {
    [SerializeField] private InputActionReference input;

    [field: SerializeField] public float DirectionReadGraceTime { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public float Duration { get; private set; }
    [field: SerializeField] public float Cooldown { get; private set; }

    public InputAction Input => input.action;
}
