using UnityEngine;
using System.Collections;

/**
 * This class is a directional door frame for Epoch Escape. Maintains all of the requirements
 * of a door frame, but limits them to specific cases. For more information about door frame
 * requirements/documentation see DoorFrame.cs
 * 
 * This DirectionalDoorFrame only allows the player to move one way through a door, back to 
 * front, but not front to back. A good analogy for how this door acts is an electrical diode.
 * Electricity can move one way through it, but not the other. Once the player exits through
 * the front side, the door closes behind the player.
 * 
 * Interface Variables
 *      frontSide: GameObject
 *          This variable is used for 'attaching' a side to a door frame, specifically the
 *          front side. 'frontSide' should contain a StandardDoorSide component, otherwise null
 *          reference exceptions will occur. This variable must be set on gameobject creation.
 *      backSide: GameObject
 *          This variable is used for 'attaching' a side to a door frame, specifically the
 *          back side. 'backSide' should contain a StandardDoorSide component, otherwise null
 *          reference exceptions will occur. This variable must be set on gameobject creation.
 *          
 * Interface Methods
 *      void triggerFrontEnter()
 *          A method that alerts the door that a gameobject has entered the front detection area.
 *      void triggerBackExit()
 *          A method that alerts the door that a gameobject has exited the back detection area.
 *      void triggerBackEnter()
 *          A method that alerts the door that a gameobject has entered the back detection area.
 *          This method directly opens the door sides.
 *      void triggerBackExit()
 *          A method that alerts the door that a gameobject has exited the back detection area.
 *      void activateSide(SIDE side)
 *          For a DirectionalDoorFrame, this method does nothing, as activating/deactivatin a specific
 *          door side would defeat the purpose of a DirectionalDoor. This is just an attempt at making
 *          the directional door as 'Black Box' as possible.
 *          Parameters:
 *              ignored
 *      void deactivateSide(SIDE side)
 *          For a DirectionalDoorFrame, this method does nothing, as activating/deactivatin a specific
 *          door side would defeat the purpose of a DirectionalDoor. This is just an attempt at making
 *          the directional door as 'Black Box' as possible.
 *          Parameters:
 *              ignored
 *      void activateSides()
 *          A method that activates both sides of the door.
 *      void deactivateSides
 *          A method that deactivates both sides of the door.
 */
namespace Game
{
    public class DirectionalDoorFrame : DoorFrame
    {
        #region Instance Variables
        bool mIsFrontHit = false;
        bool mIsBackHit = false;
        #endregion

        #region Interface Methods
        /**
     *  void triggerFrontEnter()
     *      A method that alerts the door that a gameobject has entered the front detection area.
     */
        public override void triggerFrontEnter()
        {
            mIsFrontHit = true;
        }

        /**
         *  void triggerFrontExit()
         *      A method that alerts the door that a gameobject has exited the front detection area.
         */
        public override void triggerFrontExit()
        {
            if (!mIsBackHit)
                mFrontSide.deactivate();

            mIsFrontHit = false;
        }

        /**
         *  void triggerBackEnter()
         *      A method that alerts the door that a gameobject has entered the back detection area.
         *      This method directly opens the door sides.
         */
        public override void triggerBackEnter()
        {
            if (mState != STATE.INACTIVE)
            {
                mIsBackHit = true;
                mFrontSide.activate();
            }
        }

        /**
         *  void triggerBackExit()
         *      A method that alerts the door that a gameobject has exited the back detection area.
         */
        public override void triggerBackExit()
        {
            if (!mIsFrontHit)
                mFrontSide.deactivate();

            mIsBackHit = false;
        }

        public override void activate()
        {
            mBackSide.activate();
            mState = STATE.ACTIVE;
        }

        public override void deactivate()
        {
            mBackSide.deactivate();
            mState = STATE.INACTIVE;
        }
        #endregion
    }
}
