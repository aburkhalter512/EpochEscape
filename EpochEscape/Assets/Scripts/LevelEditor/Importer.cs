/*
 * Usage: Apply to the main camera, or an empty game object. Game objects will be constructed based on the data received.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MiniJSON;

public class Importer : MonoBehaviour
{
	public void Start()
	{
		string levelDataJSON = File.ReadAllText("level2.txt");

		// Deserialize the level data, and convert it to a dictionary of keys and values.
		Dictionary<string, object> levelData = Json.Deserialize(levelDataJSON) as Dictionary<string, object>;

		// Create a list of guards.
		List<object> guards = levelData["guards"] as List<object>;

		// Temporary variables for each guard.
		Dictionary<string, object> guard = null;
		string guardType = string.Empty;
		Dictionary<string, object> guardInitialPosition = null;
		Vector3 guardPosition = Vector3.zero;
		Dictionary<string, object> guardInitialDirection = null;
		Vector3 guardDirection = Vector3.zero;
		List<object> guardPatrolPoints = null;
		Dictionary<string, object> guardPatrolPoint = null;

		GameObject guardGameObject = null;
		Guard guardGameObjectScript = null;
		
		for(int i = 0; i < guards.Count; i++)
		{
			// Create a dictionary for the ith guard.
			guard = guards[i] as Dictionary<string, object>;

			// Store the guard type.
			guardType = guard["type"] as string;

			// Create a dictionary for the initial position.
			guardInitialPosition = guard["initialPosition"] as Dictionary<string, object>;

			// Assign the coordinates of the initial position.
			guardPosition.x = CastCoordinate(guardInitialPosition["x"]);
			guardPosition.y = CastCoordinate(guardInitialPosition["y"]);
			guardPosition.z = CastCoordinate(guardInitialPosition["z"]);

			// Create a dictionary for the initial direction.
			guardInitialDirection = guard["initialDirection"] as Dictionary<string, object>;

			// Assign the coordinates of the initial direction.
			guardDirection.x = CastCoordinate(guardInitialDirection["x"]);
			guardDirection.y = CastCoordinate(guardInitialDirection["y"]);
			guardDirection.z = CastCoordinate(guardInitialDirection["z"]);
			
			if(guardType == "Guard")
			{
				guardGameObject = Resources.Load("Prefabs/Enemies/Guard") as GameObject;
				guardGameObjectScript = guardGameObject.GetComponent<Guard>();

				// Create a list of patrol points from the level data.
				guardPatrolPoints = guard["patrolPoints"] as List<object>;

				// Initialize the array of patrol points for the current guard game object.
				guardGameObjectScript.m_patrolPoints = new Vector3[guardPatrolPoints.Count];

				// Iterate over each patrol point from the level data.
				for(int j = 0; j < guardPatrolPoints.Count; j++)
				{
					// Create a dictionary for the jth patrol point.
					guardPatrolPoint = guardPatrolPoints[j] as Dictionary<string, object>;

					// Assign the patrol point from the level data to the guard's patrol point array.
					guardGameObjectScript.m_patrolPoints[j].x = CastCoordinate(guardPatrolPoint["x"]);
					guardGameObjectScript.m_patrolPoints[j].y = CastCoordinate(guardPatrolPoint["y"]);
					guardGameObjectScript.m_patrolPoints[j].z = CastCoordinate(guardPatrolPoint["z"]);
				}
			}
			else if(guardType == "StationaryGuard")
				guardGameObject = Resources.Load("Prefabs/Enemies/StationaryGuard") as GameObject;

			if(guardGameObject != null)
			{
				guardGameObject.transform.position = guardPosition;
				guardGameObject.transform.up = guardDirection;
			}

			Instantiate(guardGameObject);
		}
	}

	// The MiniJSON library automatically converts integers to longs and floating point numbers to doubles.
	// This method converts the longs and doubles to floats.
	// Doing so is acceptable since no value will ever be greater than FLOAT_MAX.
	private float CastCoordinate(object toCast)
	{
		if(toCast.GetType().Name == "Int64")
			return (float) ( (long)(toCast) );

		if(toCast.GetType().Name == "Double")
			return (float) ( (double)(toCast) );

		return 0f;
	}
}
