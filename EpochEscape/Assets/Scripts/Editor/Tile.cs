using UnityEngine;
using SIDE = Utilities.SIDE_4;

public class Tile
{
	#region Interface Variables
	#endregion
	
	#region Instance Variables
    QuadNode<Tile> _node;
    PlaceableObject _object;

    Texture2D _floorTex;

    Utilities.Vec2Int _pos;

    private static ChunkManager mCM;
	#endregion

    public Tile(QuadNode<Tile> node, PlaceableObject placeableObject, Utilities.Vec2Int v)
    {
        _node = node;
        _node.data(this);
        _object = placeableObject;
        _pos = v;
    }
	
	#region Interface Methods
    public QuadNode<Tile> node()
    {
        return _node;
    }

    public Tile getSide(SIDE side)
    {
        if (_node == null)
            return null;

        QuadNode<Tile> sideNode = _node.getSide(side);

        if (sideNode == null)
            return null;

        return sideNode.data();
    }

    public void position(Utilities.Vec2Int v)
    {
        _pos = v;
    }
    public Utilities.Vec2Int position()
    {
        return _pos.clone();
    }

    public void attachObject(PlaceableObject placeableObject)
    {
        _object = placeableObject;
    }
    public void removeObject()
    {
        _object = null;
    }
    public bool hasObject()
    {
        return _object != null;
    }
    public PlaceableObject getObject()
    {
        return _object;
    }

    public void setFloorTexture(Texture2D tex)
    {
        _floorTex = tex;

        if (!hasObject() && _floorTex != null)
            setTexture(_floorTex);
    }
    public void setTexture(Texture2D tex)
    {
        if (tex == null)
            return;

        if (mCM == null)
            mCM = ChunkManager.Get();

        mCM.setTileTexture(_pos, tex);
    }
    public void resetTexture()
    {
        setTexture(_floorTex);
    }
    public void clearTexture()
    {
        _floorTex = null;

        mCM.clearTileTexture(_pos);
    }

    public void clear()
    {
        clearTexture();

        _object = null;
        _pos = null;
    }
	#endregion

    #region Instance Methods
	#endregion
}
