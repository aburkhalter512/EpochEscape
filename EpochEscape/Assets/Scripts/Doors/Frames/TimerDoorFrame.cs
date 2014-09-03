using UnityEngine;
using System.Collections;

public class TimerDoorFrame : LockedDoorFrame
{
    #region Interface Variables
    public int time = 1;
    public bool timeActive = false;
    #endregion

    #region Instance Variables
    private int timeRemaining;
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

    public override void lockDoor()
    {
        return;
    }
    public override void unlockDoor()
    {
        base.unlockDoor();

        startTimer();
    }
    public override void toggleLock()
    {
        Debug.Log("Toggling");

        if (mCurState != STATE.UNLOCKED)
            unlockDoor();
    }
    #endregion

    #region Instance Methods
    protected void startTimer()
    {
        StartCoroutine("countDownTimer");
        timeActive = true;
    }

    private IEnumerator countDownTimer()
    {
        for (int timeLeft = time; timeLeft > 0; timeLeft--)
        {
            Debug.Log(timeLeft);
            timeRemaining = timeLeft;
            yield return new WaitForSeconds(1.0f);
        }
        stopTimer();
    }

    private void stopTimer()
    {
        base.lockDoor();
        timeActive = false;
    }
    #endregion
    
    public void OnGUI(){
    	if (timeActive) {
			GUI.Label(new Rect(650.0f, 0.0f, 100.0f, 75.0f), timeRemaining.ToString());
    	}
    }
}
