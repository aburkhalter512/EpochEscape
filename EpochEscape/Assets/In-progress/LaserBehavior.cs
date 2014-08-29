using UnityEngine;
using Vectrosity;
using System.Collections;
using System.Collections.Generic;

public class LaserBehavior : MonoBehaviour
{
	public Transform start, end;
	public float lineWidth;
	public VectorLine vLine;
	public Color color;
	public List<Vector3> positions;
	public GameObject sensor;
	protected int bounces = 0;
	
	void Start ()
	{
		positions = new List<Vector3> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		bounces = 0;
		BuildLaser (start.position);
		DrawLaser ();
		positions.Clear ();
	}
	
	public void BuildLaser (Vector3 origin)
	{
		Vector3 endPos = end.position;
		positions.Add (origin);
		Vector3 reflectionPos = origin.normalized;
		while (true) {
			if (bounces > 20) {
				break;
			}
			RaycastHit2D hit = Physics2D.Raycast (new Vector2 (origin.x, origin.y), (endPos - origin).normalized, 100);
			if (hit.collider == null) {
				positions.Add (endPos);
				break;
			}
			//if hit something
			else {
				//if object is mirror
				positions.Add (new Vector3 (hit.point.x, hit.point.y, 0));
				if (hit.collider.tag == "Mirror") {
					//reflect against the mirror
					reflectionPos = 100 * Vector3.Reflect ((new Vector3 (hit.point.x, hit.point.y, 0) - origin), hit.normal) + new Vector3 (hit.point.x, hit.point.y, 0);
					//positions.Add(new Vector3(hit.point.x, hit.point.y, 0));
					endPos = reflectionPos;
					origin = hit.point;
					bounces++;
				} 
				//if not mirror
				else {
					//if sensor is not null, then continue with sensor stuff
					if (sensor != null) {
						//if has a sensor, and is not hitting sensor or mirror
						if (hit.collider.tag != "Sensor") {
							//set alarm
							sensor.GetComponent<SensorBehavior> ().Alarm ();
						}
					}

					if(hit.collider.tag == "Collector"){
						CollectorColors c = hit.collider.gameObject.GetComponent <CollectorColors>();
						if(c.color == Color.black){
							c.color = color;
						}
						c.Collect();
					}

					if(hit.collider.tag == "Laser Switch"){
						LaserSwitchBehavior ls = hit.collider.gameObject.GetComponent<LaserSwitchBehavior>();
						if(ls.colorMatch == color){
							ls.Activate ();
						}
					}

					break;
				}
			}
		}
	}
	
	public void DrawLaser ()
	{
		vLine = VectorLine.SetLine (color, .05f, positions.ToArray ());
		vLine.lineWidth = lineWidth;
		vLine.Draw ();
	}

	public void SetColor(Color c){
		color = c;
	}
}