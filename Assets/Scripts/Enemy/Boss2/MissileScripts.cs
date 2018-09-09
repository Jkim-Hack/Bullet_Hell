using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileScripts : EnemyProjectile {

    private Vector3 positionTo;
    public ParticleSystem smokeTrail;
    private bool firstMove = true;
    public bool afterFirst;
    public bool onReady;
    float floatingStrength;
    private bool moveOne;

    // Use this for initialization
    void Start () {
        var emission = smokeTrail.emission;
        emission.enabled = false;
        speed = 1f;
        floatingStrength = Random.Range(.5f, 1f);
        moveOne = true; 
    }
	
	// Update is called once per frame
	void Update () {
        if (firstMove)
        {
            Debug.Log(positionTo);
            transform.position = Vector2.MoveTowards(transform.position, positionTo, Time.deltaTime * 60);
            if(transform.position == positionTo)
            {
                afterFirst = true;
                firstMove = false;
            }
        }
        else if (afterFirst)
        {
           
            Vector3 pos = transform.position;
            float newY = (Mathf.Sin(Time.time * floatingStrength) * .05f) + pos.y;
            transform.position = new Vector3(pos.x, newY, pos.z);
        }

        if (onReady)
        {
            afterFirst = false;
            var emission = smokeTrail.emission;
            emission.enabled = true;
            if (moveOne)
            {
                CalcDirection(GameObject.Find("Player").transform.position);
                moveOne = false;
            }
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2);
            if(transform.rotation == rotation)
            {
                Vector2 pos = transform.position;
                pos += direction * speed * Time.deltaTime;
                transform.position = pos;
            }
            
           
        }
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(-.3f, -.3f));
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1.3f, 1.3f));

        if (transform.position.x < min.x || transform.position.x > max.x || transform.position.y < min.y || transform.position.y > max.y)
        {
            Destroy(gameObject);
        }
    }

    public void AcquirePosition(Vector2 pos)
    {
        this.positionTo = pos;
    }

    public void CalcDirection(Vector3 target)
    {
        direction = target - transform.position;//direction towards player
    }
}
