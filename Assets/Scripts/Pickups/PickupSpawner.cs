using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour {

    public GameObject overCharge;

	// Use this for initialization
	void Start () {
        InvokeRepeating("SpawnPickup", 20f, 20f);
	}
	
	// Update is called once per frame
	void Update () {

       



    }


    void SpawnPickup()
    {
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(.1f, .1f));
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(.9f, .9f));

        GameObject pickup = (GameObject)Instantiate(overCharge);

        pickup.transform.position = new Vector2(Random.Range(min.x, max.x), max.y);
    }
}
