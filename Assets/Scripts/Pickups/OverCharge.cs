using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverCharge : PickUp {

    public float speed = 10;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, Time.deltaTime * 100, 0);
        Vector2 position = transform.position;

        position = new Vector2(position.x, position.y - speed * Time.deltaTime);

        transform.position = position;
    }
}
