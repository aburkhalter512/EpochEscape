using UnityEngine;

public class PowerCoreDoor : NEWDoor
{
    #region Interface Variables
    public GameObject firstOpenedSide;
    public GameObject secondOpenedSide;

    public POWER_CORES powerCores = POWER_CORES.FULL;
    #endregion

    #region Instance Variables
    protected DoorSide mFirstOriginalSide;
    protected DoorSide mSecondOriginalSide;

    protected DoorSide mFirstOpenedSide;
    protected DoorSide mSecondOpenedSide;

    Player mPlayer;

    bool mIsOpened = false;
    #endregion

    #region Class Constants
    public enum POWER_CORES
    {
        ONE,
        TWO,
        FULL
    }
    #endregion

    protected override void Start()
	{
		base.Start();

        if (mSR.sprite == null)
            mSR.sprite = Resources.Load<Sprite>("Textures/Doors/DoorFrames/1Core");

        mFirstOpenedSide = firstOpenedSide.GetComponent<DoorSide>();
        mFirstOpenedSide.gameObject.SetActive(false);
        mSecondOpenedSide = secondOpenedSide.GetComponent<DoorSide>();
        mSecondOpenedSide.gameObject.SetActive(false);

       GameObject GO = GameObject.FindGameObjectWithTag("Player");
       if (GO != null)
           mPlayer = GO.GetComponent<Player>();
	}

    protected override void Update()
    {
        base.Update();

        if (!mIsOpened)
        {
            if (mPlayer == null)
            {
                GameObject GO = GameObject.FindGameObjectWithTag("Player");
                if (GO != null)
                    mPlayer = GO.GetComponent<Player>();
            }

            if (mPlayer != null)
            {
                switch (powerCores)
                {
                    case POWER_CORES.ONE:
                        if (mPlayer.CurrentCores == 1)
                            coresFound();
                        break;
                    case POWER_CORES.TWO:
                        if (mPlayer.CurrentCores == 2)
                            coresFound();
                        break;
                    case POWER_CORES.FULL:
                        if (mPlayer.CurrentCores == mPlayer.MAX_CORES)
                            coresFound();
                        break;
                }
            }
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

    protected void coresFound()
    {
        mFirstOriginalSide = mFirstSide;
        mSecondOriginalSide = mSecondSide;

        mFirstOriginalSide.gameObject.SetActive(false);
        mSecondOriginalSide.gameObject.SetActive(false);

        mFirstSide = mFirstOpenedSide;
        mSecondSide = mSecondOpenedSide;

        mFirstSide.gameObject.SetActive(true);
        mSecondSide.gameObject.SetActive(true);

        mIsOpened = true;
    }
    #endregion
}