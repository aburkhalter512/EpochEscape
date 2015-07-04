using UnityEngine;

namespace Utilities
{
    public class Graphics
    {
        public static Texture2D subTexture(Texture2D srcTex, int x, int y, int width, int height)
        {
            if (srcTex == null)
                return null;

            Texture2D retVal = new Texture2D(width, height);
            retVal.SetPixels(srcTex.GetPixels(x, y, width, height));
            retVal.Apply();

            return retVal;
        }

        public static Mesh makeQuadMesh()
        {
            return makeQuadMesh(new Vector2(1, 1));
        }
        public static Mesh makeQuadMesh(Vector2 size)
        {
            if (size.x <= 0 || size.y <= 0)
                return makeQuadMesh();

            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[]
        {
            new Vector3(size.x / 2, size.y / 2,  0),
            new Vector3(size.x / 2, size.y / -2,  0),
            new Vector3(size.x / -2, size.y / 2, 0),
            new Vector3(size.x / -2, size.y / -2,  0)
        };

            Vector2[] uv = new Vector2[]
        {
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(0, 0),
        };

            int[] triangles = new int[]
        {
            0, 1, 2,
            2, 1, 3,
        };

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}