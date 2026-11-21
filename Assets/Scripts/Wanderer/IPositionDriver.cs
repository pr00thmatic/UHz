using UnityEngine;
using UnityEngine.AI;

public interface IPositionDriver {
    public event System.Action<IPositionDriver> OnPositionChangeRequest;
    public bool IsLocked { get; }

    public Vector3 GetPosition (Vector3 currentPosition);
    public Vector3 HandleNudgedPosition (Vector3 originalPosition, NavMeshHit hit);
}
