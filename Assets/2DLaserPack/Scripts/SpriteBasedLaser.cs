using System.Linq;
using UnityEngine;
using System.Collections;

namespace TwoDLaserPack
{
    public class SpriteBasedLaser : MonoBehaviour
    {
        /// <summary>
        /// The starting laser component 'template' to use when assembling our laser dynamically
        /// </summary>
        public GameObject laserStartPiece;

        /// <summary>
        /// The middle laser component 'template' to use when assembling our laser dynamically
        /// </summary>
        public GameObject laserMiddlePiece;

        /// <summary>
        /// The end laser component 'template' to use when assembling our laser dynamically
        /// </summary>
        public GameObject laserEndPiece;

        /// <summary>
        /// Reference to the line renderer used for the 'arc' lightning effect.
        /// </summary>
        public LineRenderer laserLineRendererArc;

        /// <summary>
        /// Specifies how many segments the arc section of the laser should use. Default is 20.
        /// </summary>
        public int laserArcSegments = 20;

        /// <summary>
        /// Reference to the RandomPositionMover script - this script is used to generate random positions, and in this case the Y position 
        /// is used to set oscillation targets for the laser beam (to give an effect of slight oscillation on the laser beam)
        /// </summary>
        public RandomPositionMover laserOscillationPositionerScript;

        /// <summary>
        /// If enabled, the laser will randomly oscillate up and down within the oscillation max thresholds.
        /// </summary>
        public bool oscillateLaser;

        /// <summary>
        /// Specify the maximum starting length of the laser. Default is 20f.
        /// </summary>
        public float maxLaserLength = 20f;

        /// <summary>
        /// Specify the speed at which the laser beam oscillates up and down if oscillation is enabled. Default value is 1f.
        /// </summary>
        public float oscillationSpeed = 1f;

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
        /// Reference to the particle effect gameobject to display on the laser hit location.
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
        /// The maximum distance to raycast when checking for targets that the laser can collide with. Ideally this should be large enough for the laser to go "off screen".
        /// </summary>
        public float maxLaserRaycastDistance;

        /// <summary>
        /// If enabled, sets lasers to face toward a designated target (targetGo). Lasers can then use lerpLaserRotation to 'lerp' track targets based on the set turningRate value, or to instantly track targets (true or false).
        /// </summary>
        public bool laserRotationEnabled;

        /// <summary>
        /// If enabled, this will slowly lerp the laser rotation to face it's target, rather than instantly facing the target. (Requires laser rotation to be enabled too).
        /// </summary>
        public bool lerpLaserRotation;

        /// <summary>
        /// Used when both laserRotationEnabled and lerpLaserRotation are set to true. Determines the rate at which lasers track their designated target gameobjects. (targetGo).
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

        public delegate void LaserHitTriggerHandler(RaycastHit2D hitInfo);

        /// <summary>
        /// Event that fires whenever the laser collides with an object (requires ignoreCollisions to be false). Subscribe to this event to be notified when the laser is hitting an object. The RaycastHit2D info will be sent through the event.
        /// </summary>
        public event LaserHitTriggerHandler OnLaserHitTriggered;

        /// <summary>
        /// If enabled, an arc lightning effect is added to the laser.
        /// </summary>
        public bool useArc;

        /// <summary>
        /// Sets the maximum radius used to find oscillation height if laser oscillation is enabled. This gets set on the referenced 'RandomPositionMover' script when this script initialises. Default is 0.2f.
        /// </summary>
        public float oscillationThreshold = 0.2f;

        /// <summary>
        /// Set in the OnEnable method to keep a stored copy of the GameObject this script is attached to.
        /// </summary>
        private GameObject gameObjectCached;

        /// <summary>
        /// Keeps track of any angle required for the laser when it uses rotation.
        /// </summary>
        private float laserAngle;

        /// <summary>
        /// Used to keep track of the current Y lerp value the laser beam will move toward. (Used for oscillation).
        /// </summary>
        private float lerpYValue;

        /// <summary>
        /// Used to track the initial starting laser length, so we can revert the laser back to this when it is not hitting objects.
        /// </summary>
        private float startLaserLength;

        /// <summary>
        /// Reference to track the laser 'start' GameObject.
        /// </summary>
        private GameObject startGoPiece;

        /// <summary>
        /// Reference to track the laser 'middle' GameObject.
        /// </summary>
        private GameObject middleGoPiece;

        /// <summary>
        /// Reference to track the laser 'end' GameObject.
        /// </summary>
        private GameObject endGoPiece;

        /// <summary>
        /// Keeps a reference to the width of the 'start' laser sprite width.
        /// </summary>
        private float startSpriteWidth;

