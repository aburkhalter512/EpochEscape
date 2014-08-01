using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class LaserBehavior : MonoBehaviour {
    public Transform start, end;
    public LineRenderer lr;
    public List<Vector3> positions;
    public Vector3 tempStart;
    private int bounces = 0;

	void Start () {
		positions = new List<Vector3> ();
        if (lr == null)
        {
            lr = GetComponent<LineRenderer>();
        }
	}
	
	// Update is called once per frame
	void Update () {
        bounces = 0;
        BuildLaser(transform.position);
        DrawLaser();
        positions.Clear();
	}

//    void BuildLaser(Vector3 origin, int bounces)
//    {
//        if (bounces > 20)
//        {
//            return;
//        }
//
//		RaycastHit2D hit = Physics2D.Raycast(new Vector2(origin.x, origin.y), (end.position - origin).normalized, 10000f);
//        Vector3 reflectionPos = origin.normalized * 10000;
//
//        if (hit.collider == null)
//        {
//            positions.Add(end.position);
//            return;
//        }
//
//        //if hit mirror
//        else if (hit.collider != null)
//        {
//            //add hit point (where to stop the laser)
//            positions.Add(new Vector3(hit.point.x, hit.point.y, 0));
//            //if object is mirror
//            if (hit.collider.tag == "Mirror")
//            {
//                //reflect against the mirror
//                reflectionPos = 10000 * Vector3.Reflect((new Vector3(hit.point.x, hit.point.y, 0) - origin), hit.normal) + new Vector3(hit.point.x, hit.point.y, 0);
//                positions.Add(reflectionPos);
//            }
//        }
//
//        BuildLaser(hit.point, bounces + 1);
//        //Debug.DrawLine(start.position, hit.point, Color.red);
//    }

	void BuildLaser(Vector3 origin)
	{
        positions.Add(origin);
		Vector3 reflectionPos = origin.normalized;
        Vector3 endPos = end.position;
		while (true)
		{
			if(bounces > 20){
				break;
			}
			RaycastHit2D hit = Physics2D.Raycast(new Vector2(origin.x, origin.y), (endPos - origin).normalized, 100);
			if (hit.collider == null)
			{
                positions.Add(endPos);
				break;
			}
			//if hit something
			else{
				//if object is mirror
                positions.Add(new Vector3(hit.point.x, hit.point.y, 0));
				if (hit.collider.tag == "Mirror")
				{
					//reflect against the mirror
                    reflectionPos = 100 * Vector3.Reflect((new Vector3(hit.point.x, hit.point.y, 0) - origin), hit.normal) + new Vector3(hit.point.x, hit.point.y, 0);
                    //positions.Add(new Vector3(hit.point.x, hit.point.y, 0));
					endPos = reflectionPos;
					origin = hit.point;
					bounces++;
				}
				else{
                    
                    break;
				}
			}
		}
		
		//BuildLaser(hit.point, bounces + 1);
		//Debug.DrawLine(start.position, hit.point, Color.red);
	}

    void DrawLaser()
    {
        lr.SetWidth(.075f, .075f);
        if (positions != null || lr != null)
        {
            lr.SetVertexCount(positions.Count);
            for (int i = 0; i < positions.Count; i++)
            {
                lr.SetPosition(i, positions[i]);
            }
        }
    }
}
