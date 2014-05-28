﻿using UnityEngine;
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
    private float originalAngle = 0.0f;
    private float destinationAngle = 0.0f;

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

        destinationAngle = rotationAngles[currentIndex] + baseAngle - transform.eulerAngles.z;

        if (destinationAngle < -180)
            destinationAngle += 360;

        originalAngle = transform.eulerAngles.z;

        /*relativeAngle = rotationAngles[currentIndex];
        originalAngle = transform.eulerAngles.z;

        if (relativeAngle < 0)
            relativeAngle = 360 + relativeAngle;

        relativeAngle -= transform.rotation.eulerAngles.z - baseAngle;

        if (relativeAngle > 180)
            relativeAngle = -(360 - relativeAngle);
        else if (relativeAngle < -180 && relativeAngle > -360)
        {
            relativeAngle = 360 + relativeAngle;
        }*/

        currentRotationChange = destinationAngle / changeTime;

        UpdateSize();

        Debug.Log(destinationAngle);

        currentState = STATES.CHANGE;
    }

    protected override void change()
    {
        if (Mathf.Approximately(currentRotationChange, 0.0f))
        {
            currentState = STATES.STATIONARY;

			// ---
			// This block was originally inside the stationary() method, but for some reason it wouldn't work.
			CameraBehavior cameraBehavior = Camera.main.GetComponent<CameraBehavior>();
			
			if(cameraBehavior.m_currentState == CameraBehavior.State.LERP_REST)
				cameraBehavior.m_currentState = CameraBehavior.State.LERP_TO_TARGET;
			// --- //*/

            return;
        }

		// ---
		// Cheap fix. Should be a state from within CameraBehavior.cs.
		Transform parent = transform.parent;

		if(parent != null && parent.tag != "WallPivot")
			Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
		// --- //*/

        Debug.Log(transform.eulerAngles.z);

        float realRotationChanged = currentRotationChange * Time.smoothDeltaTime;
        float realDestination = destinationAngle > 0 ? destinationAngle : 360 + destinationAngle;
        realDestination += originalAngle;

        Debug.Log(realDestination);

        if (Utilities.isBounded(
            realDestination - realRotationChanged,
            realDestination + realRotationChanged,
            transform.eulerAngles.z))
        {
           transform.RotateAround(realRotationPt, Vector3.forward, realDestination - transform.eulerAngles.z);
            currentRotationChange = 0.0f;
        }
        else
            transform.RotateAround(realRotationPt, Vector3.forward, currentRotationChange * Time.smoothDeltaTime);


        /*float realAngle = rotationAngles[currentIndex];

        if (realAngle < 0)
            realAngle = 360 + realAngle;

        realAngle -= transform.rotation.eulerAngles.z - baseAngle;

        if (realAngle > 180)
            realAngle = -(360 - realAngle);
        else if (realAngle < -180 && realAngle > -360)
        {
            realAngle = 360 + realAngle;
        }*/


        /*if (Utilities.isBounded(0.0f, relativeAngle, transform.eulerAngles.z - originalAngle))
        {
            Debug.Log(relativeAngle);
            transform.RotateAround(realRotationPt, Vector3.forward, currentRotationChange * Time.smoothDeltaTime);
        }
        else
        {
            float realAngle = (relativeAngle < 0 ? 360 + relativeAngle : relativeAngle) + baseAngle;
            float toAngle = realAngle - transform.eulerAngles.z;

            Debug.Log(toAngle);

            transform.RotateAround(realRotationPt, Vector3.forward, toAngle);
            currentRotationChange = 0.0f;
        }*/

        /*if (Utilities.isBounded(realAngle - currentRotationChange * Time.smoothDeltaTime,
                      realAngle + currentRotationChange * Time.smoothDeltaTime,
                      transform.rotation.eulerAngles.z - baseAngle))
        {
            transform.RotateAround(realRotationPt, Vector3.forward, transform.eulerAngles.z - realAngle);
            //transform.eulerAngles = new Vector3(0.0f, 0.0f, baseAngle + rotationAngles[currentIndex]);
            /*transform.RotateAround(realRotationPt, Vector3.forward,
                (rotationAngles[currentIndex] - transform.rotation.eulerAngles.z - baseAngle) % 360);*/
            /*currentRotationChange = 0.0f;
        }
        else if (!Mathf.Approximately(currentRotationChange, 0.0f))
        {
            transform.RotateAround(realRotationPt, Vector3.forward, currentRotationChange * Time.smoothDeltaTime);
        }*/

    }
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
