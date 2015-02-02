using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : Manager<LevelManager>
{
    #region Instance Variables
    private EntranceDoorFrame m_entranceDoor;

    private List<Chamber> m_chambers = null;

    private int m_currentChamber = 0;
    private CheckpointDoorFrame m_currentCheckpoint = null;

    Player mPlayer;
    #endregion

    protected override void Initialize()
    {
        GameObject player = null;

        switch (GameManager.Get().m_currentCharacter)
        {
            case GameManager.KNIGHT:
                player = Resources.Load("Prefabs/Players/Knight") as GameObject;
                break;

            case GameManager.CAVEGIRL:
            default:
                player = Resources.Load("Prefabs/Players/CaveGirl") as GameObject;
                break;
        }

        player = Instantiate(player) as GameObject;
        mPlayer = player.GetComponent<Player>();
    }

    protected void Start()
    {
        _Ready();
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
        GameManager.Get().PauseMovement();
        HUDManager.Show();
        MiniMapManager.Show();
        GameManager.Get().UnpauseMovement();

        m_currentCheckpoint.open();
        mPlayer.transform.position = m_currentCheckpoint.getRespawnLocation();
        mPlayer.show();

        mPlayer.Resurrect();

        CameraManager.SetPosition(mPlayer.transform.position);
        
        _ResetChamber(m_currentChamber);
    }

    private void _ExitLevel()
    {
        mPlayer.hide();
        mPlayer.clearCores();
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
        mPlayer.hide();
        mPlayer.clearCores();

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
        m_entranceDoor = EntranceDoorFrame.get();
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
        LevelManager.Get()._SetCheckpoint(checkpoint);
    }

    public static void LoadCheckpoint()
    {
        LevelManager.Get()._LoadCheckpoint();
    }

    public static void ExitLevel()
    {
        LevelManager.Get()._ExitLevel();
    }

    public static void RestartLevel()
    {
        LevelManager.Get()._RestartLevel();
    }

    public static Chamber GetCurrentChamber()
    {
        return LevelManager.Get()._GetCurrentChamber();
    }

    public static void AddChamber(Chamber chamber)
    {
        LevelManager.Get()._AddChamber(chamber);
    }

    public static void Ready()
    {
        LevelManager.Get()._Ready();
    }

    public static void ClearLevel()
    {
        LevelManager.Get()._ClearLevel();
    }
    #endregion
}
