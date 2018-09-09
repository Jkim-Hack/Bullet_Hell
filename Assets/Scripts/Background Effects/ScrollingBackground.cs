using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{

    private Rigidbody2D rigidBody;

    //speed 2 is optimal
    private float scrollSpeed = 2f;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();

        Vector2 scrollDirection = new Vector2(0, -scrollSpeed);
        rigidBody.velocity = scrollDirection;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void actionAfterGameOver()
    {
        bool gameOver = GameController.gameControllerInstance.gameOver;

        if (gameOver == true)
        {
            rigidBody.velocity = Vector2.zero;
        }
    }
}
