using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour {


    public GameObject asteroid;
    public GameObject smartMeteor;

	// Use this for initialization
	void Start () {
        InvokeRepeating("SpawnAsteroid", 1f, 1f);
        InvokeRepeating("SpawnSmartMeteor", 3f, 3f);
	}
	
	// Update is called once per frame
	void Update () {
		
        


	}

    void SpawnAsteroid()
    {
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1.2f));


        GameObject asteroids = (GameObject)Instantiate(asteroid);

        asteroids.transform.position = new Vector2(Random.Range(min.x, max.x), max.y);


    }

    void SpawnSmartMeteor()
    {
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(-.2f, 0));
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1.2f, 1));

        GameObject player = GameObject.Find("Player");

        if (player != null)
        {

            GameObject asteroids = (GameObject)Instantiate(smartMeteor);

            if (Random.Range(0, 10) % 2 == 0) asteroids.transform.position = new Vector2(max.x, Random.Range(min.y, max.y));
            else asteroids.transform.position = new Vector2(min.x, Random.Range(min.y, max.y));


            Vector2 direction = player.transform.position - asteroids.transform.position;

            asteroids.GetComponent<SmartMeteor>().SetDirection(direction);

        }
    }

}
