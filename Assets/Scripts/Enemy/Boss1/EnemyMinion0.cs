using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMinion0 : MonoBehaviour {

    private Vector3 playerPos;
    public GameObject BulletPos01;
    public GameObject BulletPos02;
    private Vector2 dir;
    private float angle;
    public GameObject Bullet;

    // Use this for initialization
    void Start () {
        InvokeRepeating("Shoot", .1f, .4f);
	}
	
	// Update is called once per frame
	void Update () {
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = q;
        Debug.Log(q);
        Vector2 pos = transform.position;
        pos += dir * .5f * Time.deltaTime;
        transform.position = pos;
        Vector2 min1 = Camera.main.ViewportToWorldPoint(new Vector2(-.4f, -.4f));
        Vector2 max1 = Camera.main.ViewportToWorldPoint(new Vector2(1.4f, 1.4f));
        if (transform.position.x < min1.x || transform.position.x > max1.x || transform.position.y < min1.y || transform.position.y > max1.y)
        {
            Destroy(gameObject);
        }
    }

    void Shoot()
    {

        GameObject bullet01 = (GameObject)Instantiate(Bullet);
        bullet01.GetComponent<EnemyBullet02>().GetComponent<EnemyProjectile>().speed = 1f;
        bullet01.transform.position = BulletPos01.transform.position;
        bullet01.GetComponent<EnemyBullet02>().GetComponent<EnemyProjectile>().direction = dir;
        bullet01.GetComponent<EnemyBullet02>().setAngle(angle + 90);


        GameObject bullet02 = (GameObject)Instantiate(Bullet);
        bullet02.GetComponent<EnemyBullet02>().GetComponent<EnemyProjectile>().speed = 1f;
        bullet02.transform.position = BulletPos02.transform.position;
        bullet02.GetComponent<EnemyBullet02>().GetComponent<EnemyProjectile>().direction = dir;
        bullet02.GetComponent<EnemyBullet02>().setAngle(angle + 90);
    }
    public void SetDirection(Vector2 direction)
    {
        dir = direction;
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle -= 270;
    }

}
