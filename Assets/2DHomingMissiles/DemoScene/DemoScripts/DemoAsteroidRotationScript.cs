using UnityEngine;
using System.Collections;

namespace TwoDHomingMissiles
{
    public class DemoAsteroidRotationScript : MonoBehaviour
    {

        public bool randomRotationEnabled;
        public float rotationsMax;
        public bool rotationEnabled;
        public float rotationsMin = 0f;

        private void Start()
        {
            if (randomRotationEnabled)
            {
                rotationsMin = Random.Range(rotationsMin, rotationsMax);
            }
        }

        private void Update()
        {
            if (rotationEnabled)
            {
                if (transform.GetComponent<Renderer>() != null)
                {
                    transform.Rotate(0, 0, rotationsMin*Time.deltaTime, Space.Self);
                }
            }
        }
    }
}