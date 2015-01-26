using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * A script that causes a popup message to be shown. The popup pauses
 * the entire game and will unpause it once it has be read and closed.
 */
public class PopupSwitch : MonoBehaviour
{
    public List<string> instructions = new List<string>();
    public bool mActive = true;
    public bool mDelay = false;

    /*
     * Shows the popup message.
     */
    private void message ()
    {
        GameManager.Get ().messages = instructions;
        GameManager.Get ().ShowPopupMessage();
        GameManager.Get ().popup = true;
        mActive = false;
    }

    /*
     * If the collidee is the player, then the popup message is shown.
     */
    void OnTriggerEnter2D(Collider2D other)
    {
        if(mActive && other.gameObject.tag == "Player")
        {
            if (!mDelay)
            {
                Player p = other.gameObject.GetComponent<Player>();
                p.audio.Stop ();
                message ();
            }
            else
            {
                Player p = other.gameObject.GetComponent<Player>();
                p.audio.Stop ();
                Invoke ("message",1); //Delay the popup for 1 second.
            }
        }
    }
}
