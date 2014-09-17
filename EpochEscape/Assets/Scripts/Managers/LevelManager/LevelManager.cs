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

    [SerializeField]
    private List<GameObject> m_chambers = null;

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
            if(m_currentCheckpoint != null)
                m_currentChamber++;

            m_currentCheckpoint = checkpoint;

            PlayerManager.SetSpawnPosition(checkpoint.respawnLocation.transform.position);
        }
    }

    private void _LoadCheckpoint()
    {
        GameManager.getInstance().PauseMovement();
        PlayerManager.Respawn();
        HUDManager.Show();
        GameManager.getInstance().UnpauseMovement();
        
        _ResetChamber(m_currentChamber);
    }

    private void _ExitLevel()
    {
        PlayerManager.HidePlayer();
        HUDManager.Hide();
        SaveManager.Save();

        FadeManager.StartAlphaFade(Color.black, false, 2f, 0f, () => { SceneManager.LoadNextLevel(); });
    }

    private void _ResetChamber(int currentChamber)
    {
        if (m_chambers != null && m_chambers.Count > 0)
        {
            Component[] resettableComponents = m_chambers[currentChamber].GetComponentsInChildren(typeof(IResettable));

            foreach (Component resettableComponent in resettableComponents)
            {
                IResettable resettable = resettableComponent as IResettable;

                if (resettable != null)
                    resettable.Reset();
            }
        }
    }

    private void _RestartLevel()
    {
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

    private void _FindLevelDoors()
    {
        GameObject entranceDoor = GameObject.Find("EntranceDoorCombo");
        GameObject exitDoor = GameObject.Find("ExitDoorCombo");

        if (entranceDoor != null)
            m_entranceDoor = entranceDoor.GetComponent<EntranceDoorFrame>();

        if (exitDoor != null)
            m_exitDoor = exitDoor.GetComponent<ExitDoorFrame>();
    }

    private List<GameObject> _GetChambers()
    {
        return m_chambers;
    }

    private void _AddChamber(GameObject chamber)
    {
        if (m_chambers == null)
            m_chambers = new List<GameObject>();

        if (m_chambers != null)
        {
            m_chambers.Add(chamber);

            Debug.Log(chamber.name + " added.");
        }
    }

    private void _ClearLevel()
    {
        if (m_chambers != null)
        {
            foreach (GameObject chamber in m_chambers)
                Destroy(chamber);

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

    public static List<GameObject> GetChambers()
    {
        return LevelManager.GetInstance()._GetChambers();
    }

    public static void AddChamber(GameObject chamber)
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
