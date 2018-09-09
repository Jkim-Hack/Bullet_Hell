using UnityEngine;
using System.Collections;

namespace TwoDHomingMissiles
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class DemoHighlightTarget : MonoBehaviour
    {
        private bool isTargeted;
        private SpriteRenderer spriteRendererCmpt;

        // Use this for initialization
        private void Start()
        {
            // Cache sprite renderer component.
            spriteRendererCmpt = gameObject.GetComponent<SpriteRenderer>();
        }

        public IEnumerator HighlightTarget()
        {
            // Flash the sprite alternating colours if the collider triggered a hit...
            if (isTargeted) yield break;
            if (spriteRendererCmpt == null) yield break;

            isTargeted = true;
            for (var n = 0; n < 5; n++)
            {
                spriteRendererCmpt.enabled = true;
                yield return new WaitForSeconds(0.1f);
                spriteRendererCmpt.enabled = false;
                yield return new WaitForSeconds(0.1f);
            }

            isTargeted = false;
        }

        // Update is called once per frame
        private void Update()
        {

        }
    }
}