using UnityEngine;
using System.Collections;

public class Utilities
{
	#region Instance Variables
	#endregion

	#region Class Constants
	public const float DEFAULT_TOLERANCE = 0.05f;
	#endregion

	#region Static Methods
    public static Vector3 toVector3(Vector2 v) { return new Vector3(v.x, v.y, 0.0f); }
    public static Vector3 toVector3(Vector2 v, float z) { return new Vector3(v.x, v.y, z); }
    public static Vector3 rotate(Vector3 v, float degrees)
    {
        return new Vector3(
            v.x * Mathf.Cos(degrees) - v.y * Mathf.Sin(degrees),
            v.x * Mathf.Sin(degrees) + v.y * Mathf.Cos(degrees),
            v.z);
    }
    public static Vector3 negate(Vector3 v) { return v * -1; }
    public static Vector3 copy(Vector3 v) { return new Vector3(v.x, v.y, v.z); }
    public static Vector2 copy(Vector2 v) { return new Vector2(v.x, v.y); }
    public static bool areClose(Vector3 v1, Vector3 v2)
    {
        return Mathf.Approximately(v1.x, v2.x) && Mathf.Approximately(v1.y, v2.y) && Mathf.Approximately(v1.z, v2.z);
    }
    public static bool isBounded(float b1, float b2, float value)
    {
        if (b2 < b1)
        {
            float swap = b2;
            b2 = b1;
            b1 = swap;
        }

        return b1 <= value && value <= b2;
    }

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
	#endregion
}
