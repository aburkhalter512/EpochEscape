using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CoroutineManager : Manager<CoroutineManager>
{
    public static readonly int yieldCount = 128;
    public const float yieldTime = 1f;

	protected override void Awaken ()
	{ }
	
	protected override void Initialize ()
	{ }
	
	#region Interface Methods
    public void delay(Action action, float delay = yieldTime)
    {
        StartCoroutine(delayHelper(action, delay));
    }
	#endregion

    #region Instance Methods
    protected IEnumerator delayHelper(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);

        action();
    }
    #endregion
}
