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
                    m_isInitialized = true;
            }
        }
    }

    #region Private Interfaces
    private bool _SetPosition(Vector3 position)
    {
        if (m_isInitialized)
        {
            m_player.transform.position = new Vector3(position.x, position.y, 0f);

            return true;
        }

        return false;
    }

    private Vector3 _GetPosition()
    {
        if (!m_isInitialized)
            return Vector3.zero;

        return new Vector3(m_player.transform.position.x, m_player.transform.position.y);
    }

    private bool _SetSpawnPosition(Vector3 spawnPosition)
    {
        if (m_isInitialized)
        {
            m_spawnPosition = spawnPosition;

            return true;
        }

        return false;
    }

    private Vector3 _GetSpawnPosition()
    {
        if(!m_isInitialized)
            return Vector3.zero;

        return m_spawnPosition;
    }

    private bool _Respawn()
    {
        return _SetPosition(m_spawnPosition);
    }
    #endregion

    #region Public Interfaces
    public static bool SetPosition(Vector3 position)
    {
        return PlayerManager.GetInstance()._SetPosition(position);
    }

    public static Vector3 GetPosition()
    {
        return PlayerManager.GetInstance()._GetPosition();
    }

    public static bool SetSpawnPosition(Vector3 spawnPosition)
    {
        return PlayerManager.GetInstance()._SetSpawnPosition(spawnPosition);
    }

    public static Vector3 GetSpawnPosition()
    {
        return PlayerManager.GetInstance()._GetSpawnPosition();
    }

    public static bool Respawn()
    {
        return PlayerManager.GetInstance()._Respawn();
    }
    #endregion
}
