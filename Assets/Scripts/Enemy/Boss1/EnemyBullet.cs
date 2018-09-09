using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : EnemyProjectile
{

    // Use this for initialization

    public float angle;
    private GameObject o;
  
    void Start ()
    {
        
    }
	
	// Update is called once per frame
	void Update () {
        Vector2 position = transform.position;


        transform.position += transform.right * Time.deltaTime * speed;


        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        if(transform.position.x < min.x || transform.position.x > max.x || transform.position.y < min.y || transform.position.y > max.y)
        {
            Destroy(gameObject);
        }


    }


   

}
