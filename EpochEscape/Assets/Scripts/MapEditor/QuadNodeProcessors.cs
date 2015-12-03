using UnityEngine;

namespace MapEditor
{
    public class QuadNodeProcessors
    {
        private static Texture2D _defaultFloorTex;

        public static void createTile(QuadNode<Tile> node, Utilities.Vec2Int pos)
        {
            if (_defaultFloorTex == null)
                _defaultFloorTex = Resources.Load<Texture2D>("Textures/Game Environment/Tile");

            Tile tile = new Tile(node, null, pos);
            tile.setFloorTexture(_defaultFloorTex);
        }
    }
}
