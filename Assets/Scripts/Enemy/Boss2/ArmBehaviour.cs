using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmBehaviour : MonoBehaviour {

    public bool readyUp;
    public float direction;
    private bool firstMove = true;
    private bool secondMove;
    private bool second2Move = true;
    private bool thirdMove;
    private bool fourthMove;
    private GameObject player;
    private GameObject waypoint;
    private Vector2 dir;

    public const int repeatAmount = 3;
    private int repeated = 0;
    private Vector2 initialPos;
    // Use this for initialization

    void Start () {
        initialPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (readyUp)
        {

            if (firstMove)
            {
                //rotate
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(90 * direction, Vector3.forward), Time.deltaTime * 40);
       
                if (transform.rotation == Quaternion.AngleAxis(90 * direction, Vector3.forward))
                {
                    firstMove = false;  secondMove = true;

                   
                }
            }
            
            if (secondMove)
            {

                transform.position = Vector2.MoveTowards(transform.position, new Vector2(140f * direction, transform.position.y), Time.deltaTime * 50f);


                if (second2Move)
                {
                    second2Move = false;
                    StartCoroutine(thirdMovement());
                }
              
            }

            if (thirdMove)
            {// needs negative 1 so that each side goes in the opposite direction
             /*
             transform.position = Vector3.MoveTowards(transform.position, new Vector2(140f, player.transform.position.y), Time.deltaTime * 50);
             if(transform.position == new Vector3(140f, player.transform.position.y))
             {
                 thirdMove = false;
             }
             */
                Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(-.65f, -.65f));
                Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1.65f, 1.65f));
                transform.Translate(0, 160*Time.deltaTime, 0, Space.Self);
                if (transform.position.x < min.x || transform.position.x > max.x || transform.position.y < min.y || transform.position.y > max.y) 
                {
                    thirdMove = false;
                    if(repeatAmount > repeated ? true:false)
                    {
                        StartCoroutine(thirdMovement());
                        repeated++;
                    }
                    else
                    {

                    }
                }


            }
            if (fourthMove)
            {
                transform.position = Vector2.MoveTowards(transform.position, initialPos, Time.deltaTime * 60);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(0, Vector3.forward), Time.deltaTime * 40);
                if (transform.rotation == Quaternion.AngleAxis(0, Vector3.forward) && transform.position == new Vector3(initialPos.x, initialPos.y))
                {
                    firstMove = false; secondMove = true;
                }
            }

        }	
	}


    void getPlayer()
    {
       player =  GameObject.Find("Player");
    }

    public IEnumerator thirdMovement()
    {
        yield return new WaitForSeconds(3f);
        getPlayer();
        transform.position = new Vector2(140f * direction, player.transform.position.y);
        thirdMove = true;
   }
    public void setDir(Vector2 dir)
    {
        this.dir = dir;
    }
}
