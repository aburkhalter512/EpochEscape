using UnityEngine;
using System.Collections;

public class RotatingWall : DynamicWall
{
    #region Inspector Variables
    public ROTATION_POINTS rotationPt;
    public float[] rotationAngles;

    public Vector3 customRotationPoint;
	#endregion

    #region Instance Variables
    private Vector3 realRotationPt = Vector3.zero;
    private float baseAngle = 0.0f;

    private float currentRotationChange = 0.0f;
	#endregion

	#region Class Constants
    public enum ROTATION_POINTS
    {
        CENTER = 0,
        TOP_RIGHT,
        BOTTOM_RIGHT,
        BOTTOM_LEFT,
        TOP_LEFT,
        CUSTOM
    };
	#endregion

	//Put all initialization code here
	//Remember to comment!
	protected new void Start()
	{
        base.Start();

        switch (rotationPt)
        {
            case ROTATION_POINTS.CENTER:
                break;
            case ROTATION_POINTS.TOP_RIGHT:
                realRotationPt.x = size.x;
                realRotationPt.y = size.y;
                break;
            case ROTATION_POINTS.BOTTOM_RIGHT:
                realRotationPt.x = size.x;
                realRotationPt.y = -size.y;
                break;
            case ROTATION_POINTS.BOTTOM_LEFT:
                realRotationPt.x = -size.x;
                realRotationPt.y = -size.y;
                break;
            case ROTATION_POINTS.TOP_LEFT:
                realRotationPt.x = -size.x;
                realRotationPt.y = size.y;
                break;
            case ROTATION_POINTS.CUSTOM:
                realRotationPt = customRotationPoint;
                break;
        }

        realRotationPt += transform.position;

        baseAngle = transform.rotation.eulerAngles.z;

        for (int i = 0; i < rotationAngles.Length; i++)
            rotationAngles[i] %= 360;
	}

	#region Initialization Methods
	#endregion

	//Put all update code here
	//Remember to comment!
	protected new void Update()
	{
        base.Update();
	}

    #region Update Methods
    protected override void stationary()
    {
        //Empty Method
    }

    protected override void toChange()
    {
        currentIndex = (currentIndex + 1) % rotationAngles.Length;

        float rotationSpeed = 0.0f;
        float realAngle = rotationAngles[currentIndex];

        if (realAngle < 0)
            realAngle = 360 + realAngle;

        realAngle -= transform.rotation.eulerAngles.z - baseAngle;

        if (realAngle > 180)
            realAngle = -(360 - realAngle);
        else if (realAngle < -180 && realAngle > -360)
        {
            realAngle = 360 + realAngle;
        }

        rotationSpeed = realAngle / changeTime;

        currentRotationChange = rotationSpeed;

        UpdateSize();

        currentState = STATES.CHANGE;
    }

    protected override void change()
    {
        if (Mathf.Approximately(currentRotationChange, 0.0f))
        {
            currentState = STATES.STATIONARY;
            return;
        }

        float realAngle = rotationAngles[currentIndex];

        if (realAngle < 0)
            realAngle = 360 + realAngle;

        if (Utilities.isBounded(realAngle - currentRotationChange * Time.smoothDeltaTime,
                      realAngle + currentRotationChange * Time.smoothDeltaTime,
                      transform.rotation.eulerAngles.z - baseAngle))
        {
            transform.eulerAngles = new Vector3(0.0f, 0.0f, baseAngle + rotationAngles[currentIndex]);
            /*transform.RotateAround(realRotationPt, Vector3.forward,
                (rotationAngles[currentIndex] - transform.rotation.eulerAngles.z - baseAngle) % 360);*/
            currentRotationChange = 0.0f;
        }
        else if (!Mathf.Approximately(currentRotationChange, 0.0f))
        {
            transform.RotateAround(realRotationPt, Vector3.forward, currentRotationChange * Time.smoothDeltaTime);
        }

    }
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
