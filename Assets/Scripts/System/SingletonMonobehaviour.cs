using UnityEngine;

public class SingletonMonobehaviour<T> : MonoBehaviour where T : SingletonMonobehaviour<T>
{
    private T instance = default(T);
    public T Instance { get { return instance; } }

    public bool dontDestroyOnLoad = false;

    private void Awake()
    {
        instance = this as T;
        if (dontDestroyOnLoad) DontDestroyOnLoad(instance.gameObject);
    }
}
