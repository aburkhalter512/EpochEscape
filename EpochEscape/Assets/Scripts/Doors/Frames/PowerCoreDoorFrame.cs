using UnityEngine;
using System.Collections;

public class PowerCoreDoorFrame : DoorFrame, ITransitional
{
    #region Interface Variables
    public CORES powerCores = CORES.FULL;
    #endregion
    
    #region Instance Variables
    protected StandardDoorSide mFrontSide;
    protected StandardDoorSide mBackSide;

    protected STATE mCurState;

    protected Player mPlayer;

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

        GameObject GO = GameObject.FindGameObjectWithTag("Player");
        if (GO != null)
            mPlayer = GO.GetComponent<Player>();
    }

    protected void Update()
    {
        if (mPlayer == null)
        {
            GameObject GO = GameObject.FindGameObjectWithTag("Player");
            if (GO != null)
                mPlayer = GO.GetComponent<Player>();
        }
        else if (!mHasUnlocked)
        {
            switch (powerCores)
            {
                case CORES.ONE:
                    if (mPlayer.currentCores == 1)
                        unlockDoor();
                    break;
                case CORES.TWO:
                    if (mPlayer.currentCores == 2)
                        unlockDoor();
                    break;
                case CORES.FULL:
                    if (mPlayer.currentCores == mPlayer.MAX_CORES)
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

    public float GetWaitTime()
    {
        return 0.33f; // Wait for 1/3 of a second after unlocking the door.
    }
}
