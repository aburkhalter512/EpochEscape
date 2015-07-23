using UnityEngine;
using System.Collections.Generic;

namespace Game
{
    public class Terminal : Activator, IInteractable, IResettable, ISerializable
    {
        #region Interface Variables
        public Sprite activatedSprite;
        public Sprite deactivatedSprited;
        #endregion

        #region InstanceVariables
        protected bool mIsActivated = false;
        protected bool mCanInteract = false;

        SpriteRenderer mSR;
        #endregion

        // Use this for initialization
        protected void Awake()
        {
            mSR = GetComponent<SpriteRenderer>();

            populateActivatables();
        }

        public virtual void Interact()
        {
            if (mCanInteract)
            {
                trigger();

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
}
