using UnityEngine;
using System.Collections;

public class TimerDoorFrame : StandardDoorFrame
{
    #region Interface Variables
    public int time = 1;
    #endregion

    #region Instance Variables
    private int mTimeRemaining = 0;
    private bool mIsTiming = false;
    #endregion

    protected new void Start()
    {
        initialState = STATE.INACTIVE;

        base.Start();
    }

    #region Interface Methods
    public override void toggle()
    {
        if (mState == STATE.INACTIVE)
            activate();
    }

    public override void activate()
    {
        StartCoroutine(countDownTimer());
    }

    public int getTimeRemaining()
    {
        return mTimeRemaining;
    }

    public bool isTiming()
    {
        return mIsTiming;
    }
    #endregion

    #region Instance Methods
    private IEnumerator countDownTimer()
    {
        base.activate();
        mIsTiming = true;
        HUDManager.SetTimer(this);

        for (mTimeRemaining = time; mTimeRemaining >= 0; mTimeRemaining--)
            yield return new WaitForSeconds(1f);

        deactivate();
        mTimeRemaining = 0;
        mIsTiming = false;
    }
    #endregion
}
