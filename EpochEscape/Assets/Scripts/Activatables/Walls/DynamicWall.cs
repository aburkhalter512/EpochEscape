using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Xml;

/*
 * This script represents a changing wall, and is thus abstract. This class
 * allows for more different type of changing walls to be added easily.
 */
public abstract class DynamicWall : MonoBehaviour, IActivatable, ISerializable, IIdentifiable
{
    #region Instance Variables
    protected int mCurrentIndex = 0;
    protected float mCurrentChangeTime = 0.0f;

    protected STATE mState;

    private string mID = "";

    private SpriteRenderer mSR;
    #endregion

    #region Class Constants
    public static readonly float CHANGE_TIME = 1.0f;

    public enum STATE
    {
        STATIONARY = 0,
        TO_CHANGE,
        CHANGE
    };
    #endregion

    /*
     * Initializes the Dynamic Wall
     */
    protected void Awake()
    {
        mState = STATE.STATIONARY;
        mSR = GetComponent<SpriteRenderer>();

        getID();
    }

    /*
     * Updates the Dynamic Wall.
     */
    protected void Update()
    {
        switch (mState)
        {
            case STATE.STATIONARY:
                break;
            case STATE.TO_CHANGE:
                toChange();
                break;
            case STATE.CHANGE:
                change();
                break;
        }
    }

    #region Interface Methods
    /*
     * Only activate() does changes the dynamic wall. Both deactivate and toggle are empty methods
     */
    public void activate() { }
    public void deactivate() { }
    public void toggle()
    {
        if (mState != STATE.CHANGE)
            mState = STATE.TO_CHANGE;
    }

    public virtual XmlElement Serialize(XmlDocument document)
    {
        XmlElement wallTag = document.CreateElement("dynamicwall");
        wallTag.SetAttribute("type", GetType().ToString());

        //Transform Component
        wallTag.AppendChild(ComponentSerializer.toXML(transform, document));

        //Sprite Renderer Component
        wallTag.AppendChild(ComponentSerializer.toXML(mSR, document));

        //All Box Collider 2D Components
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        foreach (BoxCollider2D collider in colliders)
            wallTag.AppendChild(ComponentSerializer.toXML(collider, document));

        return wallTag;
    }

    public virtual string getID()
    {
        if (mID == "")
            mID = Utilities.generateUUID(this);

        return mID;
    }

    public virtual void setID(string id)
    {
        if (mID != "")
            mID = id;
    }
    #endregion

    #region Instance Methods
    protected abstract void toChange();
    protected abstract void change();
    #endregion
}
