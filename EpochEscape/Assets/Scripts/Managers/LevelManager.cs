using UnityEngine;
using System.Collections;
using G = GameManager;

public class LevelManager : Manager<LevelManager>
{
    #region Inspector Variables
    public string m_nextLevel = "MainMenu";
    #endregion

    #region Instance Variables
    private EntranceDoorFrame m_entranceDoor;
    private ExitDoorFrame m_exitDoor;
    #endregion

    protected override void Initialize()
    {
        // First time initialization.
    }

    protected void Start()
    {
        _FindLevelDoors();
        _SetCheckpoint(m_entranceDoor);
        _LoadCheckpoint();
    }

    #region Private Interfaces
    private void _SetCheckpoint(CheckpointDoorFrame checkpoint)
    {
        if (checkpoint != null)
            PlayerManager.SetSpawnPosition(checkpoint.respawnLocation.transform.position);
    }

    private void _LoadCheckpoint()
    {
        PlayerManager.Respawn();
        HUDManager.Show();

        GameManager.getInstance().UnpauseMovement();
    }

    private void _ExitLevel()
    {
        PlayerManager.HidePlayer();
        HUDManager.Hide();

        //Set the correct current level
        if (m_nextLevel == "Level1")
            GameManager.getInstance().currentLevel = 1;
        else
            GameManager.getInstance().currentLevel++;

        SceneManager.Load(m_nextLevel);

        //Save all progress
        SaveManager.Save();
    }

    private void _FindLevelDoors()
    {
        GameObject entranceDoor = GameObject.Find("EntranceDoorCombo");
        GameObject exitDoor = GameObject.Find("ExitDoorCombo");

        if (entranceDoor != null)
            m_entranceDoor = entranceDoor.GetComponent<EntranceDoorFrame>();

        if (exitDoor != null)
            m_exitDoor = exitDoor.GetComponent<ExitDoorFrame>();
    }
    #endregion

    #region Public Interfaces
    public static void SetCheckoint(CheckpointDoorFrame checkpoint)
    {
        LevelManager.GetInstance()._SetCheckpoint(checkpoint);
    }

    public static void LoadCheckpoint()
    {
        LevelManager.GetInstance()._LoadCheckpoint();
    }

    public static void ExitLevel()
    {
        LevelManager.GetInstance()._ExitLevel();
    }
    #endregion
}
