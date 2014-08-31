using UnityEngine;
using System.Collections;

public class CaveGirlClub : MonoBehaviour
{
    private Player m_player = null;
    private AudioClip m_hitSound = null;

    public void Start()
    {
        Transform parent = transform.parent;

        if(parent != null)
            m_player = parent.GetComponent<Player>();

        m_hitSound = Resources.Load("Sounds/TerrySounds/CaveGirl/ClubHit") as AudioClip;
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if(m_player == null)
            return;

        if(other.tag == "Guard")
        {
            Guard guard = other.GetComponent<Guard>();

            if(guard == null)
                guard = other.GetComponent<StationaryGuard>();

            if(guard == null)
                return;

            if(m_player.IsAtHitFrame)
            {
                guard.m_currentState = Guard.State.STUN;
                guard.transform.up = Vector3.up;

                if(m_hitSound != null)
                    AudioSource.PlayClipAtPoint(m_hitSound, guard.transform.position);
                
                other.enabled = false;
            }
        }
    }
}
