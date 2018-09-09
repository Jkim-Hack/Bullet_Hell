using TwoDLaserPack;
using UnityEngine;
using System.Collections;

namespace TwoDLaserPack
{
    public class DemoToggleLaserActive : MonoBehaviour
    {

        public LineBasedLaser lineLaserRef;
        public SpriteBasedLaser spriteLaserRef;

        // Use this for initialization
        void Start()
        {

        }

        void OnMouseOver()
        {
            if (lineLaserRef != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    lineLaserRef.SetLaserState(!lineLaserRef.laserActive);
                }
            }

            if (spriteLaserRef != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    spriteLaserRef.SetLaserState(!spriteLaserRef.laserActive);
                }
            }

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

