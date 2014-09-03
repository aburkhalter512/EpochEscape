using UnityEngine;
using System.Collections;

public class WeightedPlate : MonoBehaviour
{
	#region Inspector Variables
	public GameObject[] actuators;
	public bool m_isLocked = false;
	public GameObject activeBox;
	
	public Sprite switchOn;
	public Sprite switchOff;
	
	public STATE currentState;
	#endregion
	
	#region Instance Variables
	protected SpriteRenderer mSR;
	
	private STATE previousState;
	private Player p;
	#endregion
	
	#region Class Constants
	public enum STATE
	{
		ON,
		OFF,
		UN_INIT
	}
	#endregion
	
	/*
     * Initializes the Pressure Plate
     */
	protected void Start()
	{
		mSR = gameObject.GetComponent<SpriteRenderer>();
		
		previousState = STATE.UN_INIT;
		currentState = STATE.ON;
		
		GameObject g2 = GameObject.FindGameObjectWithTag("Player");
		Player p = g2.GetComponent<Player>();
	}
	
	/*
     * Updates the state of the Pressure Plate
     */
	protected void Update()
	{
		if (!m_isLocked) {
			if (activeBox != null && activeBox.renderer.enabled && renderer.bounds.Intersects(activeBox.renderer.bounds) && !p.m_isHoldingBox) {
				audio.Play ();
				
				//Activate all of the connected actuators
				foreach (GameObject actuator in actuators)
				{
					Debug.Log("Triggered");

                    if (actuator != null)
                        CameraManager.AddTransition(actuator);
				}

                CameraManager.PlayTransitions();

				currentState = STATE.OFF;
				collider2D.enabled = false;
				m_isLocked = true;
			} else {	
				if (previousState != currentState)
				{
					switch (currentState)
					{
					case STATE.OFF:
						Off();
						break;
					case STATE.ON:
						On();
						break;
					}
					
					previousState = currentState;
				}
			}
		} else {
			if (activeBox != null && !renderer.bounds.Intersects(activeBox.renderer.bounds)) {
				m_isLocked = false;
				audio.Play ();
				
				//Activate all of the connected actuators
				foreach (GameObject actuator in actuators)
				{
					Debug.Log("Triggered");

                    if (actuator != null)
                        CameraManager.AddTransition(actuator);
				}

                CameraManager.PlayTransitions();

				collider2D.enabled = true;
				currentState = STATE.ON;
			}
		}
	}
	
	#region Update Methods
	/*
     * Turns the pressure plate off
     */
	virtual protected void Off()
	{
		mSR.sprite = switchOff;
	}
	
	/*
     * Turns the pressure plate on
     */
	virtual protected void On()
	{
		mSR.sprite = switchOn;
	}
	
	/*
     * If the collidee is the player, then all actuators are triggered and the
     * pressure plate is turned off.
     */
	virtual protected void OnTriggerEnter2D(Collider2D collidee)
	{
		if (collidee.tag == "Player")
		{
			audio.Play ();
			
			//Activate all of the connected actuators
			foreach (GameObject actuator in actuators)
			{
				Debug.Log("Triggered");

                if (actuator != null)
                    CameraManager.AddTransition(actuator);
			}

            CameraManager.PlayTransitions();

			currentState = STATE.OFF;
		}
	}
	
	/*
     * If the collidee is the player then the pressure plate is turned on.
     */
	virtual protected void OnTriggerExit2D(Collider2D collidee)
	{
		if (collidee.tag == "Player")
		{
			audio.Play ();
			
			//Activate all of the connected actuators
			foreach (GameObject actuator in actuators)
			{
				Debug.Log("Triggered");

                if (actuator != null)
                    CameraManager.AddTransition(actuator);
			}

            CameraManager.PlayTransitions();

			currentState = STATE.ON;
		}
	}
	#endregion
}
