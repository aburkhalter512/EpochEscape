using UnityEngine;
using System.Collections;

public abstract class DoorFrame : MonoBehaviour, IDetectable
{
    #region Interface Variables
    public GameObject frontSide;
    public GameObject backSide;
	#endregion
	
	#region Instance Variables
    protected FrontDoorDetector mFrontDetector;
    protected BackDoorDetector mBackDetector;
	#endregion 

    #region Class Constants
    public enum SIDE
    {
        FRONT,
        BACK
    }

    public enum STATE
    {
        IDLE,
        ACTIVE,
        DEACTIVE
    }
    #endregion
	
	protected void Start()
	{
        mFrontDetector = transform.GetComponentsInChildren<FrontDoorDetector>()[0];
        mBackDetector = transform.GetComponentsInChildren<BackDoorDetector>()[0];
	}
	
	#region Interface Methods
    public abstract void triggerFrontEnter();
    public abstract void triggerFrontExit();

    public abstract void triggerBackEnter();
    public abstract void triggerBackExit();

    public abstract void activateSide(SIDE side);
    public abstract void deactivateSide(SIDE side);

    public void activateSides()
    {
        activateSide(SIDE.FRONT);
        activateSide(SIDE.BACK);
    }
    public void deactivateSides()
    {
        deactivateSide(SIDE.FRONT);
        deactivateSide(SIDE.BACK);
    }
	#endregion
	
	#region Instance Methods
	#endregion
}
