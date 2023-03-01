using UnityEngine;

public class SingletonManager<T> : MonoBehaviour where T : SingletonManager<T>
{
    private static T _instance = null;
    private static object _syncobj = new object();
    private static bool appIsClosing = false;

    public static T instance
    {
        get
        {
            if (appIsClosing)
                return null;

            lock (_syncobj)
            {
                if (_instance == null)
                {
                    T[] objs = FindObjectsOfType<T>();

                    if (objs.Length > 0)
                        _instance = objs[0];

                    if (objs.Length > 1)
                        Debug.Log("There is more than one " + typeof(T).Name + " in the scene.");

                    if (_instance == null)
                    {
                        string goName = typeof(T).ToString();
                        GameObject a_go = GameObject.Find(goName);
                        if (a_go == null)
                            a_go = new GameObject(goName);
                        _instance = a_go.AddComponent<T>();
                    }
                    else
                    {
                        _instance.Init();
                    }
                }

                return _instance;
            }//lock (_syncobj)
        } // get
    }//public static T Instance

    public virtual void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(base.gameObject);
        }
        else
        {
            if (_instance != this)
            {
                DestroyImmediate(base.gameObject);
            }
        }
    }

    protected virtual void OnDestroy()
    {
        _instance = null;
    }

    protected virtual void OnApplicationQuit()
    {
        _instance = null;
        appIsClosing = true;
    }

    internal void CallInstance() { } //인스턴스 호출 전용
}

public class SingletonScene<T> : MonoBehaviour where T : SingletonScene<T>
{
    private static T _instance = null;
    private static object _syncobj = new object();
    private static bool appIsClosing = false;

    public static T instance
    {
        get
        {
            if (appIsClosing)
                return null;

            lock (_syncobj)
            {
                if (_instance == null)
                {
                    T[] objs = FindObjectsOfType<T>();

                    if (objs.Length > 0)
                        _instance = objs[0];

                    if (objs.Length > 1)
                        Debug.Log("There is more than one " + typeof(T).Name + " in the scene.");

                    if (_instance == null)
                    {
                        string goName = typeof(T).ToString();
                        GameObject a_go = GameObject.Find(goName);
                        if (a_go == null)
                            a_go = new GameObject(goName);
                        _instance = a_go.AddComponent<T>();
                    }
                    else
                    {
                        _instance.Init();
                    }
                }

                return _instance;
            }//lock (_syncobj)
        } // get
    }//public static T Instance

    public virtual void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        else
        {
            if (_instance != this)
            {
                DestroyImmediate(base.gameObject);
            }
        }
    }

    protected void OnDestroy()
    {
        _instance = null;
    }

    protected void OnApplicationQuit()
    {
        _instance = null;
        appIsClosing = true;
    }

    internal void CallInstance() { } //인스턴스 호출 전용
}