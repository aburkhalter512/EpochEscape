using UnityEngine;
using System.Collections;

/*
 * This script represents the entrance door for any level. It automatically
 * closes once the player leaves the vicinity of the door.
 */
public class EntranceDoor : Door
{
	#region Instance Variables
    protected Player player;
	#endregion

	/*
     * Initializes the Entrance Door
     */
	protected override void Start()
	{
        base.Start();
	}

	/*
     * Updates the state of the Entrance Door
     */
	protected override void Update()
	{
        base.Update();
	}

	#region Update Methods
    /*
     * If the method is triggered with the player then the door will proceed
     * to close.
     */
    protected void OnTriggerExit2D(Collider2D collidee)
    {
        if (collidee.gameObject.tag == "Player")
        {
            Object.Destroy(gameObject.GetComponent<BoxCollider2D>());

            currentState = STATE.CLOSED;
        }
    }
	#endregion
}
