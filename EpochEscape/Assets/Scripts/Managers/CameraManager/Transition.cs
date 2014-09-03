using UnityEngine;
using System.Collections;

public class Transition
{
    private bool m_isInitialized = false;
    private Vector3 m_position = Vector3.zero;
    private SpriteRenderer m_renderer = null;
    private ITransitional m_transitional = null;
    private float m_waitTime = 0f;

    public Transition(GameObject objectToTransition)
    {
        m_isInitialized =
            SetPosition(objectToTransition) &&
            SetRenderer(objectToTransition) &&
            SetTransitional(objectToTransition);
    }

    public bool SetPosition(GameObject objectToTransition)
    {
        if (objectToTransition != null)
        {
            m_position = objectToTransition.transform.position;

            return true;
        }

        return false;
    }

    public Vector3 GetPosition()
    {
        return m_position;
    }

    public bool SetRenderer(GameObject objectToTransition)
    {
        if(objectToTransition != null)
        {
            m_renderer = objectToTransition.GetComponent<SpriteRenderer>();

            if(m_renderer == null)
                return false;
        }

        return true;
    }

    public Vector3 GetSize()
    {
        if(m_renderer == null)
            return Vector3.zero;

        return m_renderer.bounds.size;
    }

    public bool SetTransitional(GameObject objectToTransition)
    {
        if(objectToTransition != null)
        {
            m_transitional = objectToTransition.GetComponent(typeof(ITransitional)) as ITransitional;

            if(m_transitional == null)
                return false;

            SetWaitTime(m_transitional.GetWaitTime());

            return true;
        }

        return false;
    }

    public float GetWaitTime()
    {
        return m_waitTime;
    }

    public bool SetWaitTime(float waitTime)
    {
        if (waitTime < 0f)
        {
            waitTime = 0f;

            return false;
        }

        m_waitTime = waitTime;

        return true;
    }

    public bool IsInitialized()
    {
        return m_isInitialized;
    }

    public void OnFinishTransition()
    {
        if(m_transitional != null)
            m_transitional.OnFinishTransition();
    }
}
