using System.Linq;
using UnityEngine;
using System.Collections;

namespace TwoDLaserPack
{
    public class LineBasedLaser : MonoBehaviour
    {
        /// <summary>
        /// Reference to the line renderer used for the 'arc' lightning effect.
        /// </summary>
        public LineRenderer laserLineRendererArc;

        /// <summary>
        /// Reference to the line renderer used for the main laser.
        /// </summary>
        public LineRenderer laserLineRenderer;

        /// <summary>
        /// Specifies how many segments the arc section of the laser should use. Default is 20.
        /// </summary>
        private int laserArcSegments = 150;

        /// <summary>
        /// Determines whether the laser is 'firing' or not.
        /// </summary>
        public bool laserActive;

        /// <summary>
        /// Ignores any collisions the laser finds by use of it's raycast check. If this is enabled, lasers will not collide with any Collider2D objects.
        /// </summary>
        public bool ignoreCollisions;

        /// <summary>
        /// The target GameObject for the laser to track if tracking is enabled.
        /// </summary>
        public GameObject targetGo;

        /// <summary>
        /// Rate at which the laser's texture offset is moved to give an 'animation' effect to the laser stream. Default is 1f.
        /// </summary>
        public float laserTexOffsetSpeed = 1f;

        /// <summary>
        /// Reference to the particle effect gameobject to display on the laser hit location
        /// </summary>
        public ParticleSystem hitSparkParticleSystem;

        /// <summary>
        /// the maximum clamp value that a laser arc is allowed to go up to on the negative Y scale.
        /// </summary>
        public float laserArcMaxYDown;

        /// <summary>
        /// the maximum clamp value that a laser arc is allowed to go up to on the positive Y scale.
        /// </summary>
        public float laserArcMaxYUp;

        /// <summary>
        /// The maximum distance to raycast when checking for targets that the laser can collide with. Ideally this should be large enough for the laser to go "off screen". Default is 20f.
        /// </summary>
        public float maxLaserRaycastDistance = 200f;

        /// <summary>
        /// If enabled, sets lasers to face toward a designated target (targetGo). Lasers can then use lerpLaserRotation to 'lerp' track targets based on the set turningRate value, or to instantly track targets (true or false).
        /// </summary>
        public bool laserRotationEnabled;

        /// <summary>
        /// If enabled, this will slowly lerp the laser rotation to face it's target, rather than instantly facing the target. (Requires laser rotation to be enabled too).
        /// </summary>
        public bool lerpLaserRotation;

        /// <summary>
        /// Used when both laserRotationEnabled and lerpLaserRotation are set to true. Determines the rate at which lasers track their designated target gameobjects. (targetGo). Default is 3f.
        /// </summary>
        public float turningRate = 3f;

        /// <summary>
        /// The time between collision trigger events. If the laser is colliding with an object, this interval is used to fire the laser hit trigger event which other objects can subscribe to for custom functionality.
        /// The lower this number is, the more demanding the laser will be on performance, but you will get more frequent updates.
        /// </summary>
        public float collisionTriggerInterval = 0.25f;

        /// <summary>
        /// A LayerMask used for the laser's raycast hit detection. Use this to customise what the laser can collide/hit against.
        /// </summary>
        public LayerMask mask;

        /// <summary>
        /// The sorting layer name to use for the laser and the laser arc
        /// </summary>
        public string sortLayer = "Default";

        /// <summary>
        /// The order in which to render the laser and laser arc on it's defined sorting layer.
        /// </summary>
        public int sortOrder = 0;

        public delegate void LaserHitTriggerHandler(RaycastHit2D hitInfo);

        /// <summary>
        /// Event that fires whenever the laser collides with an object (requires ignoreCollisions to be false). 
        /// Subscribe to this event to be notified when the laser is hitting an object. The RaycastHit2D info will be sent through the event.
        /// </summary>
        public event LaserHitTriggerHandler OnLaserHitTriggered;

        /// <summary>
        /// If enabled, an arc lightning effect is added to the laser.
        /// </summary>
        public bool useArc;

        /// <summary>
        /// Set in the OnEnable method to keep a stored copy of the GameObject this script is attached to.
        /// </summary>
        private GameObject gameObjectCached;

        /// <summary>
        /// Keeps track of any angle required for the laser when it uses rotation
        /// </summary>
        private float laserAngle;

        /// <summary>
        /// Used to keep track of laser texture offset.
        /// </summary>
        private float laserTextureOffset;

        /// <summary>
        /// The amount to scale the laser arc renderer's material texture on the X axis.
        /// </summary>
        private float laserTextureXScale;

