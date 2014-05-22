using UnityEngine;
using System.Collections;

public class PowerCore : MonoBehaviour
{
	#region Inspector Variables
	#endregion

	#region Instance Variables
	#endregion

	#region Class Constants
	#endregion

	//Put all initialization code here
	//Remember to comment!
	protected void Start()
	{
	}

	#region Initialization Methods
	#endregion

	//Put all update code here
	//Remember to comment!
	protected void Update()
	{
	}

	#region Update Methods
    protected void OnTriggerEnter2D(Collider2D collidee)
    {
        if (collidee.tag == "Player")
        {
			AudioSource.PlayClipAtPoint(gameObject.GetComponent<AudioSource>().clip, transform.position);
            collidee.gameObject.GetComponent<Player>().addPowerCore();
            Destroy(gameObject);
        }
    }
	#endregion

	#region Static Methods
	#endregion

	#region Utilities
	#endregion
}
