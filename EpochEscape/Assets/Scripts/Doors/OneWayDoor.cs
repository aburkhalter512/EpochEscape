using UnityEngine;

public class OneWayDoor : NEWDoor
{
	protected override void Start()
	{
		base.Start();

        if (mSR.sprite == null)
            mSR.sprite = Resources.Load<Sprite>("Textures/Doors/DoorFrames/Standard");
	}

    protected override void Update()
    {
        base.Update();
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
                if (mFirstSide.wasFrontHit)
                    mFirstSide.open();
                break;
            case SIDE.SECOND:
                if (mSecondSide.wasBackHit)
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
}