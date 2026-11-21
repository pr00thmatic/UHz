using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "MotionSettings", menuName = "UHz/Actions/MotionSettings")]
public class MotionSettings : ScriptableObject {
    [SerializeField] private InputActionReference motionInput;

    [field: SerializeField] public float Speed { get; private set; }

    public InputAction MotionInput => motionInput.action;
}
