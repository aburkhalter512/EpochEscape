using UnityEngine;

public class LockedDoor : NEWDoor
{
    #region Interface Variables
    public GameObject firstUnlockedSide;
    public GameObject secondUnlockedSide;
    #endregion

    #region Instance Variables
    DoorSide mFirstUnlockedSide;
    DoorSide mSecondUnlockedSide;

    DoorSide mFirstOriginalSide;
    DoorSide mSecondOriginalSide;

    bool mIsLocked = true;
    #endregion

    protected override void Start()
    {
        base.Start();

        if (mSR.sprite == null)
            mSR.sprite = Resources.Load<Sprite>("Textures/Doors/DoorFrames/NoEntry");

        mFirstUnlockedSide = firstUnlockedSide.GetComponent<DoorSide>();
        mFirstUnlockedSide.gameObject.SetActive(false);

        mSecondUnlockedSide = secondUnlockedSide.GetComponent<DoorSide>();
        mSecondUnlockedSide.gameObject.SetActive(false);
	}

    protected override void Update()
    {
        base.Update();
    }

    #region Interface Methods
    public bool isLocked()
    {
        return mIsLocked;
    }

    public void toggleLock()
    {
        if (mIsLocked)
            unlockDoor();
        else
            lockDoor();
    }

    public void unlockDoor()
    {
        if (mIsLocked)
        {
            mFirstSide.gameObject.SetActive(false);
            mSecondSide.gameObject.SetActive(false);

            mFirstOriginalSide = mFirstSide;
            mSecondOriginalSide = mSecondSide;

            mFirstSide = mFirstUnlockedSide;
            mSecondSide = mSecondUnlockedSide;

            mFirstSide.gameObject.SetActive(true);
            mSecondSide.gameObject.SetActive(true);

            mIsLocked = false;
        }
    }

    public void lockDoor()
    {
        if (!mIsLocked)
        {
            mFirstSide.gameObject.SetActive(false);
            mSecondSide.gameObject.SetActive(false);

            mFirstSide = mFirstOriginalSide;
            mSecondSide = mSecondOriginalSide;

            mFirstSide.gameObject.SetActive(true);
            mSecondSide.gameObject.SetActive(true);

            mIsLocked = true;
        }
    }
    #endregion

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

    public void Activate()
    {
        toggleLock();
    }
    #endregion
}