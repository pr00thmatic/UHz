using UnityEngine;

public class Positioner : MonoBehaviour {
    [SerializeField] private GameObject[] positionDrivers;
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
        transform.position = currentDriver.GetPosition(transform.position);
    }
}
