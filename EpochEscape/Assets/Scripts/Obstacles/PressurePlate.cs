using UnityEngine;
using System.Collections;

public class PressurePlate : MonoBehaviour
{
	#region Inspector Variables
    public GameObject[] actuators;

    public Sprite switchOn;
    public Sprite switchOff;

    public STATE currentState;
    #endregion

    #region Instance Variables
    protected SpriteRenderer mSR;

    private STATE previousState;
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
	}

	/*
     * Updates the state of the Pressure Plate
     */
	protected void Update()
    {
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
				{
					CameraBehavior cameraBehavior = Camera.main.GetComponent<CameraBehavior>();

					Transform parent = actuator.transform.parent;

                    //Add the actuators camera movement stack. They will be activated in order
                    //when the previous has finished changing.
					if(parent != null && parent.tag == "WallPivot")
                    {
						if(cameraBehavior.m_lerpTargets.Count == 0 || cameraBehavior.m_lerpTargets.Peek() != parent.gameObject)
							cameraBehavior.m_lerpTargets.Push(parent.gameObject);
                    }
					else
						cameraBehavior.m_lerpTargets.Push(actuator);
				}
            }

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
            currentState = STATE.ON;
        }
    }
	#endregion
}
