using UnityEngine;
using System.Collections;

public class BoxBehavior : MonoBehaviour
{
    public bool canMove = true;
    public float speed = .05f;
    //public CharacterController2D controller;
    private enum direction
    {
        Left, Right, Up, Down, Static
	};

    public Vector3 nextPos;

    private direction dir;
    // Use this for initialization
    void Start()
    {
        //controller = GetComponent<CharacterController2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove)
        {
            Move();
        }
    }

    void Move()
    {
        switch (dir)
            {
                case direction.Left:
                    transform.Translate(Vector3.left * speed * Time.deltaTime);
                    if (transform.position.x <= nextPos.x)
                    {
                        transform.position = new Vector3(nextPos.x, transform.position.y, transform.position.z);
                        canMove = true;
                    }
                    break;
                case direction.Right:
                    transform.Translate(Vector3.right * speed * Time.deltaTime);
                    if (transform.position.x >= nextPos.x)
                    {
                        transform.position = new Vector3(nextPos.x, transform.position.y, transform.position.z);
                        canMove = true;
                    }
                    break;
                case direction.Up:
                    transform.Translate(Vector3.up * speed * Time.deltaTime);
                    if (transform.position.y >= nextPos.y)
                    {
                        transform.position= new Vector3(transform.position.x, nextPos.y, transform.position.z);
                        canMove = true;
                    }
                    break;
                case direction.Down:
                    transform.Translate(Vector3.down * speed * Time.deltaTime);
                    if (transform.position.y <= nextPos.y)
                    {
                        transform.position = new Vector3(transform.position.x, nextPos.y, transform.position.z);
                        canMove = true;
                    }
                    break;
            }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && canMove)
        {
            
            float left = renderer.bounds.min.x;
            float right = renderer.bounds.max.x;
            float top = renderer.bounds.max.y;
            float bottom = renderer.bounds.min.y;
            Vector3 pos = new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z);
            if ((other.renderer.bounds.max.x) > left && //inside
                (other.renderer.bounds.max.x) < right && //less than right side
                other.transform.position.x < transform.position.x) // left of
            {
                pos.x = left;
            }

            else if ((other.renderer.bounds.min.x) > right && //greater than right
                (other.renderer.bounds.max.x) > left && //greater than leftside
                other.transform.position.x > transform.position.x) // right of
            {
                pos.x = right;
            }

            else if ((other.renderer.bounds.max.x) > bottom && //inside
                (other.renderer.bounds.max.x) < top && //less than right side
                other.transform.position.y < transform.position.y) // left of
            {
                pos.y = bottom;
            }

            else if ((other.renderer.bounds.max.x) > top && //inside
                (other.renderer.bounds.max.x) > bottom && //less than right side
                other.transform.position.y > transform.position.y) // left of
            {
                pos.y = top;
            }
            other.transform.position = pos;

            if ((other.transform.position.x < left) && ((other.transform.position.y < top) || (other.transform.position.y > bottom)))
            {
                nextPos = new Vector3((transform.position.x + .2f), transform.position.y, transform.position.z);
                dir = direction.Right;
            }

            else if ((other.transform.position.x > right) && ((other.transform.position.y < top) || (other.transform.position.y > bottom)))
            {
                nextPos = new Vector3((transform.position.x - .2f), transform.position.y, transform.position.z);
                dir = direction.Left;
            }

            else if ((other.transform.position.y > top) && ((other.transform.position.x > left) || (other.transform.position.x < right)))
            {
                nextPos = new Vector3(transform.position.x, (transform.position.y - .2f), transform.position.z);
                dir = direction.Down;
            }

            else if ((other.transform.position.y < bottom) && ((other.transform.position.x > left) || (other.transform.position.x < right)))
            {
                nextPos = new Vector3(transform.position.x, (transform.position.y + .2f), transform.position.z);
                dir = direction.Up;
            }

            canMove = false;
        }//end if
    }//end void

}
