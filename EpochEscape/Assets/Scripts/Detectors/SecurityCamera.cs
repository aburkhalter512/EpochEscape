using UnityEngine;
using System.Collections;
using G = GameManager;

public class SecurityCamera : MonoBehaviour, ITransitional
{
    public const float DEFAULT_PATROL_ANGLE = 45f; // 45 degrees
    public const float DEFAULT_PATROL_SPEED = 90f; // 90 degrees/sec
    public const float DEFAULT_FIXATE_SPEED = 2.5f;
    
    public enum State
    {
        IDLE,
        ROTATE_LEFT,
        ROTATE_RIGHT,
        ALERT,
        FIXATE,
        RESET,
        DISABLE
    };
    
    public float m_patrolAngle = DEFAULT_PATROL_ANGLE;
    public float m_patrolSpeed = DEFAULT_PATROL_SPEED;
    public float m_fixateSpeed = DEFAULT_FIXATE_SPEED;

    public State m_previousState;
    public State m_currentState;
    
    public Vector3 m_resetDirection;
    public float m_resetAngle;

    private Vector3 dir;
    public State cur;

    public void Start()
    {
        m_resetDirection = transform.up;
        m_resetAngle = transform.eulerAngles.z;
    }
    
    public void Update()
    {
        if(!G.Get().paused)
            UpdateCurrentState();
    }

    public void deactivate()
    {
        if (cur != State.DISABLE)
        {
            cur = State.DISABLE;

            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void activate()
    {
        if (cur == State.DISABLE)
        {
            cur = State.ROTATE_LEFT;

            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void toggle()
    {
        if (cur != State.DISABLE)
            deactivate();
        else
            activate();
    }
    
    private void UpdateCurrentState()
    {
        switch(m_currentState)
        {
        case State.ROTATE_LEFT:
            Rotate();
            break;

        case State.ROTATE_RIGHT:
            Rotate();
            break;

        case State.ALERT:
            Alert();
            break;

        case State.FIXATE:
            Fixate();
            break;
            
        case State.RESET:
            Reset();
            break;
        }
    }

    private void Rotate()
    {
        float rotationSign = (m_currentState == State.ROTATE_LEFT ? 1f : -1f);
        
        transform.Rotate(rotationSign * Vector3.forward, m_patrolSpeed * Time.smoothDeltaTime);
        
        float angle = Mathf.Acos(Vector3.Dot(transform.up, m_resetDirection)) * Mathf.Rad2Deg;
        
        if(angle > m_patrolAngle)
        {
            m_previousState = m_currentState;
            m_currentState = (m_currentState == State.ROTATE_LEFT ? State.ROTATE_RIGHT : State.ROTATE_LEFT);
            
            // Offset by 90 degrees since the textures are orientated 90 degrees.
            Vector3 boundedVector = new Vector3(Mathf.Cos((m_resetAngle + rotationSign * m_patrolAngle + 90f) * Mathf.Deg2Rad), 
                Mathf.Sin((m_resetAngle + rotationSign * m_patrolAngle + 90f) * Mathf.Deg2Rad), 0f);
            
            transform.up = boundedVector;
            transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z);
        }
    }

    private void Alert()
    {
        dir = transform.up;
        cur = m_previousState;

        m_previousState = State.ALERT;
        m_currentState = State.FIXATE;
    }

    private void Fixate()
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        Vector3 target = player.transform.position - transform.position;
        target.Normalize();

        Utilities.RotateTowards(gameObject, target, m_fixateSpeed);

        float angle = Mathf.Acos(Vector3.Dot(transform.up, m_resetDirection)) * Mathf.Rad2Deg;
        
        if(angle > m_patrolAngle)
        {
            m_previousState = State.FIXATE;
            m_currentState = cur;
            
            transform.up = dir;
            transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z);
        }
    }
    
    private void Reset()
    {
        float angle = Mathf.Acos(Vector3.Dot(transform.up, m_resetDirection)) * Mathf.Rad2Deg;
        float rotationSign = Mathf.Sign(Vector3.Cross(transform.up, m_resetDirection).z);
        
        transform.Rotate(rotationSign * Vector3.forward, angle * Time.smoothDeltaTime);
        
        if(Utilities.IsApproximately(angle, 0f))
        {
            m_previousState = State.RESET;
            m_currentState = State.IDLE;
            
            transform.up = m_resetDirection;
            transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z);
        }
    }

    public void Activate()
    {
        m_currentState = State.IDLE;

        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void OnFinishTransition()
    {
        toggle();
    }

    public void OnReadyIdle()
    {
    }

    public float GetWaitTime()
    {
        return 0.33f;
    }
}
