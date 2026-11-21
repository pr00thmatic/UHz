using UnityEngine;
using UnityEngine.AI;

public class EnvironmentMotionRestrictions : MonoBehaviour {
    private NavMeshHit hit;

    public Vector3 NudgedMotion (Vector3 origin, Vector3 destination) {
        if (NavMesh.Raycast(origin, destination, out hit, NavMesh.AllAreas))
            return hit.position;

        return destination;
    }
}
