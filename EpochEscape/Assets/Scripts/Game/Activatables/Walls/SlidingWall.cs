﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Game
{
    public class SlidingWall : DynamicWall
    {
        #region Inspector Variables
        public GameObject[] targets;
        #endregion

        #region Instance Variables
        protected Vector3[] mTargets;

        protected Vector3 mBasePosition;
        protected Vector3 mDestinationPosition;
        #endregion

        //Put all initialization code here
        //Remember to comment!
        protected new void Awake()
        {
            base.Awake();

            if (targets != null)
            {
                mTargets = new Vector3[targets.Length + 1]; //+1 To remember the original rotation
                mTargets[0] = transform.position;
                for (int i = 0; i < targets.Length; i++)
                {
                    mTargets[i + 1] = targets[i].transform.position;
                    GameObject.Destroy(targets[i]);
                    targets[i] = null;
                }
            }
        }

        #region Interface Methods
        public void setSlidingTargets(Vector3[] vecTargets)
        {
            if (vecTargets == null)
                return;

            mTargets = new Vector3[vecTargets.Length + 1]; //+1 To remember the original rotation
            mTargets[0] = transform.position;
            for (int i = 0; i < vecTargets.Length; i++)
                mTargets[i + 1] = vecTargets[i];
        }

        public override IEnumerator serialize(XmlDocument document, System.Action<XmlElement> callback)
        {
            base.serialize(document, (XmlElement wallTag) =>
                {
                    for (int i = 1; i < mTargets.Length; i++) // Start at 1 to skip the base angle
                    {
                        XmlElement child = document.CreateElement("target");
                        child.SetAttribute("position", mTargets[i].ToString());

                        wallTag.AppendChild(child);
                    }

                    callback(wallTag);
                });

            return null;
        }
        #endregion

        #region Instance Methods
        protected override void toChange()
        {
            GetComponent<AudioSource>().Play();
            mCurrentIndex = (mCurrentIndex + 1) % mTargets.Length;

            mBasePosition = transform.position;
            mDestinationPosition = mTargets[mCurrentIndex];
            mCurrentChangeTime = 0.0f;

            mState = STATE.CHANGE;
        }

        protected override void change()
        {
            mCurrentChangeTime += Time.smoothDeltaTime;

            if (mCurrentChangeTime >= CHANGE_TIME)
            {
                transform.position = mDestinationPosition;

                mState = STATE.STATIONARY;

                return;
            }

            transform.position = Vector3.Lerp(mBasePosition, mDestinationPosition, mCurrentChangeTime / CHANGE_TIME);
        }
        #endregion
    }
}
