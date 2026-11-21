using UnityEngine;
using UnityEngine.AI;

public class EnvironmentMotionRestrictions : MonoBehaviour {
    public bool FindMotionNudges (Vector3 origin, Vector3 destination, out NavMeshHit hit)
        => NavMesh.Raycast(origin, destination, out hit, NavMesh.AllAreas);
}
