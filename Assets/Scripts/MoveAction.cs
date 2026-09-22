using UnityEngine;

public class MoveAction : MonoBehaviour, IPositionDriver {
    public event System.Action<IPositionDriver> OnPositionChangeRequest;

    public Vector3 ImmediateNonZeroDirection { get; private set; }
    public bool IsLocked => false;

    [SerializeField] private MotionSettings settings;

    private Vector3 inputDirection;

    void OnEnable () {
        ImmediateNonZeroDirection = transform.forward;
    }

    void Update () {
        Vector2 input = settings.MotionInput.ReadValue<Vector2>();
        inputDirection = input.y * CameraReferences.FloorForward + input.x * CameraReferences.FloorRight;

        if (inputDirection != Vector3.zero)
            ImmediateNonZeroDirection = inputDirection;

        OnPositionChangeRequest?.Invoke(this);
    }

    public Vector3 GetPosition (Vector3 position) => position + settings.Speed * Time.deltaTime * inputDirection;
}
