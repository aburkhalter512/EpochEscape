using UnityEngine;

public abstract class NEWDoor : MonoBehaviour
{
	#region Interface Variables
	public SIDE_STATE initialFirstSideState = SIDE_STATE.OPENED;
    public SIDE_STATE initialSecondSideState = SIDE_STATE.OPENED;
    public SIDE_STATE firstSideState = SIDE_STATE.INIT;
    public SIDE_STATE secondSideState = SIDE_STATE.INIT;

    public GameObject firstSide;
    public GameObject secondSide;
	#endregion

	#region Instance Variables
    SIDE_STATE mPreviousFirstSideState = SIDE_STATE.IDLE;
    SIDE_STATE mPreviousSecondSideState = SIDE_STATE.IDLE;
	
	protected DoorSide mFirstSide;
	protected DoorSide mSecondSide;
	
	protected SpriteRenderer mSR = null;

    bool mForceUpdate = false;
	#endregion

	#region Class Constants
	public enum SIDE
	{
		FIRST,
		SECOND
	}
	
	public enum SIDE_STATE
	{
		INIT,
        IDLE,
		OPENED,
		CLOSED
	}

    public static Vector2 SIZE = new Vector2(.8f, .4f); 
	#endregion
	
	protected virtual void Start()
	{
		mSR = GetComponent<SpriteRenderer>();
		
		if (mSR == null)
			mSR = gameObject.AddComponent<SpriteRenderer>();
	}

    protected virtual void Update()
    {
        #region Update the first side
        if (mPreviousFirstSideState != firstSideState || mForceUpdate)
        {
            switch (firstSideState)
            {
                case SIDE_STATE.INIT:
                    init(SIDE.FIRST);
                    break;
                case SIDE_STATE.IDLE:
                    break;
                case SIDE_STATE.OPENED:
                    open(SIDE.FIRST);
                    break;
                case SIDE_STATE.CLOSED:
                    close(SIDE.FIRST);
                    break;
            }

            mPreviousFirstSideState = firstSideState;
            mForceUpdate = false;
        }
        #endregion

        #region Update the second side
        if (mPreviousSecondSideState != secondSideState)
        {
            switch (secondSideState)
            {
                case SIDE_STATE.INIT:
                    init(SIDE.SECOND);
                    break;
                case SIDE_STATE.IDLE:
                    break;
                case SIDE_STATE.OPENED:
                    open(SIDE.SECOND);
                    break;
                case SIDE_STATE.CLOSED:
                    close(SIDE.SECOND);
                    break;
            }

            mPreviousSecondSideState = secondSideState;
        }
        #endregion
    }

    #region Interface Methods
    protected abstract void init(SIDE side);
	
	protected abstract void open(SIDE side);
	
	protected abstract void close(SIDE side);

    public void attachSide(SIDE side, DoorSide toAttach)
    {
        if (toAttach == null)
            return;

        switch (side)
        {
            case SIDE.FIRST:
                firstSide = toAttach.gameObject;

                mFirstSide = toAttach;
                mFirstSide.side = SIDE.FIRST;
                break;
            case SIDE.SECOND:
                secondSide = toAttach.gameObject;

                mSecondSide = toAttach;
                mSecondSide.side = SIDE.SECOND;
                break;
        }
    }

    public void forceUpdate()
    {
        mForceUpdate = true;
    }
    #endregion
}