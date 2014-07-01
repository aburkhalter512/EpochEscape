using UnityEngine;
using System.Collections;

/*
 * A specialized Pressure Plate that can only be activated once.
 */
public class PressureSwitch : PressurePlate
{
	/*
     * Initializes the Pressure Switch
     */
	protected void Start()
	{
        base.Start();
	}

	/*
     * Updates the state of the pressure switch
     */
	protected void Update()
	{
        base.Update();

        mSR = gameObject.GetComponent<SpriteRenderer>();
	}

    #region Update Methods
    /*
     * If the collidee is the player, and the Pressure Switch has not been
     * activated previously, then it is activated.
     */
    override protected void OnTriggerEnter2D(Collider2D collidee)
    {
		if(collidee.tag == "Player" && currentState != STATE.OFF)
		{
			base.OnTriggerEnter2D(collidee);
		}
    }

    //Does nothing, stops the switch from reseting to ON
    override protected void OnTriggerExit2D(Collider2D collidee)
    { }
	#endregion
}
