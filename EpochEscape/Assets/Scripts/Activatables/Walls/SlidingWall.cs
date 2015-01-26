using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlidingWall : DynamicWall, ISerializable
{
    #region Inspector Variables
    public GameObject[] targets;
    #endregion

    #region Instance Variables
    public Vector3[] mTargets;
    private Vector3 mPositionDelta = Vector3.zero;
    private Vector3 mBasePosition;
    #endregion

    //Put all initialization code here
    //Remember to comment!
    protected new void Awake()
    {
        base.Awake();

        for (int i = 0; i < targets.Length; i++)
            mTargets[i] = targets[i].transform.position;

        mBasePosition = transform.position;
    }

    #region Update Methods
    protected override void toChange()
    {
        audio.Play ();
        currentIndex = (currentIndex + 1) % mTargets.Length;

        mPositionDelta = mTargets[currentIndex] + mBasePosition - transform.position;
        mPositionDelta /= changeTime;

        UpdateSize();

        mState = STATE.CHANGE;
    }

    protected override void change()
    {
        if (Utilities.areClose(mPositionDelta, Vector3.zero))
        {
            mState = STATE.STATIONARY;

            return;
        }

        if (Utilities.isBounded(0.0f, mPositionDelta.sqrMagnitude * Time.smoothDeltaTime * Time.smoothDeltaTime,
                      (transform.position - (mTargets[currentIndex] + mBasePosition)).sqrMagnitude))
        {
            transform.position = mTargets[currentIndex] + mBasePosition;
            mPositionDelta = Vector3.zero;
        }
        else
            transform.position += mPositionDelta * Time.smoothDeltaTime;

    }
    #endregion

    public void Serialize(ref Dictionary<string, object> data)
    {
        if (data != null)
        {
            if (!data.ContainsKey("changeTime"))
                data["changeTime"] = changeTime;

            if(!data.ContainsKey("positionPoints"))
                data["positionPoints"] = mTargets;
        }
    }

    public void Unserialize(ref Dictionary<string, object> data)
    {
        if (data != null)
        {
            if(data.ContainsKey("changeTime"))
                changeTime = (int)((long)data["changeTime"]);

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
