using UnityEngine;
using System.Collections;

public class TileCreator
{
    #region Class Constants
    private static Texture2D mDefaultFloorTex;

    private static EpochMap mEM;

    private static TileFinder mTF;
    #endregion
	
	#region Interface Methods
    public static void createTile(QuadMatrix<TileData>.Node node, bool debug = false)
    {
        if (mEM == null)
            mEM = EpochMap.Get();

        if (mDefaultFloorTex == null)
            mDefaultFloorTex = Resources.Load<Texture2D>("Textures/Floor Tiles/testTile");

        if (mTF == null)
            mTF = TileFinder.Get();

        TileData data = new TileData();
        node.setData(data);

        data.setFloor(mDefaultFloorTex);

        // Register Tile for searching
        mTF.registerTile(data);
    }
	#endregion
}
