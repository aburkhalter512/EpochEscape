using UnityEngine;
using System.Collections;
using G = GameManager;

public class LevelManager : MonoBehaviour
{
    #region Inspector Variables
    public enum Character
    {
        CAVE_GIRL,
        KNIGHT
    }

    public int coresToFind = 3;
    public int coresFound;

    public GameObject entranceDoor;
    public GameObject exitDoor;

    public string nextLevel;
    public 
    #endregion

    #region Instance Variables
    ArrayList tiles = null;

    GameObject m_player;
    GameObject m_playerSpecialItem;

    EntranceDoorFrame mEntranceDoor;
    ExitDoorFrame mExitDoor;
    #endregion

    #region Class Constants
    #endregion

    //Put all initialization code here
    //Remember to comment!
    protected void Start()
    {
        mEntranceDoor = entranceDoor.GetComponent<EntranceDoorFrame>();
        mExitDoor = exitDoor.GetComponent<ExitDoorFrame>();
        mExitDoor.attachLevelManager(this);

        coresFound = 0;

        if(G.getInstance().currentLevel == 1)
            InstantiateAllPlayers();
        else
            InstantiateCurrentPlayer();

        InstantiateSpecialItems();

        GameObject hudManager = new GameObject();
        hudManager.AddComponent<HUDManager>();
        hudManager.name = "HUDManager";
    }

    #region Interface Methods
    public void exitLevel()
    {
        //Set the correct current level
        if (nextLevel == "Level1")
            GameManager.getInstance().currentLevel = 1;
        else
            GameManager.getInstance().currentLevel++;

        //Load the next level
        /*if (isWinning)
            SceneManager.Win(Level);
        else//*/
        SceneManager.Load(nextLevel);

        //Save all progress
        SaveManager.Save();
    }
    #endregion

    #region Instance Methods
    private void InstantiateAllPlayers()
    {
        // Lasers
        GameObject brokenLaser = Resources.Load("Prefabs/Obstacles/JailBarBrokenLaser") as GameObject;
        GameObject staticLaser = Resources.Load("Prefabs/Obstacles/JailBarLaser") as GameObject;

        Player playerScript = null;

        // Cave Girl
        GameObject caveGirl = Resources.Load("Prefabs/Players/CaveGirl") as GameObject;
        GameObject caveGirlSpawn = GameObject.FindWithTag("CaveGirlSpawn");
        GameObject caveGirlLaserSpawn = GameObject.FindWithTag("CaveGirlLaserSpawn");

        if(caveGirl != null && caveGirlSpawn != null)
        {
            caveGirl = Instantiate(caveGirl) as GameObject;

            playerScript = caveGirl.GetComponent<Player>();
            
            if(G.getInstance().m_currentCharacter == G.CAVEGIRL)
            {
                // Set the player spawn point
                caveGirl.tag = "Player";
                
                if(playerScript != null)
                    playerScript.m_spawnLocation = caveGirlSpawn.transform.position;

                // Create the broken laser
                if(caveGirlLaserSpawn != null)
                {
                    brokenLaser = Instantiate(brokenLaser) as GameObject;
                    brokenLaser.transform.parent = GameObject.Find("JailBarLasers").transform;
                    brokenLaser.transform.position = caveGirlLaserSpawn.transform.position;
                }
            }
            else
            {
                // Set the player up as a dummy
                caveGirl.tag = "Dummy";

                if(playerScript != null)
                    playerScript.enabled = false;

                // Create the static laser
                if(caveGirlLaserSpawn != null)
                {
                    staticLaser = Instantiate(staticLaser) as GameObject;
                    staticLaser.transform.parent = GameObject.Find("JailBarLasers").transform;
                    staticLaser.transform.position = caveGirlLaserSpawn.transform.position;
                }
            }

            caveGirl.transform.position = caveGirlSpawn.transform.position;
        }

        // Knight
        GameObject knight = Resources.Load("Prefabs/Players/Knight") as GameObject;
        GameObject knightSpawn = GameObject.FindWithTag("KnightSpawn");
        GameObject knightLaserSpawn = GameObject.FindWithTag("KnightLaserSpawn");
        
        if(knight != null && knightSpawn != null)
        {
            knight = Instantiate(knight) as GameObject;
            
            playerScript = knight.GetComponent<Player>();
            
            if(G.getInstance().m_currentCharacter == G.KNIGHT)
            {
                // Set the player spawn point
                knight.tag = "Player";
                
                if(playerScript != null)
                    playerScript.m_spawnLocation = knightSpawn.transform.position;
                
                // Create the broken laser
                if(knightLaserSpawn != null)
                {
                    brokenLaser = Instantiate(brokenLaser) as GameObject;
                    brokenLaser.transform.parent = GameObject.Find("JailBarLasers").transform;
                    brokenLaser.transform.position = knightLaserSpawn.transform.position;
                }
            }
            else
            {
                // Set the player up as a dummy
                knight.tag = "Dummy";
                
                if(playerScript != null)
                    playerScript.enabled = false;
                
                // Create the static laser
                if(knightLaserSpawn != null)
                {
                    staticLaser = Instantiate(staticLaser) as GameObject;
                    staticLaser.transform.parent = GameObject.Find("JailBarLasers").transform;
                    staticLaser.transform.position = knightLaserSpawn.transform.position;
                }
            }
            
            knight.transform.position = knightSpawn.transform.position;
        }
    }

    private void InstantiateCurrentPlayer()
    {
        // Create the character.
        switch(G.getInstance().m_currentCharacter)
        {
        case G.KNIGHT:
            m_player = Resources.Load("Prefabs/Players/Knight") as GameObject;
            break;
            
        case G.CAVEGIRL:
        default:
            m_player = Resources.Load("Prefabs/Players/CaveGirl") as GameObject;
            break;
        }
        
        if(m_player != null)
        {
            Player playerScript = m_player.GetComponent<Player>();
            
            if(playerScript != null)
            {
                if (mEntranceDoor == null)
                    Debug.Log("spawnLocation is null");
                playerScript.m_spawnLocation = mEntranceDoor.respawnLocation.transform.position;
                m_player.transform.position = mEntranceDoor.respawnLocation.transform.position;
                
                Instantiate(m_player);
            }
        }
    }

    private void InstantiateSpecialItems()
    {
        switch(G.getInstance().m_currentCharacter)
        {
        case G.KNIGHT:
            m_playerSpecialItem = Resources.Load("Prefabs/Items/SpecialItems/Shield") as GameObject;
            break;
            
        case G.CAVEGIRL:
        default:
            m_playerSpecialItem = Resources.Load("Prefabs/Items/SpecialItems/Club") as GameObject;
            break;
        }

        if(m_playerSpecialItem != null)
        {
            GameObject[] specialItemSpawnLocations = GameObject.FindGameObjectsWithTag("SpecialItemSpawn");
            
            for(int i = 0; i < specialItemSpawnLocations.Length; i++)
            {
                GameObject newSpecialItem = Instantiate(m_playerSpecialItem) as GameObject;
                
                newSpecialItem.transform.position = specialItemSpawnLocations[i].transform.position;
            }
        }
    }
    #endregion
}
