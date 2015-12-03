using UnityEngine;
using Utilities;

namespace MapEditor
{
    public class TileProcessors
    {
        // Creates a static wall if and only if at least one side of the tile set (2x2)
            
        public static void createEdgeWall(Tile tile, Vec2Int pos)
        {
            if (tile == null)
                return;

            Vec2Int basePos = pos;
            if (basePos.x % 2 != 0)
            {
                tile = tile.getSide(SIDE_4.LEFT);

                if (tile == null)
                    return;
            }

            if (basePos.y % 2 != 0)
            {
                tile = tile.getSide(SIDE_4.BOTTOM);

                if (tile == null)
                    return;
            }

            Map map = Map.Get();

            // If any of the sides are null, consider the tile to be a boundary.
            if (tile.getSide(SIDE_4.LEFT) == null)
            {
                PlaceableStaticWall.createStaticWall(map.toTilePos(tile.position()));
                return;
            }
            else if (tile.getSide(SIDE_4.BOTTOM) == null)
            {
                PlaceableStaticWall.createStaticWall(map.toTilePos(tile.position()));
                return;
            }

            Tile it = tile.getSide(SIDE_4.TOP);
            if (it != null && it.getSide(SIDE_4.TOP) == null)
            {
                PlaceableStaticWall.createStaticWall(map.toTilePos(tile.position()));
                return;
            }

            it = tile.getSide(SIDE_4.RIGHT);
            if (it != null && it.getSide(SIDE_4.RIGHT) == null)
            {
                PlaceableStaticWall.createStaticWall(map.toTilePos(tile.position()));
                return;
            }
        }
    }
}
