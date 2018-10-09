using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController2 : MonoBehaviour {


    public GameObject[] Arms;
    
    public float floatStrength = 2;
    public int spawningAmount = 0;
    private float countMissiles = 4;
    private Vector3 pos;
    public GameObject missileGameObject;
    public const float maxDistanceforMissiles = 50;
    private List<GameObject> missileContainer = new List<GameObject>();
    public float currHealth;
    private float maxHealth;
    private bool startLaser;
    private Color currentcolor;
    private Shader shaderGUIText;
    private Shader shaderSpritesDefault;
    private SpriteRenderer sprite;
    public GameObject playerObj;
    // Use this for initialization

    void Start () {

        sprite = GetComponent<SpriteRenderer>();
        currentcolor = Color.white;
        shaderGUIText = Shader.Find("GUI/Text Shader");
        shaderSpritesDefault = Shader.Find("Sprites/Default");

        pos = transform.position;
        countMissiles = 4;
        maxHealth = currHealth;
        InvokeRepeating("startLaserBeam", 5f, 5f);
        InvokeRepeating("missileFire", 10f, 10f);
    }
	
	// Update is called once per frame
	void Update () {
        //get the objects current position and put it in a variable so we can access it later with less code
        
        
        //calculate what the new Y position will be //2 = speed, 1.5 = how far up adn down
        float newY = (Mathf.Sin(Time.time * 2f)*1.5f) + pos.y;
        
        //set the object's Y to the new calculated Y
        transform.position = new Vector3(pos.x, newY, pos.z);

        if (startLaser)
        {
            var dir = playerObj.transform.position - transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            angle += 90;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

    }

    void StartMissiles()
    {
        for(int i = 1; i <= spawningAmount; i++)
        {
            GameObject missile = (GameObject)Instantiate(missileGameObject);
            missile.transform.position = transform.position;
            missile.GetComponent<MissileScripts>().AcquirePosition(CalcMissilePoints(maxDistanceforMissiles, i, false, missile));
            missileContainer.Add(missile);
            GameObject missile1 = (GameObject)Instantiate(missileGameObject);
            missile1.transform.position = transform.position;
            missile1.GetComponent<MissileScripts>().AcquirePosition(CalcMissilePoints(maxDistanceforMissiles, i, true, missile1));
            missileContainer.Add(missile1);
            
        }
       

    }
    Vector2 CalcMissilePoints(float maxDistance, int i, bool isOppo, GameObject missile)
    {
        float addAmt = maxDistance / spawningAmount;
        var x = addAmt * i;
        float y;
        if (!isOppo)
            y = missile.transform.position.y + Mathf.Sqrt(Mathf.Pow(maxDistance / 2, 2) - Mathf.Pow((x - (maxDistance / 2)), 2));
        else
        {
            x *= -1;
            y = missile.transform.position.y + Mathf.Sqrt(Mathf.Pow(maxDistance / 2, 2) - Mathf.Pow((x + (maxDistance / 2)), 2));
        }


        return new Vector3(x, y - 15); //Y value will be too high off the screen so offset it -15
        

    }
   
    IEnumerator StageRocketFire(int stage)
    {
        yield return new WaitForSeconds(1.5f);   
        switch (stage)
        {
            case 1:
                print("switch");
                print(missileContainer.Count);
                for (var i = 0; i < missileContainer.Count; i++)
                {
                    print("launch");
                    missileContainer[i].GetComponent<MissileScripts>().onReady = true;
                    yield return new WaitForSeconds(1f);
                } break;
            case 2:
                for (var i = 0; i < missileContainer.Count; i++)
                {
                    missileContainer[i].GetComponent<MissileScripts>().onReady = true;
                    yield return new WaitForSeconds(.1f);
                } break;
            case 3:
                for (var i = 0; i < missileContainer.Count; i++)
                {
                    missileContainer[i].GetComponent<MissileScripts>().onReady = true;
                } break;
        }
      
    }

    float CalcHealthPercentage()
    {
       return currHealth / maxHealth;   
    }
    void missileFire()
    {
       
        int stage = (CalcHealthPercentage() >= .66f ? 1 : (CalcHealthPercentage()>=.33f ? 2:3));
        //Stage will always be 1, 2 or 3
        spawningAmount = (stage == 1 ? Random.Range(2, 4) : (stage == 2 ? Random.Range(4, 6) : Random.Range(6, 8)));
        Invoke("StartMissiles", 0f);
        Debug.Log(stage);
        StartCoroutine(StageRocketFire(stage));
        missileContainer.Clear();
       
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if ((col.tag == "PlayerBulletTag"))
        {
            currHealth -= 1;

            StartCoroutine(blink());

            if (currHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
    void whiteSprite()
    {
        sprite.material.shader = shaderGUIText;
        sprite.color = Color.white;
    }

    void normalSprite()
    {
        sprite.material.shader = shaderSpritesDefault;
        sprite.color = Color.white;
    }

    public IEnumerator blink()
    {

        normalSprite();
        yield return new WaitForSeconds(0.1f);
        whiteSprite();
        yield return new WaitForSeconds(0.1f);
        normalSprite();

    }

    void startLaserBeam()
    {
        LaserBeam laserBeam = this.gameObject.transform.GetChild(3).GetComponent<LaserBeam>();
        startLaser = true;
        laserBeam.laserStart = startLaser;
        
       
    }

  


}
