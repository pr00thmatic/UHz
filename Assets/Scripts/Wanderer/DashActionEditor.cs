using UnityEngine;

public partial class DashAction {
    private void OnDrawGizmos () {
        Gizmos.DrawLine(dashStart, FinalPosition);
    }
}
