using UnityEngine;
using System.Collections;
using System.Xml;

namespace Game
{
    public class TeleporterDoorFrame : DoorFrame
    {
        #region Interface Variables
        public GameObject teleportTarget;

        public GameObject spawn;
        #endregion

        #region Instance Variables
        protected TeleporterDoorFrame mTeleportTarget;

        protected Vector3 mSpawn;

        protected BoxCollider2D mCollider;
        protected Vector2 mBaseOffset;
        protected Vector2 mBaseSize;

        protected bool mCanTeleport = true;
        #endregion

        protected new void Start()
        {
            base.Start();

            if (teleportTarget != null)
                mTeleportTarget = teleportTarget.GetComponent<TeleporterDoorFrame>();

            mCollider = GetComponent<BoxCollider2D>();
            mBaseOffset = mCollider.offset;
            mBaseSize = mCollider.size;

            mSpawn = spawn.transform.position;
        }

        #region Interface Methods
        /**
     *  void triggerFrontEnter()
     *      A method that alerts the door that a gameobject has entered the front detection area.
     */
        public override void triggerFrontEnter()
        {
            return;
        }

        /**
         *  void triggerFrontExit()
         *      A method that alerts the door that a gameobject has exited the front detection area.
         */
        public override void triggerFrontExit()
        {
            return;
        }

        /**
         *  void triggerBackEnter()
         *      A method that alerts the door that a gameobject has entered the back detection area.
         */
        public override void triggerBackEnter()
        {
            return;
        }

        /**
         *  void triggerBackExit()
         *      A method that alerts the door that a gameobject has exited the back detection area.
         */
        public override void triggerBackExit()
        {
            return;
        }

        public override void activate()
        {
            mFrontSide.activate();
            mBackSide.deactivate();
            mState = STATE.ACTIVE;
        }
        public override void deactivate()
        {
            mFrontSide.deactivate();
            mBackSide.deactivate();
            mState = STATE.INACTIVE;
        }

        public void setTarget(TeleporterDoorFrame target)
        {
            if (target == null)
                return;

            mTeleportTarget = target;
        }

        public Vector3 getSpawnPosition()
        {
            return mSpawn;
        }

        public void teleport(Player player)
        {
            if (player == null || mTeleportTarget == null || mState == STATE.INACTIVE)
                return;

            // Woohoo! Lambda functions!
            GameManager.Get().delayFunction(() =>
                {
                    mCanTeleport = false;

                    mCollider.size = new Vector2(mBaseSize.x, mBaseSize.y * 2);
                    mCollider.offset = new Vector2(mBaseOffset.x, mBaseOffset.y + mBaseSize.y / 2);

                    player.transform.position = getSpawnPosition();
                });
        }

        public override IEnumerator serialize(XmlDocument document, System.Action<XmlElement> callback)
        {
            base.serialize(document, (XmlElement doorTag) =>
                {
                    doorTag.SetAttribute("targetID", mTeleportTarget.getID());

                    callback(doorTag);
                });

            return null; ;
        }
        #endregion

        #region Instance Methods
        protected virtual void OnTriggerEnter2D(Collider2D collidee)
        {
            Player player = collidee.GetComponent<Player>();

            if (player != null && mTeleportTarget != null && mState == STATE.ACTIVE && mCanTeleport)
                mTeleportTarget.teleport(player);
        }

        protected virtual void OnTriggerExit2D(Collider2D collidee)
        {
            Player player = collidee.GetComponent<Player>();

            if (player != null)
            {
                mCanTeleport = true;

                mCollider.size = new Vector2(mBaseSize.x, mBaseSize.y);
                mCollider.offset = new Vector2(mBaseOffset.x, mBaseOffset.y);
            }
        }
        #endregion
    }
}
