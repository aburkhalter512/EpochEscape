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

    CheckpointDoorFrame mCheckpoint = null;
    Player mCheckpointPlayer = null;

    EntranceDoorFrame mEntranceDoor;
    ExitDoorFrame mExitDoor;
    #endregion

    #region Class Constants
    #endregion

    //Put all initialization code here
    //Remember to comment!
    protected void Awake()
    {
        coresFound = 0;

        if(G.getInstance().currentLevel == 1)
            InstantiateAllPlayers();
        else
            InstantiateCurrentPlayer();

        InstantiateSpecialItems();

        GameObject hudManager = new GameObject();
        hudManager.AddComponent<HUDManager>();
        hudManager.name = "HUDManager";

        //GameObject.DontDestroyOnLoad(this.gameObject);
    }

    protected void Start()
    {
        mEntranceDoor = entranceDoor.GetComponent<EntranceDoorFrame>();
        mExitDoor = exitDoor.GetComponent<ExitDoorFrame>();
        mExitDoor.attachLevelManager(this);
    }

    #region Interface Methods
    public void registerCheckpoint(CheckpointDoorFrame checkpoint, Player player)
    {
        if (checkpoint == null || player == null)
            return;

        mCheckpoint = checkpoint;
        mCheckpointPlayer = player;

        return;
    }

    public void loadCheckpoint()
    {
        Application.LoadLevel(Application.loadedLevelName);

        //mCheckpointPlayer.transform.position = mCheckpoint.respawnLocation.transform.position;

        GameManager.getInstance().UnpauseMovement();
    }

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

        Destroy(this.gameObject);
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

                if (playerScript != null)
                {
                    playerScript.m_spawnLocation = caveGirlSpawn.transform.position;
                    playerScript.levelManager = this;

                    registerCheckpoint(mEntranceDoor, playerScript);
                }

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

                if (playerScript != null)
                {
                    playerScript.m_spawnLocation = knightSpawn.transform.position;
                    playerScript.levelManager = this;

                    registerCheckpoint(mEntranceDoor, playerScript);
                }
                
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
        GameObject GO = null;
        Player player = null;

        // Create the character.
        switch(G.getInstance().m_currentCharacter)
        {
        case G.KNIGHT:
            GO = Resources.Load("Prefabs/Players/Knight") as GameObject;
            break;
            
        case G.CAVEGIRL:
        default:
            GO = Resources.Load("Prefabs/Players/CaveGirl") as GameObject;
            break;
        }

        if (GO != null)
        {
            player = GO.GetComponent<Player>();

            if (player != null)
            {
                if (mEntranceDoor == null)
                    Debug.Log("spawnLocation is null");
                player.m_spawnLocation = mEntranceDoor.respawnLocation.transform.position;
                player.levelManager = this;
                GO.transform.position = mEntranceDoor.respawnLocation.transform.position;

                Instantiate(GO);

                registerCheckpoint(mEntranceDoor, player);
            }
        }
    }

    private void InstantiateSpecialItems()
    {
        GameObject GO = null;

        switch(G.getInstance().m_currentCharacter)
        {
        case G.KNIGHT:
                GO = Resources.Load("Prefabs/Items/SpecialItems/Shield") as GameObject;
            break;
            
        case G.CAVEGIRL:
        default:
            GO = Resources.Load("Prefabs/Items/SpecialItems/Club") as GameObject;
            break;
        }

        if (GO != null)
        {
            GameObject[] specialItemSpawnLocations = GameObject.FindGameObjectsWithTag("SpecialItemSpawn");
            
            for(int i = 0; i < specialItemSpawnLocations.Length; i++)
            {
                GameObject newSpecialItem = Instantiate(GO) as GameObject;
                
                newSpecialItem.transform.position = specialItemSpawnLocations[i].transform.position;
            }
        }
    }
    #endregion
}
