using UnityEngine;

public class SingletonMonobehaviour<T> : MonoBehaviour where T : SingletonMonobehaviour<T>
{
    private static T instance = default(T);
    public static T Instance { get { return instance; } }

    public bool dontDestroyOnLoad = false;

    private void Awake()
    {
        instance = this as T;
        if (dontDestroyOnLoad) DontDestroyOnLoad(instance.gameObject);
    }
}
