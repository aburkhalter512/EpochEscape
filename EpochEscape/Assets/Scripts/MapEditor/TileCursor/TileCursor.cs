using UnityEngine;

public class TileCursor : Manager<TileCursor>
{
	#region Instance Variables
    protected Tile mDetectedTile;

    protected InputManager mIM;
    protected Map _map;

    Vector3 mV;
    Vector2 _tileSize;

    private Vector3 mCursorPosition = Vector3.zero;
	#endregion

    protected override void Awaken()
    {
    }

	protected override void Initialize()
	{
        mIM = InputManager.Get();
        _map = Map.Get();

        _tileSize = Map.tileSize;
	}

    protected virtual void Update()
    {
        transform.position = getCursor();
        mDetectedTile = _map.getExistingTile(transform.position);
    }

    #region Interface Methods
    public Vector2 tileSize()
    {
        return _tileSize;
    }

    public Tile getDetectedTile()
    {
        return mDetectedTile;
    }
    // Implies fine resolution tiles (.5 of real tile size)
    public TileData[] getDetectedTiles()
    {
        /*Utilities.IntPair logicalCursor = getLogicalCursor(true);

        int size = 1;
        if (logicalCursor.first % 2 == 1)
            size *= 2;
        if (logicalCursor.second % 2 == 1)
            size *= 2;

        TileData[] retVal = new TileData[size];

        Utilities.IntPair basePos = new Utilities.IntPair(logicalCursor.first / 2, logicalCursor.first / 2);
        retVal[0] = mTF.findTile(basePos);

        if (logicalCursor.first % 2 == 1)
        {
            retVal[1] = mTF.findTile(Utilities.IntPair.translate(basePos, 1, 0));

            if (logicalCursor.second % 2 == 1)
            {
                retVal[2] = mTF.findTile(Utilities.IntPair.translate(basePos, 0, 1));
                retVal[3] = mTF.findTile(Utilities.IntPair.translate(basePos, 1, 1));
            }
        }
        else if (logicalCursor.second % 2 == 1)
             retVal[1] = mTF.findTile(Utilities.IntPair.translate(basePos, 0, 1));

        return retVal;*/

        return null;
    }

    public Utilities.IntPair getLogicalCursor(bool fine = false)
    {
        return toLogicalTilePos(mIM.mouse.inWorld(), fine);
    }

    public Vector3 getCursor(bool fine = false)
    {
        mCursorPosition = toTilePos(mIM.mouse.inWorld(), fine);
        return mCursorPosition;
    }

    public Vector3 toTilePos(Vector3 vec, bool fine = false)
    {
        return toTilePos(toLogicalTilePos(vec, fine), fine);
    }
    public Vector3 toTilePos(Utilities.IntPair pos, bool fine = false)
    {
        Vector3 retVal = Utilities.toVector3(pos);
        Vector2 tileSize = fine ?
            _tileSize / 2 :
            _tileSize;
        retVal.x *= tileSize.x;
        retVal.y *= tileSize.y;

        return retVal;
    }

    public Utilities.IntPair toLogicalTilePos(Vector3 vec, bool fine = false)
    {
        Vector3 snapDistance = fine ? 
            Utilities.toVector3(_tileSize / 2) : 
            Utilities.toVector3(_tileSize);

        return new Utilities.IntPair(
            Mathf.RoundToInt(vec.x / snapDistance.x),
            Mathf.RoundToInt(vec.y / snapDistance.y));
    }
    #endregion
}
