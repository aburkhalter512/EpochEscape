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
    public bool active = true;
    public bool delay = false;

    /*
     * Shows the popup message.
     */
    private void message ()
    {
        GameManager.getInstance ().messages = instructions;
        GameManager.getInstance ().ShowPopupMessage();
        GameManager.getInstance ().popup = true;
        active = false;
    }

    /*
     * If the collidee is the player, then the popup message is shown.
     */
    void OnTriggerEnter2D(Collider2D other)
    {
        if(active && other.gameObject.tag == "Player")
        {
            if (!delay)
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