        /// <summary>
        /// The start value of the laser line renderer's material texture scale on the X axis.
        /// </summary>
        private float startLaserTextureXScale;

        /// <summary>
        /// The start value of laserSegmentLength. Used to revert the laser arc segments back to their original when the laser is not colliding with objects.
        /// </summary>
        private int startLaserSegmentLength;

        /// <summary>
        /// Used to help control the firing of the laser hit trigger event.
        /// </summary>
        private bool waitingForTriggerTime;

       
        /// MyStuff
        public bool isKillable = false;

        private ParticleSystem.EmissionModule hitSparkEmission;

        // Use this for initialization
        void Start()
        {
            // Get the starting x texture scale for the material on the laser line renderer.
            startLaserTextureXScale = laserLineRenderer.material.mainTextureScale.x;
            startLaserSegmentLength = laserArcSegments;

            laserLineRenderer.sortingLayerName = sortLayer;
            laserLineRenderer.sortingOrder = sortOrder;

            laserLineRendererArc.sortingLayerName = sortLayer;
            laserLineRendererArc.sortingOrder = sortOrder;
        }

        void Awake()
        {
            hitSparkEmission = hitSparkParticleSystem.emission;
        }

        void OnEnable()
        {
            gameObjectCached = gameObject;
            if (laserLineRendererArc != null) laserLineRendererArc.SetVertexCount(laserArcSegments);
        }

        void OnDisable()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (gameObjectCached != null)
            {
                if (laserActive)
                {
                    // Animate the laser texture
                    laserLineRenderer.material.mainTextureOffset = new Vector2(laserTextureOffset, 0f);
                    laserTextureOffset -= Time.deltaTime * laserTexOffsetSpeed;

                    RaycastHit2D hit;
                    if (laserRotationEnabled && targetGo != null)
                    {
                        // Get our facing direction
                        var facingDirection = targetGo.transform.position - gameObjectCached.transform.position;

                        // Get our facing angle in radians
                        laserAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
                        if (laserAngle < 0f)
                        {
                            laserAngle = Mathf.PI * 2 + laserAngle;
                        }

                        // Convert our angle to degrees.
                        var angleDegrees = laserAngle * Mathf.Rad2Deg;

                        if (lerpLaserRotation)
                        {
                            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angleDegrees, transform.forward), Time.deltaTime * turningRate);

                            // Get the wanted facing direction so that we can 'lerp' the raycast (we don't want a raycast that instantly faces the right place, so our particle effect will hit targets as the ray lerps onto them).
                            var targetForward = transform.rotation * Vector3.right;

                            // Raycast down our 'lerp' direction (targetForward)
                            hit = Physics2D.Raycast(this.transform.position, targetForward, maxLaserRaycastDistance, mask);
                        }
                        else
                        {
                            transform.rotation = Quaternion.AngleAxis(angleDegrees, transform.forward);

                            // Raycast down our direct facing direction (facingDirection)
                            hit = Physics2D.Raycast(this.transform.position, facingDirection, maxLaserRaycastDistance, mask);
                        }
                    }
                    else
                    {
                        // Raycast down our transform right vector direction
                        hit = Physics2D.Raycast(this.transform.position, transform.right, 200f, mask);
                    }

