using UnityEngine;
using System.Collections;

public class LockedDoorFrame : DoorFrame
{
	#region Interface Variables
    public STATE initialState;
	#endregion
	
	#region Instance Variables
    StandardDoorSide mFrontSide;
    StandardDoorSide mBackSide;

    STATE mCurState;
	#endregion

    #region Class Constants
    public enum STATE
    {
        LOCKED,
        UNLOCKED
    }
    #endregion
	
	protected void Start()
	{
        mFrontSide = frontSide.GetComponent<StandardDoorSide>();
        mBackSide = backSide.GetComponent<StandardDoorSide>();

        switch (initialState)
        {
            case STATE.LOCKED:
                mFrontSide.deactivate();
                mBackSide.deactivate();
                break;
            case STATE.UNLOCKED:
                mFrontSide.activate();
                mBackSide.activate();
                break;
        }

        mCurState = initialState;
	}
	
	#region Interface Methods
    public override void triggerFrontEnter()
    {
        return;
    }
    public override void triggerFrontExit()
    {
        return;
    }

    public override void triggerBackEnter()
    {
        return;
    }
    public override void triggerBackExit()
    {
        return;
    }

    public override void activateSide(SIDE side)
    {
        switch (side)
        {
            case SIDE.FRONT:
                mFrontSide.activate();
                break;
            case SIDE.BACK:
                mBackSide.activate();
                break;
        }
    }
    public override void deactivateSide(SIDE side)
    {
        switch (side)
        {
            case SIDE.FRONT:
                mFrontSide.deactivate();
                break;
            case SIDE.BACK:
                mBackSide.deactivate();
                break;
        }
    }

    public void lockDoor()
    {
        if (mCurState != STATE.LOCKED)
        {
            mFrontSide.deactivate();
            mBackSide.deactivate();

            mCurState = STATE.LOCKED;
        }
    }
    public void unlockDoor()
    {
        if (mCurState != STATE.UNLOCKED)
        {
            mFrontSide.activate();
            mBackSide.activate();

            mCurState = STATE.UNLOCKED;
        }
    }
    public void toggleLock()
    {
        switch (mCurState)
        {
            case STATE.LOCKED:
                unlockDoor();
                break;
            case STATE.UNLOCKED:
                lockDoor();
                break;
        }
    }
	#endregion
	
	#region Instance Methods
	#endregion
}
