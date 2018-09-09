using UnityEngine;
using System.Collections;

namespace TwoDLaserPack
{
    public class BloodEffect : MonoBehaviour
    {
        public float fadespeed = 2;
        public float timeBeforeFadeStarts = 1f;

        private float elapsedTimeBeforeFadeStarts;
        private SpriteRenderer sprite;
        private Color spriteColor;

        void Awake()
        {
            sprite = gameObject.GetComponent<SpriteRenderer>();
        }

        void OnEnable()
        {
            //spriteColor = new Color(sprite.renderer.material.color.r, sprite.renderer.material.color.g, sprite.renderer.material.color.b, Mathf.Lerp(sprite.renderer.material.color.a, 0, Time.deltaTime * fadespeed));

        }

        void OnDisable()
        {
            spriteColor = new Color(sprite.GetComponent<Renderer>().material.color.r, sprite.GetComponent<Renderer>().material.color.g, sprite.GetComponent<Renderer>().material.color.b, 1f);
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            elapsedTimeBeforeFadeStarts += Time.deltaTime;

            if (elapsedTimeBeforeFadeStarts >= timeBeforeFadeStarts)
            {
                spriteColor = new Color(sprite.GetComponent<Renderer>().material.color.r, sprite.GetComponent<Renderer>().material.color.g, sprite.GetComponent<Renderer>().material.color.b, Mathf.Lerp(sprite.GetComponent<Renderer>().material.color.a, 0, Time.deltaTime * fadespeed));

                sprite.GetComponent<Renderer>().material.color = spriteColor;

                if (sprite.material.color.a <= 0f)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}