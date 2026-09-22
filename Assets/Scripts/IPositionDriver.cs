using UnityEngine;

public interface IPositionDriver {
    public event System.Action<IPositionDriver> OnPositionChangeRequest;
    public Vector3 GetPosition (Vector3 currentPosition);
    public bool IsLocked { get; }
}