        /// <summary>
        /// Used to help control the firing of the laser hit trigger event.
        /// </summary>
        private bool waitingForTriggerTime;

        private ParticleSystem.EmissionModule hitSparkEmission;

        private void Awake()
        {
            hitSparkEmission = hitSparkParticleSystem.emission;
        }

        private void OnEnable()
        {
            gameObjectCached = gameObject;
            if (laserLineRendererArc != null) laserLineRendererArc.SetVertexCount(laserArcSegments);
        }

        private void Start()
        {
            startLaserLength = maxLaserLength;
            if (laserOscillationPositionerScript != null) laserOscillationPositionerScript.radius = oscillationThreshold;
        }

        /// <summary>
        /// Oscillate the laser pieces up/down by lerping their positions to random point on the Y axis.
        /// </summary>
        private void OscillateLaserParts(float currentLaserDistance)
        {
            if (laserOscillationPositionerScript == null) return;

            lerpYValue = Mathf.Lerp(middleGoPiece.transform.localPosition.y, laserOscillationPositionerScript.randomPointInCircle.y, Time.deltaTime * oscillationSpeed);

            // Start and middle pieces are always present, oscillate them here.
            if (startGoPiece != null && middleGoPiece != null)
            {
                var newRandomPos = new Vector2(startGoPiece.transform.localPosition.x, laserOscillationPositionerScript.randomPointInCircle.y);
                var newPosition = Vector2.Lerp(startGoPiece.transform.localPosition, newRandomPos, Time.deltaTime*oscillationSpeed);
                startGoPiece.transform.localPosition = newPosition;

                var newMidPosition = new Vector2((currentLaserDistance/2f) + startSpriteWidth/4, lerpYValue);
                middleGoPiece.transform.localPosition = newMidPosition;
            }

            // End piece is only sometimes present (if the laser is colliding with an object) so handle it's oscillation separately.
            if (endGoPiece != null)
            {
                var newPosition = new Vector2(currentLaserDistance + startSpriteWidth/2, lerpYValue);
                endGoPiece.transform.localPosition = newPosition;
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
                var val = Mathf.Sin(index + Time.time*Random.Range(0.5f, 1.3f));
                var clampedYValue = Mathf.Clamp(val, laserArcMaxYDown, laserArcMaxYUp);
                var pos = new Vector2(index*1.2f, clampedYValue);

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

        private void Update()
        {
            if (gameObjectCached != null)
            {
                if (laserActive)
                {
                    // Create the laser starting piece from our prefab
                    if (startGoPiece == null)
                    {
                        InstantiateLaserPart(ref startGoPiece, laserStartPiece);
                        //startGoPiece = Instantiate(laserStartPiece);
                        startGoPiece.transform.parent = transform;
                        startGoPiece.transform.localPosition = Vector2.zero;
                        startSpriteWidth = laserStartPiece.GetComponent<Renderer>().bounds.size.x;
                    }

                    // Create the laser middle piece from our prefab
                    if (middleGoPiece == null)
                    {
                        InstantiateLaserPart(ref middleGoPiece, laserMiddlePiece);
                        //middleGoPiece = Instantiate(laserMiddlePiece);
                        middleGoPiece.transform.parent = transform;
                        middleGoPiece.transform.localPosition = Vector2.zero;
                    }

                    middleGoPiece.transform.localScale = new Vector3(maxLaserLength - startSpriteWidth + 0.2f,
                        middleGoPiece.transform.localScale.y, middleGoPiece.transform.localScale.z);

                    if (oscillateLaser)
                    {
                        OscillateLaserParts(maxLaserLength);
                    }
                    else
                    {
                        if (middleGoPiece != null)
                        {
                            middleGoPiece.transform.localPosition = new Vector2(
                                (maxLaserLength / 2f) + startSpriteWidth / 4, lerpYValue);
                        }

                        if (endGoPiece != null)
                        {
                            endGoPiece.transform.localPosition = new Vector2(maxLaserLength + startSpriteWidth / 2, 0f);
                        }
                    }

                    RaycastHit2D hit;
                    if (laserRotationEnabled && targetGo != null)
                    {
                        // Get our facing direction
                        var facingDirection = targetGo.transform.position - gameObjectCached.transform.position;

                        // Get our facing angle in radians
                        laserAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
                        if (laserAngle < 0f)
                        {
                            laserAngle = Mathf.PI*2 + laserAngle;
                        }

                        // Convert our angle to degrees.
                        var angleDegrees = laserAngle*Mathf.Rad2Deg;

                        if (lerpLaserRotation)
                        {
                            transform.rotation = Quaternion.Slerp(transform.rotation,
                                Quaternion.AngleAxis(angleDegrees, transform.forward), Time.deltaTime*turningRate);

                            // Get the wanted facing direction so that we can 'lerp' the raycast (we don't want a raycast that instantly faces the right place, so our particle effect will hit targets as the ray lerps onto them).
                            var targetForward = transform.rotation*Vector3.right;

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
                        hit = Physics2D.Raycast(this.transform.position, transform.right, maxLaserRaycastDistance, mask);
                    }

                    if (!ignoreCollisions)
                    {
                        if (hit.collider != null)
                        {
                            maxLaserLength = Vector2.Distance(hit.point, this.transform.position) + startSpriteWidth/4;
                            InstantiateLaserPart(ref endGoPiece, laserEndPiece);

                            // Position the hit particle system at the hit location
                            if (hitSparkParticleSystem != null)
                            {
                                hitSparkParticleSystem.transform.position = hit.point;
                                hitSparkEmission.enabled = true;
                            }

                            if (useArc)
                            {
                                if (!laserLineRendererArc.enabled) laserLineRendererArc.enabled = true;
                                // Set the arc laser vertices according to segment length and hit position
                                SetLaserArcVertices(maxLaserLength, true);
                                SetLaserArcSegmentLength();
                            }
                            else
                            {
                                if (laserLineRendererArc.enabled) laserLineRendererArc.enabled = false;
                            }

                            if (!waitingForTriggerTime) StartCoroutine(HitTrigger(collisionTriggerInterval, hit));
                        }
                        else
                        {
                            SetLaserBackToDefaults();

                            if (useArc)
                            {
                                if (!laserLineRendererArc.enabled) laserLineRendererArc.enabled = true;
                                SetLaserArcSegmentLength();
                                // Set the arc laser vertices according to segment length, don't use the vector2 position passed in
                                SetLaserArcVertices(0f, false);
                            }
                            else
                            {
                                if (laserLineRendererArc.enabled) laserLineRendererArc.enabled = false;
                            }
                        }
                    }
                    else
                    {
                        SetLaserBackToDefaults();
                        // Set the arc laser vertices according to segment length, don't use the vector2 position passed in
                        SetLaserArcVertices(0f, false);
                        SetLaserArcSegmentLength();
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
            if (startGoPiece != null) startGoPiece.SetActive(enabledStatus);
            if (middleGoPiece != null) middleGoPiece.SetActive(enabledStatus);
            if (endGoPiece != null) endGoPiece.SetActive(enabledStatus);
            if (laserLineRendererArc != null) laserLineRendererArc.enabled = enabledStatus;
            if (hitSparkParticleSystem != null) hitSparkEmission.enabled = enabledStatus;
        }

        /// <summary>
        /// Changes the segment count on the arc laser based on the current length of the laser.
        /// </summary>
        private void SetLaserArcSegmentLength()
        {
            var newLaserSegmentLength = (Mathf.Abs((int) maxLaserLength));
            laserLineRendererArc.SetVertexCount(newLaserSegmentLength);
            laserArcSegments = newLaserSegmentLength;
        }

        /// <summary>
        /// Sets the laser back to the default length and removes the end 'collision' section.
        /// Also removes the hit spark particle effect if it is being used.
        /// </summary>
        private void SetLaserBackToDefaults()
        {
            // Set laser back to defaults if there is no raycast collision hit.
            Destroy(endGoPiece);

            maxLaserLength = startLaserLength;

            // disable the hit spark particle system and move it back
            if (hitSparkParticleSystem != null)
            {
                hitSparkEmission.enabled = false;
                hitSparkParticleSystem.transform.position = new Vector2(maxLaserLength, transform.position.y);
            }
        }

        private void InstantiateLaserPart(ref GameObject laserComponent, GameObject laserPart)
        {
            if (laserComponent == null)
            {
                laserComponent = Instantiate(laserPart);
                laserComponent.transform.parent = gameObject.transform;
                laserComponent.transform.localPosition = Vector2.zero;
                laserComponent.transform.localEulerAngles = Vector2.zero;
            }
        }

        /// <summary>
        /// Call when you want to re-initialise the laser with new component parts (start/middle/end pieces). As soon as these are destroyed (ref becomes null),
        /// the update method will re-construct the laser from the template GameObject prefab start/mid/end pieces referenced on the script's public fields.
        /// </summary>
        public void DisableLaserGameObjectComponents()
        {
            Destroy(startGoPiece);
            Destroy(middleGoPiece);
            Destroy(endGoPiece);
        }
    }
}