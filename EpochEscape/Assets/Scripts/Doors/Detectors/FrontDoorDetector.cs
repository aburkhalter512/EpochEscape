using UnityEngine;
using System.Collections;

public class FrontDoorDetector : MonoBehaviour
{
    #region Interface Variables
    public string[] DetecteeTags;
    #endregion

    #region Instance Variables
    IDetectable detectable;
    #endregion 

    protected void Start()
    {
        detectable = transform.GetComponentInParent<DoorFrame>() as IDetectable;
    }
    
    #region Instance Methods
    protected void OnTriggerEnter2D(Collider2D collidee)
    {
        foreach (string tag in DetecteeTags)
            if (collidee.tag == tag)
                detectable.triggerFrontEnter();
    }

    protected void OnTriggerExit2D(Collider2D collidee)
    {
        foreach (string tag in DetecteeTags)
            if (collidee.tag == tag)
                detectable.triggerFrontExit();
    }
    #endregion
}
