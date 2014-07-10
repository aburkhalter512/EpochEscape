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
			GameObject guards = GameObject.Find("Guards");
			int guardCount = (guards == null ? 0 : guards.transform.childCount);

			sw.WriteLine("{");
			sw.WriteLine("\t" + escape("guards") + ":[");

			if(guards != null && guardCount > 0)
			{
				// Temporary variables for each guard.
				Transform guardTemp = null;
				string guardType = null;
				Guard guardScript = null;
				Vector3[] patrolPoints = null;
				
				for(int i = 0; i < guardCount; i++)
				{
					guardTemp = guards.transform.GetChild(i);
					guardType = guardTemp.name;
					guardScript = guardTemp.GetComponent<Guard>();

					sw.WriteLine(tab(2) + "{");
					sw.WriteLine(tab(3) + escape("type") + ":" + escape(guardType) + ",");
					sw.WriteLine(tab(3) + escape("initialPosition") + ":{");

					printVector3(sw, guardTemp.position, 4);

					sw.WriteLine(tab(3) + "},");
					sw.WriteLine(tab(3) + escape("initialDirection") + ":{");

					printVector3(sw, guardTemp.transform.up, 4);
					
					if(!(guardScript == null || guardType == "StationaryGuard"))
					{
						patrolPoints = guardScript.m_patrolPoints;

						sw.WriteLine(tab(3) + "},");
						sw.WriteLine(tab(3) + escape("patrolPoints") + ":[");

						for(int j = 0; j < patrolPoints.Length; j++)
						{
							sw.WriteLine(tab(4) + "{");

							printVector3(sw, patrolPoints[j], 5);

							sw.WriteLine(tab(4) + "}" + (j != patrolPoints.Length - 1 ? "," : ""));
						}

						sw.WriteLine(tab(3) + "]");
					}
					else
						sw.WriteLine(tab(3) + "}");

					sw.WriteLine(tab(2) + "}" + (i != guards.transform.childCount - 1 ? "," : ""));
				}
			}

			sw.WriteLine("\t]");
			sw.WriteLine("}");
		}
	}

	public string tab(int n)
	{
		return new string('\t', n);
	}

	public string escape(string str)
	{
		return "\"" + str + "\"";
	}

	public void printVector3(StreamWriter sw, Vector3 vector, int t)
	{
		if(vector == null) return;

		sw.WriteLine(tab(t) + escape("x") + ":" + vector.x + ",");
		sw.WriteLine(tab(t) + escape("y") + ":" + vector.y + ",");
		sw.WriteLine(tab(t) + escape("z") + ":" + vector.z);
	}
}
