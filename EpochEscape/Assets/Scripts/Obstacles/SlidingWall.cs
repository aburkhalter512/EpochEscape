using UnityEngine;
using System.Collections;

public class SlidingWall : DynamicWall
{
    #region Inspector Variables
    public Vector3[] positionPts;
    #endregion

    #region Instance Variables
    private Vector3 positionDelta = Vector3.zero;
    private Vector3 basePosition;
    #endregion

    #region Class Constants
    #endregion

	//Put all initialization code here
	//Remember to comment!
	protected new void Start()
	{
        base.Start();

        basePosition = Vector3.zero;
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
       //Emtpy Method
    }

    protected override void toChange()
    {
        currentIndex = (currentIndex + 1) % positionPts.Length;

        positionDelta = positionPts[currentIndex] + basePosition - transform.position;
        positionDelta /= changeTime;

        UpdateSize();

        currentState = STATES.CHANGE;
    }

    protected override void change()
    {
        if (Utilities.areClose(positionDelta, Vector3.zero))
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

        if (Utilities.isBounded(0.0f, positionDelta.sqrMagnitude * Time.smoothDeltaTime * Time.smoothDeltaTime,
                      (transform.position - (positionPts[currentIndex]) + basePosition).sqrMagnitude))
        {
            transform.position = positionPts[currentIndex] + basePosition;
            positionDelta = Vector3.zero;
        }
        else if (!Utilities.areClose(positionDelta, Vector3.zero))
        {
            transform.position += positionDelta * Time.smoothDeltaTime;
        }

    }
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
