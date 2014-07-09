/*
 * This script has a few bugs, but it works. It may consume more memory than using XML since C# isn't optimized for JSON.
 * 
 * Bug: For some reason, the initial position isn't set correctly despite being read properly.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class Importer : MonoBehaviour
{
	void Start()
	{
		// JSON data for level 2.
		string levelDataJSON = "{\"guards\":[{\"guardType\":\"Guard\",\"initialLocation\":{\"x\":-1,\"y\":-2.2,\"z\":0},\"patrolPoints\":[{\"x\":-1,\"y\":-2.2,\"z\":0},{\"x\":-1,\"y\":1,\"z\":0}]},{\"guardType\":\"Guard\",\"initialLocation\":{\"x\":-2.6,\"y\":3.4,\"z\":0},\"patrolPoints\":[{\"x\":-2.6,\"y\":-3.4,\"z\":0},{\"x\":-2.6,\"y\":3.4,\"z\":0}]},{\"guardType\":\"Guard\",\"initialLocation\":{\"x\":-2,\"y\":3.9,\"z\":0},\"patrolPoints\":[{\"x\":-2,\"y\":3.9,\"z\":0},{\"x\":2.1,\"y\":3.9,\"z\":0}]},{\"guardType\":\"StationaryGuard\",\"initialLocation\":{\"x\":0.6,\"y\":0.5,\"z\":0}}]}";

		// Deserialize the level data, and assign it to a dictionary.
		Dictionary<string, object> levelData = Json.Deserialize(levelDataJSON) as Dictionary<string, object>;

		// Create a list of guards.
		List<object> guards = levelData["guards"] as List<object>;

		// Temporary variables for each guard.
		Dictionary<string, object> guard = null;
		string guardType = null;
		Dictionary<string, object> guardInitialLocation = null;
		Vector3 guardLocation = Vector3.zero;
		List<object> guardPatrolPoints = null;
		Dictionary<string, object> guardPatrolPoint = null;

		GameObject guardGameObject = null;
		Guard guardGameObjectScript = null;

		// Iterate over each guard.
		for(int i = 0; i < guards.Count; i++)
		{
			// Create a dictionary for the ith guard.
			guard = guards[i] as Dictionary<string, object>;

			guardType = guard["guardType"] as string;

			// Create a dictionary for the initial location.
			guardInitialLocation = guard["initialLocation"] as Dictionary<string, object>;

			// Assign the ith guard's initial location coordinates from the respective keys.
			guardLocation.x = CastCoordinate(guardInitialLocation["x"]);
			guardLocation.y = CastCoordinate(guardInitialLocation["y"]);
			guardLocation.z = CastCoordinate(guardInitialLocation["z"]);

			if(guardType == "Guard")
			{
				guardGameObject = Resources.Load("Prefabs/Enemies/Guard") as GameObject;
				guardGameObjectScript = guardGameObject.GetComponent<Guard>();

				// Create a list of patrol points from the patrolPoints key.
				guardPatrolPoints = guard["patrolPoints"] as List<object>;

				// Create the array to store the patrol points.
				guardGameObjectScript.m_patrolPoints = new Vector3[guardPatrolPoints.Count];

				// Iterate over each patrol point in the object list.
				for(int j = 0; j < guardPatrolPoints.Count; j++)
				{
					// Create a dictionary for the jth patrol point.
					guardPatrolPoint = guardPatrolPoints[j] as Dictionary<string, object>;

					// Assign the patrol point from the object list to the point array.
					guardGameObjectScript.m_patrolPoints[j].x = CastCoordinate(guardPatrolPoint["x"]);
					guardGameObjectScript.m_patrolPoints[j].y = CastCoordinate(guardPatrolPoint["y"]);
					guardGameObjectScript.m_patrolPoints[j].z = CastCoordinate(guardPatrolPoint["z"]);
				}
			}
			else if(guardType == "StationaryGuard")
				guardGameObject = Resources.Load("Prefabs/Enemies/StationaryGuard") as GameObject;

			if(guardGameObject != null)
				guardGameObject.transform.position = guardLocation;

			Instantiate(guardGameObject);
		}
	}

	// Casting longs and doubles to floats is acceptable since no value will ever be greater than FLOAT_MAX.
	private float CastCoordinate(object toCast)
	{
		if(toCast.GetType().Name == "Int64")
			return (float) ( (long)(toCast) );

		if(toCast.GetType().Name == "Double")
			return (float) ( (double)(toCast) );

		return 0f;
	}
}
