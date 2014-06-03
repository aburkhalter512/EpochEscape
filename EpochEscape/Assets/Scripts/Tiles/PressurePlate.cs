using UnityEngine;
using System.Collections;

public class PressurePlate : Tile
{
	#region Inspector Variables
    public GameObject[] actuators;
	#endregion

	#region Instance Variables
	#endregion

	#region Class Constants
	#endregion

	//Put all initialization code here
	//Remember to comment!
	protected void Start()
	{
	}

	#region Initialization Methods
	#endregion

	//Put all update code here
	//Remember to comment!
	protected void Update()
	{
	}

    #region Update Methods
    protected void OnTriggerEnter2D(Collider2D collidee)
    {
        if (collidee.tag == "Player")
        {
			audio.Play ();
            foreach (GameObject actuator in actuators)
            {
                if (actuator != null)
				{
					CameraBehavior cameraBehavior = Camera.main.GetComponent<CameraBehavior>();

					Transform parent = actuator.transform.parent;

					if(parent != null && parent.tag == "WallPivot")
					{
						if(cameraBehavior.m_lerpTargets.Count == 0 || cameraBehavior.m_lerpTargets.Peek() != parent.gameObject)
							cameraBehavior.m_lerpTargets.Push(parent.gameObject);
					}
					else
					{
						cameraBehavior.m_lerpTargets.Push(actuator);
					}

					//actuator.GetComponent<DynamicWall>().currentState = DynamicWall.STATES.TO_CHANGE;
				}
            }
        }
    }
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
