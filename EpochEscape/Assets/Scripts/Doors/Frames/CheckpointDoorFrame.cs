using UnityEngine;
using System.Collections;

/**
 * This class is a checkpoint door frame for Epoch Escape. It resembles a directional door
 * highly, but the crucial difference is that a checkpoint is save the first time the player
 * walks through the door.
 * 
 * This CheckpointDoorFrame only allows the player to move one way through a door, back to 
 * front, but not front to back. Once the player moves through the door frame once, then a
 * checkpoint is saved, recording all level state data.
 * 
 * Interface Variables
 *      frontSide: GameObject
 *          This variable is used for 'attaching' a side to a door frame, specifically the
 *          front side. 'frontSide' should contain a StandardDoorSide component, otherwise null
 *          reference exceptions will occur. This variable must be set on gameobject creation.
 *      backSide: GameObject
 *          This variable is used for 'attaching' a side to a door frame, specifically the
 *          back side. 'backSide' should contain a StandardDoorSide component, otherwise null
 *          reference exceptions will occur. This variable must be set on gameobject creation.
 *      respawnLocation: GameObject
 *          This variable is used to determine the respawn location for the player. By using a
 *          gameobject instead of a Vector3, the editor can better interface with the class and
 *          the user can see exactly where the respawn location will be.
 *          
 * Interface Methods
 *      void triggerFrontEnter()
 *          A method that alerts the door that a gameobject has entered the front detection area.
 *      void triggerBackExit()
 *          A method that alerts the door that a gameobject has exited the back detection area.
 *      void triggerBackEnter()
 *          A method that alerts the door that a gameobject has entered the back detection area.
 *          This method directly opens the door sides.
 *      void triggerBackExit()
 *          A method that alerts the door that a gameobject has exited the back detection area.
 *      void activateSide(SIDE side)
 *          For a DirectionalDoorFrame, this method does nothing, as activating/deactivatin a specific
 *          door side would defeat the purpose of a DirectionalDoor. This is just an attempt at making
 *          the directional door as 'Black Box' as possible.
 *          Parameters:
 *              ignored
 *      void deactivateSide(SIDE side)
 *          For a DirectionalDoorFrame, this method does nothing, as activating/deactivatin a specific
 *          door side would defeat the purpose of a DirectionalDoor. This is just an attempt at making
 *          the directional door as 'Black Box' as possible.
 *          Parameters:
 *              ignored
 *      void activateSides()
 *          A method that activates both sides of the door.
 *      void deactivateSides
 *          A method that deactivates both sides of the door.
 */
public class CheckpointDoorFrame : DirectionalDoorFrame
{
    #region Interface Variables
    public GameObject respawnLocation;
    #endregion
    
    #region Instance Variables
    bool didRegisterCheckpoint = false;
    #endregion
    
    protected new void Start()
    {
        base.Start();
    }
    
    #region Interface Methods
    /**
     * triggerFrontExit()
     *      This method is the same as base.triggerFrontExit() except that a checkpoint
     *      is triggered the first time the player exits. Current progress is saved and
     *      the save location is moved a location based off of the Checkpoint door frame.
     */
    public override void triggerFrontEnter()
    {
        base.triggerFrontEnter();

        if (!didRegisterCheckpoint)
            registerCheckpoint();
    }

    public override void lockDoor()
    {
        return;
    }

    public override void unlockDoor()
    {
        return;
    }
    #endregion
    
    #region Instance Methods
    /**
     * This method is empty, as it is unknown how to save checkpoint data at this point
     */
    protected void registerCheckpoint()
    {
        //GameObject.Find("LevelManager").GetComponent<LevelManager>().registerCheckpoint(this, GameObject.FindObjectOfType<Player>());

        LevelManager.SetCheckoint(this);
    }
    #endregion
}
