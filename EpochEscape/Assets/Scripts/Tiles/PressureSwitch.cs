using UnityEngine;
using System.Collections;

public class PressureSwitch : PressurePlate
{
	#region Inspector Variables
    public Sprite switchOn;
    public Sprite switchOff;

    public STATE currentState;
	#endregion

	#region Instance Variables
    private SpriteRenderer mSR;

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
    protected virtual void Off()
    {
        mSR.sprite = switchOff;
    }

    protected virtual void On()
    {
        mSR.sprite = switchOn;
    }

    protected void OnTriggerEnter2D(Collider2D collidee)
    {
        base.OnTriggerEnter2D(collidee);

		if(collidee.tag == "Player")
		{
            currentState = STATE.OFF;
		}
    }
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
