using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController gameControllerInstance;
    public Camera mainCamera;
    public float cameraOrthoganalHeight;

    public bool gameOver = false;


    void Start()
    {
        mainCamera = Camera.main;
        cameraOrthoganalHeight = mainCamera.orthographicSize;
    }

    private void Awake()
    {
        if (gameControllerInstance == null)
            gameControllerInstance = this;

        else if(gameControllerInstance != this)
        {
            Destroy(gameObject);
        }
    }

  

}
