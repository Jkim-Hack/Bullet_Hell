using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

namespace TwoDLaserPack
{
    public class RotationScript : MonoBehaviour
    {
        public Slider hSlider;
        public Transform pivot;
        public bool rotationEnabled;
        public float rotationAmount;

        private Transform transformCached;

        void Start()
        {
            transformCached = transform;
        }

        void Update()
        {
            if (!rotationEnabled) return;
            transformCached.RotateAround(pivot.localPosition, Vector3.forward, rotationAmount);
                //(0, 0, rotationsPerMinute * Time.deltaTime, Space.Self);
        }

        public void OnHSliderChanged()
        {
            rotationAmount = hSlider.value;
        }
    }
}
