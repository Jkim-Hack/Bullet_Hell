using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    private float rateOfFire = .5f;
    private float timebtwnShots = 2f;
    public GameObject EnemyBullet01;

    private int i;
    public GameObject[] positionsBullet01 = new GameObject[4];

    // Use this for initialization
    void Start () {
        InvokeRepeating("FireB", 0f, 5f);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator fireBullets01()
    {

        FireBullets01();
        yield return new WaitForSeconds(rateOfFire);
        FireBullets01();
        yield return new WaitForSeconds(rateOfFire);
        FireBullets01();


    }

    void FireB()
    {
        StartCoroutine(fireBullets01());
    }

    void FireBullets01()
    {
        GameObject player = GameObject.Find("Player");

        if (player != null)
        {
            i = Random.Range(1, 4);
            GameObject clone = Instantiate(EnemyBullet01, transform.position, transform.rotation) as GameObject;
            clone.transform.position = positionsBullet01[i].transform.position;
            Vector2 dir = player.transform.position - positionsBullet01[i].transform.position;
            clone.GetComponent<EnemyBullet01>().SetDirection(dir);
            //print("Player Transform - " + player.transform.position + " Bullet Transform - " + positionsBullet01[i].transform.position + " Direction - " + dir);
           
        }

    }
}
