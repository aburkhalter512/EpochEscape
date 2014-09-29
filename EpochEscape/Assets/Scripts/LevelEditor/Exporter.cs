/*
 * Usage: Apply the script to the main camera, or an empty game object. The script will output the level data a file.
 * Note: The algorithms in this script are far from efficient, and the code is overly complicated in areas.
 */

using UnityEngine;
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
            // Efficiency here isn't the goal.
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
            ComposeDynamicWalls(sw, chamber, tab + 1);
            ComposeTriggers(sw, chamber, tab + 1);
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

                data["id"] = LevelEditorUtilities.GenerateObjectHash(door.name, door.transform.position);
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
        }
    }

    private void ComposeDynamicWalls(StreamWriter sw, GameObject parent, int tab)
    {
        Transform dynamicWalls = parent.transform.FindChild("DynamicWalls");
        Dictionary<string, object> data = new Dictionary<string, object>();
        int i = 0;

        if (dynamicWalls != null)
        {
            ISerializable dynamicWallSerializer = null;

            sw.WriteLine(LevelEditorUtilities.Tab(tab) + LevelEditorUtilities.Escape("dynamicWalls") + ":[");

            foreach (Transform dynamicWall in dynamicWalls)
            {
                dynamicWallSerializer = dynamicWall.GetComponent(typeof(ISerializable)) as ISerializable;

                sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + "{");

                data["id"] = LevelEditorUtilities.GenerateObjectHash(dynamicWall.name, dynamicWall.transform.position);
                data["name"] = dynamicWall.name;
                data["position"] = dynamicWall.transform.localPosition;
                data["direction"] = dynamicWall.transform.up;

                if (dynamicWallSerializer != null)
                    dynamicWallSerializer.Serialize(ref data);

                foreach (var datum in data)
                {
                    sw.Write(LevelEditorUtilities.Tab(tab + 2) + LevelEditorUtilities.Escape(datum.Key) + ":");

                    if (datum.Value.GetType() == typeof(int) || datum.Value.GetType() == typeof(float) || datum.Value.GetType() == typeof(bool))
                        sw.Write(datum.Value.ToString());
                    else if (datum.Value.GetType() == typeof(Vector3[]))
                    {
                        sw.WriteLine("[");

                        foreach (Vector3 vector in datum.Value as Vector3[])
                            sw.WriteLine(LevelEditorUtilities.Tab(tab + 3) + LevelEditorUtilities.Escape(vector.ToString()) + ",");

                        sw.Write(LevelEditorUtilities.Tab(tab + 2) + "]");
                    }
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
        }
    }

    private void ComposeTriggers(StreamWriter sw, GameObject parent, int tab)
    {
        Transform triggers = parent.transform.FindChild("Triggers");

        if (triggers != null)
        {
            sw.WriteLine(LevelEditorUtilities.Tab(tab) + LevelEditorUtilities.Escape("triggers") + ":{");

            ComposeTerminals(sw, triggers.gameObject, tab + 1);
            ComposePlates(sw, triggers.gameObject, tab + 1);

            sw.WriteLine(LevelEditorUtilities.Tab(tab) + "},");
        }
    }

    private void ComposeTerminals(StreamWriter sw, GameObject parent, int tab)
    {
        Transform terminals = parent.transform.FindChild("Terminals");
        Dictionary<string, object> data = new Dictionary<string, object>();
        int i = 0;

        if (terminals != null)
        {
            ISerializable terminalSerializer = null;

            sw.WriteLine(LevelEditorUtilities.Tab(tab) + LevelEditorUtilities.Escape("terminals") + ":[");

            foreach (Transform terminal in terminals)
            {
                terminalSerializer = terminal.GetComponent(typeof(ISerializable)) as ISerializable;

                sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + "{");

                data["id"] = LevelEditorUtilities.GenerateObjectHash(terminal.name, terminal.transform.localPosition);
                data["name"] = terminal.name;
                data["position"] = terminal.transform.localPosition;
                data["direction"] = terminal.transform.up;

                if (terminalSerializer != null)
                    terminalSerializer.Serialize(ref data);

                foreach (var datum in data)
                {
                    sw.Write(LevelEditorUtilities.Tab(tab + 2) + LevelEditorUtilities.Escape(datum.Key) + ":");

                    if (datum.Value.GetType() == typeof(int) || datum.Value.GetType() == typeof(float) || datum.Value.GetType() == typeof(bool))
                        sw.Write(datum.Value.ToString());
                    else if (datum.Value.GetType() == typeof(GameObject[]))
                    {
                        sw.WriteLine("[");

                        foreach (GameObject obj in datum.Value as GameObject[])
                            sw.WriteLine(LevelEditorUtilities.Tab(tab + 3) + LevelEditorUtilities.Escape(LevelEditorUtilities.GenerateObjectHash(obj.name, obj.transform.localPosition)) + ",");

                        sw.Write(LevelEditorUtilities.Tab(tab + 2) + "]");
                    }
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
        }
    }

    private void ComposePlates(StreamWriter sw, GameObject parent, int tab)
    {
        Transform plates = parent.transform.FindChild("Plates");
        Dictionary<string, object> data = new Dictionary<string, object>();
        int i = 0;

        if (plates != null)
        {
            ISerializable plateSerializer = null;

            sw.WriteLine(LevelEditorUtilities.Tab(tab) + LevelEditorUtilities.Escape("plates") + ":[");

            foreach (Transform plate in plates)
            {
                plateSerializer = plate.GetComponent(typeof(ISerializable)) as ISerializable;

                sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + "{");

                data["id"] = LevelEditorUtilities.GenerateObjectHash(plate.name, plate.transform.localPosition);
                data["name"] = plate.name;
                data["position"] = plate.transform.localPosition;
                data["direction"] = plate.transform.up;

                if (plateSerializer != null)
                    plateSerializer.Serialize(ref data);

                foreach (var datum in data)
                {
                    sw.Write(LevelEditorUtilities.Tab(tab + 2) + LevelEditorUtilities.Escape(datum.Key) + ":");

                    if (datum.Value.GetType() == typeof(int) || datum.Value.GetType() == typeof(float) || datum.Value.GetType() == typeof(bool))
                        sw.Write(datum.Value.ToString());
                    else if (datum.Value.GetType() == typeof(GameObject[]))
                    {
                        sw.WriteLine("[");

                        foreach (GameObject obj in datum.Value as GameObject[])
                            sw.WriteLine(LevelEditorUtilities.Tab(tab + 3) + LevelEditorUtilities.Escape(LevelEditorUtilities.GenerateObjectHash(obj.name, obj.transform.localPosition)) + ",");

                        sw.Write(LevelEditorUtilities.Tab(tab + 2) + "],");
                    }
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
        }
    }

    private void ComposeItems(StreamWriter sw, GameObject parent, int tab)
    {
        Transform items = parent.transform.FindChild("Items");

        if (items != null)
        {
            sw.WriteLine(LevelEditorUtilities.Tab(tab) + LevelEditorUtilities.Escape("items") + ":{");

            ComposePowerCores(sw, items.gameObject, tab + 1);
            ComposeRedPotions(sw, items.gameObject, tab + 1);
            ComposeGreenPotions(sw, items.gameObject, tab + 1);
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

    private void ComposeRedPotions(StreamWriter sw, GameObject parent, int tab)
    {
        Transform redPotions = parent.transform.FindChild("RedPotions");

        if (redPotions != null)
        {
            sw.WriteLine(LevelEditorUtilities.Tab(tab) + LevelEditorUtilities.Escape("redPotions") + ":[");

            foreach (Transform redPotion in redPotions)
            {
                sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + "{");

                sw.WriteLine(LevelEditorUtilities.Tab(tab + 2) + LevelEditorUtilities.Escape("name") + ":" + LevelEditorUtilities.Escape(redPotion.name) + ",");
                sw.WriteLine(LevelEditorUtilities.Tab(tab + 2) + LevelEditorUtilities.Escape("position") + ":" + LevelEditorUtilities.Escape(redPotion.localPosition.ToString()) + ",");
                sw.WriteLine(LevelEditorUtilities.Tab(tab + 2) + LevelEditorUtilities.Escape("direction") + ":" + LevelEditorUtilities.Escape(redPotion.up.ToString()) + ",");

                sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + "},");

                m_numberOfObjects++;
            }

            sw.WriteLine(LevelEditorUtilities.Tab(tab) + "],");
        }
    }

    private void ComposeGreenPotions(StreamWriter sw, GameObject parent, int tab)
    {
        Transform greenPotions = parent.transform.FindChild("GreenPotions");

        if (greenPotions != null)
        {
            sw.WriteLine(LevelEditorUtilities.Tab(tab) + LevelEditorUtilities.Escape("greenPotions") + ":[");

            foreach (Transform greenPotion in greenPotions)
            {
                sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + "{");

                sw.WriteLine(LevelEditorUtilities.Tab(tab + 2) + LevelEditorUtilities.Escape("name") + ":" + LevelEditorUtilities.Escape(greenPotion.name) + ",");
                sw.WriteLine(LevelEditorUtilities.Tab(tab + 2) + LevelEditorUtilities.Escape("position") + ":" + LevelEditorUtilities.Escape(greenPotion.localPosition.ToString()) + ",");
                sw.WriteLine(LevelEditorUtilities.Tab(tab + 2) + LevelEditorUtilities.Escape("direction") + ":" + LevelEditorUtilities.Escape(greenPotion.up.ToString()) + ",");

                sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + "},");

                m_numberOfObjects++;
            }

            sw.WriteLine(LevelEditorUtilities.Tab(tab) + "],");
        }
    }

    private void ComposeCrates(StreamWriter sw, GameObject parent, int tab)
    {
        Transform crates = parent.transform.FindChild("Crates");

        if (crates != null)
        {
            sw.WriteLine(LevelEditorUtilities.Tab(tab) + LevelEditorUtilities.Escape("crates") + ":[");

            foreach (Transform crate in crates)
            {
                sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + "{");

                sw.WriteLine(LevelEditorUtilities.Tab(tab + 2) + LevelEditorUtilities.Escape("name") + ":" + LevelEditorUtilities.Escape(crate.name) + ",");
                sw.WriteLine(LevelEditorUtilities.Tab(tab + 2) + LevelEditorUtilities.Escape("position") + ":" + LevelEditorUtilities.Escape(crate.localPosition.ToString()) + ",");
                sw.WriteLine(LevelEditorUtilities.Tab(tab + 2) + LevelEditorUtilities.Escape("direction") + ":" + LevelEditorUtilities.Escape(crate.up.ToString()) + ",");

                sw.WriteLine(LevelEditorUtilities.Tab(tab + 1) + "},");

                m_numberOfObjects++;
            }

            sw.WriteLine(LevelEditorUtilities.Tab(tab) + "],");
        }
    }
}
