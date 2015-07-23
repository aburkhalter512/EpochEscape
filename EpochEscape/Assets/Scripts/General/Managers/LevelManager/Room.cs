using UnityEngine;

public abstract class Room
{
    protected GameObject m_roomObject;
    protected Vector2 m_roomSize;

    public Room()
    {
        m_roomObject = new GameObject();
        m_roomSize = Vector2.zero;
    }

    public void SetName(string name)
    {
        if (m_roomObject != null)
            m_roomObject.name = name;
    }

    public string GetName()
    {
        if (m_roomObject != null)
            return m_roomObject.name;

        return string.Empty;
    }

    public void SetPosition(Vector2 position)
    {
        if (m_roomObject != null)
            m_roomObject.transform.position = position;
    }

    public void SetPosition(Vector3 position)
    {
        if (m_roomObject != null)
            m_roomObject.transform.position = new Vector3(position.x, position.y, 0f);
    }

    public Vector2 GetPosition()
    {
        if (m_roomObject != null)
            return new Vector2(m_roomObject.transform.position.x, m_roomObject.transform.position.y);

        return Vector2.zero;
    }

    public void SetSize(Vector2 size)
    {
        m_roomSize = size;
    }

    public Vector2 GetSize()
    {
        return m_roomSize;
    }

    public void SetChild(GameObject child)
    {
        if (m_roomObject != null && child != null)
            child.transform.parent = m_roomObject.transform;
    }

    public void Dispose()
    {
        // Since Room doesn't inherit from Mono, then Destroy() cannot be called.
        // We must implement our own mini garbage collector.
        GameObject.Destroy(m_roomObject);
    }

    public abstract string GetRoomType();
    public abstract int GetRoomCount();
}
