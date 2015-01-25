using UnityEngine;
using System.Collections;
using G = GameManager;

public class SaveManager : UnitySingleton<SaveManager>
{
    #region Inspector Variables
    #endregion

    #region Instance Variables
    public static int saveNum = 0;
    public static string save1 = "";
    public static string save2 = "";
    public static string save3 = "";
    public static string save4 = "";
    #endregion

    #region Class Constants
    #endregion

    //Put all initialization code here
    //Remember to comment!
//	protected void Start()
//	{
//	}

    #region Initialization Methods
    #endregion

    //Put all update code here
    //Remember to comment!
//	protected void Update()
//	{
//	}

    #region Static Methods
        public static void SetSaveName (string name){
        switch(saveNum){
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

    public static string GetSaveName(){
        if(saveNum == 0)
            return save1;
        else if(saveNum == 1)
            return save2;
        else if(saveNum == 2)
            return save3;
        else
            return save4;
    }

    public static bool SaveTaken(){
        if(saveNum == 0)
            return PlayerPrefs.HasKey ("0Current Level");
        else if(saveNum == 1)
            return PlayerPrefs.HasKey ("1Current Level");
        else if(saveNum == 2)
            return PlayerPrefs.HasKey ("2Current Level");
        else
            return PlayerPrefs.HasKey ("3Current Level");
    }

    public static void ResetOnNew(){
        G.getInstance().currentLevel = 1;
        Save ();
    }

    public static void ChooseSave(int i){
        saveNum = i;
    }

    public static void ResetGame(){
        PlayerPrefs.DeleteAll ();
        Load ();
        G.getInstance().caveUnlocked = true;
        G.getInstance().knightUnlocked = true;
    }

    public static void DeleteSave(){
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
            PlayerPrefs.SetInt("0Current Level", G.getInstance().currentLevel);
            PlayerPrefs.SetInt ("0Current Char", G.getInstance ().m_currentCharacter);
            //PlayerPrefs.SetInt ("0tutorial", G.getInstance ().tutorial == true ? 1 : 0);
            break;
        case 1:
            PlayerPrefs.SetInt("1Current Level", G.getInstance().currentLevel);	
            PlayerPrefs.SetInt ("1Current Char", G.getInstance ().m_currentCharacter);
            //PlayerPrefs.SetInt ("1tutorial", G.getInstance ().tutorial == true ? 1 : 0);
            break;
        case 2:
            PlayerPrefs.SetInt("2Current Level", G.getInstance().currentLevel);
            PlayerPrefs.SetInt ("2Current Char", G.getInstance ().m_currentCharacter);
            //PlayerPrefs.SetInt ("2tutorial", G.getInstance ().tutorial == true ? 1 : 0);
            break;
        case 3:
            PlayerPrefs.SetInt("3Current Level", G.getInstance().currentLevel);
            PlayerPrefs.SetInt ("3Current Char", G.getInstance ().m_currentCharacter);
            //PlayerPrefs.SetInt ("3tutorial", G.getInstance ().tutorial == true ? 1 : 0);
            break;
        }

        PlayerPrefs.SetInt ("CaveGirl", G.getInstance().caveUnlocked == true ? 1 : 0);
        PlayerPrefs.SetInt ("Knight", G.getInstance().knightUnlocked == true ? 1 : 0);
        PlayerPrefs.SetInt ("Ninja", G.getInstance().ninjaUnlocked == true ? 1 : 0);
        PlayerPrefs.SetInt ("Astronaut", G.getInstance().astroUnlocked == true ? 1 : 0);
        PlayerPrefs.SetInt ("Mummy", G.getInstance().mumUnlocked == true ? 1 : 0);
        PlayerPrefs.SetInt ("Robot", G.getInstance().robUnlocked == true ? 1 : 0);
        PlayerPrefs.SetInt ("Ninja Memo", G.getInstance().ninjaMemo);
        PlayerPrefs.SetInt ("Astro Memo", G.getInstance().astroMemo);
        PlayerPrefs.SetInt ("Mummy Memo", G.getInstance().mumMemo);
        PlayerPrefs.SetInt ("Robot Memo", G.getInstance().robMemo);
    }

    public static void LoadGame(){
        switch(saveNum){
        case 0:
            G.getInstance().currentLevel = PlayerPrefs.GetInt("0Current Level");
            G.getInstance ().m_currentCharacter = PlayerPrefs.GetInt ("0Current Char");
            //G.getInstance ().tutorial = PlayerPrefs.GetInt ("0tutorial") == 1 ? true : false;
            break;
        case 1:
            G.getInstance().currentLevel = PlayerPrefs.GetInt("1Current Level");
            G.getInstance ().m_currentCharacter = PlayerPrefs.GetInt ("1Current Char");
            //G.getInstance ().tutorial = PlayerPrefs.GetInt ("1tutorial") == 1 ? true : false;
            break;
        case 2:
            G.getInstance().currentLevel = PlayerPrefs.GetInt("2Current Level");
            G.getInstance ().m_currentCharacter = PlayerPrefs.GetInt ("2Current Char");
            //G.getInstance ().tutorial = PlayerPrefs.GetInt ("2tutorial") == 1 ? true : false;
            break;
        case 3:
            G.getInstance().currentLevel = PlayerPrefs.GetInt("3Current Level");
            G.getInstance ().m_currentCharacter = PlayerPrefs.GetInt ("3Current Char");
            //G.getInstance ().tutorial = PlayerPrefs.GetInt ("3tutorial") == 1 ? true : false;
            break;
        }

    }

    public static void Load(){
        G.getInstance().caveUnlocked = PlayerPrefs.GetInt ("CaveGirl") == 1 ? true : false;
        G.getInstance().knightUnlocked = PlayerPrefs.GetInt ("Knight") == 1 ? true : false;
        G.getInstance().ninjaUnlocked = PlayerPrefs.GetInt ("Ninja") == 1 ? true : false;
        G.getInstance().mumUnlocked = PlayerPrefs.GetInt ("Mummy") == 1 ? true : false;
        G.getInstance().robUnlocked = PlayerPrefs.GetInt ("Robot") == 1 ? true : false;
        G.getInstance().ninjaMemo = PlayerPrefs.GetInt ("Ninja Memo");
        G.getInstance().astroMemo = PlayerPrefs.GetInt ("Astro Memo");
        G.getInstance().mumMemo = PlayerPrefs.GetInt ("Mummy Memo");
        G.getInstance().robMemo = PlayerPrefs.GetInt ("Robot Memo");

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

    #region Utilities
    #endregion
}
