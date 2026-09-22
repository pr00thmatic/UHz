using UnityEngine.Assertions;
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T> {
    protected static T m_instance;
    public static T Instance {
        get {
            Assert.IsTrue(Lifetime.AreSingletonInitialized, "attempted to retrieve instance before singleton initialization");
            Assert.IsTrue(m_instance != null, "instance has not yet been set");
            return m_instance;
        }
    }

    protected void Awake () {
        if (Lifetime.AreSingletonInitialized) {
            HandleSingletonInitialization();
            return;
        }

        Lifetime.OnSingletonInitialization += HandleSingletonInitialization;
    }

    protected void Destroy () {
        if (m_instance == (T) this) {
            m_instance = null;
        }

        Lifetime.OnSingletonInitialization -= HandleSingletonInitialization;
    }

    protected void HandleSingletonInitialization () {
        Lifetime.OnSingletonInitialization -= HandleSingletonInitialization;

        Assert.IsTrue(m_instance == null, "there must only be 1 instance of a singleton");

        if (m_instance != null) {
            Destroy(this);
            return;
        }

        m_instance = (T) this;
    }
}
