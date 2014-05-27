using UnityEngine;
using System.Collections;

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

	#region Class Constants
	#endregion

	//Put all initialization code here
	//Remember to comment!
	protected void Start()
	{
        mIsStarted = false;
        mWasPaused = false;
        mCurrentFlicker = 0;
        mCurrentState = STATE.SOLID;

        mSR = GetComponent<SpriteRenderer>();
        mFadeColor = mSR.color;
	}

	#region Initialization Methods
	#endregion

	//Put all update code here
	//Remember to comment!
	protected void Update()
	{
        if (!GameManager.getInstance().paused)
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

    protected void FlickerOff()
    {
        mFadeColor.a -= Time.smoothDeltaTime / flickerDelay;
        mSR.color = mFadeColor;

        if (mFadeColor.a < 0f)
        {
            mFadeColor.a = 0.0f;
            mCurrentState = STATE.FLICKER_ON;
        }
    }

    protected void FlickerOn()
    {
        mFadeColor.a = 1.0f;
        mSR.color = mFadeColor;

        mCurrentFlicker++;

        if (mCurrentFlicker > flickerCount)
            mCurrentState = STATE.FINAL_FLICKER;
        else
            mCurrentState = STATE.FLICKER_OFF;
    }

    protected void FinalFlicker()
    {
        mFadeColor.a -= Time.smoothDeltaTime / finalFlickerDelay;
        mSR.color = mFadeColor;

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

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
