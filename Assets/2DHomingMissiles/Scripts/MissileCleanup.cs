using UnityEngine;
using System.Collections;

namespace TwoDHomingMissiles
{

    /// <summary>
    /// This script is used as a remote "cleanup". When a missile is destroyed, either by colliding with a target, or running out of fuel for example, it leaves behind it's children gameobjects.
    /// It does this so that the child objects (containing smoke trail particles) are left behind, to fade away for a few seconds, instead of the trail suddenly disappearing. The smoke trail fades, given enough time
    /// and then the delayed remote call of DestroyMissileLeftovers removes the left over child gameobjects to completely clean up the old missile.
    /// </summary>
    public class MissileCleanup : MonoBehaviour
    {
        public Transform originalMissileTransform; // Reference to the transform this gameobject was a child of before it got disabled. (Used with Object Pooling)
        public float destroyDelay = 3.0f;

        void Awake()
        {
            if (gameObject.transform.parent != null)
            {
                originalMissileTransform = gameObject.transform.parent;
            }
            else
            {
                Debug.LogError("Transform parent was null for the MissileCleanup script.");
            }
        }

        public void ScheduleDestroyLeftovers(bool usingObjectPool)
        {
            if (!usingObjectPool)
            {
                Invoke("DestroyMissileLeftovers", destroyDelay);
            }
            else
            {
                Invoke("DisableMissileLeftovers", destroyDelay);
            }
        }

        private void DestroyMissileLeftovers()
        {
            Destroy(gameObject);
        }

        private void DisableMissileLeftovers()
        {
            gameObject.SetActive(false);
            
            // Re-attach the missile sprite transform to the original missile it was attached to's gameobject.
            if (originalMissileTransform != null)
            {
                gameObject.transform.parent = originalMissileTransform;
                // Re-name to indicate to object pool that missile is now ready for re-use.
                originalMissileTransform.gameObject.name = originalMissileTransform.gameObject.name.Replace("NOTAVAILABLE", "READY");
            }
        }
    }
}