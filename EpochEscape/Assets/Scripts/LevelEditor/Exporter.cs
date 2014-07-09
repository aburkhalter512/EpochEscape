/*
 * This script works, but it may consume more memory than using XML since C# isn't optimized for JSON.
 * 
 * Usage: Apply the script to the main camera, or an empty game object. The script will output the level data to the console.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class Exporter : MonoBehaviour
{
	void Start()
	{
		GameObject guards = GameObject.Find("Guards");

		if(guards == null) return;

		int guardCount = guards.transform.childCount;

		if(guardCount == 0) return;

		// Create the dictionary for the level data.
		Dictionary<string, object> levelData = new Dictionary<string, object>();

		// Create the array of dictionaries for each guard, and assign it to the key "guards".
		levelData["guards"] = new Dictionary<string, object>[guardCount];

		Transform guardTemp = null;
		Vector3 initialPosition = Vector3.zero;
		string guardType = null;
		Guard guardScript = null;
		Vector3[] patrolPoints = null;

		for(int i = 0; i < guardCount; i++)
		{
			guardTemp = guards.transform.GetChild(i);
			guardType = guardTemp.name;
			initialPosition = guardTemp.position;
			guardScript = guardTemp.GetComponent<Guard>();

			// Create the dictionary for the ith guard.
			( (Dictionary<string, object>[]) (levelData["guards"]) )[i] = new Dictionary<string, object>();

			// Assign the type of the ith guard in the dictionary to the key "guardType".
			( (Dictionary<string, object>[]) (levelData["guards"]) )[i]["guardType"] = guardType;

			// Create the dictionary for the ith guard's initial location coordinates, and assign it to the key "initialLocation".
			( (Dictionary<string, object>[]) (levelData["guards"]) )[i]["initialLocation"] = new Dictionary<string, object>();

			// Store the x, y, and z coordinate of the ith guard's initial position to the respective keys.
			( (Dictionary<string, object>) ( ( (Dictionary<string, object>[]) (levelData["guards"]) ) [i]["initialLocation"] ) )["x"] = initialPosition.x;
			( (Dictionary<string, object>) ( ( (Dictionary<string, object>[]) (levelData["guards"]) ) [i]["initialLocation"] ) )["y"] = initialPosition.y;
			( (Dictionary<string, object>) ( ( (Dictionary<string, object>[]) (levelData["guards"]) ) [i]["initialLocation"] ) )["z"] = initialPosition.z;

			if(guardScript != null && guardType != "StationaryGuard")
			{
				patrolPoints = guardScript.m_patrolPoints;

				// Create the array of dictionaries for the ith guard's patrol points, and assign it to the key "patrolPoints".
				( (Dictionary<string, object>[]) (levelData["guards"]) ) [i]["patrolPoints"] = new Dictionary<string, object>[patrolPoints.Length];

				for(int j = 0; j < patrolPoints.Length; j++)
				{
					// Create the dictionary for the ith guard's jth patrol point.
					( (Dictionary<string, object>[]) ( ( (Dictionary<string, object>[]) (levelData["guards"]) ) [i]["patrolPoints"] ) ) [j] = new Dictionary<string, object>();

					// Store the x, y, and z coordinate of the ith guard's jth patrol point to the respective keys.
					( (Dictionary<string, object>) ( ( (Dictionary<string, object>[]) ( ( (Dictionary<string, object>[]) (levelData["guards"]) ) [i]["patrolPoints"] ) ) [j] ) ) ["x"] = patrolPoints[j].x;
					( (Dictionary<string, object>) ( ( (Dictionary<string, object>[]) ( ( (Dictionary<string, object>[]) (levelData["guards"]) ) [i]["patrolPoints"] ) ) [j] ) ) ["y"] = patrolPoints[j].y;
					( (Dictionary<string, object>) ( ( (Dictionary<string, object>[]) ( ( (Dictionary<string, object>[]) (levelData["guards"]) ) [i]["patrolPoints"] ) ) [j] ) ) ["z"] = patrolPoints[j].z;
				}
			}
		}

		// Serialize the data to create a JSON string.
		string json = Json.Serialize(levelData);

		Debug.Log(json);
	}
}
