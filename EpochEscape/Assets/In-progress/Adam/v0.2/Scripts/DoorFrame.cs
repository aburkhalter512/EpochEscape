using UnityEngine;
using System.Collections;

/**
 * This class a door frame for Epoch Escape. It contains two door sides along with player
 * detectors and should provide core interactability with the player.
 * 
 * This DoorFrame and derived classes represent the primary location for interacting with
 * doors via code. It provides an interface for activating (opening) and deactivating
 * (closing) sides and door sides SHOULD BE INTERACTED WITH THROUGH THE DOOR FRAME.
 * While it is possible to find the door side gameobject through GameObject.Find and directly
 * modify the door side THIS IS HIGHLY DISCOURAGED as this might break internal state 
 * variables and can unexpected behaviour.
 * 
 * Interface Variables
 *      frontSide: GameObject
 *          This variable is used for 'attaching' a side to a door frame, specifically the
 *          front side. 'frontSide' should contain a DoorSide component, otherwise null
 *          reference exceptions will occur. This variable must be set on gameobject creation.
 *      backSide: GameObject
 *          This variable is used for 'attaching' a side to a door frame, specifically the
 *          back side. 'backSide' should contain a DoorSide component, otherwise null
 *          reference exceptions will occur. This variable must be set on gameobject creation.
 *          
 * Interface Methods
 *      void triggerFrontEnter()
 *          A method that alerts the door that a gameobject has entered the front detection area.
 *      void triggerBackExit()
 *          A method that alerts the door that a gameobject has exited the back detection area.
 *      void triggerBackEnter()
 *          A method that alerts the door that a gameobject has entered the back detection area.
 *      void triggerBackExit()
 *          A method that alerts the door that a gameobject has exited the back detection area.
 *      void activateSide(SIDE side)
 *          A method that activates a specific side of a door.
 *          Parameters:
 *              side: The side of the door to activate.
 *      void deactivateSide(SIDE side)
 *          A method that deactivates a specific side of a door.
 *          Parameters:
 *              side: The side of the door to deactivate.
 *      void activateSides()
 *          A method that activates both sides of the door.
 *      void deactivateSides
 *          A method that deactivates both sides of the door.
 */
public abstract class DoorFrame : MonoBehaviour, IDetectable
{
    #region Interface Variables
    public GameObject frontSide;
    public GameObject backSide;
	#endregion
	
	#region Instance Variables
    protected FrontDoorDetector mFrontDetector;
    protected BackDoorDetector mBackDetector;
	#endregion 

    #region Class Constants
    public enum SIDE
    {
        FRONT,
        BACK
    }

    public enum STATE
    {
        IDLE,
        ACTIVE,
        DEACTIVE
    }
    #endregion
	
	protected void Start()
	{
        mFrontDetector = transform.GetComponentsInChildren<FrontDoorDetector>()[0];
        mBackDetector = transform.GetComponentsInChildren<BackDoorDetector>()[0];
	}
	
	#region Interface Methods
    /**
     *  void triggerFrontEnter()
     *      A method that alerts the door that a gameobject has entered the front detection area.
     */
    public abstract void triggerFrontEnter();

    /**
     *  void triggerFrontExit()
     *      A method that alerts the door that a gameobject has exited the front detection area.
     */
    public abstract void triggerFrontExit();

    /**
     *  void triggerBackEnter()
     *      A method that alerts the door that a gameobject has entered the back detection area.
     */
    public abstract void triggerBackEnter();

    /**
     *  void triggerBackExit()
     *      A method that alerts the door that a gameobject has exited the back detection area.
     */
    public abstract void triggerBackExit();

    /**
     * void activateSide(SIDE side)
     *      A method that activates a specific side of a door.
     *      Parameters:
     *          side: The side of the door to activate.
     */
    public abstract void activateSide(SIDE side);

    /**
     * void deactivateSide(SIDE side)
     *      A method that deactivates a specific side of a door.
     *      Parameters:
     *          side: The side of the door to deactivate.
     */
    public abstract void deactivateSide(SIDE side);

    /**
     * void activateSides()
     *      A method that activates both sides of the door.
     */
    public void activateSides()
    {
        activateSide(SIDE.FRONT);
        activateSide(SIDE.BACK);
    }

    /**
     * void deactivateSides()
     *      A method that deactivates both sides of the door.
     */
    public void deactivateSides()
    {
        deactivateSide(SIDE.FRONT);
        deactivateSide(SIDE.BACK);
    }
	#endregion
	
	#region Instance Methods
	#endregion
}
