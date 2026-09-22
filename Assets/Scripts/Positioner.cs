using UnityEngine;

public class Positioner : MonoBehaviour
{
    [SerializeField] private GameObject[] positionDrivers;
    private IPositionDriver currentDriver;
    public GameObject current;

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
        current = (driver as MonoBehaviour).gameObject;
        transform.position = currentDriver.GetPosition(transform.position);
    }
}
