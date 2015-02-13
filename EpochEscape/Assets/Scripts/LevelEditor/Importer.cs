/*
 * Usage: Apply to the main camera, or an empty game object. Game objects will be constructed based on the data file.
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MiniJSON;

public class Importer : MonoBehaviour
{
    public string levelName;

    #region Instance Variables
    private string mLevelPath;

    private DoorFactory mDoorFactory;
    private DynamicWallFactory mDynamicWallFactory;
    private ActivatorFactory mActivatorFactory;
    private ItemFactory mItemFactory;

    private List<IActivatable> mActivatables;

    private bool mImportedDoors;
    private bool mImportedDynWalls;
    private bool mImportedActivators;
    private bool mImportedItems;
    #endregion

    public void Awake()
    {
        initializeFactories();

        mActivatables = new List<IActivatable>();

        mLevelPath = "Levels\\" + levelName + ".xml";
        if (!File.Exists(mLevelPath))
        {
            Debug.Log(mLevelPath + " does not exist.");
            return;
        }

        XmlDocument document = new XmlDocument();
        document.Load(mLevelPath);

        mImportedDoors = false;
        mImportedDynWalls = false;
        mImportedActivators = false;
        mImportedItems = false;
        importLevel(document);

        StartCoroutine(waitForReady());
    }

    private void initializeFactories()
    {
        mDoorFactory = new DoorFactory();
        mDynamicWallFactory = new DynamicWallFactory();
        mActivatorFactory = new ActivatorFactory();
        mItemFactory = new ItemFactory();
    }

    private void importLevel(XmlDocument document)
    {
        XmlElement rootElem;
        foreach (XmlNode root in document.ChildNodes)
        {
            rootElem = root as XmlElement;

            if (rootElem == null)
                continue;

            XmlNode chamberIter = rootElem.FirstChild;
            XmlElement chamberElem;

            GameObject chamber;

            while (chamberIter != null)
            {
                chamberElem = chamberIter as XmlElement;
                chamberIter = chamberIter.NextSibling;

                if (chamberElem == null)
                    continue;

                chamber = new GameObject();
                chamber.transform.position =
                    Utilities.StringToVector3(chamberElem.GetAttribute("position"));
                chamber.name = chamberElem.GetAttribute("name") + "_imported";

                XmlNode categoryIter = chamberElem.FirstChild;
                XmlElement categoryElem;
                
                XmlElement doors = null;
                XmlElement dynamicWalls = null;
                XmlElement activators = null;
                XmlElement items = null;
                while (categoryIter != null)
                {
                    categoryElem = categoryIter as XmlElement;
                    categoryIter = categoryIter.NextSibling;

                    if (categoryElem == null)
                        continue;

                    switch (categoryElem.Name)
                    {
                        case "doors":
                            StartCoroutine(importDoors(categoryElem));
                            break;
                        case "dynamicwalls":
                            StartCoroutine(importDynamicWalls(categoryElem));
                            dynamicWalls = categoryElem;
                            break;
                        /*scase "activators":
                            StartCoroutine(importActivators(categoryElem));
                            activators = categoryElem;
                            break;//*/
                        case "items":
                            StartCoroutine(importItems(categoryElem));
                            items = categoryElem;
                            break;//*/
                    }
                }
            }
        }
    }

    private IEnumerator importDoors(XmlElement parent)
    {
        XmlElement doorElem;
        foreach (XmlNode node in parent.ChildNodes)
        {
            doorElem = node as XmlElement;
            if (doorElem == null)
                continue;

            mActivatables.Add(mDoorFactory.create(doorElem) as IActivatable);

            yield return null;
        }

        mImportedDoors = true;
    }

    private IEnumerator importDynamicWalls(XmlElement parent)
    {
        XmlElement wallElem;
        foreach (XmlNode node in parent.ChildNodes)
        {
            wallElem = node as XmlElement;
            if (wallElem == null)
                continue;

            mActivatables.Add(mDynamicWallFactory.create(wallElem) as IActivatable);

            yield return null;
        }

        mImportedDynWalls = true;
    }

    private IEnumerator importActivators(XmlElement parent)
    {
        //Check to see if the activatables have imported yet
        while (!(mImportedDoors && mImportedDynWalls))
        {
            Debug.Log("Waiting for activatables to import...");
            yield return new WaitForSeconds(.5f);
        }

        Debug.Log("Activatables imported.");

        XmlElement activatorElem;
        foreach (XmlNode node in parent.ChildNodes)
        {
            activatorElem = node as XmlElement;
            if (activatorElem == null)
                continue;

            if (mActivatorFactory.toConnect == null)
                mActivatorFactory.toConnect = mActivatables;

            mActivatorFactory.create(activatorElem);

            yield return null;
        }
    }

    private IEnumerator importItems(XmlElement parent)
    {
        XmlElement itemElem;
        foreach (XmlNode node in parent.ChildNodes)
        {
            itemElem = node as XmlElement;
            if (itemElem == null)
                continue;

            mItemFactory.create(itemElem);

            yield return null;
        }
    }

    private IEnumerator waitForReady()
    {
        while (!(mImportedDoors && mImportedDynWalls && mImportedActivators && mImportedItems))
            yield return new WaitForSeconds(.33f);

        LevelManager.Ready();
    }
}
