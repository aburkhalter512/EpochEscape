using UnityEngine;
using System.Collections;

public class CoroutineManager : Manager<CoroutineManager>
{
	protected override void Awaken ()
	{ }
	
	protected override void Initialize ()
	{ }
	
	#region Interface Methods
    public void StartCoroutine(IEnumerator enumerator)
    {
        base.StartCoroutine(enumerator);
    }
	#endregion
}
