/*
 * Usage: Apply the script to the main camera, or an empty game object. The script will output the level data a file.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Exporter : MonoBehaviour
{
	public void Start()
	{
		using(StreamWriter sw = new StreamWriter("level2.txt"))
		{
			sw.WriteLine("{");

			ComposePlayer(sw);
			ComposeItems(sw);
			ComposeGuards(sw);
			ComposeObstacles(sw);
			ComposeWalls(sw);
			ComposeDoors(sw);
			ComposeFloors(sw);
			
			sw.WriteLine("}");
		}
	}

	private void ComposePlayer(StreamWriter sw)
	{
		GameObject spawnLocation = GameObject.FindWithTag("SpawnLocation");

		sw.WriteLine("\t" + escape("player") + ":[");
		sw.WriteLine(tab(2) + "{");
		sw.WriteLine(tab(3) + escape("spawnLocation") + ":{");
		
		if(spawnLocation != null)
			printVector(sw, spawnLocation.transform.position, 4);
		else
			printVector(sw, Vector3.zero, 4);
		
		sw.WriteLine(tab(3) + "}");
		sw.WriteLine(tab(2) + "}");
		sw.WriteLine("\t],");
	}

	private void ComposeItems(StreamWriter sw)
	{
		GameObject items = GameObject.Find("Items");
		int itemCount = (items == null ? 0 : items.transform.childCount);
		
		sw.WriteLine("\t" + escape("items") + ":[");

		if(!(items == null || itemCount == 0))
		{
			Transform itemTemp = null;

			for(int i = 0; i < itemCount; i++)
			{
				itemTemp = items.transform.GetChild(i);

				sw.WriteLine(tab(2) + "{");
				sw.WriteLine(tab(3) + escape("name") + ":" + escape(itemTemp.name) + ",");
				sw.WriteLine(tab(3) + escape("position") + ":{");

				printVector(sw, itemTemp.position, 4);

				sw.WriteLine(tab(3) + "},");
				sw.WriteLine(tab(3) + escape("direction") + ":{");
				
				printVector(sw, itemTemp.up, 4);
				
				sw.WriteLine(tab(3) + "}");
				sw.WriteLine(tab(2) + "}" + (i != itemCount - 1 ? "," : ""));
			}
		}

		sw.WriteLine("\t],");
	}

	private void ComposeGuards(StreamWriter sw)
	{
		GameObject guards = GameObject.Find("Guards");
		int guardCount = (guards == null ? 0 : guards.transform.childCount);
		
		sw.WriteLine("\t" + escape("guards") + ":[");
		
		if(!(guards == null || guardCount == 0))
		{
			// Temporary variables for each guard.
			Transform guardTemp = null;
			Guard guardScript = null;
			Vector3[] patrolPoints = null;
			
			for(int i = 0; i < guardCount; i++)
			{
				guardTemp = guards.transform.GetChild(i);
				guardScript = guardTemp.GetComponent<Guard>();
				
				sw.WriteLine(tab(2) + "{");
				sw.WriteLine(tab(3) + escape("name") + ":" + escape(guardTemp.name) + ",");
				sw.WriteLine(tab(3) + escape("position") + ":{");
				
				printVector(sw, guardTemp.position, 4);
				
				sw.WriteLine(tab(3) + "},");
				sw.WriteLine(tab(3) + escape("direction") + ":{");
				
				printVector(sw, guardTemp.up, 4);
				
				if(!(guardScript == null || guardTemp.name == "StationaryGuard"))
				{
					patrolPoints = guardScript.m_patrolPoints;
					
					sw.WriteLine(tab(3) + "},");
					sw.WriteLine(tab(3) + escape("patrolPoints") + ":[");
					
					for(int j = 0; j < patrolPoints.Length; j++)
					{
						sw.WriteLine(tab(4) + "{");
						
						printVector(sw, patrolPoints[j], 5);
						
						sw.WriteLine(tab(4) + "}" + (j != patrolPoints.Length - 1 ? "," : ""));
					}
					
					sw.WriteLine(tab(3) + "]");
				}
				else
					sw.WriteLine(tab(3) + "}");
				
				sw.WriteLine(tab(2) + "}" + (i != guards.transform.childCount - 1 ? "," : ""));
			}
		}
		
		sw.WriteLine("\t],");
	}

	private void ComposeObstacles(StreamWriter sw)
	{
		GameObject obstacles = GameObject.Find("Obstacles");
		int obstacleCount = (obstacles == null ? 0 : obstacles.transform.childCount);

		Transform hidingSpots = obstacles.transform.FindChild("HidingSpots");
		int hidingSpotCount = (hidingSpots == null ? 0 : hidingSpots.childCount);
		
		sw.WriteLine("\t" + escape("obstacles") + ":[");

		if(!(obstacles == null || obstacleCount == 0))
		{
			Transform obstacleTemp = null;

			if(hidingSpots != null)
			{
				obstacleCount--;
				hidingSpots.parent = null;
			}

			for(int i = 0; i < obstacleCount; i++)
			{
				obstacleTemp = obstacles.transform.GetChild(i);

				sw.WriteLine(tab(2) + "{");
				sw.WriteLine(tab(3) + escape("name") + ":" + escape(obstacleTemp.name) + ",");
				sw.WriteLine(tab(3) + escape("position") + ":{");
				
				printVector(sw, obstacleTemp.position, 4);
				
				sw.WriteLine(tab(3) + "},");
				sw.WriteLine(tab(3) + escape("direction") + ":{");
				
				printVector(sw, obstacleTemp.up, 4);
				
				sw.WriteLine(tab(3) + "}");
				sw.WriteLine(tab(2) + "}" + (i != obstacleCount - 1 ? "," : ""));
			}

			if(hidingSpots != null)
				hidingSpots.parent = obstacles.transform;
		}

		sw.WriteLine("\t],");
		sw.WriteLine("\t" + escape("hidingSpots") + ":[");

		if(!(hidingSpots == null || hidingSpotCount == 0))
		{
			Transform hidingSpotTemp = null;

			for(int i = 0; i < hidingSpotCount; i++)
			{
				hidingSpotTemp = hidingSpots.GetChild(i);
				
				sw.WriteLine(tab(2) + "{");
				sw.WriteLine(tab(3) + escape("name") + ":" + escape(hidingSpotTemp.name) + ",");
				sw.WriteLine(tab(3) + escape("position") + ":{");
				
				printVector(sw, hidingSpotTemp.position, 4);
				
				sw.WriteLine(tab(3) + "},");
				sw.WriteLine(tab(3) + escape("direction") + ":{");
				
				printVector(sw, hidingSpotTemp.up, 4);
				
				sw.WriteLine(tab(3) + "}");
				sw.WriteLine(tab(2) + "}" + (i != hidingSpotCount - 1 ? "," : ""));
			}
		}

		sw.WriteLine("\t],");
	}

	private void ComposeWalls(StreamWriter sw)
	{
		GameObject wallsContainer = GameObject.Find("Walls");
		BoxCollider2D[] walls = null;
		int wallCount = 0;

		if(wallsContainer != null)
		{
			walls = wallsContainer.GetComponents<BoxCollider2D>();

			if(walls != null)
				wallCount = walls.Length;
		}

		sw.WriteLine("\t" + escape("walls") + ":[");

		if(wallCount != 0)
		{
			for(int i = 0; i < wallCount; i++)
			{
				sw.WriteLine(tab(2) + "{");
				sw.WriteLine(tab(3) + escape("size") + ":{");
				
				printVector(sw, walls[i].size, 4);

				sw.WriteLine(tab(3) + "},");
				sw.WriteLine(tab(3) + escape("center") + ":{");
				
				printVector(sw, walls[i].center, 4);
				
				sw.WriteLine(tab(3) + "}");
				sw.WriteLine(tab(2) + "}" + (i != wallCount - 1 ? "," : ""));
			}
		}

		sw.WriteLine("\t],");
	}

	private void ComposeDoors(StreamWriter sw)
	{
		GameObject doors = GameObject.Find("Doors");
		int doorCount = (doors == null ? 0 : doors.transform.childCount);

		sw.WriteLine("\t" + escape("doors") + ":[");

		if(!(doors == null || doorCount == 0))
		{
			Transform doorTemp = null;
			
			for(int i = 0; i < doorCount; i++)
			{
				doorTemp = doors.transform.GetChild(i);
				
				sw.WriteLine(tab(2) + "{");
				sw.WriteLine(tab(3) + escape("name") + ":" + escape(doorTemp.name) + ",");
				sw.WriteLine(tab(3) + escape("position") + ":{");
				
				printVector(sw, doorTemp.position, 4);
				
				sw.WriteLine(tab(3) + "},");
				sw.WriteLine(tab(3) + escape("direction") + ":{");
				
				printVector(sw, doorTemp.up, 4);
				
				sw.WriteLine(tab(3) + "}");
				sw.WriteLine(tab(2) + "}" + (i != doorCount - 1 ? "," : ""));
			}
		}

		sw.WriteLine("\t],");
	}

	private void ComposeFloors(StreamWriter sw)
	{
		GameObject floor = GameObject.Find("FloorColliders");
		int floorCount = (floor == null ? 0 : floor.transform.childCount);
		
		sw.WriteLine("\t" + escape("floors") + ":[");
		
		if(!(floor == null || floorCount == 0))
		{
			Transform floorTemp = null;
			BoxCollider2D colliderTemp = null;
			
			for(int i = 0; i < floorCount; i++)
			{
				floorTemp = floor.transform.GetChild(i);
				colliderTemp = floorTemp.GetComponent<BoxCollider2D>();

				if(colliderTemp == null)
					continue;
				
				sw.WriteLine(tab(2) + "{");
				sw.WriteLine(tab(3) + escape("name") + ":" + escape(floorTemp.name) + ",");
				sw.WriteLine(tab(3) + escape("position") + ":{");
				
				printVector(sw, floorTemp.position, 4);
				
				sw.WriteLine(tab(3) + "},");
				sw.WriteLine(tab(3) + escape("scale") + ":{");
				
				printVector(sw, floorTemp.localScale, 4);
				
				sw.WriteLine(tab(3) + "},");
				sw.WriteLine(tab(3) + escape("collider") + ":[");

				sw.WriteLine(tab(4) + "{");
				sw.WriteLine(tab(5) + escape("size") + ":{");
				
				printVector(sw, colliderTemp.size, 6);
				
				sw.WriteLine(tab(5) + "},");
				sw.WriteLine(tab(5) + escape("center") + ":{");
				
				printVector(sw, colliderTemp.center, 6);
				
				sw.WriteLine(tab(5) + "}");
				sw.WriteLine(tab(4) + "}");
				sw.WriteLine(tab(3) + "]");
				sw.WriteLine(tab(2) + "}" + (i != floorCount - 1 ? "," : ""));
			}
		}
		
		sw.WriteLine("\t]");
	}

	public string tab(int n)
	{
		return new string('\t', n);
	}

	public string escape(string str)
	{
		return "\"" + str + "\"";
	}

	public void printVector(StreamWriter sw, Vector2 vector, int t)
	{
		if(vector == null) return;

		sw.WriteLine(tab(t) + escape("x") + ":" + vector.x + ",");
		sw.WriteLine(tab(t) + escape("y") + ":" + vector.y);
	}

	public void printVector(StreamWriter sw, Vector3 vector, int t)
	{
		if(vector == null) return;
		
		sw.WriteLine(tab(t) + escape("x") + ":" + vector.x + ",");
		sw.WriteLine(tab(t) + escape("y") + ":" + vector.y + ",");
		sw.WriteLine(tab(t) + escape("z") + ":" + vector.z);
	}
}
