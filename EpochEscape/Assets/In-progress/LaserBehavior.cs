using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class LaserBehavior : MonoBehaviour {
    public Transform start, end;
    public LineRenderer lr;
	void Start () {
        if (lr == null)
            lr = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        RenderLaser() ;
	}

    void RenderLaser()
    {
        RaycastHit2D hit;

        hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.up, 10000f);

        lr.enabled = true;
        lr.SetPosition(0, start.position);
        lr.SetPosition(1, hit.point);
        Debug.DrawLine(start.position, hit.point, Color.red);
    }
}
