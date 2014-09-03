using UnityEngine;
using System.Collections;

public class TimerDoorFrame : LockedDoorFrame
{
    #region Interface Variables
    public int time = 1;
    #endregion
    
    #region Instance Variables
    private float mCurTime = -1;
    private float mDestTime = -1;
    private int mIntTime = -1;
    #endregion

    #region Class Constants
    #endregion
    
    protected void Start()
    {
        base.Start();
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
        return;
    }
    public virtual void unlockDoor()
    {
        base.unlockDoor();

        startTimer();
    }
    public virtual void toggleLock()
    {
        return;
    }
    #endregion
    
    #region Instance Methods
    protected void startTimer()
    {
        mCurTime = Time.realtimeSinceStartup;
        mDestTime = mCurTime + time;
        mIntTime = time;

        countDownTimer();
    }

    private IEnumerator countDownTimer()
    {
        for (int timeLeft = time; timeLeft > 0; timeLeft--)
            yield return new WaitForSeconds(1.0f);

        stopTimer();
    }

    private void stopTimer()
    {
        base.lockDoor();
    }
    #endregion
}
