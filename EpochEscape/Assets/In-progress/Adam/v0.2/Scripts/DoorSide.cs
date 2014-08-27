using UnityEngine;

public abstract class DoorSide : MonoBehaviour, IDetectable
{
	#region Interface Variables
    public Sprite activeSprite;
    public Sprite deactiveSprite;
	#endregion
	
	#region Instance Variables
    bool mIsInit = false;
    protected bool mIsActive = false;
    protected bool mIsFirstUpdate = true;

    protected SpriteRenderer mSR = null;
	#endregion 

    #region Class Constants
    public static readonly Vector2 SIZE = new Vector2(.8f, .25f);
    #endregion

    //base.Awake must be called to allow the derived door side to change states, otherwise the door
    //side will remain idling.
    protected void Awake()
    {
        mIsInit = true;

        mSR = GetComponent<SpriteRenderer>();
	}
	
	protected void Start()
	{
		//Put initialization code here that DOES rely on other game objects
	}
	
	protected void Update()
	{
        
	}
	
	#region Interface Methods
    public bool isActive()
    {
        return mIsActive;
    }

    public abstract void triggerFrontEnter();
    public abstract void triggerFrontExit();

    public abstract void triggerBackEnter();
    public abstract void triggerBackExit();

    public abstract void deactivate();
    public abstract void activate();
    public abstract void toggle();
	#endregion
	
	#region Instance Methods
	#endregion
}
