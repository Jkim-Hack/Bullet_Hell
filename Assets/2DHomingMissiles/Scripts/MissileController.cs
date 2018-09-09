using System;
using System.Linq;
using UnityEngine;
using System.Collections;

namespace TwoDHomingMissiles
{
    public class MissileController : MonoBehaviour
    {
        public GameObject childSpriteGameObject, childSmokeGameObject, explosionPrefab;
        public GameObject target; // The initial target used if missiles are in arc/swarm/swing mode.

        public GameObject mainTarget; // This is the main target that the missile will begin tracking after its initial 1.0 second (targetChangeTimer) target.

        public float maxSpeed; // Max speed - tweak as needed

        public float kProportionalConst = 0.45f; // Proportional constant - 0f - 1f (set this to tweak performance of missile) lower is more "swingy" higher is more "direct/concise"

        public float fuelAmount = 4.0f;
        public float initialTargetChangeTimer;

        public bool isTrackingInitialTarget; // Is the missile tracking it's original 'dummy' target or not (used when launching in swarm/arcing mode where the missiles initially swing outward)

        public bool usesFuel;
        public bool destroyTargetOnCollision;
        
        [HideInInspector]
        public bool usingObjectPool; // Set true or false so that when the missile is 'destroyed' it is handled correctly - using Object Pool is should be disabled rather than destroyed.

        public Transform missileSpriteTransform; // Reference that get's found on missile start, used to rotate the missile sprite, and critical to match up the smoke trail emission to the back of the missile sprite.

        /// <summary>
        /// Array of GameObjects dictating launch locations for missiles on the parent object.
        /// </summary>
        public GameObject[] launcherNodes;

        private Vector2 desiredVelocity; // our missiles desired velocity
        private Vector2 subValue;
        private Vector2 error;
        private Vector2 currentVelocity;

        private Vector2 sForce; // Steering force - force by which to "steer" the missile in the direction it wants to go.

        private float targetChangeTimer;

        public enum MissileArcMode
        {
            None,
            Top,
            Bottom
        };

        /// <summary>
        /// Sets the mode missiles use when arcing out from launcher nodes.
        /// </summary>
        [HideInInspector] 
        public MissileArcMode MissileArcModeSetting;

        private void OnEnable()
        {
            targetChangeTimer = initialTargetChangeTimer;

            desiredVelocity = Vector2.zero;
            subValue = Vector2.zero;
            error = Vector2.zero;
            currentVelocity = Vector2.zero;
        }

        // Use this for initialization
        private void Start()
        {
            missileSpriteTransform = transform.GetChild(0);
            subValue = new Vector2(0, 0);
            error = new Vector2(0, 0);
            currentVelocity = new Vector2(0, 0);
            sForce = new Vector2(0, 0);
            maxSpeed = 3f;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.gameObject.tag == "PlayerTag")
            {
                GracefullyDestroyMissile(gameObject.transform.position);
            }
            if (target != null)
            {
                if (other.gameObject == target)
                {
                    
                    // Do whatever you want to your target GameObject here.
                    /*
                    if (destroyTargetOnCollision)
                    {
                        if (!usingObjectPool)
                        {
                            Destroy(other.gameObject);
                        }
                        else
                        {
                            other.gameObject.SetActive(false);
                        }
                        
                    }
                    */
                    // Destroy this missile.
                    GracefullyDestroyMissile(gameObject.transform.position);
                }
            }
        }

