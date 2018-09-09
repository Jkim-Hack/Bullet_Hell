using UnityEngine;
using System.Collections;

namespace TwoDHomingMissiles
{
    public class DemoAsteroidCleanupScript : MonoBehaviour
    {

        public float destroyDelay = 6.0f;

        // Use this for initialization
        private void Start()
        {

            Invoke("DestroyAsteroid", destroyDelay);
        }

        private void DestroyAsteroid()
        {
            Destroy(gameObject);
        }

        // Update is called once per frame
        private void Update()
        {

        }
    }
}