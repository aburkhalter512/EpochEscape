using UnityEngine;

public class EntryDoor : NEWDoor
{
    #region Interface Variables
    public GameObject entranceLocation;
    #endregion

    #region Instance Variables
    bool mIsFirstUpdate = true;
    #endregion

    protected override void Start()
	{
		base.Start();

        if (mSR.sprite == null)
            mSR.sprite = Resources.Load<Sprite>("Textures/Doors/DoorFrames/Entry");
	}

    protected override void Update()
    {
        base.Update();

        if (mIsFirstUpdate)
        {
            mSecondSide.open();
            mFirstSide.open();
            mIsFirstUpdate = false;
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
        return;
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
}