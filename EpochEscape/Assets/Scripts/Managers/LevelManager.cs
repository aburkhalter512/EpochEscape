using UnityEngine;
using System.Collections;
using G = GameManager;

public class LevelManager : Manager<LevelManager>
{
    #region Inspector Variables
    public GameObject entranceDoor;
    public GameObject exitDoor;
    public string nextLevel;
    #endregion

    #region Instance Variables
    CheckpointDoorFrame mCheckpoint = null;

    [SerializeField]
    int number = 0;

    EntranceDoorFrame mEntranceDoor;
    ExitDoorFrame mExitDoor;
    #endregion

    #region Class Constants
    #endregion

    /*
    public override void Awake()
    {
        base.Awake();
        findLevelDoors();

        //PlacePlayerAtCheckpoint();

        Debug.Log(number);
    }*/

    protected override void Initialize()
    {
        findLevelDoors();
        Debug.Log("LevelManager initialized.");
    }

    protected void Start()
    {
        if (mCheckpoint == null)
            PlayerManager.SetSpawnPosition(mEntranceDoor.respawnLocation.transform.position);
        else
            PlayerManager.SetSpawnPosition(mCheckpoint.respawnLocation.transform.position);

        PlayerManager.Respawn();
    }

    #region Interface Methods
    /*
    public void registerCheckpoint(CheckpointDoorFrame checkpoint, Player player)
    {
        if (checkpoint == null || player == null)
            return;

        mCheckpoint = checkpoint;
        //mCheckpointPlayer = player;

        return;
    }*/

    public void loadCheckpoint()
    {
        Debug.Log("Loading checkpoint");

        Application.LoadLevel(Application.loadedLevelName);

        //mCheckpointPlayer.transform.position = mCheckpoint.respawnLocation.transform.position;
        //mCheckpointPlayer.m_spawnLocation = mCheckpoint.respawnLocation.transform.position;

        findLevelDoors();

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

    public void findLevelDoors()
    {
        mEntranceDoor = GameObject.Find("EntranceDoorCombo").GetComponent<EntranceDoorFrame>();
        mExitDoor = GameObject.Find("ExitDoorCombo").GetComponent<ExitDoorFrame>();
        mExitDoor.attachLevelManager(this);
    }
    #endregion

    #region Instance Methods
    private void PlacePlayerAtCheckpoint()
    {
        //mInstantiatedPlayer.m_spawnLocation = mEntranceDoor.respawnLocation.transform.position;
        //mInstantiatedPlayer.levelManager = this;
        //mInstantiatedPlayer.gameObject.transform.position = mEntranceDoor.respawnLocation.transform.position;
    }
    #endregion

    #region Private Interfaces
    private void _SetCheckpoint(CheckpointDoorFrame checkpoint)
    {
        mCheckpoint = checkpoint;
        number = 9001;

        Debug.Log("Checkpoint set.");
    }
    #endregion

    #region Public Interfaces
    public static void SetCheckoint(CheckpointDoorFrame checkpoint)
    {
        LevelManager.GetInstance()._SetCheckpoint(checkpoint);
    }
    #endregion
}
