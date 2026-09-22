using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PlayerMotionSettings", menuName = "UHz/Player/MotionSettings")]
public class PlayerMotionSettings : ScriptableObject {
    [SerializeField] private InputActionReference motionInput;

    [field: SerializeField] public float Speed { get; private set; }

    public InputAction MotionInput => motionInput.action;
}
