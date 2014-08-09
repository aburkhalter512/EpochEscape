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
	
	/*
     * Initializes the Rotating Wall
     */
	protected new void Start()
	{
		base.Start();
		
        //Finds the rotation point
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
	
	//Put all update code here
	//Remember to comment!
	protected new void Update()
	{
		base.Update();

		// Needed to ensure that the real rotation point is updated if the object is moved within the editor.
		realRotationPt = transform.position + customRotationPoint;
	}
	
	#region Update Methods
	protected override void stationary()
	{
		//Empty Method
	}
	
	protected override void toChange()
	{
		if(rotationAngles.Length == 0)
			return;

		audio.Play ();
		currentIndex = (currentIndex + 1) % rotationAngles.Length;
		
		destinationAngle = rotationAngles[currentIndex] - transform.eulerAngles.z;
		
		if (destinationAngle > 180)
			destinationAngle = destinationAngle - 360;
		else if (destinationAngle < -180)
			destinationAngle = destinationAngle + 360;
		
		/*if (destinationAngle < -180)
			destinationAngle += 360;*/
		
		originalAngle = transform.eulerAngles.z;
		
		currentRotationChange = destinationAngle / changeTime;
		
		UpdateSize();
		
		//Debug.Log(destinationAngle);
		
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

			if(cameraBehavior != null && cameraBehavior.m_currentState == CameraBehavior.State.LERP_REST)
				cameraBehavior.m_currentState = CameraBehavior.State.LERP_TO_TARGET;
			// --- //*/
			
			for(int i = 0; i < transform.childCount; i++)
			{
				if(transform.GetChild(i).tag == "SecurityCamera")
				{
					SecurityCamera cam = transform.GetChild(i).GetComponent<SecurityCamera>();

					if(cam != null)
					{
						float sn = Mathf.Sin(destinationAngle * Mathf.Deg2Rad);
						float cs = Mathf.Cos(destinationAngle * Mathf.Deg2Rad);

						float px = cam.m_resetDirection.x * cs - cam.m_resetDirection.y * sn;
						float py = cam.m_resetDirection.x * sn + cam.m_resetDirection.y * cs;
						
						cam.m_resetDirection = new Vector3(px, py, cam.m_resetDirection.z);
						cam.m_resetAngle += destinationAngle;
					}
				}
			}
			
			return;
		}
		
		// ---
		// Cheap fix. Should be a state from within CameraBehavior.cs.
		Transform parent = transform.parent;
		
		if(parent != null && parent.tag != "WallPivot")
			Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
		// --- //*/
		
		float realRotationChanged = currentRotationChange * Time.smoothDeltaTime;
		
		if (Utilities.isBounded(
			rotationAngles[currentIndex] - realRotationChanged,
			rotationAngles[currentIndex] + realRotationChanged,
			transform.eulerAngles.z))
		{
			transform.RotateAround(realRotationPt, Vector3.forward, rotationAngles[currentIndex] - transform.eulerAngles.z);
			currentRotationChange = 0.0f;
		}
		else
			transform.RotateAround(realRotationPt, Vector3.forward, currentRotationChange * Time.smoothDeltaTime);
	}
	#endregion
}
