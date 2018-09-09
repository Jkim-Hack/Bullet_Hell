using UnityEngine;
using System.Collections;

namespace TwoDHomingMissiles
{
    [RequireComponent(typeof (SpriteRenderer))]
    public class DemoAlienHullHit : MonoBehaviour
    {
        private bool isHit;
        private SpriteRenderer spriteRendererCmpt;

        // Use this for initialization
        private void Start()
        {
            // Cache sprite renderer component.
            spriteRendererCmpt = gameObject.GetComponent<SpriteRenderer>();
        }

        private IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            // Flash the sprite alternating colours if the collider triggered a hit...
            if (isHit) yield break;
            if (spriteRendererCmpt == null) yield break;

            isHit = true;
            var redColour = Color.red;
            for (var n = 0; n < 3; n++)
            {
                GetComponent<Renderer>().material.color = Color.white;
                yield return new WaitForSeconds(0.1f);
                GetComponent<Renderer>().material.color = redColour;
                yield return new WaitForSeconds(0.1f);
            }
            GetComponent<Renderer>().material.color = Color.white;

            isHit = false;
        }

        // Update is called once per frame
        private void Update()
        {

        }
    }
}