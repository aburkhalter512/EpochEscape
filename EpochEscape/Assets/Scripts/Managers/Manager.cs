using UnityEngine;

public abstract class Manager<T> : MonoBehaviour 
    where T : Component
{
    private static T m_instance = null;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (m_instance == null)
        {
            m_instance = this as T;

            Initialize();
        }
        else
            Destroy(gameObject);
    }

    public static T Get()
    {
        if (m_instance == null)
        {
            m_instance = FindObjectOfType(typeof(T)) as T;

            if (m_instance == null)
            {
                GameObject manager = new GameObject();
                manager.name = typeof(T).ToString();

                m_instance = manager.AddComponent<T>();
            }
        }

        return m_instance;
    }

    protected abstract void Initialize();
}
