using UnityEngine;

public class TimerDoor : NEWDoor
{
    #region Interface Variables
    public GameObject firstTimedSide;
    public GameObject secondTimedSide;

    public float timerDuration;

    public bool startTimer = false;
    #endregion

    #region Instance Variables
    protected DoorSide mFirstOriginalSide;
    protected DoorSide mSecondOriginalSide;

    protected DoorSide mFirstTimedSide;
    protected DoorSide mSecondTimedSide;

    float mStartTime;
    float mCurrentTime;

    bool mIsTiming = false;
    #endregion

    #region Class Constants
    public const float DEFAULT_TIMER_DURATION = 1.0f;
    #endregion

    protected override void Start()
	{
		base.Start();

        if (mSR.sprite == null)
            mSR.sprite = Resources.Load<Sprite>("Textures/Doors/DoorFrames/Timer");

        if (timerDuration <= 0.0f)
            timerDuration = DEFAULT_TIMER_DURATION;

        mFirstTimedSide = firstTimedSide.GetComponent<DoorSide>();
        mFirstTimedSide.gameObject.SetActive(false);
        mSecondTimedSide = secondTimedSide.GetComponent<DoorSide>();
        mSecondTimedSide.gameObject.SetActive(false);
	}

    protected override void Update()
    {
        base.Update();

        if (startTimer)
        {
            startTimer = false;
            beginTimer();
        }

        if (mIsTiming)
        {
            mCurrentTime = Time.realtimeSinceStartup;

            if (mCurrentTime - mStartTime > timerDuration)
                stopTimer();
        }
    }

    #region Update Methods
    protected override void init(NEWDoor.SIDE side)
    {
        switch (side)
        {
            case SIDE.FIRST:
                mFirstSide.init();
                break;
            case SIDE.SECOND:
                mSecondSide.init();
                break;
        }
    }

    protected override void open(NEWDoor.SIDE side)
    {
        switch (side)
        {
            case SIDE.FIRST:
                mFirstSide.open();
                break;
            case SIDE.SECOND:
                mSecondSide.open();
                break;
        }
    }

    protected override void close(NEWDoor.SIDE side)
    {
        switch (side)
        {
            case SIDE.FIRST:
                mFirstSide.close();
                break;
            case SIDE.SECOND:
                mSecondSide.close();
                break;
        }
    }
    #endregion

    #region Interface Methods
    public void beginTimer()
    {
        mStartTime = Time.realtimeSinceStartup;

        mFirstOriginalSide = mFirstSide;
        mSecondOriginalSide = mSecondSide;

        mFirstOriginalSide.gameObject.SetActive(false);
        mSecondOriginalSide.gameObject.SetActive(false);

        mFirstTimedSide.gameObject.SetActive(true);
        mSecondTimedSide.gameObject.SetActive(true);

        mFirstSide = mFirstTimedSide;
        mSecondSide = mSecondTimedSide;

        mIsTiming = true;

        forceUpdate();
    }

    public void stopTimer()
    {
        mStartTime = -1;

        mFirstOriginalSide.gameObject.SetActive(true);
        mSecondOriginalSide.gameObject.SetActive(true);

        mFirstSide = mFirstOriginalSide;
        mSecondSide = mSecondOriginalSide;

        mFirstTimedSide.gameObject.SetActive(false);
        mSecondTimedSide.gameObject.SetActive(false);

        mIsTiming = false;

        forceUpdate();
    }
    #endregion
}