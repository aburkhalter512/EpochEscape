using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlidingWall : DynamicWall, ISerializable
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

        mTargets = new Vector3[targets.Length + 1]; //To remember the original rotation
        mTargets[0] = transform.position;
        for (int i = 0; i < targets.Length; i++)
        {
            mTargets[i + 1] = targets[i].transform.position;
            GameObject.Destroy(targets[i]);
            targets[i] = null;
        }
    }

    #region Instance Methods
    protected override void toChange()
    {
        audio.Play ();
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

    public void Serialize(ref Dictionary<string, object> data)
    {
        if (data != null)
        {
            if(!data.ContainsKey("positionPoints"))
                data["positionPoints"] = mTargets;
        }
    }

    public void Unserialize(ref Dictionary<string, object> data)
    {
        if (data != null)
        {
            if (data.ContainsKey("positionPoints"))
            {
                List<object> points = data["positionPoints"] as List<object>;

                if (points != null && points.Count > 0)
                {
                    mTargets = new Vector3[points.Count];

                    for (int i = 0; i < points.Count; i++)
                        mTargets[i] = LevelEditorUtilities.StringToVector3(points[i] as string);
                }
            }
        }
    }
}
