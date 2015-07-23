using UnityEngine;

/**
 * This class represents a door side, two of which are contained within a door frame and then
 * compose a whole door. A side can be activated/deactivated independently of the other side
 * and each side processes it's own code. This is to say that a door side processes a player
 * 'going through' it without knowing what the other side is processing. The goal of this design
 * is to allow customizability and easy interfaceability.
 * 
 * Each side is 'dumb' as will process it's own code without referencing any other code. If a user
 * triggers it's methods, it will respond accordingly and NOT DO ANY LOGICAL ERROR CHECKING. The
 * door frame should do ALL of the logical error checking.
 * 
 * This forces the DoorFrame to maintain and record the door side's state, forcing the entire door
 * not to have multiple states for the same object.
 * 
 * Interface Variables
 *      activeSprite: Sprite
 *          The sprite to display when the door side is active
 *      deactiveSprite: Sprite
 *          The sprite to display when the door side is inactive
 *          
 * Interface Methods
 *      bool isActive()
 *          Returns true if the door side is activated, false if not.
 *      void triggerFrontEnter()
 *          A method that alerts the door side that a gameobject has entered the front detection area.
 *      void triggerBackExit()
 *          A method that alerts the door side that a gameobject has exited the back detection area.
 *      void triggerBackEnter()
 *          A method that alerts the door side that a gameobject has entered the back detection area.
 *      void triggerBackExit()
 *          A method that alerts the door side that a gameobject has exited the back detection area.
 *      void activate()
 *          A method that activates (opens) the door side.
 *      void deactivate()
 *          A method that deactivates (closes) the door side.
 *      void toggle()
 *          A method that either activates or deactivates the door side based on which state it is
 *          currently in. If the door side is activated, it deactivates it, and vice versa.
 */
namespace Game
{
    public abstract class DoorSide : MonoBehaviour, IActivatable, IDetectable
    {
        #region Interface Variables
        public Sprite activeSprite;
        public Sprite inactiveSprite;
        #endregion

        #region Instance Variables
        protected bool mIsActive = false;
        protected bool mIsFirstUpdate = true;

        protected SpriteRenderer mSR = null;
        #endregion

        #region Class Constants
        public static readonly Vector2 SIZE = new Vector2(.8f, .25f);
        #endregion

        //base.Awake must be called to allow the derived door side to change states, otherwise the door
        //side will remain idling.
        protected void Awake()
        {
            mSR = GetComponent<SpriteRenderer>();
        }

        #region Interface Methods
        public bool isActive()
        {
            return mIsActive;
        }

        public abstract void triggerFrontEnter();
        public abstract void triggerFrontExit();

        public abstract void triggerBackEnter();
        public abstract void triggerBackExit();

        public abstract void deactivate();
        public abstract void activate();
        public void toggle()
        {
            if (mIsActive)
                deactivate();
            else
                activate();
        }
        #endregion
    }
}
