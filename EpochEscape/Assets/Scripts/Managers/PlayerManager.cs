using UnityEngine;
using System.Collections;

public class PlayerManager : Manager<PlayerManager>
{
    private bool m_isInitialized = false;
    private Player m_player = null;
    private Vector3 m_spawnPosition = Vector3.zero;

    protected override void Initialize()
    {
        InstantiatePlayer();

        //_HidePlayer();
    }

    private void InstantiatePlayer()
    {
        GameObject player = null;

        switch(GameManager.getInstance().m_currentCharacter)
        {
            case GameManager.KNIGHT:
                player = Resources.Load("Prefabs/Players/Knight") as GameObject;
                break;

            case GameManager.CAVEGIRL:
            default:
                player = Resources.Load("Prefabs/Players/CaveGirl") as GameObject;
                break;
        }

        if (player != null)
        {
            player = Instantiate(player) as GameObject;

            if (player != null)
            {
                m_player = player.GetComponent<Player>();

                if (m_player != null)
                {
                    m_isInitialized = true;

                    DontDestroyOnLoad(m_player);
                }
            }
        }
    }

    #region Private Interfaces
    private void _SetPosition(Vector3 position)
    {
        if (m_isInitialized)
            m_player.transform.position = new Vector3(position.x, position.y, 0f);
    }

    private Vector3 _GetPosition()
    {
        if (!m_isInitialized)
            return Vector3.zero;

        return new Vector3(m_player.transform.position.x, m_player.transform.position.y);
    }

    private void _SetSpawnPosition(Vector3 spawnPosition)
    {
        if (m_isInitialized)
            m_spawnPosition = spawnPosition;
    }

    private Vector3 _GetSpawnPosition()
    {
        if(!m_isInitialized)
            return Vector3.zero;

        return m_spawnPosition;
    }

    private void _Respawn()
    {
        if (m_isInitialized)
        {
            _SetPosition(m_spawnPosition);
            _ShowPlayer();

            m_player.Resurrect();

            CameraManager.SetPosition(PlayerManager.GetPosition());
        }
    }

    private void _ShowPlayer()
    {
        m_player.gameObject.SetActive(true);
    }

    private void _HidePlayer()
    {
        m_player.gameObject.SetActive(false);
    }

    private void _AddCore()
    {
        if (m_isInitialized)
            m_player.addCore();
    }

    public void _RemoveCore()
    {
        if (m_isInitialized && m_player.getCurrentCores() > 0)
            m_player.removeCore();
    }

    private int _GetCores()
    {
        if (m_isInitialized)
            return m_player.getCurrentCores();

        return 0;
    }

    private void _ClearCores()
    {
        if (m_isInitialized)
            m_player.clearCores();
    }
    #endregion

    #region Public Interfaces
    public static void SetPosition(Vector3 position)
    {
        PlayerManager.Get()._SetPosition(position);
    }

    public static Vector3 GetPosition()
    {
        return PlayerManager.Get()._GetPosition();
    }

    //I don't know if I agree with the position of these three
    //methods. The LevelManager should manage these methods instead...
    //In fact, the level manager should keep all checkpoint data
    //instead of the player
    public static void SetSpawnPosition(Vector3 spawnPosition)
    {
        PlayerManager.Get()._SetSpawnPosition(spawnPosition);
    }

    public static Vector3 GetSpawnPosition()
    {
        return PlayerManager.Get()._GetSpawnPosition();
    }

    public static void Respawn()
    {
        PlayerManager.Get()._Respawn();
    }

    public static void ShowPlayer()
    {
        PlayerManager.Get()._ShowPlayer();
    }

    public static void HidePlayer()
    {
        PlayerManager.Get()._HidePlayer();
    }

    public static void AddCore()
    {
        PlayerManager.Get()._AddCore();
    }

    public static void RemoveCore()
    {
        PlayerManager.Get()._RemoveCore();
    }

    public static int GetCores()
    {
        return PlayerManager.Get()._GetCores();
    }

    public static void ClearCores()
    {
        PlayerManager.Get()._ClearCores();
    }
    #endregion
}
