using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController2 : MonoBehaviour {


    public GameObject[] Arms;
    
    public float floatStrength = 2;
    public int spawningAmount = 4;
    private float countMissiles = 4;
    private Vector3 pos;
    public GameObject missileGameObject;
    public const float maxDistanceforMissiles = 20;
    // Use this for initialization
    void Start () {
        pos = transform.position;
        countMissiles = 4;
        Invoke("StartMissiles", 0f);
    }
	
	// Update is called once per frame
	void Update () {
        //get the objects current position and put it in a variable so we can access it later with less code
        
        
        //calculate what the new Y position will be //2 = speed, 1.5 = how far up adn down
        float newY = (Mathf.Sin(Time.time * 2f)*1.5f) + pos.y;
        
        //set the object's Y to the new calculated Y
        transform.position = new Vector3(pos.x, newY, pos.z);   
        
    }

    void StartMissiles()
    {
        for(int i = 0; i < spawningAmount; i++)
        {
            GameObject missile = (GameObject)Instantiate(missileGameObject);
            missile.transform.position = transform.position;
            missile.GetComponent<MissileScripts>().AcquirePosition(CalcMissilePoints(maxDistanceforMissiles));
            GameObject missile1 = (GameObject)Instantiate(missileGameObject);
            missile1.transform.position = transform.position;
            missile1.GetComponent<MissileScripts>().AcquirePosition(CalcMissilePoints(-maxDistanceforMissiles));
        }
       

    }
    Vector2 CalcMissilePoints(float maxDistance)
    {

        Debug.Log(countMissiles);
        //TODO: Need vector positions for the next x amount of missiles per side 
        var angle = 180 / countMissiles;
        var angleInRad = Mathf.Deg2Rad * angle;
        var tan = Mathf.Atan(angleInRad);

        var y = tan * (maxDistance / countMissiles); 
        var x = maxDistance / countMissiles;
        countMissiles--;
        Debug.Log(new Vector2(x,y));
        return new Vector3(x, y);
        
//l
    }
   

}
