using UnityEngine;
using System.Collections;

public class DirectionalDoorFrame : DoorFrame
{
	#region Interface Variables
	#endregion
	
	#region Instance Variables
    protected StandardDoorSide mFrontSide;
    protected StandardDoorSide mBackSide;

    bool mIsFrontHit = false;
    bool mIsBackHit = false;
	#endregion

    #region Class Constants
    #endregion
	
	protected void Start()
	{
        mFrontSide = frontSide.GetComponent<StandardDoorSide>();
        mBackSide = backSide.GetComponent<StandardDoorSide>();

        mFrontSide.deactivate();
        mBackSide.activate();
	}
	
	#region Interface Methods
    public override void triggerFrontEnter()
    {
        mIsFrontHit = true;
    }
    public override void triggerFrontExit()
    {
        if (!mIsBackHit)
            mFrontSide.deactivate();

        mIsFrontHit = false;
    }

    public override void triggerBackEnter()
    {
        mIsBackHit = true;
        mFrontSide.activate();
    }
    public override void triggerBackExit()
    {
        if (!mIsFrontHit)
            mFrontSide.deactivate();

        mIsBackHit = false;
    }

    public override void activateSide(SIDE side)
    {
        return;
    }
    public override void deactivateSide(SIDE side)
    {
        return;
    }
	#endregion
	
	#region Instance Methods
	#endregion
}
