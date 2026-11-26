using UnityEngine;

public partial class DashWallBounce {
    private void OnDrawGizmos ()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(origin, origin + normal * 3);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + reflection * 3);
    }
}
