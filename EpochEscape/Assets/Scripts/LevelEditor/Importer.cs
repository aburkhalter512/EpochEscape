/*
 * Usage: Apply to the main camera, or an empty game object. Game objects will be constructed based on the data file.
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
		if(File.Exists("level1.json"))
		{
			string levelDataJSON = File.ReadAllText("level1.json");

			// Deserialize the level data, and convert it to a dictionary of keys and values.
			Dictionary<string, object> levelData = Json.Deserialize(levelDataJSON) as Dictionary<string, object>;

			ConstructItems(levelData);
			ConstructObstacles(levelData);
			ConstructHidingSpots(levelData);
			ConstructWalls(levelData);
			ConstructDoors(levelData);
			ConstructFloors(levelData);
		}
	}

	private void ConstructItems(Dictionary<string, object> levelData)
	{
		List<object> itemData = levelData["items"] as List<object>;

		if(itemData != null && itemData.Count > 0)
		{
			GameObject items = GameObject.Find("Items");

			if(items == null)
			{
				items = new GameObject();
				items = Instantiate(items) as GameObject;
				items.name = "Items";
			}

			Dictionary<string, object> itemDict = null;
			string itemName = string.Empty;
			Dictionary<string, object> itemPositionDict = null;
			Vector3 itemPosition = Vector3.zero;
			Dictionary<string, object> itemDirectionDict = null;
			Vector3 itemDirection = Vector3.zero;

			GameObject item = null;

			for(int i = 0; i < itemData.Count; i++)
			{
				itemDict = itemData[i] as Dictionary<string, object>;
				itemName = itemDict["name"] as string;
				itemPositionDict = itemDict["position"] as Dictionary<string, object>;
				itemDirectionDict = itemDict["direction"] as Dictionary<string, object>;

				itemPosition.x = CastCoordinate(itemPositionDict["x"]);
				itemPosition.y = CastCoordinate(itemPositionDict["y"]);
				itemPosition.z = CastCoordinate(itemPositionDict["z"]);

				itemDirection.x = CastCoordinate(itemDirectionDict["x"]);
				itemDirection.y = CastCoordinate(itemDirectionDict["y"]);
				itemDirection.z = CastCoordinate(itemDirectionDict["z"]);

				item = Resources.Load("Prefabs/Items/" + itemName) as GameObject;

				if(item != null)
				{
					item = Instantiate(item) as GameObject;

					if(item != null)
					{
						item.name = itemName;
						item.transform.position = itemPosition;
						item.transform.up = itemDirection;
						item.transform.parent = items.transform;
					}
				}
			}
		}
	}

	private void ConstructObstacles(Dictionary<string, object> levelData)
	{
		List<object> obstacleData = levelData["obstacles"] as List<object>;

		if(obstacleData != null && obstacleData.Count > 0)
		{
			GameObject obstacles = GameObject.Find("Obstacles");

			if(obstacles == null)
			{
				obstacles = new GameObject();
				obstacles = Instantiate(obstacles) as GameObject;
				obstacles.name = "Obstacles";
			}

			Dictionary<string, object> obstacleDict = null;
			string obstacleName = string.Empty;
			Dictionary<string, object> obstaclePositionDict = null;
			Vector3 obstaclePosition = Vector3.zero;
			Dictionary<string, object> obstacleDirectionDict = null;
			Vector3 obstacleDirection = Vector3.zero;
			
			GameObject obstacle = null;
			
			for(int i = 0; i < obstacleData.Count; i++)
			{
				obstacleDict = obstacleData[i] as Dictionary<string, object>;
				obstacleName = obstacleDict["name"] as string;
				obstaclePositionDict = obstacleDict["position"] as Dictionary<string, object>;
				obstacleDirectionDict = obstacleDict["direction"] as Dictionary<string, object>;
				
				obstaclePosition.x = CastCoordinate(obstaclePositionDict["x"]);
				obstaclePosition.y = CastCoordinate(obstaclePositionDict["y"]);
				obstaclePosition.z = CastCoordinate(obstaclePositionDict["z"]);
				
				obstacleDirection.x = CastCoordinate(obstacleDirectionDict["x"]);
				obstacleDirection.y = CastCoordinate(obstacleDirectionDict["y"]);
				obstacleDirection.z = CastCoordinate(obstacleDirectionDict["z"]);
				
				obstacle = Resources.Load("Prefabs/Obstacles/" + obstacleName) as GameObject;

				if(obstacle != null)
				{
					obstacle = Instantiate(obstacle) as GameObject;
					
					if(obstacle != null)
					{
						obstacle.name = obstacleName;
						obstacle.transform.position = obstaclePosition;
						obstacle.transform.up = obstacleDirection;
						obstacle.transform.parent = obstacles.transform;
					}
				}
			}
		}
	}

	private void ConstructHidingSpots(Dictionary<string, object> levelData)
	{
		List<object> hidingSpotData = levelData["hidingSpots"] as List<object>;
		
		if(hidingSpotData != null && hidingSpotData.Count > 0)
		{
			GameObject obstacles = GameObject.Find("Obstacles");
			GameObject hidingSpots = GameObject.Find("hidingSpots");
			
			if(hidingSpots == null)
			{
				hidingSpots = new GameObject();
				hidingSpots = Instantiate(hidingSpots) as GameObject;
				hidingSpots.name = "HidingSpots";

				if(obstacles != null)
					hidingSpots.transform.parent = obstacles.transform;
			}
			
			Dictionary<string, object> hidingSpotDict = null;
			string hidingSpotName = string.Empty;
			Dictionary<string, object> hidingSpotPositionDict = null;
			Vector3 hidingSpotPosition = Vector3.zero;
			Dictionary<string, object> hidingSpotDirectionDict = null;
			Vector3 hidingSpotDirection = Vector3.zero;
			
			GameObject hidingSpot = null;
			
			for(int i = 0; i < hidingSpotData.Count; i++)
			{
				hidingSpotDict = hidingSpotData[i] as Dictionary<string, object>;
				hidingSpotName = hidingSpotDict["name"] as string;
				hidingSpotPositionDict = hidingSpotDict["position"] as Dictionary<string, object>;
				hidingSpotDirectionDict = hidingSpotDict["direction"] as Dictionary<string, object>;
				
				hidingSpotPosition.x = CastCoordinate(hidingSpotPositionDict["x"]);
				hidingSpotPosition.y = CastCoordinate(hidingSpotPositionDict["y"]);
				hidingSpotPosition.z = CastCoordinate(hidingSpotPositionDict["z"]);
				
				hidingSpotDirection.x = CastCoordinate(hidingSpotDirectionDict["x"]);
				hidingSpotDirection.y = CastCoordinate(hidingSpotDirectionDict["y"]);
				hidingSpotDirection.z = CastCoordinate(hidingSpotDirectionDict["z"]);
				
				hidingSpot = Resources.Load("Prefabs/Obstacles/" + hidingSpotName) as GameObject;
				
				if(hidingSpot != null)
				{
					hidingSpot = Instantiate(hidingSpot) as GameObject;
					
					if(hidingSpot != null)
					{
						hidingSpot.name = hidingSpotName;
						hidingSpot.transform.position = hidingSpotPosition;
						hidingSpot.transform.up = hidingSpotDirection;
						hidingSpot.transform.parent = hidingSpots.transform;
					}
				}
			}
		}
	}

	private void ConstructWalls(Dictionary<string, object> levelData)
	{
		List<object> wallData = levelData["walls"] as List<object>;

		if(wallData != null && wallData.Count > 0)
		{
			GameObject walls = GameObject.Find("Walls");

			if(walls == null)
			{
				walls = new GameObject();
				walls = Instantiate(walls) as GameObject;
				walls.name = "Walls";
			}

			Dictionary<string, object> wallDict = null;
			Dictionary<string, object> wallSizeDict = null;
			Vector2 wallSize = Vector2.zero;
			Dictionary<string, object> wallCenterDict = null;
			Vector2 wallCenter = Vector2.zero;

			BoxCollider2D wallColliderTemp = null;

			for(int i = 0; i < wallData.Count; i++)
			{
				wallDict = wallData[i] as Dictionary<string, object>;
				wallColliderTemp = walls.AddComponent<BoxCollider2D>();

				wallSizeDict = wallDict["size"] as Dictionary<string, object>;
				wallCenterDict = wallDict["center"] as Dictionary<string, object>;

				wallSize.x = CastCoordinate(wallSizeDict["x"]);
				wallSize.y = CastCoordinate(wallSizeDict["y"]);

				wallCenter.x = CastCoordinate(wallCenterDict["x"]);
				wallCenter.y = CastCoordinate(wallCenterDict["y"]);

				wallColliderTemp.size = wallSize;
				wallColliderTemp.center = wallCenter;
			}
		}
	}

	private void ConstructDoors(Dictionary<string, object> levelData)
	{
		List<object> doorData = levelData["doors"] as List<object>;

		if(doorData != null && doorData.Count > 0)
		{
			GameObject doors = GameObject.Find("doors");

			if(doors == null)
			{
				doors = new GameObject();
				doors = Instantiate(doors) as GameObject;
				doors.name = "Doors";
			}

			Dictionary<string, object> doorDict = null;
			string doorName = string.Empty;
			Dictionary<string, object> doorPositionDict = null;
			Vector3 doorPosition = Vector3.zero;
			Dictionary<string, object> doorDirectionDict = null;
			Vector3 doorDirection = Vector3.zero;

			GameObject door = null;

			for(int i = 0; i < doorData.Count; i++)
			{
				doorDict = doorData[i] as Dictionary<string, object>;
				doorName = doorDict["name"] as string;
				doorPositionDict = doorDict["position"] as Dictionary<string, object>;
				doorDirectionDict = doorDict["direction"] as Dictionary<string, object>;

				doorPosition.x = CastCoordinate(doorPositionDict["x"]);
				doorPosition.y = CastCoordinate(doorPositionDict["y"]);
				doorPosition.z = CastCoordinate(doorPositionDict["z"]);

				doorDirection.x = CastCoordinate(doorDirectionDict["x"]);
				doorDirection.y = CastCoordinate(doorDirectionDict["y"]);
				doorDirection.z = CastCoordinate(doorDirectionDict["z"]);

				door = Resources.Load("Prefabs/Obstacles/" + doorName) as GameObject;

				if(door != null)
				{
					door = Instantiate(door) as GameObject;

					if(door != null)
					{
						door.name = doorName;
						door.transform.position = doorPosition;
						door.transform.up = doorDirection;
						door.transform.parent = doors.transform;
					}
				}
			}
		}
	}

	private void ConstructFloors(Dictionary<string, object> levelData)
	{
		List<object> floorData = levelData["floors"] as List<object>;

		if(floorData != null && floorData.Count > 0)
		{
			GameObject floors = GameObject.Find("Floors");

			if(floors == null)
			{
				floors = new GameObject();
				floors = Instantiate(floors) as GameObject;
				floors.name = "FloorColliders";
			}

			Dictionary<string, object> floorDict = null;
			string floorName = string.Empty;
			Dictionary<string, object> floorPositionDict = null;
			Vector3 floorPosition = Vector3.zero;
			Dictionary<string, object> floorScaleDict = null;
			Vector3 floorScale = Vector3.zero;
			Dictionary<string, object> floorColliderData = null;
			Dictionary<string, object> floorColliderSizeDict = null;
			Vector2 floorColliderSize = Vector2.zero;
			Dictionary<string, object> floorColliderCenterDict = null;
			Vector2 floorColliderCenter = Vector2.zero;

			GameObject floor = null;
			BoxCollider2D floorCollider = null;

			for(int i = 0; i < floorData.Count; i++)
			{
				floorDict = floorData[i] as Dictionary<string, object>;
				floorName = floorDict["name"] as string;
				floorPositionDict = floorDict["position"] as Dictionary<string, object>;
				floorScaleDict = floorDict["scale"] as Dictionary<string, object>;

				floorColliderData = floorDict["collider"] as Dictionary<string, object>;
				floorColliderSizeDict = floorColliderData["size"] as Dictionary<string, object>;
				floorColliderCenterDict = floorColliderData["center"] as Dictionary<string, object>;

				floorPosition.x = CastCoordinate(floorPositionDict["x"]);
				floorPosition.y = CastCoordinate(floorPositionDict["y"]);
				floorPosition.z = CastCoordinate(floorPositionDict["z"]);

				floorScale.x = CastCoordinate(floorScaleDict["x"]);
				floorScale.y = CastCoordinate(floorScaleDict["y"]);
				floorScale.z = CastCoordinate(floorScaleDict["z"]);

				floorColliderSize.x = CastCoordinate(floorColliderSizeDict["x"]);
				floorColliderSize.y = CastCoordinate(floorColliderSizeDict["y"]);

				floorColliderCenter.x = CastCoordinate(floorColliderCenterDict["x"]);
				floorColliderCenter.y = CastCoordinate(floorColliderCenterDict["y"]);

				floor = Resources.Load("Prefabs/Floors/" + floorName) as GameObject;

				if(floor != null)
				{
					floor = Instantiate(floor) as GameObject;

					if(floor != null)
					{
						floor.name = floorName;
						floor.transform.position = floorPosition;
						floor.transform.localScale = floorScale;
						floor.transform.parent = floors.transform;

						floorCollider = floor.GetComponent<BoxCollider2D>();

						if(floorCollider != null)
						{
							floorCollider.size = floorColliderSize;
							floorCollider.center = floorColliderCenter;
						}
					}
				}
			}
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
