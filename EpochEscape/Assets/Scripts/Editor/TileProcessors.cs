using UnityEngine;
using SIDE = Utilities.SIDE_4;

public class TileProcessors
{
    // Creates a static wall if and only if at least one side of the tile set (2x2)
    // is null
	public static void createEdgeWall(Tile tile, Utilities.Vec2Int pos)
    {
        if (tile == null)
            return;

        Utilities.Vec2Int basePos = pos;
        if (basePos.x % 2 == 1)
        {
            tile = tile.getSide(SIDE.LEFT);

            if (tile == null)
                return;
        }

        if (basePos.y % 2 == 1)
        {
            tile = tile.getSide(SIDE.BOTTOM);

            if (tile == null)
                return;
        }

        Map map = Map.Get();

        // If any of the sides are null, consider the tile to be a boundary.
        if (tile.getSide(SIDE.LEFT) == null)
        {
            PlaceableStaticWall.createStaticWall(map.toTilePos(tile.position()));
            return;
        }
        else if (tile.getSide(SIDE.BOTTOM) == null)
        {
            PlaceableStaticWall.createStaticWall(map.toTilePos(tile.position()));
            return;
        }

        Tile it = tile.getSide(SIDE.TOP);
        if (it != null && it.getSide(SIDE.TOP) == null)
        {
            PlaceableStaticWall.createStaticWall(map.toTilePos(tile.position()));
            return;
        }

        it = tile.getSide(SIDE.RIGHT);
        if (it != null && it.getSide(SIDE.RIGHT) == null)
        {
            PlaceableStaticWall.createStaticWall(map.toTilePos(tile.position()));
            return;
        }
    }
}
