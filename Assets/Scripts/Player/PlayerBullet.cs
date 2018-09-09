using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour {


    public float speed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        Vector2 position = transform.position;

        position = new Vector2(position.x, position.y + speed * Time.deltaTime);


        transform.position = position;

        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        if(transform.position.y > max.y)
        {
            Destroy(gameObject);
        }


	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if ((col.tag == "EnemyTag"))
        {
            Destroy(gameObject);
        }

        if ((col.tag == "AsteroidTag"))
        {

            Destroy(gameObject);

        }

    }

}
