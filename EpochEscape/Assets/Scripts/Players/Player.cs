using UnityEngine;
using System.Collections;
using G = GameManager;

public class Player : MonoBehaviour
{
	#region Inspector Variables
	public float mForce = FORCE;
	public float mDynamicFriction = DYNAMIC_FRICTION;
	public float mAirResistance = AIR_RESTIANCE_MODIFIER;

	private bool m_isMoving;
	private bool m_isMovingForward;
	private bool m_isMovingDown;
	private bool m_isMovingLeft;
	private bool m_isMovingRight;
	private bool m_isAttacking;
	private bool m_hasSpecialItem;
	private float m_directionAngle;

	public bool m_isDetected;
	public bool m_isHiding;
	public bool m_isWithinEarshot;

	public float m_rotationSpeed = 15f;
	public float m_detectionLevel = 0f;
	public float m_detectionRate = 50f; // rate/second at which character becomes detected by cameras
	public float m_detectionFade = 1.5f;
	public float m_detectionMax = 0f;
	public float m_detectionThres = 15f;

	public PlayerState m_currentState;
	public Vector3 m_spawnLocation;

	public Inventory inventory;
	private int m_selectedSlot; // only private because not yet necessary
	public int m_specItems;

	public int MAX_CORES = 3;

	private Vector3 m_previousMouseScreenPos;
	#endregion

	#region Instance Variables
	private Animator m_animator;

	private Vector3 transForces = Vector3.zero; //Translation forces
	private Vector3 velocity = Vector3.zero;

	public int currentCores = 0;
	#endregion

	#region Class Constants
	public const float FORCE = 1.0f;
	public const float DYNAMIC_FRICTION = 1.0f;
	public const float AIR_RESTIANCE_MODIFIER = 0.5f;
	public const float MAX_DETECTION_LEVEL = 100.0f;
	public const int UNIQUE_ITEM_SLOTS = 1;

	public enum PlayerState
	{
		ALIVE,
		DEAD
	}
	#endregion

	#region Class Variables
	public bool Special = false;
	#endregion

	//Put all initialization code here
	//Remember to comment!
	protected void Start()
	{
		FadeManager.StartAlphaFade (Color.black, true, 1f, 0f);

		m_animator = GetComponent<Animator>();

		m_isMoving = false;
		m_isMovingForward = false;
		m_isMovingDown = false;
		m_isMovingLeft = false;
		m_isMovingRight = false;
		m_isAttacking = false;
		m_hasSpecialItem = false;

		m_isDetected = false;
		m_isHiding = false;
		m_isWithinEarshot = false;
		m_currentState = PlayerState.ALIVE;

		inventory = new Inventory();
		m_selectedSlot = 0;
		transform.position = m_spawnLocation;

		m_previousMouseScreenPos = Vector3.zero;
	}

	#region Initialization Methods
	#endregion

	//Put all update code here
	//Remember to comment!
	protected void Update()
	{
		if(!G.getInstance ().paused)
		{
			UpdateCurrentState();
			//UpdateDebug();
		}
		else
		{
            Debug.Log("Game is paused");
			m_isMoving = false;
			m_isMovingForward = false;
			m_isMovingDown = false;
			m_isMovingLeft = false;
			m_isMovingRight = false;
		}

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

	private void UpdateDebug()
	{
		if(Input.GetKeyUp(KeyCode.C))
			m_hasSpecialItem = !m_hasSpecialItem;
	}

	private void Alive()
	{
		UpdateUserControl();
		UpdateDetection();
		UpdateMovement();
		UpdateDirection();

		ApplyEnvironmentalForces();
		ApplyForces();
	}

	private void UpdateDirection()
	{
		if(!m_isMoving)
			return;

		if(m_isMovingForward)
		{
			if(m_isMovingLeft)
				m_directionAngle = 135f;
			else if(m_isMovingRight)
				m_directionAngle = 45f;
			else
				m_directionAngle = 90f;
		}
		else if(m_isMovingDown)
		{
			if(m_isMovingLeft)
				m_directionAngle = 225f;
			else if(m_isMovingRight)
				m_directionAngle = 315f;
			else
				m_directionAngle = 270f;
		}
		else if(m_isMovingLeft)
			m_directionAngle = 180f;
		else if(m_isMovingRight)
			m_directionAngle = 0f;

		// Calculate the target vector and normalize it.
		Vector3 target = new Vector3(Mathf.Cos(m_directionAngle * Mathf.Deg2Rad), Mathf.Sin(m_directionAngle * Mathf.Deg2Rad), 0f);
		target.Normalize();

		float currentAngle = Utilities.RotateTowards(gameObject, target, m_rotationSpeed);
		
		if(Utilities.IsApproximately(currentAngle, 0f))
		{
			// Align the up transformation perfectly with the target vector.
			transform.up = target;
			
			// Reset the y Euler angle in case it changed randomly.
			transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z);
		}
	}

