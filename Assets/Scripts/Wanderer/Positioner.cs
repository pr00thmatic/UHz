using UnityEngine;

/// <summary>
/// Many behaviors can contest this object's position.
/// Positioner helps keep Geneva Convention respected in this contested territory xD
/// </summary>
public class Positioner : MonoBehaviour {
    [SerializeField] private GameObject[] positionDrivers;
    [field: SerializeField] public EnvironmentMotionRestrictions EnvironmentRestrictions { get; private set; }

    private IPositionDriver currentDriver;

    void OnEnable () {
        foreach (GameObject driverGameObject in positionDrivers) {
            if (!driverGameObject.TryGetComponent(out IPositionDriver driver))
                continue;

            driver.OnPositionChangeRequest += HandlePositionChangeRequest;
        }
    }

    void OnDisable () {
        foreach (GameObject driverGameObject in positionDrivers) {
            if (!driverGameObject.TryGetComponent(out IPositionDriver driver))
                continue;

            driver.OnPositionChangeRequest -= HandlePositionChangeRequest;
        }
    }

    public void HandlePositionChangeRequest (IPositionDriver driver) {
        if (driver != currentDriver && currentDriver != null && currentDriver.IsLocked)
            return;

        currentDriver = driver;
        Vector3 originalTargetPosition = currentDriver.GetPosition(transform.position);

        if (EnvironmentRestrictions.FindMotionNudges(transform.position, originalTargetPosition, out var hit))
            transform.position = currentDriver.HandleNudgedPosition(originalTargetPosition, hit);
        else
            transform.position = originalTargetPosition;
    }
}
