using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet01 : EnemyProjectile
{
    private float p;
    private GameObject l;
  
    // Use this for initialization
    void Start () {
       
        

        
    }
	
	// Update is called once per frame
	void Update()
    {
        Vector2 pos = transform.position;
        pos += direction * speed * Time.deltaTime;
        transform.position = pos;
        

        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        if (transform.position.x < min.x || transform.position.x > max.x || transform.position.y < min.y || transform.position.y > max.y)
        {
            Destroy(gameObject);
        }

    }

    

    public void SetDirection(Vector2 _dir)
    {
        direction = _dir.normalized;
        print(direction);
    }

}