                    if (!ignoreCollisions)
                    {
                        if (hit.collider != null)
                        {
                            Debug.Log(hit.collider);
                            //SetLaserEndToTargetLocation(hit);
                            if (isKillable)
                            {
                               if (hit.collider.gameObject.name == "Player") hit.collider.gameObject.GetComponent<PlayerController>().setHealth(0f);
                            }
                           
                           // if (!waitingForTriggerTime) StartCoroutine(HitTrigger(collisionTriggerInterval, hit));
                        }
                        else
                        {
                            
                            // Set laser back to defaults if there is no raycast collision hit.
                            SetLaserToDefaultLength();
                        }
                    }
                    else
                    {
                        // Set laser back to defaults if there is no raycast collision hit.
                        SetLaserToDefaultLength();
                    }
                }
            }
        }

        /// <summary>
        /// Fires the OnLaserHitTriggered event every triggerInterval seconds.
        /// </summary>
        /// <param name="triggerInterval"></param>
        /// <param name="hit"></param>
        /// <returns></returns>
        private IEnumerator HitTrigger(float triggerInterval, RaycastHit2D hit)
        {
            waitingForTriggerTime = true;
            if (OnLaserHitTriggered != null) OnLaserHitTriggered(hit);
            yield return new WaitForSeconds(triggerInterval);
            waitingForTriggerTime = false;
        }

        /// <summary>
        /// Sets the laser state to enabled or disabled
        /// </summary>
        public void SetLaserState(bool enabledStatus)
        {
            laserActive = enabledStatus;
            laserLineRenderer.enabled = enabledStatus;
            if (laserLineRendererArc != null) laserLineRendererArc.enabled = enabledStatus;
            if (hitSparkParticleSystem != null) hitSparkEmission.enabled = enabledStatus;

        }

        /// <summary>
        /// Sets the laser length to match that of the target it's raycast has hit and scales the texture of the line renderer accordingly.
        /// Enables and positions the hit spark particle effect.
        /// </summary>
        /// <param name="hit"></param>
        private void SetLaserEndToTargetLocation(RaycastHit2D hit)
        {
            // Get laser length from the line renderer transform location to the hit point of the ray.
            var currentLaserSize = (Vector2.Distance(hit.point, laserLineRenderer.transform.position));
            laserLineRenderer.SetPosition(1, new Vector2(currentLaserSize, 0f));

            laserTextureXScale = startLaserTextureXScale * currentLaserSize;

            // Change the tiling of the laser texture to scale with the laser length
            laserLineRenderer.material.mainTextureScale = new Vector2(laserTextureXScale, 1f);

            if (useArc)
            {
                if (!laserLineRendererArc.enabled) laserLineRendererArc.enabled = true;

                var newLaserSegmentLength = (Mathf.Abs((int)currentLaserSize));
                laserLineRendererArc.SetVertexCount(newLaserSegmentLength);
                laserArcSegments = newLaserSegmentLength;

                // Set the arc laser vertices according to segment length and hit position
                SetLaserArcVertices(currentLaserSize, true);
            }
            else
            {
                if (laserLineRendererArc.enabled) laserLineRendererArc.enabled = false;
            }

            // Position the hit particle system at the hit location
            if (hitSparkParticleSystem != null)
            {
                hitSparkParticleSystem.transform.position = hit.point;
                hitSparkEmission.enabled = true;
            }
        }

        /// <summary>
        /// Sets the laser back to its default length based on laserSegmentLength and scales the texture to suitable value.
        /// Disables and re-positions the hit spark particle effect.
        /// </summary>
        private void SetLaserToDefaultLength()
        {
            // Set the laser length back to normal
            laserLineRenderer.SetPosition(1, new Vector2(laserArcSegments, 0f));

            // Change the tiling of the laser texture back to start scale
            laserTextureXScale = startLaserTextureXScale * laserArcSegments;
            laserLineRenderer.material.mainTextureScale = new Vector2(laserTextureXScale, 1f);

            if (useArc)
            {
                if (!laserLineRendererArc.enabled) laserLineRendererArc.enabled = true;
                // Change the arc laser segment length back to the original
                laserLineRendererArc.SetVertexCount(startLaserSegmentLength);
                laserArcSegments = startLaserSegmentLength;

                // Set the arc laser vertices according to segment length, don't use the vector2 position passed in
                SetLaserArcVertices(0f, false);
            }
            else
            {
                if (laserLineRendererArc.enabled) laserLineRendererArc.enabled = false;

                // Change the arc laser segment length back
                laserLineRendererArc.SetVertexCount(startLaserSegmentLength);
                laserArcSegments = startLaserSegmentLength;
            }

            // disable the hit spark particle system and move it back
            if (hitSparkParticleSystem != null)
            {
                hitSparkEmission.enabled = false;
                hitSparkParticleSystem.transform.position = new Vector2(laserArcSegments, transform.position.y);
            }
        }

        /// <summary>
        /// Sets the laser's arc vertices' positions based on clamping values as well as distance.
        /// This provides a lightning-like effect to the arc that is customisable.
        /// </summary>
        /// <param name="distancePoint"></param>
        /// <param name="useHitPoint"></param>
        private void SetLaserArcVertices(float distancePoint, bool useHitPoint)
        {
            for (var index = 1; index < laserArcSegments; index++)
            {
                var val = Mathf.Sin(index + Time.time * Random.Range(0.5f, 1.3f));
                var clampedYValue = Mathf.Clamp(val, laserArcMaxYDown, laserArcMaxYUp);
                var pos = new Vector2(index * 1.2f, clampedYValue);

                if (useHitPoint && index == laserArcSegments - 1)
                {
                    laserLineRendererArc.SetPosition(index, new Vector2(distancePoint, 0f));
                }
                else
                {
                    laserLineRendererArc.SetPosition(index, pos);
                }
            }
        }
    }
}
