﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlidingWall : DynamicWall, ISerializable
{
    #region Inspector Variables
    public Vector3[] positionPts;
    #endregion

    #region Instance Variables
    private Vector3 positionDelta = Vector3.zero;
    private Vector3 basePosition;
    #endregion

    //Put all initialization code here
    //Remember to comment!
    protected new void Start()
    {
        base.Start();

        basePosition = transform.position;
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
       //Emtpy Method
    }

    protected override void toChange()
    {
        audio.Play ();
        currentIndex = (currentIndex + 1) % positionPts.Length;

        positionDelta = positionPts[currentIndex] + basePosition - transform.position;
        positionDelta /= changeTime;

        UpdateSize();

        currentState = STATES.CHANGE;
    }

    protected override void change()
    {
        if (Utilities.areClose(positionDelta, Vector3.zero))
        {
            currentState = STATES.STATIONARY;

            return;
        }

        if (Utilities.isBounded(0.0f, positionDelta.sqrMagnitude * Time.smoothDeltaTime * Time.smoothDeltaTime,
                      (transform.position - (positionPts[currentIndex] + basePosition)).sqrMagnitude))
        {
            transform.position = positionPts[currentIndex] + basePosition;
            positionDelta = Vector3.zero;
        }
        else if (!Utilities.areClose(positionDelta, Vector3.zero))
        {
            transform.position += positionDelta * Time.smoothDeltaTime;
        }

    }
    #endregion

    public void Serialize(ref Dictionary<string, object> data)
    {
        if (data != null)
        {
            if (!data.ContainsKey("changeTime"))
                data["changeTime"] = changeTime;

            if(!data.ContainsKey("positionPoints"))
                data["positionPoints"] = positionPts;
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
                    positionPts = new Vector3[points.Count];

                    for (int i = 0; i < points.Count; i++)
                        positionPts[i] = LevelEditorUtilities.StringToVector3(points[i] as string);
                }
            }
        }
    }
}
