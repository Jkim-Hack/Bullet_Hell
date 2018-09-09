using UnityEngine;
using System.Collections;

namespace TwoDHomingMissiles
{
    public class DemoAsteroidMovement : MonoBehaviour
    {

        public Vector2 speed = new Vector2(2f, 0f);
        public Vector2 direction = new Vector2(-1f, 0f);

        // Use this for initialization
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {

            var movement = new Vector2(speed.x*direction.x, speed.y*direction.y);
            movement *= Time.deltaTime;

            // Important: because of parent/child relations of this object, we need to translate it relative to Space.World, otherwise rotating the object, can actually change its position too...
            transform.Translate(movement, Space.World);

        }
    }
}