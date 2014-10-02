using UnityEngine;
using System.Collections;

public class HoldableBox : InteractiveObject {
    protected enum direction {
        Left, Right, Up, Down, Static
    };

    //public float k_speed = 0.05f;
    protected bool m_isInUse = false;
    protected direction m_direction;
    protected Player p = null;
    
    public void Start () {
        m_direction = direction.Static;
        //GameObject g = GameObject.FindGameObjectWithTag("Player");
        //p = g.GetComponent<Player>();
    }

    public void Update () {
        transform.localScale = new Vector3(1f,1f,1f);

        if(p == null)
            FindPlayer();
    }

    public override void Interact() {
        collider2D.enabled = false;
        m_isInUse = true;
        
        GameObject[] list;
        list = GameObject.FindGameObjectsWithTag("WeightedPlate");
        foreach(GameObject go in list) {
        	WeightedPlate plateScript = go.GetComponent<WeightedPlate>();
        	if (plateScript != null) {
        		if (!plateScript.m_isLocked)
        			plateScript.activeBox = gameObject;
        	}
        }

        if(p != null)
        {
            p.m_isHoldingBox = true;
            transform.parent = p.transform;
        }
    }
    
    public void Place() { //places the box in front of the player
        //transform.position = p.transform.position;
        transform.localScale = new Vector3(1f,1f,1f);
        collider2D.enabled = true;
        transform.parent = null;
		Vector3 vec = p.transform.eulerAngles;
		vec.z = Mathf.Round(vec.z / 90) * 90;
		transform.eulerAngles = vec;
        m_isInUse = false;

        if(p != null)
            p.m_isHoldingBox = false;
    }
    
    public void Die() {
        Destroy(gameObject);
    }

    private void FindPlayer()
    {
        if(p == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            
            if(player != null)
                p = player.GetComponent<Player>();
        }
    }
}
