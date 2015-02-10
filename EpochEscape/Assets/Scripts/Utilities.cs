using UnityEngine;
using System;
using System.Text;
using System.Collections;
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
    #endregion
}
