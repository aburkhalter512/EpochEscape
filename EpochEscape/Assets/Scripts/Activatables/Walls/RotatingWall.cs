using UnityEngine;
using System.Collections;

public class RotatingWall : DynamicWall
{
    #region Inspector Variables
    public GameObject rotationPoint;

    public DIRECTION[] rotationTargets;
    #endregion

    #region Instance Variables
    protected Vector3 mRotationPoint;
    protected float[] mRotationAngles;
    protected float mRotationRadius;

    protected float mBaseAngle = 0.0f;
    protected float mPrevAngle;
    protected float mDestinationAngle = 0.0f;
    #endregion

    #region Class Constants
    public enum DIRECTION
    {
        EAST,
        NORTH,
        WEST,
        SOUTH
    }
    #endregion

    /*
     * Initializes the Rotating Wall
     */
    protected new void Awake()
    {
        base.Awake();

        mRotationPoint = rotationPoint.transform.position;
        GameObject.Destroy(rotationPoint);

        mRotationAngles = new float[rotationTargets.Length + 1];
        mRotationAngles[0] = transform.eulerAngles.z;

        mRotationRadius = Vector3.Distance(mRotationPoint, transform.position);

        for (int i = 0; i < rotationTargets.Length; i++)
        {
            switch (rotationTargets[i])
            {
                case DIRECTION.EAST:
                    mRotationAngles[i + 1] = 0.0f;
                    break;
                case DIRECTION.NORTH:
                    mRotationAngles[i + 1] = 90.0f;
                    break;
                case DIRECTION.WEST:
                    mRotationAngles[i + 1] = 180.0f;
                    break;
                case DIRECTION.SOUTH:
                    mRotationAngles[i + 1] = 270.0f;
                    break;
            }
        }
    }
    
    #region Instance Methods
    protected override void toChange()
    {
        audio.Play ();
        mCurrentChangeTime = 0.0f;
        mCurrentIndex = (mCurrentIndex + 1) % mRotationAngles.Length;

        mBaseAngle = transform.eulerAngles.z;
        mPrevAngle = mBaseAngle;
        mDestinationAngle = mRotationAngles[mCurrentIndex];
        
        mState = STATE.CHANGE;
    }
    
    protected override void change()
    {
        mCurrentChangeTime += Time.smoothDeltaTime;

        float currentAngle = 0.0f;

        if (mCurrentChangeTime >= CHANGE_TIME)
        {
            mState = STATE.STATIONARY;
            currentAngle = mDestinationAngle * Mathf.Deg2Rad;
            transform.position = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0) * mRotationRadius;
            transform.position += mRotationPoint;
            transform.eulerAngles = new Vector3(0, 0, mDestinationAngle);

            return;
        }

        currentAngle = Mathf.LerpAngle(mBaseAngle, mDestinationAngle, mCurrentChangeTime / CHANGE_TIME);
        transform.RotateAround(mRotationPoint, Vector3.forward, currentAngle - mPrevAngle);
        mPrevAngle = Mathf.LerpAngle(mBaseAngle, mDestinationAngle, mCurrentChangeTime / CHANGE_TIME);
    }
    #endregion
}
