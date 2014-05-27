using UnityEngine;
using System.Collections;

public class Guard : Enemy
{
	public const int FIRST_PATROL_POINT = 0;
	
	public enum State
	{
		IDLE,
		REST,
		PATROL,
		ROTATE,
		ALERT,
        STUN,
		DEAD
	}
	
	public float m_speed;
	public float m_rotationSpeed;
	public float m_restInterval;
	public float m_alertInterval;
	public State m_currentState;
	public int m_currentPatrolPoint;
	public Vector3[] m_patrolPoints;
    public float m_stunTime;
	public bool m_isDead;
	
	protected Animator m_animator;
    protected bool m_isMoving;
    protected float m_timeLastRested;

    protected bool m_isAlerted;
    protected float m_timeLastAlerted;
    protected Vector3 m_searchTarget;

    protected float m_startStunTime = -1;
	
	public virtual void Start()
	{
		m_animator = GetComponent<Animator>();
		m_timeLastRested = 0f;

		m_isAlerted = false;
		m_timeLastAlerted = 0f;
		m_searchTarget = Vector3.zero;
		
		IncreasePatrolPoint();
		SetInitialPosition();
		SetInitialDirection();
	}

    public virtual void Update()
	{
		UpdateCurrentState();
		UpdateAnimator();
	}

    protected virtual void SetInitialPosition()
	{
		if(m_patrolPoints.Length > 0)
			transform.position = m_patrolPoints[FIRST_PATROL_POINT];
	}

    protected virtual void SetInitialDirection()
	{
		if(!IsValidPatrolPoint(m_currentPatrolPoint))
			return;
		
		Vector3 target = m_patrolPoints[m_currentPatrolPoint] - transform.position;
		target.Normalize();
		
		transform.up = target;
		transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z);
	}

    protected virtual void UpdateCurrentState()
	{
		switch(m_currentState)
		{
		case State.PATROL:
			Patrol();
			break;
			
		case State.REST:
			Rest();
			break;
			
		case State.ROTATE:
			Rotate();
			break;

		case State.ALERT:
			Alert();
			break;

        case State.STUN:
            Stun();
            break;
		}
	}

    protected virtual void Patrol()
	{
		if(!IsValidPatrolPoint(m_currentPatrolPoint))
			return;
		
		transform.position += transform.up * m_speed * Time.smoothDeltaTime;
		
		Vector3 target = m_patrolPoints[m_currentPatrolPoint] - transform.position;
		
		if(Utilities.IsApproximately(target.magnitude, 0f, Utilities.DEFAULT_TOLERANCE))
		{
			m_currentState = State.REST;
			m_timeLastRested = Time.time;
			
			transform.position = m_patrolPoints[m_currentPatrolPoint];
			
			IncreasePatrolPoint();
		}
	}

    protected virtual void Rest()
	{
		if(Time.time - m_timeLastRested > m_restInterval)
		{
			m_currentState = State.ROTATE;
			m_timeLastRested = 0f;
		}
	}

    protected virtual void Rotate()
	{
		if(!IsValidPatrolPoint(m_currentPatrolPoint))
			return;
		
		// Calculate the target vector and normalize it.
		Vector3 target = m_patrolPoints[m_currentPatrolPoint] - transform.position;
		target.Normalize();

		float currentAngle = Utilities.RotateTowards(gameObject, target, m_rotationSpeed);
		
		if(Utilities.IsApproximately(currentAngle, 0f))
		{
			// Align the up transformation perfectly with the target vector.
			transform.up = target;
			
			// Reset the y Euler angle in case it was randomly changed.
			transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z);

			m_currentState = State.PATROL;
		}
	}

    public virtual void Stun()
    {
		int numChildren = gameObject.transform.childCount;

		if(numChildren == 0) return;

        for (int i = 0; i < numChildren; i++)
        {
            if (transform.GetChild(i).GetComponent<DetectionArea>())
                if (transform.GetChild(i).GetComponent<DetectionArea>().detectedPlayer != null)
                    transform.GetChild(i).GetComponent<DetectionArea>().detectedPlayer.m_isDetected = false;
            transform.GetChild(i).gameObject.SetActive(false);
        }

		m_currentState = State.DEAD;
		m_isDead = true;
    }

    protected virtual void UpdateAnimator()
	{
		if(m_animator != null)
		{
			m_animator.SetBool("isPatrolling", m_currentState == State.PATROL ? true : false);
			m_animator.SetBool("isAlerted", m_currentState == State.ALERT ? true : false);
			m_animator.SetBool("isDead", m_isDead);
		}
	}

    protected virtual void IncreasePatrolPoint()
	{
		m_currentPatrolPoint++;
		
		if(m_currentPatrolPoint >= m_patrolPoints.Length)
			m_currentPatrolPoint = FIRST_PATROL_POINT;
	}

    protected virtual bool IsValidPatrolPoint(int patrolPoint)
	{
		return patrolPoint >= FIRST_PATROL_POINT && patrolPoint < m_patrolPoints.Length;
	}

    protected virtual void Alert()
	{
		if(!m_isAlerted)
		{
			m_isAlerted = true;
			m_timeLastAlerted = Time.time;
		}

		GameObject player = GameObject.FindGameObjectWithTag("Player") as GameObject;
		Player playerManager = player.GetComponent<Player>() as Player;
		
		if(!(player == null || playerManager == null) && playerManager.m_isWithinEarshot)
		{
			m_searchTarget = player.transform.position - transform.position;
			m_searchTarget.Normalize();
		}

		if(Time.time - m_timeLastAlerted > m_alertInterval)
		{
			float angle = Utilities.RotateTowards(gameObject, m_searchTarget, m_rotationSpeed);

			if(Utilities.IsApproximately(angle, 0f))
			{
				m_isAlerted = false;
				m_timeLastAlerted = 0f;
				m_searchTarget = Vector3.zero;
				m_currentState = State.ROTATE;
			}
		}
	}

    public virtual void Hit()
    {
		Player player = (Player)GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		float angle = Vector3.Angle (player.transform.up, transform.position - player.transform.position);
		float dist = Vector3.Distance (player.transform.position, transform.position);
		if(angle < 20f && dist < .5f && player.Special){
			//kill or whatever
			player.m_specItems--;
		}
	}

}
