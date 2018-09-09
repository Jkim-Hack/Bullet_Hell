using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwoDHomingMissiles;

public class AsteroidSpawner2 : MonoBehaviour
{

    public GameObject TargetPosition0;
    public GameObject TargetPosition1;
    public GameObject TargetPosition2;
    public GameObject smartRocket;
    Vector2 min;
    Vector2 max;


    // Use this for initialization
    void Start()
    {
        min = Camera.main.ViewportToWorldPoint(new Vector2(-.2f, 0));
        max = Camera.main.ViewportToWorldPoint(new Vector2(1.2f, 1));


       // InvokeRepeating("StartInvokes", 30f, 30f);

    }

    // Update is called once per frame
    void Update()
    {


    }

    void SpawnSmartMeteor()
    {




        if (TargetPosition0 != null)
        {
            //variable name suppose to be rocket LUL
           // GameObject asteroids = (GameObject)Instantiate(smartRocket);

            //asteroids.GetComponent<MissileController>().target = TargetPosition0;

           

        }





        if (TargetPosition1 != null)
        {

          //  GameObject asteroids = (GameObject)Instantiate(smartRocket);

          //  asteroids.GetComponent<MissileController>().target = TargetPosition1;

        }





        if (TargetPosition2 != null)
        {

           // GameObject asteroids = (GameObject)Instantiate(smartRocket);

            //asteroids.GetComponent<MissileController>().target = TargetPosition2;

        }
    }


    void fire()
    {




        SpawnSmartMeteor();
        SpawnSmartMeteor();
        SpawnSmartMeteor();


    }

    IEnumerator wait()
    {
        MultiTargets t = TargetPosition0.GetComponent<MultiTargets>();
        MultiTargets t1 = TargetPosition1.GetComponent<MultiTargets>();
        MultiTargets t2 = TargetPosition2.GetComponent<MultiTargets>();

        yield return new WaitUntil(() => t.getDone() && t1.getDone() && t2.getDone());
        yield return new WaitForSeconds(2f);
        fire();
        yield return new WaitForSeconds(5f);

        t.setDone(false);
        t1.setDone(false);
        t2.setDone(false);


        t.setSet(true);
        t1.setSet(true);
        t2.setSet(true);



    }

    void StartInvokes()
    {

        Vector2 min1 = Camera.main.ViewportToWorldPoint(new Vector2(0, 0f));
        Vector2 max1 = Camera.main.ViewportToWorldPoint(new Vector2(1, .65f));

        MultiTargets t = TargetPosition0.GetComponent<MultiTargets>();
        MultiTargets t1 = TargetPosition1.GetComponent<MultiTargets>();
        MultiTargets t2 = TargetPosition2.GetComponent<MultiTargets>();
        t.setSet(false);
        t1.setSet(false);
        t2.setSet(false);
        t.setDone(false);
        t1.setDone(false);
        t2.setDone(false);

        t.setTargetVector(new Vector2(Random.Range(min1.x, max1.x), Random.Range(min1.y, max1.y)));
        t1.setTargetVector(new Vector2(Random.Range(min1.x, max1.x), Random.Range(min1.y, max1.y)));
        t2.setTargetVector(new Vector2(Random.Range(min1.x, max1.x), Random.Range(min1.y, max1.y)));

        StartCoroutine(wait());
    }

}
