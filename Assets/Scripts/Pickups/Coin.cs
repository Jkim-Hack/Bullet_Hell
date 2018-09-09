using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : PickUp
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTriggerEnter2D(Collider2D col)
    {

        if (col.tag.Contains("PlayerTag"))
        {
            GUIDelegate.updateCoinUI(1);
            Destroy(gameObject);
        }
    }
}
