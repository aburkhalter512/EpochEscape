using UnityEngine;
using System;
using System.Collections.Generic;

public class ToolManager : Manager<ToolManager>, IActivatable
{
	#region Instance Variables
    IActivatable mActive;

    TileManipulator _tiler;
    ObjectManipulator _placer;

    InputManager mIM;
	#endregion
	
	protected override void Awaken ()
	{
	}

    protected override void Initialize()
    {
        _placer = ObjectManipulator.Get();
        _placer.activate();

        _tiler = TileManipulator.Get();
        _tiler.deactivate();

        mIM = InputManager.Get();

        mActive = _placer;
    }
	
	protected void Update ()
	{
        bool didChange = false;
        if (mIM.toolChanger.get())
        {
            if (mActive != null)

            if (mIM.toolSelection[0].getUp())
            {
                mActive.deactivate();
                didChange = true;

                mActive = _placer;
            }
            else if (mIM.toolSelection[1].getUp())
            {
                mActive.deactivate();
                didChange = true;

                mActive = _tiler;
            }

            if (didChange)
                mActive.activate();
        }
	}

    public void activate()
    {
        if (mActive == null)
            return;

        mActive.activate();
    }

    public void deactivate()
    {
        if (mActive == null)
            return;

        mActive.deactivate();
    }
}
