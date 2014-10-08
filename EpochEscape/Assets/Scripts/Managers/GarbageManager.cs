using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GarbageManager : Manager<GarbageManager>
{
	#region Instance Variables
    private Queue<Object> m_garbage = null;
	#endregion

    protected override void Initialize()
    {
        m_garbage = new Queue<Object>();
    }

    protected void Update()
    {
        if (m_garbage.Count > 0)
            Destroy(m_garbage.Dequeue());
    }

	#region Interface Methods
    public static void Add(ref GameObject obj)
    {
        GarbageManager.GetInstance()._Add(ref obj);
    }
	#endregion

	#region Instance Methods
    private void _Add(ref GameObject obj)
    {
        if (obj != null)
            m_garbage.Enqueue(obj);
    }
	#endregion
}
