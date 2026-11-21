using UnityEngine;

[DefaultExecutionOrder(100)]
public class Lifetime : MonoBehaviour
{
    public static event System.Action OnSingletonInitialization;
    public static bool AreSingletonInitialized { get; private set; }

    void Awake ()
    {
        OnSingletonInitialization?.Invoke();
        AreSingletonInitialized = true;
    }
}
