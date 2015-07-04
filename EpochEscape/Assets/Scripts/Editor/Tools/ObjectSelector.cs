using UnityEngine;

public class ObjectSelector// : Manager<ObjectSelector>, IActivatable
{
	/*#region Interface Variables
	#endregion
	
	#region Instance Variables
    private TileCursor mTC;
    private InputManager mIM;

    private bool mIsActive = false;

    private IPlaceable mSelection;
	#endregion
	
	protected override void Awaken()
	{
	
	}
	
	protected override void Initialize ()
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

        if (mSelection != null)
        {
            mSelection.deselect();
            mSelection = null;
        }
    }
	#endregion
	
	#region Instance Methods
    private void updateSelection()
    {
        if (!mIM.primaryPlace.getUp())
            return;

        TileData tile = mTC.getDetectedTile();
        if (tile == null || !tile.hasAttached())
        {
            Debug.Log("No attached");

            if (mSelection != null)
            {
                mSelection.deselect();
                mSelection = null;
            }

            return;
        }

        IPlaceable newSelection = tile.getAttached();
        if (mSelection == null)
        {
            mSelection = newSelection;
            mSelection.select();
        }
        else if (mSelection.getID() == newSelection.getID())
        {
            mSelection.deselect();
            mSelection = null;
        }
        else
        {
            IConnector connector = mSelection as IConnector;

            if (mIM.auxillaryInput.get() && mSelection != null && connector != null)
                connector.connect(newSelection as IConnectable);
            else
            {
                mSelection.deselect();

                mSelection = newSelection;
                mSelection.select();
            }
        }
    }
	#endregion*/
}
