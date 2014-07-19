using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class LaserBehavior : MonoBehaviour {
    public Transform start, end;
    public LineRenderer lr;
    public List<Vector3> positions;
    public Vector3 tempStart;
    int count = 2;

	void Start () {
        if (lr == null)
        {
            lr = GetComponent<LineRenderer>();
        }
	}
	
	// Update is called once per frame
	void Update () {
        RenderLaser() ;
	}

    void RenderLaser()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.up, 10000f);
        
        lr.SetPosition(0, start.position);


        if (hit.collider != null)
        {
            lr.SetPosition(1, hit.point);
            if (hit.collider.tag == "Mirror")
            {
                Vector2 reflectionPos = 10000 * Reflect((hit.point - new Vector2(start.position.x, start.position.y)).normalized, hit.normal);
                lr.SetVertexCount(++count);
                lr.SetPosition(count - 1, reflectionPos);
                //reflect it here
            }

        }
        else
        {
            lr.SetPosition(1, end.position);
        }
        //Debug.DrawLine(start.position, hit.point, Color.red);
    }

    private Vector2 Reflect(Vector2 vector, Vector2 normal)
    {
        return vector - 2 * Vector2.Dot(vector, normal) * normal;
    }

}
