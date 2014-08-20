using UnityEngine;
using System.Collections;

public class DoorSwapper : MonoBehaviour
{
	#region Inspector Variables
    public GameObject firstDoor;
    public GameObject secondDoor;
	#endregion

	#region Instance Variables
    Door mFirstDoor;
    Door mSecondDoor;

    bool mIsSwapped = false;
	#endregion

	//Put all initialization code here
	//Remember to comment!
	protected void Start()
	{
        mFirstDoor = firstDoor.GetComponent<Door>();
        mFirstDoor.gameObject.SetActive(true);

        mSecondDoor = secondDoor.GetComponent<Door>();
        mSecondDoor.gameObject.SetActive(false);
	}

	#region Initialization Methods
	#endregion

	//Put all update code here
	//Remember to comment!
	protected void Update()
	{
	}
	
	#region Interface Methods
    public void swap()
    {
        if (mIsSwapped)
        {
            mFirstDoor.gameObject.SetActive(true);
            mSecondDoor.gameObject.SetActive(false);
        }
        else
        {
            mFirstDoor.gameObject.SetActive(false);
            mSecondDoor.gameObject.SetActive(true);
        }
    }
	#endregion
}
