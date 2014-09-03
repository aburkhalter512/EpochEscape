using UnityEngine;
using System.Collections.Generic;

public class OneTimeTerminal : Terminal
{
    #region Interface Variables
    #endregion

    #region InstanceVariables
    bool mHasInteracted = false;
    #endregion

    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start ()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
    
    public override void Interact()
    {
        if (mHasInteracted)
            return;

        base.Interact();

        mHasInteracted = true;
    }
}
