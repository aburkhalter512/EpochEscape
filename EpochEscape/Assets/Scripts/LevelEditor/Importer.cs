/*
 * Usage: Apply to the main camera, or an empty game object. Game objects will be constructed based on the data file.
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Game
{
    public class Importer : MonoBehaviour
    {
        #region Instance Variables
        private string mLevelPath;

        private DoorFactory mDoorFactory;
        private DynamicWallFactory mDynamicWallFactory;
        private ActivatorFactory mActivatorFactory;
        private ItemFactory mItemFactory;

        private List<IActivatable> mActivatables;

        private List<bool> mImportCoroutines;

        private bool mImportedDoors;
        private bool mImportedDynWalls;
        private bool mImportedActivators;
        private bool mImportedItems;

        private bool mImportedChambers;

        private bool mImportedChunks;
        private bool mImportedStaticWalls;

        private bool mImportedEnvironment;

        private string mImportLevelName;

        private Player mPlayer;
        #endregion

        public void playLevel(string levelPath)
		{
            mPlayer = Player.Get();
            mPlayer.pause();

            initializeFactories();

            mActivatables = new List<IActivatable>();

			if (!Directory.Exists(levelPath))
            {
				Debug.Log("The level " + levelPath + " does not exist.");
                return;
            }

            mImportLevelName = "gamedata.xml";

			if (!File.Exists(levelPath + mImportLevelName))
            {
				Debug.Log(levelPath + mImportLevelName + " does not exist.");
                return;
            }

            XmlDocument document = new XmlDocument();
			document.Load(levelPath + mImportLevelName);

			XmlElement tilesElement;
			XmlElement staticWallElement;
			XmlElement doorsElement;
			XmlElement activatorsElement;

			foreach (XmlNode child in document.ChildNodes)
			{
				XmlElement iterator = child as XmlElement;
				if (iterator == null)
					continue;

				switch (iterator.Name)
				{
					case "tiles":
						tilesElement = iterator;
						break;
					case "staticwalls":
						staticWallElement = iterator;
						break;
					case "doors":
						doorsElement = iterator;
						break;
					case "activators":
						activatorsElement = iterator;
						break;
				}
			}

            mImportedDoors = true;
            mImportedDynWalls = true;
            mImportedActivators = true;
            mImportedItems = true;

            mImportedChambers = false;

            mImportedChunks = true;
            mImportedStaticWalls = true;

            mImportedEnvironment = false;

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

                StartCoroutine(importEnvironment(rootElem.FirstChild));
                StartCoroutine(importChambers(rootElem.FirstChild));
            }
        }

        private IEnumerator importEnvironment(XmlNode environmentNode)
        {
            XmlNode nodeIt = environmentNode;
            XmlElement element;

            while (nodeIt != null)
            {
                element = nodeIt as XmlElement;
                nodeIt = nodeIt.NextSibling;

                if (element == null)
                    continue;

                Debug.Log("environment name: " + element.Name);

                switch (element.Name)
                {
                    case "staticwalls":
                        mImportedStaticWalls = false;
                        StartCoroutine(importStaticWalls(element));
                        break;
                    case "chunks":
                        mImportedEnvironment = false;
                        StartCoroutine(importChunks(element));
                        break;
                }
            }

            while (!finishedEnvironment())
                yield return new WaitForSeconds(.33f);

            mImportedEnvironment = true;

            yield break;
        }

        private bool finishedEnvironment()
        {
            return mImportedChunks && mImportedStaticWalls;
        }

        private IEnumerator importChunks(XmlElement parent)
        {
        	yield break;
            /*if (parent == null || parent.Name != "chunks")
                yield break;

            Debug.Log("Importing chunks...");

            Utilities.Vec2Int chunkSize = new Utilities.Vec2Int(parent.GetAttribute("chunkSize"));
            Vector2 chunkRealSize = Utilities.Serialization.toVector2(parent.GetAttribute("chunkRealSize"));

            XmlNode nodeIt = parent.FirstChild;
            XmlElement chunkNode;

            Utilities.Vec2Int logicalPosition;

            GameObject go;

            while (nodeIt != null)
            {
                chunkNode = nodeIt as XmlElement;
                nodeIt = nodeIt.NextSibling;

                if (chunkNode == null || chunkNode.Name != "chunk")
                    continue;

                logicalPosition = new Utilities.Vec2Int(chunkNode.GetAttribute("logicalPosition"));

                byte[] chunkData = File.ReadAllBytes(mImportDirectory + "Chunks/" + chunkNode.GetAttribute("filename"));
                if (chunkData == null)
                    continue;

                go = new GameObject();
                logicalPosition = new Utilities.Vec2Int(chunkNode.GetAttribute("logicalPosition"));

                Vector3 chunkPos = new Vector3(chunkRealSize.x * logicalPosition.x,
                    chunkRealSize.y * logicalPosition.y,
                    0.0f);
                chunkPos += Utilities.Math.toVector3(chunkRealSize / 2);
                chunkPos += new Vector3(-.1f, -.1f, 0);
                go.transform.position = chunkPos;

                Mesh chunkMesh = Utilities.Graphics.makeQuadMesh(chunkRealSize);

                MeshFilter mf = go.AddComponent<MeshFilter>();
                mf.mesh = chunkMesh;

                MeshRenderer mr = go.AddComponent<MeshRenderer>();
                mr.material.shader = Shader.Find("Self-Illumin/Diffuse");

                Texture2D tex = new Texture2D(chunkSize.x, chunkSize.y);
                tex.LoadImage(chunkData);
                tex.Apply();
                mr.material.mainTexture = tex;

                yield return null;
            }

            Debug.Log("Done importing chunks");

            mImportedChunks = true;*/
        }

        private IEnumerator importStaticWalls(XmlElement parent)
        {
            if (parent == null || parent.Name != "staticwalls")
                yield break;

            GameObject staticWalls = new GameObject();
            staticWalls.transform.position = new Vector3(-.1f, -.1f, 0);

            Debug.Log("Adding the static walls");

            XmlNode nodeIt = parent.FirstChild;
            XmlElement colliderElement;

            while (nodeIt != null)
            {
                colliderElement = nodeIt as XmlElement;
                nodeIt = nodeIt.NextSibling;

                if (colliderElement == null || colliderElement.Name != "boxcollider2d")
                    continue;

                BoxCollider2D collider = staticWalls.AddComponent<BoxCollider2D>();
                Utilities.Serialization.deserialize(collider, colliderElement);
                collider.isTrigger = false;
            }

            mImportedStaticWalls = true;
        }

        private IEnumerator importChambers(XmlNode chamberNode)
        {
            XmlNode chamberIter = chamberNode;
            XmlElement chamberElem;

            GameObject chamber;

            while (chamberIter != null)
            {
                chamberElem = chamberIter as XmlElement;
                chamberIter = chamberIter.NextSibling;

                if (chamberElem == null || chamberElem.Name != "chamber")
                    continue;

                mImportedChambers = false;

                chamber = new GameObject();
                chamber.transform.position =
                    Utilities.Serialization.toVector3(chamberElem.GetAttribute("position"));
                chamber.name = chamberElem.GetAttribute("name") + "_imported";

                XmlNode categoryIter = chamberElem.FirstChild;
                XmlElement categoryElem;

                while (categoryIter != null)
                {
                    categoryElem = categoryIter as XmlElement;
                    categoryIter = categoryIter.NextSibling;

                    if (categoryElem == null)
                        continue;

                    switch (categoryElem.Name)
                    {
                        case "doors":
                            mImportedDoors = false;
                            StartCoroutine(importDoors(categoryElem));
                            break;
                        case "dynamicwalls":
                            mImportedDynWalls = false;
                            StartCoroutine(importDynamicWalls(categoryElem));
                            break;
                        case "activators":
                            mImportedActivators = false;
                            StartCoroutine(importActivators(categoryElem));
                            break;//*/
                        case "items":
                            mImportedItems = false;
                            StartCoroutine(importItems(categoryElem));
                            break;
                    }
                }

                while (!finishedWithChamber())
                    yield return new WaitForSeconds(.33f);
            }

            mImportedChambers = true;

            yield break;
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
                yield return new WaitForSeconds(.1f);
            }

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

            mImportedActivators = true;
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

            mImportedItems = true;
        }

        private bool finishedWithChamber()
        {
            return mImportedDoors && mImportedDynWalls && mImportedActivators && mImportedItems;
        }

        private IEnumerator waitForReady()
        {
            while (!(mImportedEnvironment && mImportedChambers))
            {
                Debug.Log("Waiting...");
                yield return new WaitForSeconds(.33f);
            }

            LevelManager.Ready();
            mPlayer.unpause();
        }
    }
}
