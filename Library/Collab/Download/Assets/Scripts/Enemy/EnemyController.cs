using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyController : MonoBehaviour
{


    public float speed;
    private float direction;
    private SpriteRenderer sprite;

    public GameObject target;
    public Sprite[] sprites = new Sprite[2];

    public GameObject EnemyBullet;

    public GameObject firefly;
    public GameObject fireFlyup;
    public GameObject BulletPosition01;
    public GameObject BulletPosition02;
    public GameObject BulletPosition03;
    public GameObject BulletPosition04;
    public GameObject BulletPosition05;
    public GameObject BulletPosition06;
    public GameObject BulletPosition07;
    public GameObject BulletPosition08;
    public GameObject BulletPosition09;
    public GameObject BulletPosition10;
    public GameObject BulletPosition11;

    public GameObject[] waypoints = new GameObject[6];
    public GameObject minion;

    public float life;
    private Color currentcolor;
    private Shader shaderGUIText;
    private Shader shaderSpritesDefault;
    private Vector2 playerPos;
    private float additionSpin = 0;

    public float fireRate = 5F;

    private bool stopped = true;
    
    private float angle;
    

    private float direction1 = 1f;

    private float RotateSpeed = 1f;
    private float Radius = 10f;

    private Vector2 _centre;
    private float _angle = 0;

   
    private int count = 0;
    private bool isRdy = true;
    private bool firstMove = false;
    private bool secondMove = true;
    private float originalY;

    public float floatStrength = 1;

    private float FireFlySpawnRate;
    private float MinionSpawnRate;

    Vector2 min;
    Vector2 max;

    private float posX, posY;
    // Use this for initialization
    void Start()
    {
        direction = -1;
        sprite = GetComponent<SpriteRenderer>();
        currentcolor = Color.white;
        shaderGUIText = Shader.Find("GUI/Text Shader");
        shaderSpritesDefault = Shader.Find("Sprites/Default");
        playerPos = GameObject.Find("Player").transform.position;
        this.originalY = this.transform.position.y;
        FireFlySpawnRate = 60f;
        MinionSpawnRate = 8f;
        StartInvokes();
        _centre = transform.position;
        
       
       // GameObject obj1 = (GameObject)Instantiate(target);

    }

    // Update is called once per frame
    void Update()
    {
     
        if (stopped)
        {
            transform.position = new Vector2(transform.position.x,
            originalY + ((float)Mathf.Sin(Time.time) * floatStrength));
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(posX, posY), Time.deltaTime * 40);
        }
        if (firstMove)
        {
        
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(50, Vector3.forward), Time.deltaTime * 40);
            if (transform.rotation == Quaternion.AngleAxis(50, Vector3.forward))
            {
               
                firstMove = false; stopped = false;
                sprite.sprite = sprites[1];
                InvokeRepeating("FireBulletsStr8", 0f, .1f);
            }
        }
        if (secondMove)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(0, Vector3.forward), Time.deltaTime * 40);
            if (transform.rotation == Quaternion.AngleAxis(0, Vector3.forward))
            {
                secondMove = false; StartCoroutine(StartMovement());
            }
        }
       
        if (!stopped)
        {
            if (isRdy)
            {
                StartCoroutine(isReady());
            }
            if (angle > 50.0f)
            { 
                direction1 = 1f;            
            }
            else if (angle < -50)
            {
                direction1 = -1f;          
            }
             
            _angle += direction1 * RotateSpeed * Time.deltaTime;
            var offset = new Vector2(Mathf.Sin(_angle), Mathf.Cos(_angle)) * Radius;
            angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
            Debug.Log(angle);
            bool first = false;
            if (angle > 50) first = true;
            if(first) transform.rotation = Quaternion.AngleAxis(50, Vector3.forward);
            else transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        }
        /* //Move to 70 degrees then shoot until hit 0 degress. The go -70 degrees shoot until 0 degrees
        if (fifthMove)
        {
            if (firstMove)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(70, Vector3.forward), Time.deltaTime * 40);
                if (transform.rotation == Quaternion.AngleAxis(70, Vector3.forward))
                {
                    firstMove = false; stopped1 = false;
                    sprite.sprite = sprites[1];
                    InvokeRepeating("FireBulletsStr8", 0f, .1f);
                }
            }
            

        }
        if (!stopped1)
        {
            if (isRdy)
            {
                StartCoroutine(isReady());
            }
            if (angle > 70.0f)
            {
                direction1 = 1f;
            }
            else if (angle < -70)
            {
                direction1 = -1f;
            }

            _angle += direction1 * RotateSpeed * Time.deltaTime;
            var offset = new Vector2(Mathf.Sin(_angle), Mathf.Cos(_angle)) * Radius;
            angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
            bool first = false;
            if (angle > 70) first = true;
            if (first) transform.rotation = Quaternion.AngleAxis(70, Vector3.forward);
            else transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        }

        */


    }

    void moveAround()
    {
      
        min = Camera.main.ViewportToWorldPoint(new Vector2(.1f, .7f));
        max = Camera.main.ViewportToWorldPoint(new Vector2(.9f, .9f));

        posX = Random.Range(min.x, max.x);
        posY = Random.Range(min.y, max.y);
    }

    IEnumerator isReady()
    {
        isRdy = false;
        yield return new WaitForSeconds(5f);
        stopped = true;
        secondMove = true;
        sprite.sprite = sprites[0];
        CancelInvoke("FireBulletsStr8");

    }

    void FireBulletsStr8()
    {
        GameObject bullet02 = (GameObject)Instantiate(EnemyBullet);
        bullet02.transform.position = BulletPosition02.transform.position;
        bullet02.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        GameObject bullet09 = (GameObject)Instantiate(EnemyBullet);
        bullet09.transform.position = BulletPosition09.transform.position;
        bullet09.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        
        GameObject bullet10 = (GameObject)Instantiate(EnemyBullet);
        bullet10.transform.position = BulletPosition10.transform.position;
        bullet10.transform.rotation = Quaternion.AngleAxis(angle - 90 , Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if ((col.tag == "PlayerBulletTag"))
        {
            life -= 1;

            StartCoroutine(blink());

            if (life <= 0)
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

    void StartInvokes()
    {
        
        InvokeRepeating("SpawnFireFlies", FireFlySpawnRate, FireFlySpawnRate);
        InvokeRepeating("isStopped", 5f, 20f);
        InvokeRepeating("SpawnBulletMinions", MinionSpawnRate, MinionSpawnRate);
        InvokeRepeating("moveAround", 10f, 10f);

    }
    void SpawnFireFlies()
    {
        StartCoroutine(spawnWait());
    }

    IEnumerator spawnWait()
    {
        GameObject spawnPoint = GameObject.Find("FireFlyRightSpawn");
        GameObject spawnPointU = GameObject.Find("FireFlyUpSpawn");
        if(life/1000f >= .66f)
        {
            GameObject fireFly = (GameObject)Instantiate(firefly);
            fireFly.transform.position = spawnPoint.transform.position;
        } else if(life/1000f <= .33f && life/1000f < .66f)
        {
            GameObject fireFly = (GameObject)Instantiate(firefly);
            fireFly.transform.position = spawnPoint.transform.position;
            yield return new WaitForSeconds(1f);
            GameObject fireFly1 = (GameObject)Instantiate(firefly);
            fireFly1.transform.position = spawnPoint.transform.position;
        }
        else if(life/1000f < .33f)
        {

            GameObject fireFly = (GameObject)Instantiate(firefly);
            fireFly.transform.position = spawnPoint.transform.position;
            yield return new WaitForSeconds(1f);
            GameObject fireFly1 = (GameObject)Instantiate(firefly);
            fireFly1.transform.position = spawnPoint.transform.position;
            yield return new WaitForSeconds(1f);
            GameObject fireflyUp = (GameObject)Instantiate(fireFlyup);
            fireflyUp.transform.position = spawnPointU.transform.position;
            yield return new WaitForSeconds(1f);
            GameObject fireflyUp1 = (GameObject)Instantiate(fireFlyup);
            fireflyUp1.transform.position = spawnPointU.transform.position;
        }
       

    }

    public void OnParticleCollision(GameObject other)
    {
        print("LUCK");
        /*
        if (other.tag == "EnemyTag")
        {
            var enemyController = other.GetComponent<EnemyController>();

            enemyController.life = -10;
            print("Life after Flamethrower damage " + enemyController.life);
        }
        */
    }
   
    IEnumerator StartMovement()
    {
        yield return new WaitForSeconds(5f);
        angle = 0f;
        _angle = 0f;
        sprite.sprite = sprites[1];
        firstMove = true;
        isRdy = true;
    }
    void SpawnBulletMinions()
    {
        GameObject o = GameObject.Find("Player");
        GameObject minion0 = (GameObject)Instantiate(minion);
        minion0.transform.position = Camera.main.ViewportToWorldPoint(new Vector2(1.2f, 1.2f));
        minion0.GetComponent<EnemyMinion0>().SetDirection(o.transform.position - minion0.transform.position);
        GameObject minion1 = (GameObject)Instantiate(minion);
        minion1.transform.position = Camera.main.ViewportToWorldPoint(new Vector2(-.2f, 1.2f));
        minion1.GetComponent<EnemyMinion0>().SetDirection(o.transform.position - minion1.transform.position);

    }
}


