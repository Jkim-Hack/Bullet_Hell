using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {

    public GameObject PlayerBullet;
    public GameObject BulletPosition01;
    public GameObject BulletPosition02;
    public float speed;

    private Health health;
    private HealthUI healthUI;

    private SpriteRenderer sprite;
    private Shader shaderGUIText;
    private Shader shaderSpritesDefault;

    private ShipDamageController damageController;

    private void Awake()
    {
        healthUI = this.GetComponent<HealthUI>();
        health = GetComponent<Health>();
    }

    // Use this for initialization
    void Start ()
    {
        getObjectComponents();

        InvokeRepeating("Shoot", 0f, .3f);

        GUIDelegate.updateHealthUI(health);

       
    }
	
	void Update ()
    {
        setClampPositions();
        if(health.getHealth() <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void getObjectComponents()
    {
        sprite = GetComponent<SpriteRenderer>();
        shaderGUIText = Shader.Find("GUI/Text Shader");
        shaderSpritesDefault = Shader.Find("Sprites/Default");
        damageController = GetComponent<ShipDamageController>();
        
    }

    private void setClampPositions()
    {
        float x = CrossPlatformInputManager.GetAxisRaw("Horizontal");
        float y = CrossPlatformInputManager.GetAxisRaw("Vertical");

        Vector2 dir = new Vector2(x, y);
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(.03f, .03f));
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(.95f, .95f));

        Vector2 pos = transform.position;

        pos += dir * speed * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, min.x, max.x);
        pos.y = Mathf.Clamp(pos.y, min.y, max.y);


        transform.position = pos;

    }

    void Shoot()
    {
        GameObject bullet01 = (GameObject)Instantiate(PlayerBullet);
        bullet01.transform.position = BulletPosition01.transform.position;

        GameObject bullet02 = (GameObject)Instantiate(PlayerBullet);
        bullet02.transform.position = BulletPosition02.transform.position;

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<EnemyProjectile>() != null)
        {
            EnemyProjectile enemyBullet = col.gameObject.GetComponent<EnemyProjectile>();
            afterHitByBullet(enemyBullet);
        }

        if (col.tag == "EnemyMissile")
        {
            float currentHealth = health.getHealth();
            float healthAfterHit = currentHealth - 20;
            StartCoroutine(blink());
            health.setHealth(healthAfterHit);
            
        }

        if(col.gameObject.GetComponent<PickUp>() != null)
        {
            //OverCharge
            StartCoroutine(overCharge());
        }

    }

    //Instructions after player ship is hit by an enemy projectile
    private void afterHitByBullet(EnemyProjectile enemyProjectile)
    {
        updateLife(enemyProjectile);
        damageController.updateDamageEffect(health.getHealthPercentage());
        StartCoroutine(blink());
       
        if (isDead())
            Destroy(gameObject);
    }

    private void updateLife(EnemyProjectile enemyProjectile)
    {
         health.takeDamage(enemyProjectile.getDamage());
    }

    private bool isDead()
    {
        return health.getHealth() <= 0;
    }

    IEnumerator blink()
    {
        PolygonCollider2D poly = GetComponent<PolygonCollider2D>();
        poly.isTrigger = false;
        for (int i = 0; i < 5; i++)
        {
            normalSprite();
            yield return new WaitForSeconds(0.1f);
            whiteSprite();
            yield return new WaitForSeconds(0.1f);
        }
        normalSprite();
        poly.isTrigger = true;
    }

    void normalSprite()
    {
        sprite.material.shader = shaderSpritesDefault;
        sprite.color = Color.white;
    }

    void whiteSprite()
    {
        sprite.material.shader = shaderGUIText;
        sprite.color = Color.white;
    }
    public void setHealth(float h)
    {
        health.setHealth(h);
    }
    IEnumerator overCharge()
    {
        CancelInvoke("Shoot");
        InvokeRepeating("Shoot", 0f, .1f);
        yield return new WaitForSeconds(5f);
        CancelInvoke("Shoot");
        InvokeRepeating("Shoot", 0f, .3f);
    }
}
