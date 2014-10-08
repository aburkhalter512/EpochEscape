using UnityEngine;

public class Chamber : Room
{
    private static int s_count = 0;

    public Chamber()
    {
        s_count++;
    }

    public void EnableMiniMapLayer()
    {
        if (m_roomObject != null)
        {
            // Hard-coded layer. Probably won't change.
            int layer = LayerMask.NameToLayer("CurrentChamber");

            SetLayer(m_roomObject.transform, layer);
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
        if (m_roomObject != null)
        {
            // Hard-coded layer. Probably won't change.
            int layer = LayerMask.NameToLayer("Default");

            SetLayer(m_roomObject.transform, layer);
        }
    }

    public void Reset()
    {
        if (m_roomObject != null)
        {
            Component[] resettableComponents = m_roomObject.GetComponentsInChildren(typeof(IResettable));

            foreach (Component resettableComponent in resettableComponents)
            {
                IResettable resettable = resettableComponent as IResettable;

                if (resettable != null)
                    resettable.Reset();
            }
        }
    }

    public void Dispose()
    {
        s_count--;

        // Just some precautions.
        if (s_count < 0)
            s_count = 0;

        base.Dispose();
    }

    public override string GetRoomType()
    {
        return "Chamber";
    }

    public override int GetRoomCount()
    {
        return s_count;
    }
}
