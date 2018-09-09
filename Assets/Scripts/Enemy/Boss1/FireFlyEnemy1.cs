using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFlyEnemy1 : MonoBehaviour {
    
    public GameObject laser;
    private bool isDone = false;
    private Vector2 initial;
    private bool isSet = false;
    private Vector2 target;
    // Use this for initialization
    void Start()
    {
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0.4f, 0f));
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(.6f, 1f));
        target = new Vector2(Random.Range(min.x, max.x), transform.position.y);

        initial = transform.position;
        StartCoroutine(wait());

    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, target) <= 0)
        {
            isDone = true;

        }

        if (isSet)
        {
            Vector2 min1 = Camera.main.ViewportToWorldPoint(new Vector2(-.1f, -.1f));
            Vector2 max1 = Camera.main.ViewportToWorldPoint(new Vector2(1.2f, 1.2f));
            transform.position = Vector2.MoveTowards(transform.position, initial, Time.deltaTime * 20);
            if (transform.position.x < min1.x || transform.position.x > max1.x)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * 20);
        }

    }

    IEnumerator wait()
    {
        yield return new WaitUntil(() => isDone);
        yield return new WaitForSeconds(2f);
        laser.GetComponent<Laser>().getRenderer().enabled = true;
       
        yield return new WaitForSeconds(3f);
        laser.GetComponent<Laser>().getRenderer().enabled = false;
        isSet = true;
    }
   
}