        public virtual void GracefullyDestroyMissile(Vector2 position)
        {
            // Play hit effect, and create a randomly sized small explosionPrefab
            AudioSource.PlayClipAtPoint(GetComponent<AudioSource>().clip, position);

            // If we have an explosionPrefab prefab attached, then summon the explosionPrefab at the point where the missile was destroyed.
            if (explosionPrefab != null)
            {
                // Note, you should ideally use Object Pooling rather than instantiation for mass missile use.
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }

            // Release the child objects of this missile. (This is done so that the smoke particle effect and gameobject persist, leaving a trail of smoke behind where the missile has travelled.
            // Note, we don't want to detach when using ObjectPooling for missiles
            if (!usingObjectPool)
            {
                transform.DetachChildren();
            }
            else
            {
                gameObject.name = gameObject.name.Replace("READY", "NOTAVAILABLE");
                transform.DetachChildren();
            }
            
            // Disable the missile sprite (as the sprite sits on a child object of the root missile, in order to have it's rotation handled correctly in conjunction with the smoke trail)
            childSpriteGameObject.GetComponent<Renderer>().enabled = false;

            // Stop the smoke trail by setting it's emission rate to 0 until it is destroyed.
            childSmokeGameObject.GetComponent<ParticleSystem>().emissionRate = 0f;
            //(ParticleSystem)childSmokeGameObject.GetComponent(typeof(ParticleSystem)).

            // We want to schedule the left over smoke/child objects for destruction - we don't want them hanging around forever! So we use a simple Invoke with a delay.
            var missileCleanupReference = (MissileCleanup) childSpriteGameObject.GetComponent(typeof (MissileCleanup));
            missileCleanupReference.ScheduleDestroyLeftovers(usingObjectPool);

            // Destroy the root missile object that was detached from it's children.
            if (!usingObjectPool)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.name = gameObject.name.Replace("READY", "NOTREADY");
                gameObject.SetActive(false);
            }
            
        }

        // Update is called once per frame
        private void Update()
        {
            if (target != null)
            {
                if (usesFuel)
                {
                    fuelAmount -= Time.deltaTime;
                    if (fuelAmount <= 0f) GracefullyDestroyMissile(transform.position);
                }

                // Used to determine when to set the main target up for the missile after launch. (If this is true, then the missile will initially target an 'offset' node above or below the launcher node they were launched from until the timer runs out).
                if (isTrackingInitialTarget)
                {
                    if (targetChangeTimer >= 0.0f)
                    {
                        targetChangeTimer -= Time.deltaTime;
                    }
                    else
                    {
                        // Switch over to our main designated target.
                        targetChangeTimer = initialTargetChangeTimer;
                        target = mainTarget;
                        isTrackingInitialTarget = false;
                    }
                }

                subValue = target.transform.position - transform.position;

                // Normalize the subtracted value
                subValue.Normalize();

                maxSpeed = 2f;
                // Multiply the normalized value by the missile's maxSpeed to get the desired Velocity.
                desiredVelocity = subValue*maxSpeed;

                // Calculate missile error
                error = desiredVelocity - currentVelocity;
              
                // The force we apply to minimize our error is our error times our constant called kProportionalConst
                sForce = error*kProportionalConst;
               
                // Assign the current velocity by adding itself to steering force * deltaTime
                currentVelocity = currentVelocity + (sForce*Time.deltaTime);

                // Move the missile according to all of the above calculation on the 2D plane
                transform.Translate(new Vector3(currentVelocity.x, currentVelocity.y, 0));

                // Rotate the sprite to face the target - note: using a missile sprite nested in a parent gameobject so that the rotation works correctly. Otherwise it goes way out and the missile
                // position is messed up. So we rotate the nested sprite, but move the actual gameobject, which also houses the collider2D bounding box
                float targetAngle = Mathf.Atan2(currentVelocity.y, currentVelocity.x)*Mathf.Rad2Deg;
                missileSpriteTransform.rotation = Quaternion.Slerp(missileSpriteTransform.rotation,
                    Quaternion.Euler(0, 0, targetAngle), 100.0f*Time.deltaTime);
            }
            else
            {
                AudioSource.PlayClipAtPoint(GetComponent<AudioSource>().clip, gameObject.transform.position);
                GracefullyDestroyMissile(transform.position);
            }
        }
    }
}