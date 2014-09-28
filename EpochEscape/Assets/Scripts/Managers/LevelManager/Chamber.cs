using UnityEngine;

public class Chamber
{
    private GameObject m_chamberObject;
    private Vector2 m_chamberSize;

    private static int s_chamberCount = 0;

    public Chamber()
    {
        m_chamberObject = new GameObject();
        m_chamberSize = Vector2.zero;

        s_chamberCount++;
    }

    public void SetName(string name)
    {
        if (m_chamberObject != null)
            m_chamberObject.name = name;
    }

    public string GetName()
    {
        if (m_chamberObject != null)
            return m_chamberObject.name;

        return string.Empty;
    }

    public void SetPosition(Vector2 position)
    {
        if (m_chamberObject != null)
            m_chamberObject.transform.position = position;
    }

    public void SetPosition(Vector3 position)
    {
        if (m_chamberObject != null)
            m_chamberObject.transform.position = new Vector3(position.x, position.y, 0f);
    }

    public Vector2 GetPosition()
    {
        if (m_chamberObject != null)
            return new Vector2(m_chamberObject.transform.position.x, m_chamberObject.transform.position.y);

        return Vector2.zero;
    }

    public void SetSize(Vector2 size)
    {
        if (m_chamberSize != null)
            m_chamberSize = size;
    }

    public Vector2 GetSize()
    {
        if (m_chamberSize != null)
            return m_chamberSize;

        return Vector2.zero;
    }

    public void SetChild(GameObject child)
    {
        if (m_chamberObject != null && child != null)
            child.transform.parent = m_chamberObject.transform;
    }

    public void EnableMiniMapLayer()
    {
        if (m_chamberObject != null)
        {
            // Hard-coded layer. Probably won't change.
            int layer = LayerMask.NameToLayer("CurrentChamber");

            SetLayer(m_chamberObject.transform, layer);
        }
    }

    private void SetLayer(Transform parent, int layer)
    {
        if (parent != null)
        {
            parent.gameObject.layer = layer;

            foreach (Transform child in parent)
            {
                child.gameObject.layer = layer;

                SetLayer(child.transform, layer);
            }
        }
    }

    public void DisableMiniMapLayer()
    {
        if (m_chamberObject != null)
        {
            // Hard-coded layer. Probably won't change.
            int layer = LayerMask.NameToLayer("Default");

            SetLayer(m_chamberObject.transform, layer);
        }
    }

    public void Reset()
    {
        if (m_chamberObject != null)
        {
            Component[] resettableComponents = m_chamberObject.GetComponentsInChildren(typeof(IResettable));

            foreach (Component resettableComponent in resettableComponents)
            {
                IResettable resettable = resettableComponent as IResettable;

                if (resettable != null)
                    resettable.Reset();
            }
        }
    }

    public static int GetChamberCount()
    {
        return s_chamberCount;
    }

    public void Dispose()
    {
        s_chamberCount--;

        // Just some precautions.
        if (s_chamberCount < 0)
            s_chamberCount = 0;

        // Since Chamber doesn't inherit from Mono, then Destroy() cannot be called.
        // We must implement our own mini garbage collector.
        GarbageManager.Add(ref m_chamberObject);
    }
}
