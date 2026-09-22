using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMotion : MonoBehaviour {
    [SerializeField] private PlayerMotionSettings settings;
    [SerializeField] private Dash dash;
    public Vector3 ImmediateNonZeroDirection { get; private set; }

    private Vector3 inputDirection;

    void OnEnable () {
        ImmediateNonZeroDirection = transform.forward;
    }

    void Update () {
        Vector2 input = settings.MotionInput.ReadValue<Vector2>();
        Vector3 inputDirection = input.y * CameraReferences.FloorForward + input.x * CameraReferences.FloorRight;
        if (inputDirection != Vector3.zero)
            ImmediateNonZeroDirection = inputDirection;

        if (dash.IsDashing)
            return;

        transform.position += settings.Speed * Time.deltaTime * inputDirection;
        transform.forward = ImmediateNonZeroDirection;
    }
}
