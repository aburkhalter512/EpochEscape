using UnityEngine;

public class EntryDoor : NEWDoor
{
    #region Interface Variables
    public GameObject entranceLocation;

    public GameObject firstAuxillarySide;
    public GameObject secondAuxillarySide;
    #endregion

    #region Instance Variables
    DoorSide mFirstAuxillarySide;
    DoorSide mSecondAuxillarySide;

    bool mIsFirstUpdate = true;
    bool mIsFirstOpen = true;
    #endregion

    protected override void Start()
	{
		base.Start();

        if (mSR.sprite == null)
            mSR.sprite = Resources.Load<Sprite>("Textures/Doors/DoorFrames/Entry");

        mFirstAuxillarySide = firstAuxillarySide.GetComponent<DoorSide>();
        mFirstAuxillarySide.gameObject.SetActive(false);

        mSecondAuxillarySide = secondAuxillarySide.GetComponent<DoorSide>();
        mSecondAuxillarySide.gameObject.SetActive(false);
	}

    protected override void Update()
    {
        base.Update();

        /*if (mIsFirstUpdate)
        {
            mSecondSide.open();
            mFirstSide.open();
            mIsFirstUpdate = false;
        }*/
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
        if (mIsFirstOpen)
        {
            mFirstSide.open();
            mSecondSide.open();

            mIsFirstOpen = false;
        }

        return;
    }

    protected override void close(NEWDoor.SIDE side)
    {
        if (!mIsFirstOpen)
		{
			switch (side)
			{
			case SIDE.FIRST:
					mFirstSide.close ();
					mFirstSide.gameObject.SetActive (false);
					mFirstSide = mFirstAuxillarySide;
					mFirstSide.gameObject.SetActive (true);
					break;
			case SIDE.SECOND:
					mSecondSide.close ();
					mSecondSide.gameObject.SetActive (false);
					mSecondSide = mSecondAuxillarySide;
					mSecondSide.gameObject.SetActive (true);
					break;
			}
		}
    }
    #endregion
}