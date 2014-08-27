using UnityEngine;
using System.Collections;

public class CheckpointDoorFrame : DirectionalDoorFrame
{
	#region Interface Variables
    public GameObject respawnLocation;
	#endregion
	
	#region Instance Variables
    bool didRegisterCheckpoint = false;
	#endregion
	
	protected void Start()
	{
        base.Start();
	}
	
	#region Interface Methods
    public override void triggerFrontExit()
    {
        base.triggerFrontExit();

        if (!didRegisterCheckpoint)
            registerCheckpoint();
    }
	#endregion
	
	#region Instance Methods
    protected void registerCheckpoint()
    {
        return;
    }

    protected void loadCheckpoint()
    {
        didRegisterCheckpoint = true;
    }
	#endregion
}