	private void Dead()
	{
		m_currentState = PlayerState.ALIVE;

		Application.LoadLevel(Application.loadedLevelName);
	}

	private void ResetVariables(){
		inventory.clear();
		currentCores = 0;
		m_detectionLevel = 0;
	}

	#region Update Methods
	#region UpdateUserControl
	private void UpdateUserControl()
	{
		UpdateMouse();
		UpdateKeyboard();
	}
	
	private void UpdateMouse()
	{
		if(!m_isMoving)
		{
			Vector3 mouseScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z);
			Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
			Vector3 toMousePosition = mouseWorldPosition - transform.renderer.bounds.center;
			toMousePosition.z = 0f;

			if(Utilities.IsApproximately(mouseScreenPosition.x, m_previousMouseScreenPos.x) && 
			   Utilities.IsApproximately(mouseScreenPosition.y, m_previousMouseScreenPos.y))
			{
				return;
			}

			m_previousMouseScreenPos = mouseScreenPosition;

			CircleCollider2D playerCollider = GetComponent<CircleCollider2D>();

			if(playerCollider != null && toMousePosition.magnitude > playerCollider.radius)
			{
				toMousePosition.Normalize();

				float angle = Utilities.RotateTowards(gameObject, toMousePosition, m_rotationSpeed);

				if(Utilities.IsApproximately(angle, 0f))
				{
					transform.up = toMousePosition;
					transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z);
				}
			}
		}
	}

	private void UpdateKeyboard()
	{
		m_isMoving = false;
		m_isMovingForward = false;
		m_isMovingDown = false;
		m_isMovingLeft = false;
		m_isMovingRight = false;

		if(m_isAttacking)
			return;

		if(Input.GetKey(KeyCode.W)) m_isMovingForward = true;
		if(Input.GetKey(KeyCode.S)) m_isMovingDown = true;
		if(Input.GetKey(KeyCode.A)) m_isMovingLeft = true;
		if(Input.GetKey(KeyCode.D)) m_isMovingRight = true;

		if(m_isMovingForward && m_isMovingDown)
			m_isMovingForward = m_isMovingDown = false;

		if(m_isMovingLeft && m_isMovingRight)
			m_isMovingLeft = m_isMovingRight = false;

		if(m_isMovingForward || m_isMovingDown || m_isMovingLeft || m_isMovingRight)
			m_isMoving = true;

		SelectSlot();

		// 0th subscript indicates empty flask.
		if(Input.GetButtonDown("Fire1") && inventory.inventory[m_selectedSlot] != null)
			m_isAttacking = true;

		if (Input.GetButtonDown ("Fire2"))
			SpecialAbility ();

		if(Input.GetAxisRaw ("Mouse ScrollWheel") > 0)
			ScrollSlotIncrement ();
		if(Input.GetAxisRaw ("Mouse ScrollWheel") < 0)
			ScrollSlotDecrement();
	}

	public void ScrollSlotIncrement(){
		m_selectedSlot++;
		if(m_selectedSlot > UNIQUE_ITEM_SLOTS)
			m_selectedSlot = 0;
	}

	public void ScrollSlotDecrement(){
		m_selectedSlot--;
		if(m_selectedSlot < 0)
			m_selectedSlot = UNIQUE_ITEM_SLOTS;
	}

	public void FinishedAttacking()
	{
		m_isAttacking = false;

		inventory.activateItem(m_selectedSlot);
	}

	private void UpdateMovement()
	{
		Vector3 movement = Vector3.zero;

		if(m_isMovingForward) movement.y = mForce * mDynamicFriction;
		if(m_isMovingDown) movement.y = -mForce * mDynamicFriction;
		if(m_isMovingLeft) movement.x = -mForce * mDynamicFriction;
		if(m_isMovingRight) movement.x = mForce * mDynamicFriction;

		transForces += movement * Time.smoothDeltaTime;
		transForces -= Utilities.toVector3(velocity * mDynamicFriction);

		if ((Input.GetButtonDown ("Horizontal") || Input.GetButtonDown ("Vertical")) && !audio.isPlaying) {
			audio.loop = true;
			audio.Play ();
		}
		else if (!Input.GetButton ("Horizontal") && !Input.GetButton ("Vertical") && audio.isPlaying)
			audio.loop = false;
	}
	#endregion

	#region UpdateDetection
	private void UpdateDetection() 
	{
		if (m_isDetected) {
			m_detectionLevel += m_detectionRate * Time.deltaTime;
			if (m_detectionLevel >= MAX_DETECTION_LEVEL)
			{
				m_currentState = PlayerState.DEAD;
				m_detectionLevel = 0;
			}
			if (m_detectionLevel > m_detectionMax) {
				m_detectionMax = m_detectionLevel;
			}
		} else {
			if (m_detectionLevel > (m_detectionMax - m_detectionThres) && m_detectionLevel > 0)
				m_detectionLevel -= m_detectionFade * Time.deltaTime;
		}
	}
	#endregion

	#region ApplyEnvironmentalForces methods
	void ApplyEnvironmentalForces()
	{
		ApplyAirResistance();
	}

	void ApplyAirResistance()
	{
		transForces -= Utilities.toVector3(rigidbody2D.velocity * mAirResistance);
	}
	#endregion

	protected void ApplyForces()
	{
		velocity += transForces;
		transForces = Vector3.zero;

		rigidbody2D.AddForce(velocity * rigidbody2D.mass);
	}

	private void UpdateAnimator()
	{
		if(m_animator != null)
		{
			m_animator.SetBool("isMoving", m_isMoving);
			m_animator.SetBool("isMovingForward", m_isMovingForward);
			m_animator.SetBool("isMovingBackward", m_isMovingDown);
			m_animator.SetBool("isMovingLeft", m_isMovingLeft);
			m_animator.SetBool("isMovingRight", m_isMovingRight);
			m_animator.SetBool("isAttacking", m_isAttacking);
			m_animator.SetBool("hasSpecialItem", m_hasSpecialItem);
		}
	}

	private void SelectSlot()
	{
		//m_selectedSlot = 0; //currently only one item can be used (potion)
		if(Input.GetKeyDown(KeyCode.Alpha1)){
			m_selectedSlot = 0;
			Debug.Log("Slot 1");
		}
		else if(Input.GetKeyDown(KeyCode.Alpha2)){
			m_selectedSlot = 1;
			Debug.Log ("Slot 2");
		}
	}

	public void addPowerCore()
	{
		if (currentCores < MAX_CORES)
			currentCores++;
	}

	public bool isPowerCoreComplete()
	{
		return currentCores == MAX_CORES;
	}
	#endregion

	public void SpecialAbility(){
		if (m_isAttacking) {
			Special = true;
			//play animation
		}
		else{
			Special = false;
		}
	}

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
