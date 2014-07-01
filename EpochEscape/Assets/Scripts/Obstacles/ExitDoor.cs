using UnityEngine;
using System.Collections;

/*
 * An exit door for any level that opens up when all power cores are collected.
 */
public class ExitDoor : Door
{
	#region Instance Variables
    protected Player player;
	#endregion

	/*
     * Initializes the exit door
     */
	protected override void Start()
	{
        base.Start();
	}

	#region Initialization Methods
	#endregion

	/*
     * Updates the state of the Exit Door
     */
	protected override void Update()
	{
        findPlayer();
        checkPowerCore();

        base.Update();
	}

	#region Update Methods
    /*
     * Finds the player (to process if the power cores have been found)
     */
    protected void findPlayer()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    
    /*
     * Checks to see if all power cores have been collected and opens the door
     * if they have.
     */
    protected void checkPowerCore()
    {
        if (player != null && player.isPowerCoreComplete())
            currentState = STATE.OPENED;
    }

    /*
     * Destroys the exit door.
     */
    protected override void Destroy()
    {
        base.Destroy();
    }
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
