﻿using UnityEngine;
using System.Collections;

public class ExitDoor : MonoBehaviour
{
	#region Inspector Variables
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
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        if (player != null)
            if (player.isPowerCoreComplete())
                DestroyDoor();
	}

	#region Update Methods
    public void DestroyDoor()
    {
        Destroy(gameObject);
    }
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
