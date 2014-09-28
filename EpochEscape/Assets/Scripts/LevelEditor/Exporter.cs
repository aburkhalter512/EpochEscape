/*
 * Usage: Apply the script to the main camera, or an empty game object. The script will output the level data a file.
 */

using UnityEngine;
//using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class Exporter : MonoBehaviour
{
    public string m_levelName = string.Empty;
    private int m_numberOfObjects = 0;

	public void Start()
	{
        if (m_levelName == string.Empty)
            m_levelName = "level";

		using(StreamWriter sw = new StreamWriter("Assets/Resources/Data/Levels/" + m_levelName + ".txt"))
		{
			sw.WriteLine("{");

            ComposeChambers(sw, 1);
            ComposeLevel(sw, 1);
			
			sw.WriteLine("}");
		}
	}

    private void ComposeLevel(StreamWriter sw, int tab)
    {
        if (sw != null)
        {
            sw.WriteLine(LevelEditorUtilities.Tab(tab) + LevelEditorUtilities.Escape("level") + ":{");
            sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + LevelEditorUtilities.Escape("name") + ":" + LevelEditorUtilities.Escape(m_levelName) + ",");
            sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + LevelEditorUtilities.Escape("numberOfObjects") + ":" + m_numberOfObjects.ToString());
            sw.WriteLine(LevelEditorUtilities.Tab(tab) + "},");
        }
    }

    private void ComposeChambers(StreamWriter sw, int tab)
    {
        if (sw != null)
        {
            sw.WriteLine(LevelEditorUtilities.Tab(tab) + LevelEditorUtilities.Escape("chambers") + ":[");

            // Get all chambers that are currently tagged.
            GameObject[] chambers = GameObject.FindGameObjectsWithTag("Chamber");

            // Sort the chambers by name using a delegate.
            // The order returned by GameObject.FindGameObjectsWithTag() is random.
            // Efficiency isn't the goal.
            Array.Sort(chambers, (GameObject chamberA, GameObject chamberB) =>
            {
                return chamberA.name.CompareTo(chamberB.name);
            });

            // Compose each chamber.
            for (int chamber = 0; chamber < chambers.Length; chamber++)
                ComposeChamber(sw, chambers[chamber], tab + 1);

            sw.WriteLine(LevelEditorUtilities.Tab(tab) + "],");
        }
    }

    private void ComposeChamber(StreamWriter sw, GameObject chamber, int tab)
    {
        Vector2 chamberSize = Vector2.zero;

        if (sw != null)
        {
            sw.WriteLine(LevelEditorUtilities.Tab(tab) + "{");
            sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + LevelEditorUtilities.Escape("name") + ":" + LevelEditorUtilities.Escape(chamber.name) + ",");
            sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + LevelEditorUtilities.Escape("position") + ":" + LevelEditorUtilities.Escape(chamber.transform.position) + ",");

            ComposeChunks(sw, chamber, ref chamberSize, tab + 1);
            ComposeDoors(sw, chamber, tab + 1);
            ComposeItems(sw, chamber, tab + 1);

            sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + LevelEditorUtilities.Escape("size") + ":" + LevelEditorUtilities.Escape(chamberSize.ToString()));
            sw.WriteLine(LevelEditorUtilities.Tab(tab) + "},");
        }
    }

    private void ComposeChunks(StreamWriter sw, GameObject parent, ref Vector2 chamberSize, int tab)
    {
        Transform chunks = parent.transform.FindChild("Chunks");
        int chunksComposed = 0;
        float previousY = 0f;
        bool canStopX = false;

        if (chunks != null)
        {
            sw.WriteLine(LevelEditorUtilities.Tab(tab) + LevelEditorUtilities.Escape("chunks") + ":[");

            foreach (Transform chunk in chunks)
            {
                sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + "{");

                sw.WriteLine(LevelEditorUtilities.Tab(tab + 2) + LevelEditorUtilities.Escape("name") + ":" + LevelEditorUtilities.Escape(chunk.name) + ",");
                sw.WriteLine(LevelEditorUtilities.Tab(tab + 2) + LevelEditorUtilities.Escape("position") + ":" + LevelEditorUtilities.Escape(chunk.transform.localPosition) + ",");
                
                sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + "},");

                if (chunksComposed == 0)
                {
                    chamberSize.x += chunk.renderer.bounds.size.x;
                    chamberSize.y += chunk.renderer.bounds.size.y;
                }
                else if (chunk.position.y > previousY && !Utilities.IsApproximately(chunk.position.y, previousY))
                {
                    chamberSize.y += chunk.renderer.bounds.size.y;

                    canStopX = true;
                }
                else if (!canStopX)
                    chamberSize.x += chunk.renderer.bounds.size.x;

                chunksComposed++;
                previousY = chunk.position.y;

                m_numberOfObjects++;
            }

            sw.WriteLine(LevelEditorUtilities.Tab(tab) + "],");
        }
    }

    private void ComposeDoors(StreamWriter sw, GameObject parent, int tab)
    {
        Transform doors = parent.transform.FindChild("Doors");
        Dictionary<string, object> data = new Dictionary<string,object>();
        int i = 0;

        if (doors != null)
        {
            ISerializable doorSerializer = null;

            sw.WriteLine(LevelEditorUtilities.Tab(tab) + LevelEditorUtilities.Escape("doors") + ":[");

            foreach (Transform door in doors)
            {
                doorSerializer = door.GetComponent(typeof(ISerializable)) as ISerializable;

                sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + "{");

                data["name"] = door.name;
                data["position"] = door.transform.localPosition;
                data["direction"] = door.transform.up;

                if (doorSerializer != null)
                    doorSerializer.Serialize(ref data);

                foreach (var datum in data)
                {
                    sw.Write(LevelEditorUtilities.Tab(tab + 2) + LevelEditorUtilities.Escape(datum.Key) + ":");

                    if (datum.Value.GetType() == typeof(int) || datum.Value.GetType() == typeof(float) || datum.Value.GetType() == typeof(bool))
                        sw.Write(datum.Value.ToString());
                    else
                        sw.Write(LevelEditorUtilities.Escape(datum.Value));

                    i++;

                    if (i != data.Count)
                        sw.Write(",");

                    sw.WriteLine("");
                }

                sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + "},");

                m_numberOfObjects++;

                data.Clear();
                i = 0;
            }

            sw.WriteLine(LevelEditorUtilities.Tab(tab) + "],");
            
            // ---

            Transform teleportDoors = doors.transform.FindChild("TeleporterDoors");

            if (teleportDoors != null)
            {
                List<GameObject> teleDoors = new List<GameObject>();

                foreach (Transform teleportDoor in teleportDoors)
                {
                    CustomDoorFrame teleDoor = teleportDoor.GetComponent<CustomDoorFrame>();

                    if (teleDoor != null)
                        teleDoors.Add(teleDoor.gameObject);
                }

                Debug.Log(teleDoors.Count);
            }
        }
    }

    private void ComposeItems(StreamWriter sw, GameObject parent, int tab)
    {
        Transform items = parent.transform.FindChild("Items");

        if (items != null)
        {
            sw.WriteLine(LevelEditorUtilities.Tab(tab) + LevelEditorUtilities.Escape("items") + ":{");

            ComposePowerCores(sw, items.gameObject, tab + 1);
            ComposePotions(sw, items.gameObject, tab + 1);
            ComposeCrates(sw, items.gameObject, tab + 1);

            sw.WriteLine(LevelEditorUtilities.Tab(tab) + "},");
        }
    }

    private void ComposePowerCores(StreamWriter sw, GameObject parent, int tab)
    {
        Transform powerCores = parent.transform.FindChild("Cores");

        if (powerCores != null)
        {
            sw.WriteLine(LevelEditorUtilities.Tab(tab) + LevelEditorUtilities.Escape("powerCores") + ":[");

            foreach (Transform powerCore in powerCores)
            {
                sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + "{");

                sw.WriteLine(LevelEditorUtilities.Tab(tab + 2) + LevelEditorUtilities.Escape("name") + ":" + LevelEditorUtilities.Escape(powerCore.name) + ",");
                sw.WriteLine(LevelEditorUtilities.Tab(tab + 2) + LevelEditorUtilities.Escape("position") + ":" + LevelEditorUtilities.Escape(powerCore.localPosition.ToString()) + ",");
                sw.WriteLine(LevelEditorUtilities.Tab(tab + 2) + LevelEditorUtilities.Escape("direction") + ":" + LevelEditorUtilities.Escape(powerCore.up.ToString()) + ",");

                sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + "},");

                m_numberOfObjects++;
            }

            sw.WriteLine(LevelEditorUtilities.Tab(tab) + "],");
        }
    }

    private void ComposePotions(StreamWriter sw, GameObject parent, int tab)
    {
        Transform potions = parent.transform.FindChild("Potions");

        if (potions != null)
        {
            sw.WriteLine(LevelEditorUtilities.Tab(tab) + LevelEditorUtilities.Escape("potions") + ":{");

            ComposeRedPotions(sw, potions.gameObject, tab + 1);
            ComposeGreenPotions(sw, potions.gameObject, tab + 1);

            sw.WriteLine(LevelEditorUtilities.Tab(tab) + "},");
        }
    }

    private void ComposeRedPotions(StreamWriter sw, GameObject parent, int tab)
    {
        Transform redPotions = parent.transform.FindChild("RedPotions");

        if (redPotions != null)
        {
            ISerializable redPotionSerializer = null;

            sw.WriteLine(LevelEditorUtilities.Tab(tab) + LevelEditorUtilities.Escape("redPotions") + ":[");

            foreach (Transform redPotion in redPotions)
            {
                redPotionSerializer = redPotion.GetComponent(typeof(ISerializable)) as ISerializable;

                if (redPotionSerializer != null)
                {
                    sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + "{");

                    //redPotionSerializer.Serialize(sw, tab + 1);

                    sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + "},");

                    m_numberOfObjects++;
                }
            }

            sw.WriteLine(LevelEditorUtilities.Tab(tab) + "],");
        }
    }

    private void ComposeGreenPotions(StreamWriter sw, GameObject parent, int tab)
    {
        Transform greenPotions = parent.transform.FindChild("GreenPotions");

        if (greenPotions != null)
        {
            ISerializable greenPotionSerializer = null;

            sw.WriteLine(LevelEditorUtilities.Tab(tab) + LevelEditorUtilities.Escape("greenPotions") + ":[");

            foreach (Transform greenPotion in greenPotions)
            {
                greenPotionSerializer = greenPotion.GetComponent(typeof(ISerializable)) as ISerializable;

                if (greenPotionSerializer != null)
                {
                    sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + "{");

                    //greenPotionSerializer.Serialize(sw, tab + 1);

                    sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + "},");

                    m_numberOfObjects++;
                }
            }

            sw.WriteLine(LevelEditorUtilities.Tab(tab) + "],");
        }
    }

    private void ComposeCrates(StreamWriter sw, GameObject parent, int tab)
    {
        Transform crates = parent.transform.FindChild("Crates");

        if (crates != null)
        {
            ISerializable createSerializer = null;

            sw.WriteLine(LevelEditorUtilities.Tab(tab) + LevelEditorUtilities.Escape("crates") + ":[");

            foreach (Transform crate in crates)
            {
                createSerializer = crate.GetComponent(typeof(ISerializable)) as ISerializable;

                if (createSerializer != null)
                {
                    sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + "{");

                    //createSerializer.Serialize(sw, tab + 1);

                    sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + "},");

                    m_numberOfObjects++;
                }
            }

            sw.WriteLine(LevelEditorUtilities.Tab(tab) + "],");
        }
    }
}
