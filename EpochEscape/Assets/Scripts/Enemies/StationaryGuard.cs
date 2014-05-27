using UnityEngine;
using System.Collections;

public class StationaryGuard : Guard
{
    private Vector3 mBaseUp;

    public override void Start()
	{
		m_animator = GetComponent<Animator>();

		m_isAlerted = false;
		m_timeLastAlerted = 0f;
		m_searchTarget = Vector3.zero;
		
		SetInitialDirection();
	}

    public override void Update()
	{
        base.Update();
	}

    protected override void SetInitialDirection()
	{
        mBaseUp = transform.up;
	}

    protected override void UpdateCurrentState()
	{
		switch(m_currentState)
		{
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

    protected override void Rotate()
	{
		float currentAngle = Utilities.RotateTowards(gameObject, mBaseUp, m_rotationSpeed);
		
		if(Utilities.IsApproximately(currentAngle, 0f))
		{
			// Align the up transformation perfectly with the target vector.
            transform.up = mBaseUp;
			
			// Reset the y Euler angle in case it was randomly changed.
			transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z);

			m_currentState = State.IDLE;
		}
	}

    protected override void UpdateAnimator()
	{
		if(m_animator != null)
		{
			m_animator.SetBool("isAlerted", m_currentState == State.ALERT ? true : false);
			m_animator.SetBool("isDead", m_isDead);
		}
	}

    protected override void Alert()
	{
		if(!m_isAlerted)
		{
			m_isAlerted = true;
			m_timeLastAlerted = Time.time;
		}

		GameObject player = GameObject.FindGameObjectWithTag("Player") as GameObject;
		Player playerManager = player.GetComponent<Player>() as Player;

        if (!(player == null || playerManager == null) && playerManager.m_isWithinEarshot)
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
}
