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
            foreach (GameObject actuator in actuators)
            {
                if (actuator != null)
				{
					MapBehavior mapBehavior = GameObject.FindWithTag("Map").GetComponent<MapBehavior>();

					Transform parent = actuator.transform.parent;

					if(parent != null && parent.tag == "WallPivot")
					{
						if(mapBehavior.m_lerpTargets.Count == 0 || mapBehavior.m_lerpTargets.Peek() != parent.gameObject)
							mapBehavior.m_lerpTargets.Push(parent.gameObject);
					}
					else
					{
						mapBehavior.m_lerpTargets.Push(actuator);
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
