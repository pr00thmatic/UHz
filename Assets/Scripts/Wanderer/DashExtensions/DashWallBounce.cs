using UnityEngine;
using UnityEngine.AI;

public partial class DashWallBounce : MonoBehaviour {
    [SerializeField] private DashAction manager;

    Vector3 normal;
    Vector3 reflection;
    Vector3 origin;

    void OnEnable () => manager.OnNudgedPosition += HandleNudgedPosition;
    void OnDisable () => manager.OnNudgedPosition -= HandleNudgedPosition;

    public Vector3 HandleNudgedPosition (Vector3 originalPosition, NavMeshHit hit) {
        origin = hit.position;
        normal = hit.normal;
        reflection = manager.DashDirection - 2 * Vector3.Dot(manager.DashDirection, hit.normal) * hit.normal;

        manager.ForgiveDash();
        manager.Dash(reflection);

        return hit.position;
    }
}
