using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TwoDHomingMissiles
{
    public class ObjectPoolManager : MonoBehaviour
    {
        public static ObjectPoolManager instance;

        public GameObject missilePrefab;

        public int numMissilesToSpawn;

        public static List<GameObject> missileObjectPool;

        private GameObject pooledObjectFolder;

        // Use this for initialization
        private void Start()
        {
            instance = this;
            pooledObjectFolder = gameObject;

            // Missile object pool
            missileObjectPool = new List<GameObject>();

            for (var i = 1; i <= numMissilesToSpawn; i++)
            {
                var missile = (GameObject) Instantiate(missilePrefab);

                SetParentTransform(missile);

                missile.name = missile.name + "_READY";
                missile.SetActive(false);
                missileObjectPool.Add(missile);
            }
        }

        private void SetParentTransform(GameObject gameObjectRef)
        {
            if (pooledObjectFolder != null)
            {
                gameObjectRef.transform.parent = pooledObjectFolder.transform;
            }
        }

        /// <summary>
        /// Returns a missile GameObject from the missile object pool. If there are no usable missiles, then a new one is instantiated and added to the pool, expanding the pooled missiles at the same time.
        /// </summary>
        /// <returns>Missile GameObject</returns>
        public GameObject GetUsableMissileFromObjectPool()
        {
            var obj = (from item in missileObjectPool
                       where item.activeSelf == false && !item.name.Contains("NOTAVAILABLE")
                select item).FirstOrDefault();

            if (obj != null)
            {
                var missileSpriteTransform = obj.transform.GetChild(0);
                if (missileSpriteTransform != null)
                {
                    // Re-enable sprite transform gameobject
                    missileSpriteTransform.gameObject.SetActive(true);

                    // Re-enable sprite renderer
                    var missileSprite = (SpriteRenderer)missileSpriteTransform.gameObject.GetComponent(typeof(SpriteRenderer));
                    missileSprite.enabled = true;

                    // Re-enable smoke emission rate on the particle for the trail
                    var smokeTransform = missileSpriteTransform.GetChild(0);
                    if (smokeTransform != null)
                    {
                        var ps = (ParticleSystem)smokeTransform.gameObject.GetComponent(typeof(ParticleSystem));
                        ps.emissionRate = 150f;
                    }
                }

                return obj;
            }

            Debug.Log("<color=orange>Ran out of reusable missile objects! Now instantiating a new one</color>");
            var missile = (GameObject) Instantiate(instance.missilePrefab);
            missile.name = missile.name + "_INSTANTIATED_READY";

            SetParentTransform(missile);

            missile.SetActive(false);
            missileObjectPool.Add(missile);

            return missile;
        }
    }
}