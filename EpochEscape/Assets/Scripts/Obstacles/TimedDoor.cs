using UnityEngine;
using System.Collections;

/*
 * A script that can emulate the opening and closing of a door.
 * There are two primary states: opened and closed.
 * 
 * It automatically generates necessary colliders on the game
 * object, so it is not recommended to manually put them on it
 * via Unity's editor. It may cause issues with the opening and
 * closing, but this has not been tested.
 */
public class TimedDoor : Obstacle
{
	#region Inspector Variables
	public STATE currentState;
	
	public Sprite closedDoor;
	public Sprite openedDoor;
	
	public Vector2 mSize;
	#endregion
	
	#region Instance Variables
	private STATE mPreviousState;
	
	private BoxCollider2D mMainCollider = null;
	
	private SpriteRenderer mSR;
	#endregion
	
	public bool isTimerActive = false;
	private float startTime;
	private float restSeconds;
	private int roundedRestSeconds;
	private float displaySeconds;
	private float displayMinutes;
	public int CountDownSeconds = 30;
	private float Timeleft = 1;
	string timetext;
	
	#region Class Constants
	public enum STATE
	{
		OPENED,
		CLOSED,
		UN_INIT,
		DESTROY
	}
	#endregion
	
	/*
     * Initializes the door
     */
	protected virtual void Start()
	{
		mPreviousState = STATE.UN_INIT;
		
		mSR = gameObject.GetComponent<SpriteRenderer>();
	}
	
	/*
     * Updates the state of the door
     */
	protected virtual void Update()
	{
		if (isTimerActive && restSeconds <= 0) {
			isTimerActive = false;
			currentState = STATE.CLOSED;
		}
		
		if (mPreviousState != currentState)
		{
			switch (currentState)
			{
			case STATE.UN_INIT:
				UnInit();
				break;
			case STATE.CLOSED:
				Close();
				break;
			case STATE.OPENED:
				Open();
				break;
			case STATE.DESTROY:
				Destroy();
				break;
			}
			
			mPreviousState = currentState;
		}
	}
	
	void OnGUI() {
		if (isTimerActive) {
			Timeleft= Time.time-startTime;
			
			restSeconds = CountDownSeconds-(Timeleft);
			
			roundedRestSeconds=Mathf.CeilToInt(restSeconds);
			displaySeconds = roundedRestSeconds % 60;
			displayMinutes = (roundedRestSeconds / 60)%60;
			
			timetext = (displayMinutes.ToString()+":");
			if (displaySeconds > 9)
			{
				timetext = timetext + displaySeconds.ToString();
			}
			else 
			{
				timetext = timetext + "0" + displaySeconds.ToString();
			}
			GUI.Label(new Rect(650.0f, 0.0f, 100.0f, 75.0f), timetext);
		}
	}
	
	#region Update Methods
	/*
     * A blank method to satisfy the state machine
     */
	protected virtual void UnInit()
	{
	}
	
	/*
     * "Closes" the door by changing the sprite of the gameobject and by
     * adding specified 2D colliders.
     */
	protected virtual void Close()
	{
		//Do the colliders exist?
		if (mMainCollider == null)
		{
			gameObject.AddComponent<BoxCollider2D>();
			mMainCollider = gameObject.GetComponent<BoxCollider2D>();
		}
		
		//Update collider variables
		mMainCollider.size = mSize;
		mMainCollider.center = Vector2.zero;
		
		mSR.sprite = closedDoor;
	}
	
	protected virtual void Open()
	{
		//Do the colliders exist?
		if (mMainCollider != null)
		{
			Object.Destroy(mMainCollider);
			mMainCollider = null;
		}
		
		mSR.sprite = openedDoor;
	}
	
	protected virtual void Destroy()
	{
		Destroy(gameObject);
	}
	#endregion
	
	#region Interface Methods
	/*
     * Toggles between opened and closed. The door will automatically change
     * to the next state when this method is called.
     */
	public STATE Toggle()
	{
		switch (currentState)
		{
		case STATE.CLOSED:
			currentState = STATE.OPENED;
			break;
		case STATE.OPENED:
			currentState = STATE.CLOSED;
			break;
		}
		
		startTime = Time.realtimeSinceStartup;
		isTimerActive = true;
		return currentState;
	}
	#endregion
}
