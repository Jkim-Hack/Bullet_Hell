using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet02 : EnemyProjectile {

    private float angle;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = q;

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

    public void setAngle(float angle)
    {
        this.angle = angle;
    }
  
}
