using UnityEngine;
using System.Collections;
using G = GameManager;

public class Player : Manager<Player>
{
    #region Interface Variables
    public float m_interactionDistance = .2f;
    public int m_interactionMask = 1 << 9;

    public AudioClip[] grateFoot;
	public AudioClip[] solidFoot;
	public int m_floorType; // 0 = solid, 1 = grate

	public float m_detectionLevel = 0f;
	public float m_detectionMax = 0f;
	
    public float Boost = 1f;
    public float BoostTime = 0f;

    public bool m_isDetected;
    #endregion

    #region Instance Variables
	Animator m_animator;
	
	#region Movement Variables
	bool m_isMoving;
    Vector2 mMovementDirection = Vector2.zero;
	
	float m_rotationSpeed = 15f;
	#endregion
	
	#region Audio Variables
	int m_footCounter;
	#endregion
	
	//Detection Variables
	protected float m_detectionRate = 50f; // rate/second at which character becomes detected by cameras
	protected float m_detectionFade = 1.5f;
	protected float m_detectionThres = 15f;
	
    //Power Core Variables
	protected int mCollectedCores = 0;

	PlayerState m_currentState;

    InputManager mIM = null;
    #endregion
    #region Class Constants
    public static readonly float SPEED = 2f;
    public static readonly float MAX_DETECTION_LEVEL = 100.0f;
	public static readonly int UNIQUE_ITEM_SLOTS = 1;
	
	public static readonly int MAX_CORES = 3;

    public enum PlayerState
    {
        ALIVE,
        DEAD
    }
    #endregion

    protected override void Initialize()
    {
        m_floorType = 0;
        m_isMoving = false;

        m_isDetected = false;
        m_currentState = PlayerState.ALIVE;
    }

    protected void Start()
    {
        m_animator = GetComponent<Animator>();

        mIM = InputManager.Get();
    }

    protected void Update()
    {
        if(!G.Get ().paused)
            UpdateCurrentState();
        else
            m_isMoving = false;

        UpdateAnimator();
    }

    private void UpdateCurrentState()
    {
        switch(m_currentState)
        {
        case PlayerState.ALIVE:
            Alive();
            break;

        case PlayerState.DEAD:
            Dead();
            break;
        }
    }

    #region State Methods
    protected virtual void Alive()
    {
        UpdateUserControl();
        UpdateDetection();
        UpdateMovement();
    }

    protected virtual void Dead()
    {
        G.Get().PauseMovement();

        HUDManager.Hide();
        MiniMapManager.Hide();
        
        GameObject playerCaught = Resources.Load("Prefabs/PlayerCaught") as GameObject;
        
        if(playerCaught != null)
        {
            playerCaught = Instantiate(playerCaught) as GameObject;
            playerCaught.transform.position = transform.position;
        }
    }
    #endregion

    #region Interface Methods
    public void load(Vector3 position, Vector3 rotation)
    {
        return;
    }

    public void addCore()
    {
        mCollectedCores++;

        if (mCollectedCores > MAX_CORES)
            mCollectedCores = MAX_CORES;
    }

    public void removeCore()
    {
        mCollectedCores--;

        if (mCollectedCores < 0)
            mCollectedCores = 0;
    }

	public int getCurrentCores()
	{
		return mCollectedCores;
	}

    public void clearCores()
    {
        mCollectedCores = 0;
    }
	#endregion

    private void UpdateUserControl()
    {
		#region Determine the movement direction
        m_isMoving = false;
        
        Vector3 playerMovement = mIM.primaryJoystick.getRaw();

        //Vertical Movement
        mMovementDirection = Vector2.zero;

        if (Utilities.IsApproximately(playerMovement.y, 0.0f))
            mMovementDirection.y = 0.0f;
        else if (playerMovement.y > 0.0f)
        {
            m_isMoving = true;
            mMovementDirection.y = 1.0f;
        }
        else if (playerMovement.y < 0.0f)
        {
            m_isMoving = true;
            mMovementDirection.y = -1.0f;
        }

        //Horizontal Movement
        if (Utilities.IsApproximately(playerMovement.x, 0.0f))
            mMovementDirection.x = 0.0f;
        else if (playerMovement.x > 0.0f)
        {
            m_isMoving = true;
            mMovementDirection.x = 1.0f;
        }
        else if (playerMovement.x < 0.0f)
        {
            m_isMoving = true;
            mMovementDirection.x = -1.0f;
        }

        mMovementDirection.Normalize();
    	#endregion

        //No interacting for right now.
        //Interact ();
    }

    private void UpdateMovement()
	{
		if(!m_isMoving) return;
		
		#region Update euler angles
        float currentAngle = Utilities.RotateTowards(gameObject, mMovementDirection, m_rotationSpeed);
		
		if(Utilities.IsApproximately(currentAngle, 0f))
		{
			// Align the up transformation perfectly with the target vector.
            transform.up = mMovementDirection;
			
			// Reset the y Euler angle in case it changed randomly.
			transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z);
		}
		#endregion
		
	    transform.position += (Utilities.toVector3(mMovementDirection) * SPEED * Time.smoothDeltaTime);
    }

    private void UpdateDetection() 
    {
        if (m_isDetected)
        {
            m_detectionLevel += m_detectionRate * Time.deltaTime;
            if (m_detectionLevel >= MAX_DETECTION_LEVEL)
            {
                m_currentState = PlayerState.DEAD;
                m_detectionLevel = 0;
            }
            if (m_detectionLevel > m_detectionMax) {
                m_detectionMax = m_detectionLevel;
            }
        }
        else
        {
            if (m_detectionLevel > (m_detectionMax - m_detectionThres) && m_detectionLevel > 0)
                m_detectionLevel -= m_detectionFade * Time.deltaTime;
        }
    }

    protected virtual void UpdateAnimator()
    {
        m_animator.SetBool("isMoving", m_isMoving);
    }

    private void PlayFootstep()
    {
        switch (m_floorType)
        {
            case 0:
                if (solidFoot.Length > 0)
                {
                    audio.clip = solidFoot[m_footCounter % solidFoot.Length];
                    m_footCounter = (m_footCounter + 1) % solidFoot.Length;
                }
                break;
            case 1:
                if (solidFoot.Length > 0)
                {
                    audio.clip = grateFoot[m_footCounter % grateFoot.Length];
                    m_footCounter = (m_footCounter + 1) % grateFoot.Length;
                }
                break;
        }
        audio.Play ();
    }

    public void Resurrect()
    {
        m_currentState = PlayerState.ALIVE;
    }
}
