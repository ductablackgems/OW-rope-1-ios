using UnityEngine;

public abstract class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] protected bool _dontDestroyOnLoad;
    private static object syncRoot = new object();

    protected static volatile T ins;

    //private static bool applicationIsQuitting = false;
    public static T instance
    {
        get
        {
            lock (syncRoot)
            {
                if (ins == null)
                {
                    ins = FindObjectOfType(typeof(T)) as T;
                    if (ins == null)
                    {
                        ins = new GameObject().AddComponent<T>();
                        ins.gameObject.name = ins.GetType().Name;
                    }
                }
            }

            return ins;
        }
    }

    public static bool IsNull
    {
        get { return ins == null; }
    }

    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    void Awake()
    {
        // check if there's another instance already exist in scene
        if (ins != null && ins.GetInstanceID() != this.GetInstanceID())
        {
            // Destroy this instances because already exist the singleton of EventsDispatcer
            //Common.Log("An instance of T already exist : <{0}>, So destroy this instance : <{1}>!!", instance.name, name);
            Destroy(gameObject);
        }
        else
        {
            // set instance
            ins = this as T;
        }

        if (_dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

        Init();
    }

    protected virtual void Init()
    {
    }
}