using UnityEngine;
using System.Collections;

namespace TwoDHomingMissiles
{
    public class DemoRandomPositionMover : MonoBehaviour
    {

        public float pickerInterval, radius;

        public GameObject centralPointObject;

        public Vector2 randomPointInCircle, originalStartPosition;

        // Use this for initialization
        private void Start()
        {

            originalStartPosition = transform.position;

            if (pickerInterval == 0f)
            {
                pickerInterval = 3f;
            }

            randomPointInCircle = Vector2.zero;
            InvokeRepeating("PickRandomPointInCircle", Random.Range(0f, pickerInterval), pickerInterval);

        }

        private void PickRandomPointInCircle()
        {
            //transform.position = centralPointObject.transform.position;
            randomPointInCircle = originalStartPosition + Random.insideUnitCircle*radius;
            transform.position = randomPointInCircle;
        }

        // Update is called once per frame
        private void Update()
        {

        }
    }
}