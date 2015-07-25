using UnityEngine;

public class ObjectEraser// : Manager<ObjectEraser>, IActivatable
{
	/*#region Interface Variables
	#endregion
	
	#region Instance Variables
    TileCursor mTC;
    InputManager mIM;

    private bool mIsActive = false;
	#endregion
	
	protected override void Awaken()
	{
	}
	
	protected override void Initialize()
    {
        mTC = TileCursor.Get();
        mIM = InputManager.Get();
	}

    protected void Update()
    {
        if (mIsActive)
            updateSelection();
    }
	
	#region Interface Methods
    public void activate()
    {
        mIsActive = true;
    }
    public void deactivate()
    {
        mIsActive = false;
    }
	#endregion
	
	#region Instance Methods
    private void updateSelection()
    {
        if (!mIM.primaryPlace.getDown())
            return;

        TileData tile = mTC.getDetectedTile();

        if (tile == null || !tile.hasAttached())
            return;

        IRemovable removable = tile.getAttached() as IRemovable;

        if (removable != null)
            removable.remove();

        tile.detachObject();
    }
	#endregion*/
}
