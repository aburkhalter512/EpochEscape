using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class LockedDoorFrame : DoorFrame, ITransitional, IResettable
{
    #region Interface Variables
    public STATE initialState;
    #endregion
    
    #region Instance Variables
    protected StandardDoorSide mFrontSide;
    protected StandardDoorSide mBackSide;

    protected STATE mCurState;
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
        
        Reset();
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

    public virtual void lockDoor()
    {
        if (mCurState != STATE.LOCKED)
        {
            mFrontSide.deactivate();
            mBackSide.deactivate();

            mCurState = STATE.LOCKED;
        }
    }
    public virtual void unlockDoor()
    {
        if (mCurState != STATE.UNLOCKED)
        {
            mFrontSide.activate();
            mBackSide.activate();

            mCurState = STATE.UNLOCKED;
        }
    }
    public virtual void toggleLock()
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

    public virtual void OnFinishTransition()
    {
        toggleLock();
    }

    public virtual void OnReadyIdle()
    {
    }

    public virtual float GetWaitTime()
    {
        return 0.33f;
    }

    public void Reset()
    {
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

    public override void Serialize(ref Dictionary<string, object> data)
    {
        if (data != null)
        {
            base.Serialize(ref data);

            data["initialState"] = (int)initialState;
        }
    }

    public override void Unserialize(ref Dictionary<string, object> data)
    {
        if (data != null)
        {
            if (data.ContainsKey("initialState"))
                initialState = (STATE)int.Parse(data["initialState"].ToString());
        }
    }
}
