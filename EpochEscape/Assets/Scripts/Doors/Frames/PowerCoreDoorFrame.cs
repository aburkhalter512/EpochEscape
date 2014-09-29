using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PowerCoreDoorFrame : DoorFrame, ITransitional, IResettable, ISerializable
{
    #region Interface Variables
    public CORES powerCores = CORES.FULL;
    #endregion
    
    #region Instance Variables
    protected StandardDoorSide mFrontSide;
    protected StandardDoorSide mBackSide;

    protected STATE mCurState;

    protected bool mHasUnlocked = false;
    #endregion

    #region Class Constants
    public enum STATE
    {
        LOCKED,
        UNLOCKED
    }

    public enum CORES
    {
        NONE,
        ONE,
        TWO,
        FULL
    }
    #endregion
    
    protected void Start()
    {
        mFrontSide = frontSide.GetComponent<StandardDoorSide>();
        mFrontSide.deactivate();

        mBackSide = backSide.GetComponent<StandardDoorSide>();
        mBackSide.deactivate();
    }

    protected void Update()
    {
        if (!mHasUnlocked)
        {
            switch (powerCores)
            {
                case CORES.ONE:
                    if (PlayerManager.GetCores() == 1)
                        unlockDoor();
                    break;
                case CORES.TWO:
                    if (PlayerManager.GetCores() == 2)
                        unlockDoor();
                    break;
                case CORES.FULL:
                    if (PlayerManager.GetCores() == 3)
                        unlockDoor();
                    break;
            }
        }
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
        return;
    }
    public override void deactivateSide(SIDE side)
    {
        return;
    }
    #endregion
    
    #region Instance Methods
    protected void lockDoor()
    {
        mHasUnlocked = false;

        mFrontSide.deactivate();
        mBackSide.deactivate();
    }

    protected void unlockDoor()
    {
        mHasUnlocked = true;

        CameraManager.AddTransition(gameObject);
        CameraManager.PlayTransitions();
    }
    #endregion

    public void OnFinishTransition()
    {
        mFrontSide.activate();
        mBackSide.activate();
    }

    public void OnReadyIdle()
    {
    }

    public float GetWaitTime()
    {
        return 0.33f; // Wait for 1/3 of a second after unlocking the door.
    }

    public void Reset()
    {
        lockDoor();
    }

    public void Serialize(ref Dictionary<string, object> data)
    {
        if (data != null)
            data["powerCores"] = (int)powerCores;
    }

    public void Unserialize(ref Dictionary<string, object> data)
    {
        if (data != null)
        {
            if (data.ContainsKey("powerCores"))
                powerCores = (CORES)int.Parse(data["powerCores"].ToString());
        }
    }
}
