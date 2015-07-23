using UnityEngine;
using System.Collections;

/*
 * A script that causes a game object to flicker and then finally destroy itself.
 * It is useful for making the illusion of a flickering laser/light
 */
namespace Game
{
    public class BrokenLaser : MonoBehaviour
    {
        #region Inspector Variables
        public float initialDelay = 1f;
        public int flickerCount = 3;
        public float flickerDelay = .1f;
        public float finalFlickerDelay = .5f;
        #endregion

        #region Instance Variables
        private float mStartTime;
        private int mCurrentFlicker;

        private bool mIsStarted;
        private bool mWasPaused;

        private enum STATE
        {
            SOLID,
            FLICKER_ON,
            FLICKER_OFF,
            FINAL_FLICKER,
            DESTROY
        }
        private STATE mCurrentState;

        private SpriteRenderer mSR;
        private Color mFadeColor;
        #endregion

        /*
     * Initializes the broken laser.
     */
        protected void Start()
        {
            mIsStarted = false;
            mWasPaused = false;
            mCurrentFlicker = 0;
            mCurrentState = STATE.SOLID;

            mSR = GetComponent<SpriteRenderer>();
            mFadeColor = mSR.color;
        }

        /*
         * Updates the broken laser's state
         */
        protected void Update()
        {
            if (!GameManager.Get().paused)
            {
                if (mWasPaused)
                {
                    mWasPaused = false;
                    mStartTime = Time.realtimeSinceStartup - mStartTime;
                }

                switch (mCurrentState)
                {
                    case STATE.SOLID:
                        Solid();
                        break;
                    case STATE.FLICKER_OFF:
                        FlickerOff();
                        break;
                    case STATE.FLICKER_ON:
                        FlickerOn();
                        break;
                    case STATE.FINAL_FLICKER:
                        FinalFlicker();
                        break;
                    case STATE.DESTROY:
                        Destroy();
                        break;
                }
            }
            else
            {
                if (!mWasPaused)
                {
                    mWasPaused = true;
                    mStartTime = Time.realtimeSinceStartup - mStartTime;
                }
            }
        }

        #region Update Methods
        /*
     * Displays the initial laser before it starts to flicker
     */
        protected void Solid()
        {
            if (!mIsStarted)
            {
                mStartTime = Time.realtimeSinceStartup;
                mIsStarted = true;
            }

            if (Time.realtimeSinceStartup - mStartTime >= initialDelay)
                mCurrentState = STATE.FLICKER_ON;
        }

        /*
         * Fades the laser over a set period of time
         */
        protected void FlickerOff()
        {
            mFadeColor.a -= Time.smoothDeltaTime / flickerDelay;
            mSR.color = mFadeColor;

            if (mFadeColor.a <= 0f)
            {
                mFadeColor.a = 0.0f;
                mCurrentState = STATE.FLICKER_ON;
            }
        }

        /*
         * Causes the laser to brighten/flicker before fading again.
         */
        protected void FlickerOn()
        {
            //Reset the laser to full on
            mFadeColor.a = 1.0f;
            mSR.color = mFadeColor;

            mCurrentFlicker++;

            //Lets wait a bit before fading again
            if (mCurrentFlicker > flickerCount)
                mCurrentState = STATE.FINAL_FLICKER;
            else
                mCurrentState = STATE.FLICKER_OFF;
        }

        /*
         * The final flicker, which can be different than the standard flickers.
         * After it has completely flickered, the game object destroys itself.
         */
        protected void FinalFlicker()
        {
            mFadeColor.a -= Time.smoothDeltaTime / finalFlickerDelay;
            mSR.color = mFadeColor;

            //Is it time to break?
            if (mFadeColor.a < 0f)
            {
                mFadeColor.a = 0.0f;
                mCurrentState = STATE.DESTROY;
            }
        }

        protected void Destroy()
        {
            Destroy(gameObject);
        }
        #endregion
    }
}
