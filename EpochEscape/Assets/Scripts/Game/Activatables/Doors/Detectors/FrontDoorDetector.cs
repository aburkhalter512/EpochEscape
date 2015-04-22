using UnityEngine;
using System.Collections;

/**
 * This class acts as a directional detector for a door frame, which needs to be
 * the detector's parent.  Any gameobject that collides with this gameobject as has
 * a tag specified by 'DetecteeTags[]' then a signal is sent to the door frame via
 * triggerFrontEnter()/triggerFrontExit(). 'DetecteeTags[]' can be modified in the
 * Unity Editor to easily change which tagged gameobjects trigger the collision.
 *          
 * Interface Methods
 *      There are no interface methods.
 */
public class FrontDoorDetector : MonoBehaviour
{
    #region Instance Variables
    IDetectable detectable;
    #endregion 

    protected void Start()
    {
        detectable = transform.parent.GetComponent<MonoBehaviour>() as IDetectable;
    }

    #region Instance Methods
    protected void OnTriggerEnter2D(Collider2D collidee)
    {
        Player player = collidee.GetComponent<Player>();

        if (player != null)
            detectable.triggerFrontEnter();
    }

    protected void OnTriggerExit2D(Collider2D collidee)
    {
        Player player = collidee.GetComponent<Player>();

        if (player != null)
            detectable.triggerFrontExit();
    }
    #endregion
}
