using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiTargets : MonoBehaviour {

    // Use this for initialization

    private bool isSet = false;
    private bool isDone = true;
    private Vector2 initialPosition;
    private Vector2 target;
    Vector2 min;
    Vector2 max;
    void Start () {
        initialPosition = transform.position;
        min = Camera.main.ViewportToWorldPoint(new Vector2(.3f, .3f));
        max = Camera.main.ViewportToWorldPoint(new Vector2(.7f, .7f));
        target = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
        
    }

    // Update is called once per frame
    void Update()
    {


        if (isSet)
        {
            transform.position = Vector2.MoveTowards(transform.position, initialPosition, Time.deltaTime * 80);
        }
        else
        {
           
            if (!isDone)
            {
               
                transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * 80);
            }
        }
            
        if(Vector2.Distance(transform.position, target) <= 0)
        {
            isDone = true;
        }
        

    }

    public void setSet(bool l)
    {
        isSet = l;
    }
    public bool getDone()
    {
        return isDone;
    }
    public void setDone(bool done)
    {
        isDone = done;
    }

    public void setTargetVector(Vector2 t)
    {
        target = t;
    }
   
}
