using UnityEngine;
using System.Collections;

public class CustomDoorFrame : DoorFrame
{
	#region Interface Variables
    public STATE frontInitialState;
    public STATE backInitialState;
	#endregion
	
	#region Instance Variables
    DoorSide mFrontSide;
    DoorSide mBackSide;
	#endregion 
	
	protected void Start()
	{
        base.Start();

        mFrontSide = frontSide.GetComponent<DoorSide>();
        switch (frontInitialState)
        {
            case STATE.ACTIVE:
                mFrontSide.activate();
                break;
            case STATE.DEACTIVE:
                mFrontSide.deactivate();
                break;
        }

        mBackSide = backSide.GetComponent<DoorSide>();
        switch (backInitialState)
        {
            case STATE.ACTIVE:
                mBackSide.activate();
                break;
            case STATE.DEACTIVE:
                mBackSide.deactivate();
                break;
        }
	}
	
	protected void Update()
	{
		//Put update code here
	}
	
	#region Interface Methods
    public override void triggerFrontEnter()
    {
        mFrontSide.triggerFrontEnter();
        mBackSide.triggerBackEnter();
    }
    public override void triggerFrontExit()
    {
        mFrontSide.triggerFrontExit();
        mBackSide.triggerBackExit();
    }

    public override void triggerBackEnter()
    {
        mFrontSide.triggerBackEnter();
        mBackSide.triggerFrontEnter();
    }
    public override void triggerBackExit()
    {
        mFrontSide.triggerBackExit();
        mBackSide.triggerFrontExit();
    }

    public override void activateSide(SIDE side)
    {
        switch (side)
        {
            case SIDE.FRONT:
                mFrontSide.activate();
                break;
            case SIDE.BACK:
                mBackSide.activate();
                break;
        }
    }
    public override void deactivateSide(SIDE side)
    {
        switch (side)
        {
            case SIDE.FRONT:
                mFrontSide.deactivate();

                break;
            case SIDE.BACK:
                mBackSide.deactivate();
                break;
        }
    }
	#endregion
	
	#region Instance Methods
	#endregion
}
