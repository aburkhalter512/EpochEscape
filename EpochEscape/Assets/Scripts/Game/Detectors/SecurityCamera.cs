using UnityEngine;
using System.Collections;
using System.Xml;
using G = Game.GameManager;

public class SecurityCamera : MonoBehaviour, IActivatable, ISerializable
{
    #region Interface Variables
    public float m_patrolAngle = DEFAULT_PATROL_ANGLE;
    #endregion

    #region Instance Variables
    protected State m_currentState;

    protected float mStartAngle;
    protected float mBaseAngle;
    protected float mDestAngle;

    protected Vector3 mCurrentDirection;

    protected float mCurrentScanTime;
    protected float mScanTime;
    #endregion

    #region Class Constants
    public const float DEFAULT_PATROL_ANGLE = 45f; // 45 degrees
    public const float DEFAULT_PATROL_SPEED = 90f; // 90 degrees/sec
    
    public enum State
    {
        IDLE,
        ROTATING
    };
    #endregion

    protected Vector3 dir;
    public State cur;

    protected void Start()
    {
        mBaseAngle = transform.localEulerAngles.z;

        mStartAngle = Utilities.Math.WrapAngle(mBaseAngle - m_patrolAngle / 2);
        mDestAngle = Utilities.Math.WrapAngle(mBaseAngle + m_patrolAngle / 2);

        mCurrentDirection = new Vector3(0, 0, mStartAngle);
        transform.localEulerAngles = mCurrentDirection;

        mCurrentScanTime = 0.0f;
        mScanTime = m_patrolAngle / DEFAULT_PATROL_SPEED;

        m_currentState = State.ROTATING;
    }
    
    protected void Update()
    {
        if (!G.Get().paused)
            UpdateCurrentState();
    }

    #region Interface Variables
    public void deactivate()
    {
        cur = State.IDLE;

        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void activate()
    {
        cur = State.ROTATING;

        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void toggle()
    {
        if (cur != State.IDLE)
            deactivate();
        else
            activate();
    }

    public IEnumerator serialize(XmlDocument document, System.Action<XmlElement> callback)
    {
        XmlElement element = document.CreateElement("securitycamera");
        element.SetAttribute("type", GetType().ToString());
        element.SetAttribute("patrolangle", m_patrolAngle.ToString());

        element.AppendChild(Utilities.Serialization.toXML(transform, document));

        callback(element);

        return null;
    }
    #endregion

    #region Instance Methods
    private void UpdateCurrentState()
    {
        switch(m_currentState)
        {
            case State.IDLE:
                break;

            case State.ROTATING:
                Rotate();
                break;
        }
    }

    private void Rotate()
    {
        mCurrentScanTime += Time.smoothDeltaTime;

        if (mCurrentScanTime >= mScanTime)
        {
            mCurrentDirection.z = mDestAngle;
            transform.localEulerAngles = mCurrentDirection;

            float swap = mStartAngle;
            mStartAngle = mDestAngle;
            mDestAngle = swap;

            mCurrentScanTime = 0;

            return;
        }

        mCurrentDirection.z = Mathf.LerpAngle(mStartAngle, mDestAngle, mCurrentScanTime / mScanTime);
        transform.localEulerAngles = mCurrentDirection;
    }
    #endregion
}
