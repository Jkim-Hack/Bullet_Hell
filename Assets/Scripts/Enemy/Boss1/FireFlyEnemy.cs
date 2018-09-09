using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwoDLaserPack;

public class FireFlyEnemy : MonoBehaviour {
    private Vector2 target;
    public GameObject laserKill;
    public GameObject laser;
    private bool isDone = false;
    private Vector2 initial;
    private bool isSet = false;
    public GameObject panel;
	// Use this for initialization
	void Start () {
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, .2f));
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, .6f));
        target = new Vector2(transform.position.x, Random.Range(min.y, max.y));
        initial = transform.position;
        laserKill.GetComponent<LineBasedLaser>().SetLaserState(false);
        StartCoroutine(wait());
        
    }
	
	// Update is called once per frame
	void Update () {

      


        if (Vector2.Distance(transform.position, target) <= 0)
        {
            isDone = true;

        }

        if (isSet)
        {
            Vector2 min1 = Camera.main.ViewportToWorldPoint(new Vector2(0, -.2f));
            Vector2 max1 = Camera.main.ViewportToWorldPoint(new Vector2(1, 1.2f));
            transform.position = Vector2.MoveTowards(transform.position, initial, Time.deltaTime * 20);
            if (transform.position.y > max1.y)
            {
                Destroy(gameObject);
            }
        } else
        {
            transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * 20);
        }

    }

    IEnumerator wait()
    {
        yield return new WaitUntil(() => isDone);
        yield return new WaitForSeconds(2f);
        panel = GameObject.Find("Canvas");
        Debug.Log(panel);
        Debug.Log(getPanelObject(panel, 9));
        getPanelObject(panel, 9).SetActive(true);
        yield return new WaitForSeconds(.1f);
        laserKill.GetComponent<LineBasedLaser>().SetLaserState(true);
        laserKill.GetComponent<LineBasedLaser>().isKillable = true;
        laser.GetComponent<LineBasedLaser>().SetLaserState(false);
        yield return new WaitForSeconds(3f);
        laserKill.GetComponent<LineBasedLaser>().SetLaserState(false);
        laser.GetComponent<LineBasedLaser>().SetLaserState(true);
        isSet = true;
        getPanelObject(panel, 9).SetActive(false);
    }
    private GameObject getPanelObject(GameObject parent, int childNumber)
    {
        Transform[] objects = parent.GetComponentsInChildren<Transform>(true);
        return objects[childNumber].gameObject;
    }


}
