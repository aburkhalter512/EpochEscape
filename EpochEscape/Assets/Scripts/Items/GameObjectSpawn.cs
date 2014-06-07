using UnityEngine;
using System.Collections;

public class GameObjectSpawner : MonoBehaviour
{
	#region Inspector Variables
    public GameObject item = null;
	#endregion

	#region Instance Variables
	#endregion

	#region Class Constants
	#endregion

	//Put all initialization code here
	//Remember to comment!
	protected void Start()
	{
        if (item != null)
        {
            GameObject toPlace = GameObject.Instantiate(item) as GameObject;
            toPlace.transform.position = transform.position;
        }

        Destroy(gameObject);
	}

	#region Initialization Methods
	#endregion

	//Put all update code here
	//Remember to comment!
	protected void Update()
	{
	}

	#region Update Methods
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
