using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public class Utilities
{
    #region Instance Variables
    #endregion

    #region Class Constants
    public const float DEFAULT_TOLERANCE = 0.05f;

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

        public override string ToString()
        {
            return first.ToString() + ", " + second.ToString();
        }
    }

    public class IntPair : Pair<int, int>, IComparer<IntPair>
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
    #endregion

    #region Static Methods
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

    public static Vector3 toVector3(Vector2 v) { return new Vector3(v.x, v.y, 0.0f); }
    public static Vector3 copy(Vector3 v) { return new Vector3(v.x, v.y, v.z); }
    public static Vector2 copy(Vector2 v) { return new Vector2(v.x, v.y); }

    public static float RotateTowards(GameObject objToRotate, Vector3 target, float rotationSpeed)
    {
        // Normalize the target vector just in case.
        target.Normalize();
        
        // Compute the angle between the up transformation and the target.
        float angle = Mathf.Acos(Vector3.Dot(objToRotate.transform.up, target)) * Mathf.Rad2Deg;
        
        // If the angle is 180 degrees, then offset it by one, so the interpolation doesn't get confused.
        if(IsApproximately(Mathf.Abs(angle), 180f, 0.1f))
            objToRotate.transform.eulerAngles = new Vector3(0f, 0f, objToRotate.transform.eulerAngles.z + 1f);
        
        // Linear interpolate between the up transformation and the target vector.
        objToRotate.transform.up = Vector3.Lerp(objToRotate.transform.up, target, rotationSpeed * Time.smoothDeltaTime);
        
        // Sometimes the Euler angle in the y direction gets randomly changed to 180 degrees. This line resets it.
        objToRotate.transform.eulerAngles = new Vector3(0f, 0f, objToRotate.transform.eulerAngles.z);

        return angle;
    }

    public static bool IsApproximately(float a, float b, float tolerance = DEFAULT_TOLERANCE)
    {
        return Mathf.Abs(a - b) < tolerance;
    }

    public static float WrapAngle(float angle)
    {
        angle %= 360;
        return angle < 0.0f ? angle + 360 : angle;
    }

    #region Color Utilities
    public static bool areEqualColors(Color a, Color b){
		for (int i = 0; i < 3; i++) {
			if(a[i] != b[i]){
				return false;
			}
		}
		return true;
	}

	public static Color subtractColors(Color a, Color b){
		Color retCol = new Color(0,0,0,255);
		for (int i = 0; i < 3; i++) {
			retCol[i] = a[i] > b[i] ? a[i] - b[i] : b[i] - a[i];
			if(retCol[i] < 0){
				retCol[i] = 0;
			}
		}
		return retCol;
	}

	public static Color addColors(Color a, Color b){
		Color retCol = new Color (0,0,0,255);
		for (int i = 0; i < 3; i++) {
			retCol[i] = a[i] + b[i];
		}
		return retCol;
    }
    #endregion

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
    #endregion
}
