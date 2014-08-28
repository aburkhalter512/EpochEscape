using UnityEngine;
using System.Collections;

public class EndOfLevelDetector : MonoBehaviour
{
    #region Interface Variables
    #endregion

    #region Instance Variables
    ExitDoorFrame detectable;
    #endregion

    protected void Start()
    {
        detectable = transform.GetComponentInParent<ExitDoorFrame>();
    }

    #region Instance Methods
    protected void OnTriggerEnter2D(Collider2D collidee)
    {
        Player player = collidee.GetComponent<Player>();

        if (player != null)
            detectable.exitLevel(player);
    }
    #endregion
}
