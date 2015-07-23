using UnityEngine;

namespace Game
{
    public class FloorActivator : Activator
    {
        protected void Awake()
        {
            populateActivatables();
        }

        #region Instance Methods
        protected void OnTriggerEnter2D(Collider2D collidee)
        {
            Player player = collidee.GetComponent<Player>();

            if (player != null)
                trigger();
        }

        protected void OnTriggerExit2D(Collider2D collidee)
        {
            Player player = collidee.GetComponent<Player>();

            if (player != null)
                trigger();
        }
        #endregion
    }
}
