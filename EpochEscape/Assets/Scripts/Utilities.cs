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
    #endregion
}
