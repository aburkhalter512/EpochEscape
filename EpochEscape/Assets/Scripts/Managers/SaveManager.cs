using UnityEngine;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Xml;

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

    private InputManager IM = null;
    private ChunkManager mCM = null;
    private StaticWallManager mSWM = null;
    private DoorManager mDM = null;
    private ActivatorManager mAM = null;
    private GameManager mGM;
    private CoroutineManager mCoM;

    private readonly string mOutputFolder = "Levels";

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

        doc.AppendChild(levelNode);
        XmlElement chamber = doc.CreateElement("chamber");
        chamber.SetAttribute("name", "chamber1");
        chamber.SetAttribute("position", Vector3.zero.ToString());

        levelNode.AppendChild(chamber);

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
            chamber.AppendChild(element);
        }));
        mCoM.StartCoroutine(mAM.serialize(doc, (XmlElement element) =>
        {
            this.mSerializedActivators = true;
            chamber.AppendChild(element);
        }));
        mCoM.StartCoroutine(saveLevelHelper(doc, levelDirectory, levelName));
    }
    #endregion

    #region Instace Methods
    private IEnumerator saveLevelHelper(XmlDocument doc, string levelDirectory, string levelName)
    {
        while (!mHasChunked || !mSerializedStaticWalls || !mSerializedDoors || !mSerializedActivators)
            yield return new WaitForSeconds(.1f);

        doc.Save(levelDirectory + "/gamedata.xml");

        mIsSaving = false;

        Debug.Log("Saved");
    }
    #endregion

    #region Static Methods
    // OLD DEPRECATED SAVE INFORMATION
	public static void SetSaveName (string name)
	{
        switch(saveNum)
		{
			case 0:
				save1 = name;
				PlayerPrefs.SetString ("Save1", save1);
				break;
			case 1:
				save2 = name;
				PlayerPrefs.SetString ("Save2", save2);
				break;
			case 2:
				save3 = name;
				PlayerPrefs.SetString ("Save3", save3);
				break;
			case 3:
				save4 = name;
				PlayerPrefs.SetString ("Save4", save4);
				break;
        }
    }

    public static string GetSaveName()
	{
        if(saveNum == 0)
            return save1;
        else if(saveNum == 1)
            return save2;
        else if(saveNum == 2)
            return save3;
        else
            return save4;
    }

    public static bool SaveTaken()
	{
        if(saveNum == 0)
            return PlayerPrefs.HasKey ("0Current Level");
        else if(saveNum == 1)
            return PlayerPrefs.HasKey ("1Current Level");
        else if(saveNum == 2)
            return PlayerPrefs.HasKey ("2Current Level");
        else
            return PlayerPrefs.HasKey ("3Current Level");
    }

    public static void ResetOnNew()
	{
        GameManager.Get().currentLevel = 1;
        Save ();
    }

    public static void ChooseSave(int i){
        saveNum = i;
    }

    public static void ResetGame(){
        PlayerPrefs.DeleteAll ();
        Load ();
        GameManager.Get().caveUnlocked = true;
        GameManager.Get().knightUnlocked = true;
    }

    public static void DeleteSave()
	{
        switch(saveNum){
        case 0:
            PlayerPrefs.DeleteKey ("0Current Level");
            PlayerPrefs.DeleteKey ("0New Game");
            PlayerPrefs.DeleteKey ("Save1");
            break;
        case 1:
            PlayerPrefs.DeleteKey ("1Current Level");
            PlayerPrefs.DeleteKey ("1New Game");
            PlayerPrefs.DeleteKey ("Save2");
            break;
        case 2:
            PlayerPrefs.DeleteKey ("2Current Level");
            PlayerPrefs.DeleteKey ("2New Game");
            PlayerPrefs.DeleteKey ("Save3");
            break;
        case 3:
            PlayerPrefs.DeleteKey ("3Current Level");
            PlayerPrefs.DeleteKey ("3New Game");
            PlayerPrefs.DeleteKey ("Save4");
            break;
        }
        Load ();
    }

    public static void Save(){
        switch(saveNum){
        case 0:
                PlayerPrefs.SetInt("0Current Level", GameManager.Get().currentLevel);
                PlayerPrefs.SetInt("0Current Char", GameManager.Get().m_currentCharacter);
            //PlayerPrefs.SetInt ("0tutorial", GameManager.gGet ().tutorial == true ? 1 : 0);
            break;
        case 1:
            PlayerPrefs.SetInt("1Current Level", GameManager.Get().currentLevel);
            PlayerPrefs.SetInt("1Current Char", GameManager.Get().m_currentCharacter);
            //PlayerPrefs.SetInt ("1tutorial", GameManager.Get ().tutorial == true ? 1 : 0);
            break;
        case 2:
            PlayerPrefs.SetInt("2Current Level", GameManager.Get().currentLevel);
            PlayerPrefs.SetInt("2Current Char", GameManager.Get().m_currentCharacter);
            //PlayerPrefs.SetInt ("2tutorial", GameManager.Get ().tutorial == true ? 1 : 0);
            break;
        case 3:
            PlayerPrefs.SetInt("3Current Level", GameManager.Get().currentLevel);
            PlayerPrefs.SetInt("3Current Char", GameManager.Get().m_currentCharacter);
            //PlayerPrefs.SetInt ("3tutorial", GameManager.Get ().tutorial == true ? 1 : 0);
            break;
        }

        PlayerPrefs.SetInt("CaveGirl", GameManager.Get().caveUnlocked == true ? 1 : 0);
        PlayerPrefs.SetInt("Knight", GameManager.Get().knightUnlocked == true ? 1 : 0);
        PlayerPrefs.SetInt("Ninja", GameManager.Get().ninjaUnlocked == true ? 1 : 0);
        PlayerPrefs.SetInt("Astronaut", GameManager.Get().astroUnlocked == true ? 1 : 0);
        PlayerPrefs.SetInt("Mummy", GameManager.Get().mumUnlocked == true ? 1 : 0);
        PlayerPrefs.SetInt("Robot", GameManager.Get().robUnlocked == true ? 1 : 0);
        PlayerPrefs.SetInt("Ninja Memo", GameManager.Get().ninjaMemo);
        PlayerPrefs.SetInt("Astro Memo", GameManager.Get().astroMemo);
        PlayerPrefs.SetInt("Mummy Memo", GameManager.Get().mumMemo);
        PlayerPrefs.SetInt("Robot Memo", GameManager.Get().robMemo);
    }

    public static void LoadGame(){
        switch(saveNum){
        case 0:
            GameManager.Get().currentLevel = PlayerPrefs.GetInt("0Current Level");
            GameManager.Get ().m_currentCharacter = PlayerPrefs.GetInt ("0Current Char");
            //GameManager.Get ().tutorial = PlayerPrefs.GetInt ("0tutorial") == 1 ? true : false;
            break;
        case 1:
            GameManager.Get().currentLevel = PlayerPrefs.GetInt("1Current Level");
            GameManager.Get ().m_currentCharacter = PlayerPrefs.GetInt ("1Current Char");
            //GameManager.Get ().tutorial = PlayerPrefs.GetInt ("1tutorial") == 1 ? true : false;
            break;
        case 2:
            GameManager.Get().currentLevel = PlayerPrefs.GetInt("2Current Level");
            GameManager.Get ().m_currentCharacter = PlayerPrefs.GetInt ("2Current Char");
            //GameManager.Get ().tutorial = PlayerPrefs.GetInt ("2tutorial") == 1 ? true : false;
            break;
        case 3:
            GameManager.Get().currentLevel = PlayerPrefs.GetInt("3Current Level");
            GameManager.Get ().m_currentCharacter = PlayerPrefs.GetInt ("3Current Char");
            //GameManager.Get ().tutorial = PlayerPrefs.GetInt ("3tutorial") == 1 ? true : false;
            break;
        }

    }

    public static void Load(){
        GameManager.Get().caveUnlocked = PlayerPrefs.GetInt ("CaveGirl") == 1 ? true : false;
        GameManager.Get().knightUnlocked = PlayerPrefs.GetInt ("Knight") == 1 ? true : false;
        GameManager.Get().ninjaUnlocked = PlayerPrefs.GetInt ("Ninja") == 1 ? true : false;
        GameManager.Get().mumUnlocked = PlayerPrefs.GetInt ("Mummy") == 1 ? true : false;
        GameManager.Get().robUnlocked = PlayerPrefs.GetInt ("Robot") == 1 ? true : false;
        GameManager.Get().ninjaMemo = PlayerPrefs.GetInt ("Ninja Memo");
        GameManager.Get().astroMemo = PlayerPrefs.GetInt ("Astro Memo");
        GameManager.Get().mumMemo = PlayerPrefs.GetInt ("Mummy Memo");
        GameManager.Get().robMemo = PlayerPrefs.GetInt ("Robot Memo");

        save1 = PlayerPrefs.GetString ("Save1");
        if(save1 == "")
            save1 = "Empty";
        save2 = PlayerPrefs.GetString ("Save2");
        if(save2 == "")
            save2 = "Empty";
        save3 = PlayerPrefs.GetString ("Save3");
        if(save3 == "")
            save3 = "Empty";
        save4 = PlayerPrefs.GetString ("Save4");
        if(save4 == "")
            save4 = "Empty";
        
    }
    #endregion
}
