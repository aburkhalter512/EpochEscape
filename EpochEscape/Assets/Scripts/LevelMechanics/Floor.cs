using UnityEngine;
using System.Collections;

public class Floor : MonoBehaviour
{
    #region Inspector Variables
    public float mDynamicFriction = 0.001f;
    #endregion

    #region Instance Variables
    #endregion

    #region Class Constants
    public const float DEFAULT_FRICTION = 1.0f;
    public const float MIN_FRICTION = 0.0f;
    public const float MAX_FRICTION = 1.0f;
    #endregion

    #region Update Methods
    protected void OnTriggerEnter2D(Collider2D collidee)
    {
        Player player = collidee.GetComponent<Player>();

        if (player != null)
        {
            mDynamicFriction = Mathf.Clamp(mDynamicFriction, MIN_FRICTION, MAX_FRICTION);
            //player.mDynamicFriction = mDynamicFriction;
        }
    }
    #endregion

    #region Static Methods
    #endregion

    #region Utilities
    #endregion
}
