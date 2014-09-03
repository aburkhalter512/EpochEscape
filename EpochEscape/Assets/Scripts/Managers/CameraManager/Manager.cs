using UnityEngine;

public class Manager<T> : MonoBehaviour where T : Component
{
	private static T m_instance = null;

	public virtual void Awake()
	{
		DontDestroyOnLoad(gameObject);

		if(m_instance == null)
			m_instance = this as T;
		else
			Destroy(gameObject);
	}

	protected static T GetInstance()
	{
		if(m_instance == null)
		{
			m_instance = FindObjectOfType(typeof(T)) as T;

			if(m_instance == null)
			{
				GameObject obj = new GameObject();

				m_instance = obj.AddComponent<T>();
			}
		}

		return m_instance;
	}

    protected static bool IsInstantiated()
    {
        return m_instance != null;
    }
}
