using UnityEngine;

public class NoEntryDoor : NEWDoor
{
	protected override void Start()
	{
		base.Start();

        if (mSR.sprite == null)
            mSR.sprite = Resources.Load<Sprite>("Textures/Doors/DoorFrames/NoEntry");
	}

    protected override void Update()
    {
        base.Update();
    }

    #region Update Methods
    protected override void init(NEWDoor.SIDE side)
    {
        return;
    }

    protected override void open(NEWDoor.SIDE side)
    {
        return;
    }

    protected override void close(NEWDoor.SIDE side)
    {
        return;
    }
    #endregion
}