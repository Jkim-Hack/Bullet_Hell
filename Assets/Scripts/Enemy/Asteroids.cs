using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroids : EnemyProjectile {

 
    public float life;
    private SpriteRenderer sprite;
    public Sprite[] sprites = new Sprite[5];

    private Shader shaderGUIText;
    private Shader shaderSpritesDefault;

    public GameObject Coin;

    // Use this for initialization
    void Start () {

        sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = sprites[Random.Range(0, 5)];


        shaderGUIText = Shader.Find("GUI/Text Shader");
        shaderSpritesDefault = Shader.Find("Sprites/Default");


        speed = Random.Range(5f, 10f);

    }
	
	// Update is called once per frame
	void Update () {

        

        Vector2 position = transform.position;

        position = new Vector2(position.x, position.y - speed * Time.deltaTime);

        transform.position = position;

        transform.Rotate(0f, 0f, 1f, Space.Self);

        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, -.2f));
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        if (transform.position.x < min.x || transform.position.x > max.x || transform.position.y < min.y )
        {
            Destroy(gameObject);
        }

    }
    public override void OnTriggerEnter2D(Collider2D col)
    {
        
        if ((col.tag == "PlayerBulletTag"))
        {
            life -= 2;
            StartCoroutine(blink());
            if (life <= 0)
            {
                int i = Random.Range(0, 10);
                if(i % 2 == 0)
                {
                    GameObject coin = (GameObject)Instantiate(Coin);
                    coin.transform.position = transform.position;
                }
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

    IEnumerator blink()
    {

        normalSprite();
        yield return new WaitForSeconds(0.1f);
        whiteSprite();
        yield return new WaitForSeconds(0.1f);
        normalSprite();

    }
}
