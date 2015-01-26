using UnityEngine;
using System.Collections;

public class RotatingWall : DynamicWall
{
    #region Inspector Variables
    public GameObject rotationPoint;

    public DIRECTION[] rotationTargets;
    #endregion

    #region Instance Variables
    protected float[] mRotationAngles;

    protected float destinationAngle = 0.0f;
    
    protected float currentRotationChange = 0.0f;
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

        mRotationAngles = new float[rotationTargets.Length];

        for (int i = 0; i < mRotationAngles.Length; i++)
        {
            switch (rotationTargets[i])
            {
                case DIRECTION.EAST:
                    mRotationAngles[i] = 0.0f;
                    break;
                case DIRECTION.NORTH:
                    mRotationAngles[i] = 90.0f;
                    break;
                case DIRECTION.WEST:
                    mRotationAngles[i] = 180.0f;
                    break;
                case DIRECTION.SOUTH:
                    mRotationAngles[i] = 270.0f;
                    break;
            }
        }
    }
    
    //Put all update code here
    //Remember to comment!
    protected new void Update()
    {
        base.Update();
    }
    
    #region Update Methods
    protected override void toChange()
    {
        audio.Play ();
        currentIndex = (currentIndex + 1) % mRotationAngles.Length;
        
        destinationAngle = mRotationAngles[currentIndex];

        if (transform.localEulerAngles.z < 0)
            transform.localEulerAngles.Set(
                transform.localEulerAngles.x, 
                transform.localEulerAngles.y,
                360 + transform.localEulerAngles.z % 360);

        float rotationDistance = 0.0f;

        if (transform.localEulerAngles.z + (360 - destinationAngle) <= 180.0f)
            rotationDistance = -transform.localEulerAngles.z - (360 - destinationAngle);
        else
            rotationDistance = destinationAngle - transform.localEulerAngles.z;
        
        currentRotationChange = rotationDistance / changeTime;
        
        UpdateSize();
        
        mState = STATE.CHANGE;
    }
    
    protected override void change()
    {
        if (Mathf.Approximately(currentRotationChange, 0.0f))
        {
            mState = STATE.STATIONARY;
            
            for(int i = 0; i < transform.childCount; i++)
            {
                if(transform.GetChild(i).tag == "SecurityCamera")
                {
                    SecurityCamera cam = transform.GetChild(i).GetComponent<SecurityCamera>();
                    
                    float sn = Mathf.Sin(destinationAngle * Mathf.Deg2Rad);
                    float cs = Mathf.Cos(destinationAngle * Mathf.Deg2Rad);
                    
                    float px = cam.m_resetDirection.x * cs - cam.m_resetDirection.y * sn;
                    float py = cam.m_resetDirection.x * sn + cam.m_resetDirection.y * cs;
                    
                    cam.m_resetDirection = new Vector3(px, py, cam.m_resetDirection.z);
                    cam.m_resetAngle += destinationAngle;
                }
            }
            
            return;
        }
        
        float realRotationChanged = currentRotationChange * Time.smoothDeltaTime;
        
        if (Utilities.isBounded(
            destinationAngle - realRotationChanged,
            destinationAngle + realRotationChanged,
            transform.localEulerAngles.z))
        {
            transform.eulerAngles.Set(0, 0, destinationAngle);
            currentRotationChange = 0.0f;
        }
        else
            transform.RotateAround(
                rotationPoint.transform.position, 
                Vector3.forward, 
                realRotationChanged);
    }
    #endregion
}
