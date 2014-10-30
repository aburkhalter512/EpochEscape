using UnityEngine;
using Vectrosity;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class LaserBehavior : MonoBehaviour
{
	protected GameObject lastObject;
    public Transform start, end;
    public float lineWidth;
	public LineRenderer lr;
    //public VectorLine vLine;
    public Color color;
	public bool colorChange;
	protected Animator anim;
    public List<Vector3> positions;
    //public GameObject sensor;
    public bool on = true;
    //protected int bounces = 0;
    
    void Start ()
    {
		//VectorLine.SetCamera ();
        positions = new List<Vector3> ();
		anim = GetComponent<Animator> ();
		anim.SetBool ("On", on);
		if (lr == null) {
			lr = GetComponent<LineRenderer>();
		}
    }
    
    // Update is called once per frame
    void Update ()
    {
        if(on){
            //bounces = 0;
            BuildLaser (start.position);
            DrawLaser ();
            positions.Clear ();
        }
    }
    
    public void BuildLaser (Vector3 origin)
    {
        Vector3 endPos = end.position;
        positions.Add (origin);
        Vector3 reflectionPos = origin.normalized;
        while (true) {
//            if (bounces > 20) {
//                break;
//            }
            RaycastHit2D hit = Physics2D.Raycast (new Vector2 (origin.x, origin.y), (endPos - origin).normalized, 100);
            if (hit.collider == null) {
				if(lastObject != null){
					resetLast (null);
				}
                positions.Add (endPos);
                break;
            }
            //if hit something
            else{ 
				detectCollision(hit);
				break;
			}
    	}
	}

	protected void detectCollision(RaycastHit2D hit){
		//if object is mirror
		positions.Add (new Vector3 (hit.point.x, hit.point.y, 0));
//		if (hit.collider.tag == "Mirror") {
//			if(lastObject != null){	
//				resetLast(hit.collider.gameObject);
//			}
//			lastObject = hit.collider.gameObject;
//			//reflect against the mirror
//			reflectionPos = 100 * Vector3.Reflect ((new Vector3 (hit.point.x, hit.point.y, 0) - origin), hit.normal) + new Vector3 (hit.point.x, hit.point.y, 0);
//			endPos = reflectionPos;
//			origin = hit.point;
//			bounces++;
//		} 
		//if Collector
		//else 
		if(hit.collider.name == "Collector"){
			CollectorBehavior cb = hit.collider.gameObject.GetComponent<CollectorBehavior>();
			if(resetLast (cb.gameObject)){
				cb.Activate (color, (EdgeCollider2D)hit.collider);
			}
		}
		//if Laser Switch
		else if(hit.collider.tag == "Laser Switch"){
			LaserSwitchBehavior ls = hit.collider.gameObject.GetComponent<LaserSwitchBehavior>();
			if(resetLast (ls.gameObject)){
				ls.Activate (color);
			}
		}
		else{
			resetLast (hit.collider.gameObject);
		}
	}
    

	protected bool resetLast(GameObject g){
		if (lastObject == null) {
			goto endOfReset;
		}
		else if(g == null){
			goto reset;
		}
		else if (g.GetInstanceID () == lastObject.GetInstanceID ()) {
			return false;
		}
		reset:
		if(lastObject.name == "Collector") {
			lastObject.GetComponent<CollectorBehavior>().resetActivate(color); 
		}
		else if(lastObject.tag == "Laser Switch"){
			lastObject.GetComponent<LaserSwitchBehavior>().resetActivate();
		}
		endOfReset:
		lastObject = g;
		return true;
	}
    
//    public void DrawLaser ()
//    {
//		vLine = VectorLine.SetLine (color, 0.0001f, positions.ToArray ());
//		vLine.lineWidth = lineWidth;
//        vLine.Draw ();
//    }

	public void DrawLaser(){
		lr.SetColors (color,color);
		lr.SetWidth (0.05f, 0.05f);
		if (positions != null && lr != null) {
			lr.SetVertexCount (positions.Count);
			for(int i = 0; i < positions.Count; i++){
				lr.SetPosition (i, positions[i]);
			}
		}
	}
                   
    public void SetColor(Color c){
        color = c;
    }

	public void Activate(){
        on = true;
		anim.SetBool ("On", on);
    }
}