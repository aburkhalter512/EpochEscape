using UnityEngine;
using System.Collections;

public class ExitDoor : Door
{
	#region Inspector Variables
	#endregion

	#region Instance Variables
    protected Player player;
	#endregion

	#region Class Constants
	#endregion

	//Put all initialization code here
	//Remember to comment!
	protected override void Start()
	{
        base.Start();
	}

	#region Initialization Methods
	#endregion

	//Put all update code here
	//Remember to comment!
	protected override void Update()
	{
        findPlayer();
        checkPowerCore();

        base.Update();
	}

	#region Update Methods
    protected void findPlayer()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    protected void checkPowerCore()
    {
        if (player != null && player.isPowerCoreComplete())
            currentState = STATE.OPENED;
    }

    protected override void Destroy()
    {
        Debug.Log("Exit door opened");

        base.Destroy();
    }
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
