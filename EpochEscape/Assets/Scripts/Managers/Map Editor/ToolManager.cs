using UnityEngine;
using System.Collections.Generic;

public class ToolManager : Manager<ToolManager>
{
	#region Instance Variables
    IActivatable mActive;

    List<IActivatable> mTools;
    List<IActivatable> mAltTools;

    InputManager mIM;
	#endregion
	
	protected override void Awaken ()
	{
        mTools = new List<IActivatable>();
        mAltTools = new List<IActivatable>();
	}

    protected override void Initialize()
    {
        mIM = InputManager.Get();

        mTools.Add(TilePlacer.Get());
        mAltTools.Add(TileEraser.Get());

        mTools.Add(StaticWallPlacer.Get());
        mAltTools.Add(StaticWallEraser.Get());

        // Padding until there are scripts to fill them
        mTools.Add(DoorPlacer.Get());
        mAltTools.Add(null);

        mTools.Add(ActivatorPlacer.Get());
        mAltTools.Add(null);

        mTools.Add(DynamicWallPlacer.Get());
        mAltTools.Add(null);

        mTools.Add(ObjectSelector.Get());
        mAltTools.Add(null);

        mTools.Add(ObjectPlacer.Get());
        mAltTools.Add(null);
        mTools.Add(null);
        mAltTools.Add(null);
        mTools.Add(null);
        mAltTools.Add(null);

        mTools.Add(ObjectEraser.Get());
        mAltTools.Add(null);

        mTools[0].activate();
        mActive = mTools[0];
    }
	
	protected void Update ()
	{
        IActivatable selected = null;

        if (mIM.toolChanger.get())
        {
            for (int i = 0; i < mTools.Count && i < mIM.toolSelection.Count; i++)
            {
                if (mIM.toolSelection[i].getUp())
                {
                    if (selected != null)
                    {
                        selected = null;
                        break;
                    }

                    if (mIM.alternateInput.get())
                    {
                        selected = mAltTools[i];
                        break;
                    }
                    else
                    {
                        selected = mTools[i];
                        break;
                    }
                }
            }
        }

        if (selected == null)
            return;

        mActive.deactivate();
        mActive = selected;
        mActive.activate();
	}
}
