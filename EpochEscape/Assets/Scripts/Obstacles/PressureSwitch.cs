using UnityEngine;
using System.Collections;

public class PressureSwitch : PressurePlate
{
    #region Inspector Variables
	#endregion

	#region Instance Variables
	#endregion

	#region Class Constants
	#endregion

	//Put all initialization code here
	//Remember to comment!
	protected void Start()
	{
        base.Start();
	}

	#region Initialization Methods
	#endregion

	//Put all update code here
	//Remember to comment!
	protected void Update()
	{
        base.Update();

        mSR = gameObject.GetComponent<SpriteRenderer>();
	}

    #region Update Methods
    override protected void OnTriggerEnter2D(Collider2D collidee)
    {
		if(collidee.tag == "Player" && currentState != STATE.OFF)
		{
			base.OnTriggerEnter2D(collidee);
		}
    }

    //Stop the switch from reseting to ON
    override protected void OnTriggerExit2D(Collider2D collidee)
    { }
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
