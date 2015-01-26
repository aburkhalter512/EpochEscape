using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PowerCoreDoorFrame : DoorFrame<StandardDoorSide, StandardDoorSide>
{
    #region Interface Variables
    public CORES powerCores = CORES.FULL;
    #endregion
    
    #region Instance Variables
    protected bool mHasUnlocked = false;
    #endregion

    #region Class Constants
    public new enum STATE
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
    
    protected new void Start()
    {
        initialState = DoorFrame<StandardDoorSide, StandardDoorSide>.STATE.INACTIVE;

        base.Start();
    }

    protected void Update()
    {
        if (!mHasUnlocked)
        {
            switch (powerCores)
            {
                case CORES.ONE:
                    if (Player.Get().getCurrentCores() == 1)
                        unlockDoor();
                    break;
                case CORES.TWO:
                    if (Player.Get().getCurrentCores() == 2)
                        unlockDoor();
                    break;
                case CORES.FULL:
                    if (Player.Get().getCurrentCores() == 3)
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

    public override void activate()
    {
        return;
    }
    public override void deactivate()
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

    public override void OnFinishTransition()
    {
        mFrontSide.activate();
        mBackSide.deactivate();
    }

    public override void Reset()
    {
        mFrontSide.deactivate();
        mBackSide.deactivate();
    }

    public override void Serialize(ref Dictionary<string, object> data)
    {
        if (data != null)
            data["powerCores"] = (int)powerCores;
    }

    public override void Unserialize(ref Dictionary<string, object> data)
    {
        if (data != null)
        {
            if (data.ContainsKey("powerCores"))
                powerCores = (CORES)int.Parse(data["powerCores"].ToString());
        }
    }
}
