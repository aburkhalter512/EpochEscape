using UnityEngine;

public class ManagerContainer
{
	private static GameObject _go;

	private ManagerContainer() { }

	public static GameObject Get()
	{
		if (_go == null)
		{
			_go = new GameObject();
			_go.name = "Managers";
		}

		return _go;
	}
}

public abstract class Manager<T> : MonoBehaviour 
    where T : Component
{
    private static T m_instance = null;

    protected void Awake()
    {
        if (m_instance == null)
        {
            DontDestroyOnLoad(gameObject);

            m_instance = this as T;

            Awaken();
        }
        else
            Destroy(GetComponent<T>());
    }

    protected void Start()
    {
        Initialize();
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

				manager.transform.SetParent(ManagerContainer.Get().transform);

                m_instance = manager.AddComponent<T>();
            }
        }

        return m_instance;
    }

    public virtual void destroy()
    {
    	GameObject.Destroy(m_instance.gameObject);
    	m_instance = null;
    }

    //The following two methods provide full access to the gameobject's original functionality
    //and the start up order. Gameobject creation/referencing can be done exactly the same in
    //awaken and initialize as they would be done in Awake/Start.

    //Awaken is called when the singleton awake method is called for the first time
    protected abstract void Awaken();

    //Initialize is called when the singleton start method is called for the first time
    protected abstract void Initialize();
}
