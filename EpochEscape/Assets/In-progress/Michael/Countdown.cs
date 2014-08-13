using UnityEngine;
using System.Collections;

public class Countdown : MonoBehaviour {
	public float startTime;
	public float restSeconds;
	private int roundedRestSeconds;
	private float displaySeconds;
	private float displayMinutes;
	public int CountDownSeconds = 30;
	private float Timeleft;
	string timetext;
	
	
	// Use this for initialization
	
	void Start () 
	{
		startTime=Time.realtimeSinceStartup;
		
	}
	
	void OnGUI()
	{
		
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
	}}