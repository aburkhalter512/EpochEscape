using UnityEngine;
using System.Collections;

public class BoxDoorFrame : LockedDoorFrame
{
    private Player m_player = null;

    public void Start()
    {
        base.Start();
    }

    public void Update()
    {
        if(m_player != null)
        {
            if(m_player.m_isHoldingBox && mCurState != STATE.UNLOCKED)
                unlockDoor();
            else if (!m_player.m_isHoldingBox && mCurState != STATE.LOCKED)
                lockDoor();
        }
        else
            FindPlayer();
    }

    public void FindPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
            m_player = player.GetComponent<Player>();
    }
}
