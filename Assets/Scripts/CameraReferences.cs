using UnityEngine;

public class CameraReferences : SingletonMonoBehaviour<CameraReferences> {
    [field: SerializeField] public Camera MainCamera { get; private set; }
    public static Vector3 FloorForward => Vector3.ProjectOnPlane(Instance.MainCamera.transform.forward, Vector3.up).normalized;
    public static Vector3 FloorRight => Vector3.ProjectOnPlane(Instance.MainCamera.transform.right, Vector3.up).normalized;
}
