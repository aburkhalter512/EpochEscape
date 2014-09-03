using UnityEngine;
using System.Collections;

public class RotatingWall : DynamicWall
{
    #region Inspector Variables
    public GameObject rotationPoint;

    public ROTATION_POINTS rotationPt;
    public float[] rotationAngles;
    
    public Vector3 customRotationPoint;
    #endregion
    
    #region Instance Variables
    private Vector3 realRotationPt = Vector3.zero;
    private float baseAngle = 0.0f;
    private float originalAngle = 0.0f;
    private float destinationAngle = 0.0f;

    private bool mRotationDirection; //False for counter-clockwise, true for clockwise
    
    private float currentRotationChange = 0.0f;
    #endregion
    
    #region Class Constants
    public enum ROTATION_POINTS
    {
        CENTER = 0,
        TOP_RIGHT,
        BOTTOM_RIGHT,
        BOTTOM_LEFT,
        TOP_LEFT,
        CUSTOM
    }
    #endregion
    
    /*
     * Initializes the Rotating Wall
     */
    protected new void Start()
    {
        base.Start();
        
        baseAngle = transform.rotation.eulerAngles.z;

        for (int i = 0; i < rotationAngles.Length; i++)
        {
            if (rotationAngles[i] < 0)
                rotationAngles[i] = 360 + rotationAngles[i] % 360;
            else
                rotationAngles[i] %= 360;
        }
    }
    
    //Put all update code here
    //Remember to comment!
    protected new void Update()
    {
        base.Update();
    }
    
    #region Update Methods
    protected override void stationary()
    {
        //Empty Method
    }
    
    protected override void toChange()
    {
        audio.Play ();
        currentIndex = (currentIndex + 1) % rotationAngles.Length;
        
        destinationAngle = rotationAngles[currentIndex];

        if (transform.localEulerAngles.z < 0)
            transform.localEulerAngles.Set(
                transform.localEulerAngles.x, 
                transform.localEulerAngles.y,
                360 + transform.localEulerAngles.z % 360);

        float rotationDistance = 0.0f;

        if (transform.localEulerAngles.z + (360 - destinationAngle) <= 180.0f)
        {
            mRotationDirection = true;
            rotationDistance = -transform.localEulerAngles.z - (360 - destinationAngle);
        }
        else
        {
            mRotationDirection = false;
            rotationDistance = destinationAngle - transform.localEulerAngles.z;
        }
        
        originalAngle = transform.eulerAngles.z;
        
        currentRotationChange = rotationDistance / changeTime;
        
        UpdateSize();
        
        currentState = STATES.CHANGE;
    }
    
    protected override void change()
    {
        if (Mathf.Approximately(currentRotationChange, 0.0f))
        {
            currentState = STATES.STATIONARY;
            
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
