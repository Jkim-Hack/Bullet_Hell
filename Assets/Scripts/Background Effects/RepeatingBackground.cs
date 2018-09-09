using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatingBackground : MonoBehaviour
{
    private SpriteRenderer sprite;
    private float spriteMaxY;

	void Start ()
    {
        sprite = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {

        spriteMaxY = sprite.bounds.max.y;

        float camYPos = GameController.gameControllerInstance.mainCamera.transform.position.y;
        float camHeight = GameController.gameControllerInstance.cameraOrthoganalHeight;
        float cameraLowerBound = camYPos - camHeight;

        if (spriteMaxY  <= cameraLowerBound)
        {
            repositionBackground();
        }

        
	}

    private void repositionBackground()
    {
        float cameraHeight = GameController.gameControllerInstance.cameraOrthoganalHeight;
        float cameraDistanceFactor = 4f;
        Vector2 offset = new Vector2(0, cameraDistanceFactor * cameraHeight);
        transform.position = (Vector2)transform.position + offset;
    }
}
