using UnityEngine;
using System.Collections.Generic;
using System.Xml;

namespace Game
{
    public class PressurePlate : Activator
    {
        #region Inspector Variables
        public Sprite switchOn;
        public Sprite switchOff;
        #endregion

        #region Instance Variables
        protected SpriteRenderer mSR;

        protected BoxCollider2D mCollider;
        protected Vector2 mBaseSize;

        protected STATE previousState;
        protected STATE currentState;
        #endregion

        #region Class Constants
        public enum STATE
        {
            ON,
            OFF,
            UN_INIT
        }
        #endregion

        /*
     * Initializes the Pressure Plate
     */
        protected void Start()
        {
            mSR = gameObject.GetComponent<SpriteRenderer>();
            mCollider = gameObject.GetComponent<BoxCollider2D>();
            mBaseSize = mCollider.size;

            previousState = STATE.UN_INIT;
            currentState = STATE.ON;

            populateActivatables();
        }

        /*
         * Updates the state of the Pressure Plate
         */
        protected void Update()
        {
            if (previousState != currentState)
            {
                switch (currentState)
                {
                    case STATE.OFF:
                        Off();
                        break;
                    case STATE.ON:
                        On();
                        break;
                }

                previousState = currentState;
            }
        }

        #region Instance Methods
        /*
     * Turns the pressure plate off
     */
        virtual protected void Off()
        {
            mSR.sprite = switchOff;
        }

        /*
         * Turns the pressure plate on
         */
        virtual protected void On()
        {
            mSR.sprite = switchOn;
        }

        /*
         * If the collidee is the player, then all actuators are triggered and the
         * pressure plate is turned off.
         */
        virtual protected void OnTriggerEnter2D(Collider2D collidee)
        {
            Player player = collidee.GetComponent<Player>();

            if (player != null)
            {
                mCollider.size *= 2;

                GetComponent<AudioSource>().Play();

                GameManager.Get().delayFunction(() =>
                    {
                        //Move the player to the center of the pressure plate
                        player.transform.position = transform.position;
                    });

                trigger();

                currentState = (currentState == STATE.ON ? STATE.OFF : STATE.ON);
            }
        }

        virtual protected void OnTriggerExit2D(Collider2D collidee)
        {
            Player player = collidee.GetComponent<Player>();

            if (player != null)
                mCollider.size = mBaseSize;
        }
        #endregion
    }
}
