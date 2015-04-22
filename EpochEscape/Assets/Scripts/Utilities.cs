using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public class Utilities
{
    #region Class Constants
    public const float DEFAULT_TOLERANCE = 0.05f;

    public enum SIDE_4
    {
        RIGHT,
        TOP,
        LEFT,
        BOTTOM
    }

    public class Vec2<T>
    {
        public T x;
        public T y;

        public Vec2(T x, T y)
        {
            this.x = x;
            this.y = y;
        }
        public Vec2(Vec2<T> v)
        {
            x = v.x;
            y = v.y;
        }

        public bool Equals(Vec2<T> v)
        {
            return x.Equals(v.x) && y.Equals(v.y);
        }
        public bool Equals(T x, T y)
        {
            return this.x.Equals(x) && this.y.Equals(y);
        }

        public override string ToString()
        {
            return x.ToString() + ", " + y.ToString();
        }
    }

    // Just a simple typedef
    public class Vec2Int : Vec2<int>
    {
        public Vec2Int(int x, int y) : base(x, y)
        { }
        public Vec2Int(Vec2Int v) : base(v)
        { }

        public Vec2Int translate(int x, int y)
        {
            this.x += x;
            this.y += y;

            return this;
        }

        public static Vec2Int operator +(Vec2Int v1, Vec2Int v2)
        {
            return new Vec2Int(v1.x + v2.x, v1.y + v2.y);
        }
        public static Vec2Int operator -(Vec2Int v1, Vec2Int v2)
        {
            return new Vec2Int(v1.x - v2.x, v1.y - v2.y);
        }
        public static Vec2Int operator *(Vec2Int v1, int s)
        {
            return new Vec2Int(v1.x * s, v1.y * s);
        }
        public static Vec2Int operator /(Vec2Int v1, int s)
        {
            return new Vec2Int(v1.x / s, v1.y / s);
        }
        public static Vec2Int operator %(Vec2Int v1, int s)
        {
            return new Vec2Int(v1.x % s, v1.y % s);
        }
    }

    // Just a simple typedef
    public class Vec2Float : Vec2<float>
    {
        public Vec2Float(float x, float y) : base(x, y)
        { }
        public Vec2Float(Vec2Float v) : base(v)
        { }

        public Vec2Float translate(float x, float y)
        {
            this.x += x;
            this.y += y;

            return this;
        }
    }

    public class Vec2IntComparer : IEqualityComparer<Vec2Int>, IComparer<Vec2Int>
    {
        private static Vec2IntComparer _instance;

        public static Vec2IntComparer Get()
        {
            if (_instance == null)
                _instance = new Vec2IntComparer();

            return _instance;
        }

        public int Compare(Vec2Int x, Vec2Int y)
        {
            if (x.x < y.x)
                return -1;
            else if (x.x > y.x)
                return 1;
            else if (x.y < y.y)
                return -1;
            else if (x.y > y.y)
                return 1;
            else
                return 0;
        }

        public bool Equals(Vec2Int x, Vec2Int y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(Vec2Int toHash)
        {
            return (toHash.y << 16) ^ toHash.x;
        }
    }

    public class Pair<T, U>
    {
        public T first;
        public U second;

        public Pair() { }

        public Pair(T firstVal, U secondVal)
        {
            first = firstVal;
            second = secondVal;
        }

        public Pair(Pair<T, U> toCopy)
        {
            first = toCopy.first;
            second = toCopy.second;
        }

        public virtual Pair<T, U> assign(Pair<T, U> toAssign)
        {
            return assign(toAssign.first, toAssign.second);
        }

        public virtual Pair<T, U> assign(T firstVal, U secondVal)
        {
            first = firstVal;
            second = secondVal;

            return this;
        }

        public virtual bool equals(Pair<T, U> p)
        {
            return first.Equals(p.first) && second.Equals(p.second);
        }

        public virtual bool equals(T f, U s)
        {
            return first.Equals(f) && second.Equals(s);
        }
		
        public override string ToString()
        {
            return first.ToString() + ", " + second.ToString();
        }
    }

    public class IntPair : Pair<int, int>
    {
        public IntPair()
        {
            first = 0;
            second = 0;
        }

        public IntPair(int f, int s)
        {
            first = f;
            second = s;
        }

        public IntPair(IntPair pair)
        {
            first = pair.first;
            second = pair.second;
        }

        public IntPair floorToEven()
        {
            IntPair retVal = new IntPair(this);

            if (retVal.first % 2 == 1)
                retVal.first--;
            if (retVal.second % 2 == 1)
                retVal.second--;

            return retVal;
        }
		
        public static IntPair deserialize(string s)
        {
            string[] components = s.Split(new [] {','});
            if (components.Length != 2)
                return null;

            components[0].Trim();
            components[1].Trim();
            return new IntPair(Convert.ToInt32(components[0]), Convert.ToInt32(components[1]));
        }

        public static IntPair operator +(IntPair p1, IntPair p2)
        {
            return new IntPair(p1.first + p2.first, p1.second + p2.second);
        }

        public static IntPair operator -(IntPair p1, IntPair p2)
        {
            return new IntPair(p1.first - p2.first, p1.second - p2.second);
        }

        public static IntPair operator *(IntPair p1, IntPair p2)
        {
            return new IntPair(p1.first * p2.first, p1.second * p2.second);
        }

        public static IntPair operator /(IntPair p1, IntPair p2)
        {
            return new IntPair(p1.first / p2.first, p1.second / p2.second);
        }

        // Non-destructive translate; Makes a new int pair
        public static IntPair translate(IntPair pair, int x, int y)
        {
            Utilities.IntPair retVal = new IntPair(pair);
            retVal.first += x;
            retVal.second += y;

            return retVal;
        }
    }

    public class IntPairComparer : IEqualityComparer<IntPair>, IComparer<IntPair>
    {
        private static IntPairComparer mInstance;

        public static IntPairComparer Get()
        {
            if (mInstance == null)
                mInstance = new IntPairComparer();

            return mInstance;
        }

        public int Compare(IntPair x, IntPair y)
        {
            if (x.first < y.first)
                return -1;
            else if (x.first > y.first)
                return 1;

            if (x.second < y.second)
                return -1;
            else if (x.second > y.second)
                return 1;
            else
                return 0;
        }

        public bool Equals(IntPair x, IntPair y)
        {
            return x.equals(y);
        }

        public int GetHashCode(IntPair toHash)
        {
            return toHash.first * toHash.second;
        }
    }
	public class Comparer
    {
        public enum OPERATOR
        {
            LT,
            LE,
            GT,
            GE,
            EQ,
            NE,
        }

        public static bool Compare(int t1, OPERATOR op, int t2)
        {
            switch (op)
            {
                case OPERATOR.LT:
                    return t1 < t2;
                case OPERATOR.LE:
                    return t1 <= t2;
                case OPERATOR.GT:
                    return t1 > t2;
                case OPERATOR.GE:
                    return t1 >= t2;
                case OPERATOR.EQ:
                    return t1 == t2;
                case OPERATOR.NE:
                    return t1 != t2;
            }

            return false;
        }
    }

    public class StringComparer : IComparer<string>
    {
        private static StringComparer mInstance;

        private StringComparer() { }

        public static StringComparer Get()
        {
            if (mInstance == null)
                mInstance = new StringComparer();

            return mInstance;
        }

        public int Compare(string f, string s)
        {
            return f.CompareTo(s);
        }
    }
    #endregion

    #region Static Methods
    public static string toString(Vector3 vec)
    {
        return string.Format("({0:0.00}, {1:0.00}, {2:0.00})", vec.x, vec.y, vec.z);
    }
    public static string toString(Vector2 vec)
    {
        return string.Format("({0:0.00}, {1:0.00})", vec.x, vec.y);
    }

    public static Vector2 StringToVector2(string data)
    {
        string[] vectorComponents = null;
        Vector2 vector = Vector2.zero;

        data = data.Substring(1, data.Length - 2); // Trim paranthesis.

        vectorComponents = data.Split(','); // Get each component.

        // If the number of components is less than 2, then the string is corrupt.
        if (vectorComponents.Length < 2)
            return vector;

        // Trim any whitespace.
        vectorComponents[0] = vectorComponents[0].Trim();
        vectorComponents[1] = vectorComponents[1].Trim();

        // Parse the results.
        vector.x = float.Parse(vectorComponents[0]);
        vector.y = float.Parse(vectorComponents[1]);

        return vector;
    }
    public static Vector3 StringToVector3(string data)
    {
        if (data == "" || data.Length < 8)
        {
            Debug.Log("Could not convert [" + data + "] to a Vector3");
            return Vector3.zero;
        }

        string[] vectorComponents = null;
        Vector3 vector = Vector3.zero;

        data = data.Substring(1, data.Length - 2); // Trim paranthesis.

        vectorComponents = data.Split(','); // Get each component.

        // If the number of components is less than 3, then the string is corrupt.
        if (vectorComponents.Length < 3)
            return vector;

        // Trim any whitespace.
        vectorComponents[0] = vectorComponents[0].Trim();
        vectorComponents[1] = vectorComponents[1].Trim();
        vectorComponents[2] = vectorComponents[2].Trim();

        // Parse the results.
        vector.x = float.Parse(vectorComponents[0]);
        vector.y = float.Parse(vectorComponents[1]);
        vector.z = float.Parse(vectorComponents[2]);

        return vector;
    }

    public static Vector2 copy(Vector2 v) { return new Vector2(v.x, v.y); }
    public static Vector2 toVector2(IntPair p) { return new Vector2(p.first, p.second); }

    public static Vector3 toVector3(Vector2 v) { return new Vector3(v.x, v.y, 0.0f); }
    public static Vector3 toVector3(Vector2 v, float z) { return new Vector3(v.x, v.y, z); }
    public static Vector3 toVector3(IntPair p) { return new Vector3(p.first, p.second, 0); }
    public static Vector3 toVector3(Vec2Int v) { return new Vector3(v.x, v.y, 0); }

    public static Vector3 copy(Vector3 v) { return new Vector3(v.x, v.y, v.z); }

    public static bool IsApproximately(float a, float b, float tolerance = DEFAULT_TOLERANCE)
    {
        return Mathf.Abs(a - b) < tolerance;
    }
    public static bool IsApproximately(Vector3 a, Vector3 b, float tolerance = DEFAULT_TOLERANCE)
    {
        return IsApproximately(a.x, b.x) && IsApproximately(a.y, b.y) && IsApproximately(a.z, b.z);
    }
    public static int roundToMultiple(float num, int multiple)
    {
        return Mathf.RoundToInt(num / multiple) * multiple;
    }

    public static float WrapAngle(float angle)
    {
        angle %= 360;
        return angle < 0.0f ? angle + 360 : angle;
    }

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

    public static string generateUUID(UnityEngine.Object o)
    {
        UnityEngine.Random.seed = Mathf.RoundToInt(Time.realtimeSinceStartup * 1000);
        string preHash = o.GetType().ToString() + UnityEngine.Random.value;

        StringBuilder hash = new StringBuilder();

        using (MD5 md5 = MD5.Create())
        {
            byte[] hashData = md5.ComputeHash(Encoding.UTF8.GetBytes(preHash));

            for (int i = 0; i < hashData.Length; i++)
                hash.Append(hashData[i].ToString("x2"));
        }

        return hash.ToString();
    }

    public static T ParseEnum<T>(string toParse)
    {
        return (T)Enum.Parse(typeof(T), toParse);
    }

    public static SIDE_4 rotateLeft(SIDE_4 side, int turnCount = 1)
    {
        SIDE_4 retVal = side;

        for (int i = 0; i < turnCount; i++)
        {
            switch (retVal)
            {
                case SIDE_4.RIGHT:
                    retVal = SIDE_4.TOP;
                    break;
                case SIDE_4.TOP:
                    retVal = SIDE_4.LEFT;
                    break;
                case SIDE_4.LEFT:
                    retVal = SIDE_4.BOTTOM;
                    break;
                case SIDE_4.BOTTOM:
                    retVal = SIDE_4.RIGHT;
                    break;
            }
        }

        return retVal;
    }
    public static SIDE_4 rotateRight(SIDE_4 side, int turnCount = 1)
    {
        SIDE_4 retVal = side;

        for (int i = 0; i < turnCount; i++)
        {
            switch (retVal)
            {
                case SIDE_4.RIGHT:
                    retVal = SIDE_4.BOTTOM;
                    break;
                case SIDE_4.TOP:
                    retVal = SIDE_4.RIGHT;
                    break;
                case SIDE_4.LEFT:
                    retVal = SIDE_4.TOP;
                    break;
                case SIDE_4.BOTTOM:
                    retVal = SIDE_4.LEFT;
                    break;
            }
        }

        return retVal;
    }
    public static SIDE_4 flip(SIDE_4 side)
    {
        return rotateLeft(side, 2);
    }
    #endregion
}
