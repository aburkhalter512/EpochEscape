using UnityEngine;

public class SimpleDoor : PlaceableDoor
{
	#region Interface Variables
    public TYPE doorType = TYPE.StandardDoorFrame;

    // These enum values are converted directly into strings for the xml; DON'T MODIFY
    public enum TYPE
    {
        StandardDoorFrame,
        DirectionalDoorFrame,
        EntranceDoorFrame,
        PowerCoreDoorFrame,
        ExitDoorFrame,
        CheckpointDoorFrame
    }
	#endregion
	
	#region Interface Methods
    public override void activate()
    {
        switch (doorType)
        {
            case TYPE.StandardDoorFrame:
                mFrontSide.activate();
                mBackSide.activate();
                break;
            case TYPE.DirectionalDoorFrame:
                mBackSide.activate();
                break;
        }

        mIsActive = true;
    }
    public override void deactivate()
    {
        switch (doorType)
        {
            case TYPE.StandardDoorFrame:
                mFrontSide.deactivate();
                mBackSide.deactivate();
                break;
            case TYPE.DirectionalDoorFrame:
                mBackSide.deactivate();
                break;
        }

        mIsActive = false;
    }
	#endregion

    #region Instance Methods
    protected override string getType()
    {
        return doorType.ToString();
    }
    #endregion
}
