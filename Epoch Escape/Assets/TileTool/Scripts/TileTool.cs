using UnityEngine;
//using UnityEditor;
using System.Collections;

public class TileTool : MonoBehaviour
{
    #region Inspector Variables
    public Vector2 mTileSize = Vector2.zero;
	#endregion

    #region Instance Variables
    private Vector3 mTilePos = Vector3.zero;
    private ArrayList mTiles = new ArrayList();
    private ArrayList mTileNames = new ArrayList();
    private Tile mToPlace = null;
	#endregion

	#region Class Constants
	#endregion

	//Put all initialization code here
	//Remember to comment!
	protected void Start()
	{
        UpdateCrossHairs();
	}

	#region Initialization Methods
    public void InitializeTool()
    {
        if (mTileNames.Count == 0)
            mTileNames.Add(new GUIContent("Select One"));

        UpdateCrossHairs();
    }
	#endregion

	//Put all update code here
	//Remember to comment!
	protected void Update()
	{
	}

	#region Update Methods
    public void UpdateCrossHairs()
    {
        transform.GetChild(0).transform.localPosition = Vector3.zero;
        transform.GetChild(1).transform.localPosition = new Vector3(mTileSize.x, 0.0f, 0.0f);
        transform.GetChild(2).transform.localPosition = new Vector3(0.0f, mTileSize.y, 0.0f);
        transform.GetChild(3).transform.localPosition = Utilities.toVector3(mTileSize);
    }

    public Vector3 getPos()
    {
        return Utilities.copy(mTilePos);
    }

    public void AddTile(Tile tile)
    {
        if (tile == null)
            return;

        mTiles.Add(tile);
        mTileNames.Add(new GUIContent(tile.name));
    }

    public void ClearTiles()
    {
        mTiles.Clear();
        mTileNames.Clear();

        InitializeTool();
    }

    public void TileToPlace(int index)
    {
        if (index < 1 || index >= mTileNames.Count)
        {
            mToPlace = null;
            return;
        }

        mToPlace = (Tile) mTiles[index - 1];
    }

    public GUIContent[] TileNames()
    {
        if (mTileNames.Count == 0)
        {
            return null;
        }

        GUIContent[] retVal = new GUIContent[mTileNames.Count];

        for (int i = 0; i < mTileNames.Count; i++)
            retVal[i] = (GUIContent) mTileNames[i];

        return retVal;
    }

    public void Shift(float x, float y)
    {
        mTilePos.x += x;
        mTilePos.y += y;
        transform.Translate(x, y, 0.0f);
    }

    public void PlaceTile()
    {
        if (mToPlace == null)
            return;

        Tile toPlace = GameObject.Instantiate(mToPlace) as Tile;
        toPlace.transform.position = transform.position + Utilities.toVector3(mTileSize / 2);
    }
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
