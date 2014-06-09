using UnityEngine;
using System.Collections;

public class PressurePlate : Tile
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

    //Put all initialization code here
	//Remember to comment!
	protected void Start()
    {
        mSR = gameObject.GetComponent<SpriteRenderer>();

        previousState = STATE.UN_INIT;
        currentState = STATE.ON;
	}

	#region Initialization Methods
	#endregion

	//Put all update code here
	//Remember to comment!
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
    virtual protected void Off()
    {
        mSR.sprite = switchOff;
    }

    virtual protected void On()
    {
        mSR.sprite = switchOn;
    }

    virtual protected void OnTriggerEnter2D(Collider2D collidee)
    {
        if (collidee.tag == "Player")
        {
			audio.Play ();
            //Activate all of the actuators
            foreach (GameObject actuator in actuators)
            {
                Debug.Log("Triggered");

                if (actuator != null)
				{
					CameraBehavior cameraBehavior = Camera.main.GetComponent<CameraBehavior>();

					Transform parent = actuator.transform.parent;

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

    virtual protected void OnTriggerExit2D(Collider2D collidee)
    {
        if (collidee.tag == "Player")
        {
            currentState = STATE.ON;
        }
    }
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
