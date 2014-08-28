using UnityEngine;
using System.Collections;

/**
 * This class acts as a directional detector for a door frame, which needs to be
 * the detector's parent.  Any gameobject that collides with this gameobject as has
 * a tag specified by 'DetecteeTags[]' then a signal is sent to the door frame via
 * triggerBackEnter()/triggerBackExit(). 'DetecteeTags[]' can be modified in the
 * Unity Editor to easily change which tagged gameobjects trigger the collision.
 * 
 * Interface Variables
 *      DetecteeTags: string[]
 *          An array of strings that specifies vaild tags for gameobject collision.
 *          If a gameobject does not have a tag listed in 'DetecteeTags[]' then the
 *          collision is not triggered.
 *          
 * Interface Methods
 *      There are not interface methods.
 */
public class BackDoorDetector : MonoBehaviour
{
    #region Interface Variables
    public string[] DetecteeTags;
    #endregion

    #region Instance Variables
    DoorFrame detectable;
    #endregion

    protected void Start()
    {
        detectable = transform.GetComponentInParent<DoorFrame>();
    }

    #region Instance Methods
    protected void OnTriggerEnter2D(Collider2D collidee)
    {
        foreach (string tag in DetecteeTags)
            if (collidee.tag == tag)
                detectable.triggerBackEnter();
    }

    protected void OnTriggerExit2D(Collider2D collidee)
    {
        foreach (string tag in DetecteeTags)
            if (collidee.tag == tag)
                detectable.triggerBackExit();
    }
    #endregion
}
