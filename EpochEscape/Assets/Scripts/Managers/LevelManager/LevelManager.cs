using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : Manager<LevelManager>
{
    #region Inspector Variables
    #endregion

    #region Instance Variables
    private EntranceDoorFrame m_entranceDoor;
    private ExitDoorFrame m_exitDoor;

    private List<Chamber> m_chambers = null;

    private int m_currentChamber = 0;
    private CheckpointDoorFrame m_currentCheckpoint = null;
    #endregion

    protected override void Initialize()
    {
        // First time initialization.
    }

    private void _Ready()
    {
        _FindLevelDoors();
        _SetCheckpoint(m_entranceDoor);
        _LoadCheckpoint();
    }

    #region Private Interfaces
    private void _SetCheckpoint(CheckpointDoorFrame checkpoint)
    {
        if (checkpoint != null && checkpoint != m_currentCheckpoint)
        {
            if (m_currentCheckpoint != null)
            {
                _DisableMiniMapLayer();

                m_currentChamber++;
            }

            m_currentCheckpoint = checkpoint;

            _EnableMiniMapLayer();

            PlayerManager.SetSpawnPosition(checkpoint.respawnLocation.transform.position);
            MiniMapManager.SetChamber();
        }
    }

    private void _DisableMiniMapLayer()
    {
        if (m_chambers != null && m_currentChamber >= 0 && m_currentChamber < m_chambers.Count)
            m_chambers[m_currentChamber].DisableMiniMapLayer();
    }

    private void _EnableMiniMapLayer()
    {
        if (m_chambers != null && m_currentChamber >= 0 && m_currentChamber < m_chambers.Count)
            m_chambers[m_currentChamber].EnableMiniMapLayer();
    }

    private void _LoadCheckpoint()
    {
        GameManager.getInstance().PauseMovement();
        PlayerManager.Respawn();
        HUDManager.Show();
        MiniMapManager.Show();
        GameManager.getInstance().UnpauseMovement();
        
        _ResetChamber(m_currentChamber);
    }

    private void _ExitLevel()
    {
        PlayerManager.HidePlayer();
        PlayerManager.ClearCores();
        HUDManager.Hide();
        MiniMapManager.Hide();
        SaveManager.Save();

        FadeManager.StartAlphaFade(Color.black, false, 2f, 0f, () => { SceneManager.LoadNextLevel(); });

        m_currentCheckpoint = null;
        m_currentChamber = 0;
    }

    private void _RestartLevel()
    {
        // Possible fade here?
        PlayerManager.HidePlayer();
        PlayerManager.ClearCores();

        _SetCheckpoint(m_entranceDoor);
        _LoadCheckpoint();

        if (m_chambers != null)
        {
            for (int currentChamber = 0; currentChamber <= m_currentChamber; currentChamber++)
                _ResetChamber(currentChamber);
        }

        m_currentChamber = 0;
    }

    private void _ResetChamber(int chamberIndex)
    {
        if (m_chambers != null && m_chambers.Count > 0 && chamberIndex > 0 && chamberIndex < m_chambers.Count)
            m_chambers[chamberIndex].Reset();
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

    private Chamber _GetCurrentChamber()
    {
        if(m_chambers != null && m_currentChamber >= 0 && m_currentChamber < m_chambers.Count)
            return m_chambers[m_currentChamber];

        return null;
    }

    private void _AddChamber(Chamber chamber)
    {
        if (m_chambers == null)
            m_chambers = new List<Chamber>();

        if (m_chambers != null)
        {
            m_chambers.Add(chamber);

            Debug.Log(chamber.GetName() + " created.");
        }
    }

    private void _ClearLevel()
    {
        if (m_chambers != null)
        {
            foreach (Chamber chamber in m_chambers)
                chamber.Dispose();

            m_chambers.Clear();
        }
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

    public static void RestartLevel()
    {
        LevelManager.GetInstance()._RestartLevel();
    }

    public static Chamber GetCurrentChamber()
    {
        return LevelManager.GetInstance()._GetCurrentChamber();
    }

    public static void AddChamber(Chamber chamber)
    {
        LevelManager.GetInstance()._AddChamber(chamber);
    }

    public static void Ready()
    {
        LevelManager.GetInstance()._Ready();
    }

    public static void ClearLevel()
    {
        LevelManager.GetInstance()._ClearLevel();
    }
    #endregion
}
