using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Text))]
    public class ActivatablePhrase : MonoBehaviour, IActivatable
    {
        #region Instance Variables
        Text mText;

        float mBaseAlpha;
        Color mCurrentColor;

        float mCurrentTime;

        bool mIsActive;
        STATE mCurrentState;
        #endregion

        #region Class Constants
        public static readonly float FADE_TIME = 0.5f;

        public enum STATE
        {
            IDLE,
            FADE,
            FADING,
            APPEAR,
            APPEARING
        }
        #endregion

        protected void Awake()
        {
            mIsActive = false;
            mCurrentState = STATE.IDLE;
            mText = GetComponent<Text>();

            mCurrentColor = mText.color;
            mCurrentColor.a = 0.0f;

            mText.color = mCurrentColor;
        }

        protected void Update()
        {
            switch (mCurrentState)
            {
                case STATE.IDLE:
                    break;
                case STATE.FADE:
                    mCurrentTime = 0.0f;

                    mCurrentColor.a = 1.0f;
                    mText.color = mCurrentColor;

                    mCurrentState = STATE.FADING;
                    break;
                case STATE.FADING:
                    fading();
                    break;
                case STATE.APPEAR:
                    mCurrentTime = 0.0f;

                    mCurrentColor.a = 0.0f;
                    mText.color = mCurrentColor;

                    mCurrentState = STATE.APPEARING;
                    break;
                case STATE.APPEARING:
                    appearing();
                    break;
            }
        }

        #region Interface Methods
        public void activate()
        {
            if (mCurrentState == STATE.IDLE)
            {
                mIsActive = true;
                mCurrentState = STATE.APPEAR;
            }
        }

        public void deactivate()
        {
            if (mCurrentState == STATE.IDLE)
            {
                mIsActive = false;
                mCurrentState = STATE.FADE;
            }
        }

        public void toggle()
        {
            if (mIsActive)
                deactivate();
            else
                activate();
        }
        #endregion

        #region Instance Methods
        private void fading()
        {
            mCurrentTime += Time.smoothDeltaTime;

            if (mCurrentTime >= FADE_TIME)
            {
                mCurrentColor.a = 0.0f;
                mText.color = mCurrentColor;

                mCurrentState = STATE.IDLE;
            }
            else
            {
                mCurrentColor.a = Mathf.Lerp(1.0f, 0.0f, mCurrentTime / FADE_TIME);

                mText.color = mCurrentColor;
            }
        }

        private void appearing()
        {
            mCurrentTime += Time.smoothDeltaTime;

            if (mCurrentTime >= FADE_TIME)
            {
                mCurrentColor.a = 1.0f;
                mText.color = mCurrentColor;

                mCurrentState = STATE.IDLE;
            }
            else
            {
                mCurrentColor.a = Mathf.Lerp(0.0f, 1.0f, mCurrentTime / FADE_TIME);

                mText.color = mCurrentColor;
            }
        }
        #endregion
    }
}
