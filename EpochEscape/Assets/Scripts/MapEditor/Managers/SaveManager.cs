using UnityEngine;

using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Xml;

using Utilities;

namespace MapEditor
{
    public class SaveManager
    {
        #region Interface Variables
        public string levelName = "";
        #endregion

        #region Instance Variables

        private bool mSerializedMapEditor = false;
        private bool mHasChunked = false;
        private bool mSerializedStaticWalls = false;
        private bool mSerializedDoors = false;
        private bool mSerializedActivators = false;

        protected static SaveManager _instance;
        #endregion


        protected SaveManager()
        { }

        #region Interface Methods
        public void destroy()
        {
        	_instance = null;
        }

        public static SaveManager Get()
        {
            if (_instance == null)
                _instance = new SaveManager();

            return _instance;
        }

        public void saveLevel(string levelPath)
        {
            Debug.Log("Saving...");

            mHasChunked = false;
            mSerializedStaticWalls = false;
            mSerializedDoors = false;
            mSerializedActivators = false;

			if (!Directory.Exists(levelPath))
				Directory.CreateDirectory(levelPath);

            XmlDocument doc = new XmlDocument();

            XmlElement levelNode = doc.CreateElement("level");
            doc.AppendChild(levelNode);

            levelNode.SetAttribute("levelName", levelName);
            levelNode.SetAttribute("objectCount", "" + 0); // Easy int to string conversion

            CoroutineManager.Get().StartCoroutine(MapEditor.Get().serialize(doc, (element) =>
            {
                this.mSerializedMapEditor = true;
                levelNode.AppendChild(element);
            }));

			ChunkManager.Get().exportDir(levelPath);
            CoroutineManager.Get().StartCoroutine(ChunkManager.Get().serialize(doc, (element) =>
            {
                this.mHasChunked = true;
                levelNode.AppendChild(element);
            }));
            CoroutineManager.Get().StartCoroutine(StaticWallManager.Get().serialize(doc, (element) =>
            {
                this.mSerializedStaticWalls = true;
                levelNode.AppendChild(element);
            }));
            CoroutineManager.Get().StartCoroutine(DoorManager.Get().serialize(doc, (element) =>
            {
                this.mSerializedDoors = true;
                levelNode.AppendChild(element);
            }));
            CoroutineManager.Get().StartCoroutine(ActivatorManager.Get().serialize(doc, (element) =>
            {
                this.mSerializedActivators = true;
                levelNode.AppendChild(element);
            }));
			CoroutineManager.Get().StartCoroutine(saveLevelHelper(doc, levelPath, levelName));
        }

        public void loadLevel(string levelPath)
        {
			CoroutineManager.Get().StartCoroutine(loadLevelHelper(levelPath));
        }
        #endregion

        #region Instace Methods
        private IEnumerator saveLevelHelper(XmlDocument doc, string levelDirectory, string levelName)
        {
            while (!mSerializedMapEditor || 
                !mHasChunked || 
                !mSerializedStaticWalls || 
                !mSerializedDoors || 
                !mSerializedActivators)
                yield return new WaitForSeconds(.1f);

            doc.Save(levelDirectory + "/gamedata.xml");

            Debug.Log("Saved");
        }

        private IEnumerator loadLevelHelper(string levelPath)
        {
            Debug.Log("Loading...");

            MapEditor.Get().destroy();

            yield return null; // Wait for managers to be destroyed

            Map map = MapEditor.Get().map(); // Creates the new managers

            yield return null; // Wait for all managers Start() methods to be called

			if (!Directory.Exists(levelPath))
				yield break;

            XmlDocument doc = new XmlDocument();
            doc.Load(levelPath + "/gamedata.xml");

            XmlElement root = doc.FirstChild as XmlElement;

			XmlElement tilesElement = null;
			XmlElement staticWallsElement = null;
			XmlElement doorsElement = null;
			XmlElement activatorsElement = null;

			foreach (XmlNode child in root)
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
						staticWallsElement = iterator;
						break;
					case "doors":
						doorsElement = iterator;
						break;
					case "activators":
						activatorsElement = iterator;
						break;
				}
			}

            // Create Tiles
            foreach (XmlNode _tile in tilesElement.ChildNodes)
            {
            	XmlElement tileElement = _tile as XmlElement;
				if (tileElement == null)
            		continue;

				string stringPosition = tileElement.GetAttribute("position");
				Vec2Int tilePos = new Vec2Int(stringPosition);

            	Tile tile = map.getTile(tilePos);
            	QuadNodeProcessors.createTile(tile.node(), tile.position());
            }

			// Create Static Walls
			StaticWallFactory staticWallFactory = new StaticWallFactory();
            if (staticWallsElement != null)
            {
	            foreach (XmlNode _staticWallElem in staticWallsElement.ChildNodes)
	            {
					XmlElement staticWallElement = _staticWallElem as XmlElement;
					if (staticWallElement == null)
	            		continue;

					staticWallFactory.create(staticWallElement);
	            }
            }

			// Create Static Walls to Place Doors Over
			if (doorsElement != null)
			{
	            foreach (XmlNode _doorElem in doorsElement.ChildNodes)
	            {
					XmlElement doorElement = _doorElem as XmlElement;
					if (doorElement == null)
	            		continue;

					staticWallFactory.create(doorElement);
	            }
	        }

			// Create Doors
			DoorFactory doorFactory = new DoorFactory();
			if (doorsElement != null)
			{
	            foreach (XmlNode _doorElem in doorsElement.ChildNodes)
	            {
					XmlElement doorElement = _doorElem as XmlElement;
					if (doorElement == null)
	            		continue;

	            	doorFactory.create(doorElement);
	            }
	        }

            // Create Activators
            ActivatorFactory activatorFactory = new ActivatorFactory(doorFactory);
            if (activatorsElement != null)
            {
				foreach (XmlNode _activatorElem in activatorsElement.ChildNodes)
	            {
					XmlElement activatorElement = _activatorElem as XmlElement;
					if (activatorElement == null)
	            		continue;

	            	activatorFactory.create(activatorElement);
	            }
	        }

			ObjectManipulator.Get().activate();
        }
        #endregion
    }
}
