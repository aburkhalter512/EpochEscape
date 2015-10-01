using UnityEngine;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace MapEditor
{
    public class SaveManager
    {
        #region Interface Variables
        public string levelName = "";
        #endregion

        #region Instance Variables
        public static int saveNum = 0;
        public static string save1 = "";
        public static string save2 = "";
        public static string save3 = "";
        public static string save4 = "";

        private MapEditor mME = null;
        private InputManager IM = null;
        private ChunkManager mCM = null;
        private StaticWallManager mSWM = null;
        private DoorManager mDM = null;
        private ActivatorManager mAM = null;
        private CoroutineManager mCoM;

        private readonly string mOutputFolder = "Levels";

        private bool mSerializedMapEditor = false;
        private bool mHasChunked = false;
        private bool mSerializedStaticWalls = false;
        private bool mSerializedDoors = false;
        private bool mSerializedActivators = false;
        private bool mIsSaving = false;

        protected static SaveManager _instance;
        #endregion


        protected SaveManager()
        {
            IM = InputManager.Get();
            mCM = ChunkManager.Get();
            mSWM = StaticWallManager.Get();
            mDM = DoorManager.Get();
            mAM = ActivatorManager.Get();
            mCoM = CoroutineManager.Get();
        }

        #region Interface Methods
        public static SaveManager Get()
        {
            if (_instance == null)
                _instance = new SaveManager();

            return _instance;
        }

        public void saveLevel(string levelName)
        {
            if (mIsSaving)
                return;

            Debug.Log("Saving...");

            string levelDirectory = mOutputFolder + "/" + levelName;

            mIsSaving = true;
            mHasChunked = false;
            mSerializedStaticWalls = false;
            mSerializedDoors = false;
            mSerializedActivators = false;

            if (!Directory.Exists(mOutputFolder))
                Directory.CreateDirectory(mOutputFolder);

            if (!Directory.Exists(levelDirectory))
                Directory.CreateDirectory(levelDirectory);

            XmlDocument doc = new XmlDocument();

            XmlElement levelNode = doc.CreateElement("level");
            levelNode.SetAttribute("levelName", levelName);
            levelNode.SetAttribute("objectCount", "" + 0); // Easy int to string conversion

            mCoM.StartCoroutine(mME.serialize(doc, (element) =>
            {
                this.mSerializedMapEditor = true;
                levelNode.AppendChild(element);
            }));

            mCM.exportDir(levelDirectory);
            mCoM.StartCoroutine(mCM.serialize(doc, (XmlElement element) =>
            {
                this.mHasChunked = true;
                levelNode.AppendChild(element);
            }));
            mCoM.StartCoroutine(mSWM.serialize(doc, (XmlElement element) =>
            {
                this.mSerializedStaticWalls = true;
                levelNode.AppendChild(element);
            }));
            mCoM.StartCoroutine(mDM.serialize(doc, (XmlElement element) =>
            {
                this.mSerializedDoors = true;
                levelNode.AppendChild(element);
            }));
            mCoM.StartCoroutine(mAM.serialize(doc, (XmlElement element) =>
            {
                this.mSerializedActivators = true;
                levelNode.AppendChild(element);
            }));
            mCoM.StartCoroutine(saveLevelHelper(doc, levelDirectory, levelName));
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

            mIsSaving = false;

            Debug.Log("Saved");
        }
        #endregion
    }
}
