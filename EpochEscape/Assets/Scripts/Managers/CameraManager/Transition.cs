using UnityEngine;
using System.Collections;

public class Transition
{
    private Vector3 m_position;
    private ITransitional m_transitional;
    private float m_duration;

    public Transition(Vector3 position, ITransitional transitional, float duration)
    {
        SetPosition(position);
        SetTransitional(transitional);
        SetDuration(duration);
    }

    public Vector3 GetPosition()
    {
        if(m_position == null)
            return Vector3.zero;

        return m_position;
    }

    public void SetPosition(Vector3 position)
    {
        if(position != null)
            m_position = position;
    }

    public ITransitional GetTransitional()
    {
        return m_transitional;
    }

    public void SetTransitional(ITransitional transitional)
    {
        if(transitional != null)
            m_transitional = transitional;
    }

    public float GetDuration()
    {
        return m_duration;
    }

    public void SetDuration(float duration)
    {
        if(duration < 0f)
            m_duration = 0f;

        m_duration = duration;
    }

    public IEnumerator OnFinishTransition()
    {
        if(m_transitional == null)
            return null;

        return m_transitional.OnFinishTransition();
    }
}
