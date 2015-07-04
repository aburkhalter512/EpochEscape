using UnityEngine;

public class PlaceableDoorSide : MonoBehaviour, IActivatable, IToggleable
{
	#region Interface Variables
    public Sprite activeSprite;
    public Sprite inactiveSprite;
	#endregion
	
	#region Instance Variables
    private SpriteRenderer mSR;

    private bool mIsActive = true;
	#endregion
	
	protected void Awake ()
	{
        mSR = GetComponent<SpriteRenderer>();

        activate();
	}
	
	#region Interface Methods
    public void activate()
    {
        mSR.sprite = activeSprite;
    }
    public void deactivate()
    {
        mSR.sprite = inactiveSprite;
    }
    public void toggle()
    {
        if (mIsActive)
            deactivate();
        else
            activate();
    }
	#endregion
	
	#region Instance Methods
	#endregion
}
