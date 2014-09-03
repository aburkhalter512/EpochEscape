using UnityEngine;
using System.Collections;

/**
 * This class is a directional door frame for Epoch Escape. Maintains all of the requirements
 * of a door frame, but limits them to specific cases. For more information about door frame
 * requirements/documentation see DoorFrame.cs
 * 
 * This DirectionalDoorFrame only allows the player to move one way through a door, back to 
 * front, but not front to back. A good analogy for how this door acts is an electrical diode.
 * Electricity can move one way through it, but not the other. Once the player exits through
 * the front side, the door closes behind the player.
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
public class DirectionalDoorFrame : LockedDoorFrame
{
    #region Interface Variables
    #endregion
    
    #region Instance Variables
    bool mIsFrontHit = false;
    bool mIsBackHit = false;
    #endregion

    #region Class Constants
    #endregion
    
    protected void Start()
    {
        mFrontSide = frontSide.GetComponent<StandardDoorSide>();
        mBackSide = backSide.GetComponent<StandardDoorSide>();

        mFrontSide.deactivate();

        mCurState = initialState;

        switch (mCurState)
        {
            case STATE.UNLOCKED:
                mBackSide.activate();
                break;
            case STATE.LOCKED:
                mBackSide.deactivate();
                break;
        }
    }

    #region Interface Methods
    /**
     *  void triggerFrontEnter()
     *      A method that alerts the door that a gameobject has entered the front detection area.
     */
    public override void triggerFrontEnter()
    {
        mIsFrontHit = true;
    }

    /**
     *  void triggerFrontExit()
     *      A method that alerts the door that a gameobject has exited the front detection area.
     */
    public override void triggerFrontExit()
    {
        if (!mIsBackHit)
            mFrontSide.deactivate();

        mIsFrontHit = false;
    }

    /**
     *  void triggerBackEnter()
     *      A method that alerts the door that a gameobject has entered the back detection area.
     *      This method directly opens the door sides.
     */
    public override void triggerBackEnter()
    {
        if (mCurState != STATE.LOCKED)
        {
            mIsBackHit = true;
            mFrontSide.activate();
        }
    }

    /**
     *  void triggerBackExit()
     *      A method that alerts the door that a gameobject has exited the back detection area.
     */
    public override void triggerBackExit()
    {
        if (!mIsFrontHit)
            mFrontSide.deactivate();

        mIsBackHit = false;
    }

    /**
     * void activateSide(SIDE side)
     *      For a DirectionalDoorFrame, this method does nothing, as activating/deactivatin a specific
     *      door side would defeat the purpose of a DirectionalDoor. This is just an attempt at making
     *      the directional door as 'Black Box' as possible.
     *      Parameters:
     *          ignored
     */
    public override void activateSide(SIDE side)
    {
        return;
    }

    /**
     * void deactivateSide(SIDE side)
     *      For a DirectionalDoorFrame, this method does nothing, as activating/deactivatin a specific
     *      door side would defeat the purpose of a DirectionalDoor. This is just an attempt at making
     *      the directional door as 'Black Box' as possible.
     *      Parameters:
     *          ignored
     */
    public override void deactivateSide(SIDE side)
    {
        return;
    }

    public override void unlockDoor()
    {
        if (mCurState != STATE.UNLOCKED)
        {
            mBackSide.activate();
            mCurState = STATE.LOCKED;
        }
    }

    public override void lockDoor()
    {
        if (mCurState != STATE.LOCKED)
        {
            mBackSide.deactivate();
            mCurState = STATE.UNLOCKED;
        }
    }
    #endregion
    
    #region Instance Methods
    #endregion
}
