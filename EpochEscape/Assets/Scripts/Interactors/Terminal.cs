using UnityEngine;
using System.Collections.Generic;

public class Terminal : InteractiveObject, IResettable
{
    #region Interface Variables
    public Sprite activatedSprite;
    public Sprite deactivatedSprited;

    public GameObject[] actuators;
    #endregion

    #region InstanceVariables
    protected bool mIsActivated = false;
    protected bool mCanInteract = false;

    SpriteRenderer mSR;
    #endregion

    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();

        mSR = GetComponent<SpriteRenderer>();
    }
    
    public override void Interact()
    {
        if (mCanInteract)
        {
            foreach (GameObject actuator in actuators)
                CameraManager.AddTransition(actuator);

            CameraManager.PlayTransitions();

            mSR.sprite = (mIsActivated ? deactivatedSprited : activatedSprite);
            mIsActivated = !mIsActivated;
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D collidee)
    {
        Player player = collidee.GetComponent<Player>();

        if (player != null)
            mCanInteract = true;
    }

    public virtual void OnTriggerExit2D(Collider2D collidee)
    {
        Player player = collidee.GetComponent<Player>();

        if (player != null)
            mCanInteract = false;
    }

    public void Reset()
    {
        mCanInteract = false;
        mIsActivated = false;
        
        mSR.sprite = deactivatedSprited;
    }
}
