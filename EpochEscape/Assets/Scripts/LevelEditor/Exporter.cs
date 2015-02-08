/*
 * Usage: Apply the script to the main camera, or an empty game object. The script will output the level data a file.
 * Note: The algorithms in this script are far from efficient, and the code is overly complicated in areas.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Xml;

public class Exporter : MonoBehaviour
{
    public string m_levelName = string.Empty;

    private int m_numberOfObjects = 0;
    private XmlDocument xmlDocument = null;

    public void Update()
    {
        if (m_levelName == string.Empty)
            m_levelName = "level";

        Debug.Log("Exporting...");

        xmlDocument = new XmlDocument();
        XmlElement levelTag = xmlDocument.CreateElement("level");
        xmlDocument.AppendChild(levelTag);

        levelTag.SetAttribute("name", m_levelName);

        SerializeLevel(levelTag);

        levelTag.SetAttribute("objectCount", m_numberOfObjects.ToString());

        xmlDocument.Save(m_levelName);

        Debug.Log("Finished exporting.");

        GameObject.Destroy(this);
    }

    private void SerializeLevel(XmlElement parent)
    {
        if (parent == null || xmlDocument == null)
            return;

        // Get all chambers that are currently tagged.
        GameObject[] chambers = GameObject.FindGameObjectsWithTag("Chamber");

        // Sort the chambers by name using a delegate.
        // The order returned by GameObject.FindGameObjectsWithTag() is random.
        // Efficiency here isn't the goal.
        Array.Sort(chambers, (GameObject chamberA, GameObject chamberB) =>
        {
            return chamberA.name.CompareTo(chamberB.name);
        });

        foreach (GameObject chamber in chambers)
        {
            XmlElement chamberTag = xmlDocument.CreateElement("chamber");
            chamberTag.SetAttribute("name", chamber.name);

            List<XmlElement> serializedObjects = new List<XmlElement>();
            m_numberOfObjects = serializeChildren(serializedObjects, chamber.transform);
            List<XmlElement> organizedObjects = organizeChildren(serializedObjects);

            foreach (XmlElement element in organizedObjects)
                chamberTag.AppendChild(element);

            parent.AppendChild(chamberTag);
        }
    }

    private int serializeChildren(List<XmlElement> serializedObjects, Transform parent)
    {
        if (parent == null || serializedObjects == null)
            return 0;

        MonoBehaviour[] convertors = parent.GetComponents<MonoBehaviour>();
        ISerializable serializable = null;
        int childrenCount = 0;

        foreach (MonoBehaviour convertor in convertors)
        {
            serializable = convertor as ISerializable;

            if (serializable == null)
                continue;

            serializedObjects.Add(serializable.Serialize(xmlDocument));
            childrenCount++;
        }

        foreach (Transform child in parent)
            childrenCount += serializeChildren(serializedObjects, child);

        return childrenCount;
    }

    private List<XmlElement> organizeChildren(List<XmlElement> serializedObjects)
    {
        List<XmlElement> retVal = new List<XmlElement>();

        XmlElement doors = xmlDocument.CreateElement("doors");
        XmlElement dynamicWalls = xmlDocument.CreateElement("dynamicwalls");
        XmlElement activators = xmlDocument.CreateElement("activators");
        XmlElement items = xmlDocument.CreateElement("items");

        foreach (XmlElement element in serializedObjects)
        {
            switch (element.Name)
            {
                case "door":
                    doors.AppendChild(element);
                    break;
                case "dynamicwall":
                    dynamicWalls.AppendChild(element);
                    break;
                case "activator":
                    activators.AppendChild(element);
                    break;
                case "item":
                    items.AppendChild(element);
                    break;

            }
        }

        retVal.Add(doors);
        retVal.Add(dynamicWalls);
        retVal.Add(activators);
        retVal.Add(items);

        return retVal;
    }
}
