using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

namespace TwoDHomingMissiles
{
    public class MissileLaunchScript : MonoBehaviour
    {
        public GameObject missilePrefab;

        [HideInInspector] 
        public Sprite launcherNodeSprite;

        public float missileSpeed = 1.35f;
        public float missileProportionalConst = 0.55f;
        public float launcherNodeOffsetRandomXAmount = 0f;
        public float launcherNodeOffsetRandomYAmount = 0f;
        public float launcherNodeOffsetDistance = 2f;
        public float stagedLaunchDelay = 0.25f;
        public float initialTargetChangeTimer = 0.5f;
        public bool useObjectPoolToSpawnMissiles;
        public bool useLauncherNodes;
        public bool applyLauncherNodeRandomness;
        public bool stagedLaunch;
        public bool autoFireEnabled;
        public bool autoFireAtClosestTargets;
        public string autoFireTagToTarget;
        public float autoFireInterval = .1f;

        public GameObject TargetPosition0;
        public GameObject TargetPosition1;
        public GameObject TargetPosition2;

        public List<GameObject> launcherNodes;

        public GameObject playerPos;

        // Use this for initialization
        private void Start()
        {
           
            if (autoFireEnabled)
            {
                InvokeRepeating("StartInvokes", 30f, 30f);
            }
            
        }

        private void AutoFireMissiles()
        {
            var targetGameObjects = GameObject.FindGameObjectsWithTag(autoFireTagToTarget);
            if (targetGameObjects != null && targetGameObjects.Length > 0)
            {
                var enemyDictionary = new Dictionary<float, GameObject>();
                if (autoFireAtClosestTargets)
                {
                    foreach (var enemy in targetGameObjects)
                    {
                        var distance = Vector2.Distance(enemy.transform.position, gameObject.transform.position);
                        enemyDictionary.Add(distance, enemy);
                    }

                    // Sort dictionary from closest to farthest targets
                    var orderedDictionary = enemyDictionary.OrderBy(k => k.Key);
                    // Create a temporary target list and add the closest to farthest targets in order.
                    var tempEnemyList = new List<GameObject>();
                    foreach (var entry in orderedDictionary)
                    {
                        tempEnemyList.Add(entry.Value);
                    }

                    // Take X number of targets from the target list based on how many missile nodes there are configured and fire missiles at each target (random targeting)
                    var enemyTargetArray = tempEnemyList.Take(launcherNodes.Count).ToArray();
                    StartCoroutine(FireMissileRandomTargeting(enemyTargetArray, launcherNodes, stagedLaunchDelay, true));
                }
                else
                {
                    Debug.Log(launcherNodes.ElementAt(0));
                    var enemyTargetArray = targetGameObjects.Take(launcherNodes.Count).ToArray();
                    StartCoroutine(FireMissileRandomTargeting(enemyTargetArray, launcherNodes, stagedLaunchDelay, true));
                }
            }
        }

        // Update is called once per frame
        private void Update()
        {
            TargetPosition0 = GameObject.Find("TargetPosition0");
            TargetPosition1 = GameObject.Find("TargetPosition1");
            TargetPosition2 = GameObject.Find("TargetPosition2");
            playerPos = GameObject.Find("Player");
        }

        private IEnumerator FireMissileRandomTargeting(GameObject[] targetGameObjects, List<GameObject> launcherNodes, float delay, bool swarmMissilesOutward)
        {
            Debug.Log(launcherNodes);
            foreach (var launcherNode in launcherNodes)
            {
                Debug.Log(launcherNode);
                var launcherNodeScriptRef = (MissileLauncherNode)launcherNode.GetComponent(typeof(MissileLauncherNode));
                
                if (launcherNodeScriptRef == null) Debug.LogError("There is no MissileLauncherNode script attached to " + launcherNode.name + ". Please ensure this is added to the launcher node gameobject correctly.");

                var newMissile = FetchAndPositionAMissile(launcherNode);
                var missileScriptReference = SetMissileControllerParameters(newMissile);
                newMissile.SetActive(true);

                if (swarmMissilesOutward)
                {
                    if (launcherNodeScriptRef.applyLauncherNodeRandomness)
                    {
                        // Apply randomness to the node position so that missiles get a bit of variety as they launch/swarm outward.
                        launcherNodeScriptRef.nodeMissileSwarmTarget.transform.position = GetRandomOffsetVector2(launcherNodeScriptRef.nodeOffsetPositionOriginalPos, launcherNodeScriptRef.launcherNodeOffsetRandomXAmount, launcherNodeScriptRef.launcherNodeOffsetRandomYAmount);
                    }
                    else
                    {
                        launcherNodeScriptRef.nodeMissileSwarmTarget.transform.position = launcherNodeScriptRef.nodeOffsetPositionOriginalPos;
                    }

                    // Initially target a swarm node target (offset from the launcher node) and also set relevant main target too.
                    if (launcherNodeScriptRef.nodeMissileSwarmTarget != null)
                    {
                        missileScriptReference.target = launcherNodeScriptRef.nodeMissileSwarmTarget;
                        missileScriptReference.mainTarget = targetGameObjects[Random.Range(0, targetGameObjects.Length)];
                    }
                    else
                    {
                        Debug.LogError("Could not determine a valid launcher node swarm target to set as the missile target.");
                    }
                }
                else
                {
                    missileScriptReference.target = targetGameObjects[Random.Range(0, targetGameObjects.Length)];
                }

                if (stagedLaunch)
                {
                    yield return new WaitForSeconds(delay);
                }
                else
                {
                    yield return new WaitForSeconds(0f);
                }
            }
        }

        private IEnumerator FireMissileUniformTargeting(GameObject[] targetGameObjects, List<GameObject> launcherNodes, int launcherNodeCount, float delay, bool swarmMissilesOutward)
        {
            for (var i = 0; i < launcherNodeCount; i++)
            {
                var launcherNode = launcherNodes[i % launcherNodeCount];
                var targetCount = targetGameObjects.Length;
                var missilesLaunched = 0;

                var launcherNodeScriptRef = (MissileLauncherNode)launcherNode.GetComponent(typeof(MissileLauncherNode));
                if (launcherNodeScriptRef == null)
                {
                    Debug.LogError("There is no MissileLauncherNode script attached to " + launcherNode.name + ". Please ensure this is added to the launcher node gameobject correctly.");
                }

                if (launcherNodeCount > targetCount)
                {
                    // There are more launcher nodes (and hence missiles) than targets, so we fire missiles at each target, and then use any remaining missiles to fire at targets again...
                    var targetIndex = (i % targetCount);
                    var currentTarget = targetGameObjects[targetIndex];

                    missilesLaunched++;

                    var newMissile = FetchAndPositionAMissile(launcherNode);
                    var missileScriptReference = SetMissileControllerParameters(newMissile);
                    newMissile.name = newMissile.name + "_" + missilesLaunched;
                    newMissile.SetActive(true);

                    if (swarmMissilesOutward)
                    {
                        if (launcherNodeScriptRef.applyLauncherNodeRandomness)
                        {
                            // Apply randomness to the node position so that missiles get a bit of variety as they launch/swarm outward.
                            launcherNodeScriptRef.nodeMissileSwarmTarget.transform.position = GetRandomOffsetVector2(launcherNodeScriptRef.nodeOffsetPositionOriginalPos, launcherNodeScriptRef.launcherNodeOffsetRandomXAmount, launcherNodeScriptRef.launcherNodeOffsetRandomYAmount);
                        }
                        else
                        {
                            launcherNodeScriptRef.nodeMissileSwarmTarget.transform.position = launcherNodeScriptRef.nodeOffsetPositionOriginalPos;
                        }

                        // Initially target a swarm node target (offset from the launcher node) and also set relevant main target too.
                        if (launcherNodeScriptRef.nodeMissileSwarmTarget != null)
                        {
                            missileScriptReference.target = launcherNodeScriptRef.nodeMissileSwarmTarget;
                            missileScriptReference.mainTarget = currentTarget;
                        }
                        else
                        {
                            Debug.LogError("Could not determine a valid launcher node swarm target to set as the missile target.");
                        }
                    }
                    else
                    {
                        missileScriptReference.target = currentTarget;
                    }
                }
                else
                {
                    // There are more targets than launcher nodes, so we fire missiles at each target, and stop on the last launcher node.
                    var targetIndexToLaunchAt = i;

                    var newMissile = FetchAndPositionAMissile(launcherNode);
                    var missileScriptReference = SetMissileControllerParameters(newMissile);
                    newMissile.SetActive(true);

                    if (swarmMissilesOutward)
                    {
                        if (launcherNodeScriptRef.applyLauncherNodeRandomness)
                        {
                            // Apply randomness to the node position so that missiles get a bit of variety as they launch/swarm outward.
                            launcherNodeScriptRef.nodeMissileSwarmTarget.transform.position = GetRandomOffsetVector2(launcherNodeScriptRef.nodeOffsetPositionOriginalPos, launcherNodeScriptRef.launcherNodeOffsetRandomXAmount, launcherNodeScriptRef.launcherNodeOffsetRandomYAmount);
                        }
                        else
                        {
                            launcherNodeScriptRef.nodeMissileSwarmTarget.transform.position = launcherNodeScriptRef.nodeOffsetPositionOriginalPos;
                        }

                        // Initially target a swarm node target (offset from the launcher node) and also set relevant main target too.
                        if (launcherNodeScriptRef.nodeMissileSwarmTarget != null)
                        {
                            missileScriptReference.target = launcherNodeScriptRef.nodeMissileSwarmTarget;
                            missileScriptReference.mainTarget = targetGameObjects[targetIndexToLaunchAt];
                        }
                        else
                        {
                            Debug.LogError("Could not determine a valid launcher node swarm target to set as the missile target.");
                        }
                    }
                    else
                    {
                        missileScriptReference.target = targetGameObjects[targetIndexToLaunchAt];
                    }
                }

                if (stagedLaunch)
                {
                    yield return new WaitForSeconds(delay);
                }
                else
                {
                    yield return new WaitForSeconds(0f);
                }
            }
        }

        /// <summary>
        /// Generate a new missile either from the object pool, or by instantiating a new one, and place it at the launcher node GameObject position.
        /// </summary>
        /// <param name="launcherNode"></param>
        /// <returns></returns>
        private GameObject FetchAndPositionAMissile(GameObject launcherNode)
        {
            GameObject newMissile;
            // Create the missile GameObject according to whether we are instantiating new missiles, or using the ObjectPool to spawn missiles.
            if (!useObjectPoolToSpawnMissiles)
            {
                newMissile = (GameObject)Instantiate(missilePrefab, new Vector3(launcherNode.transform.position.x, launcherNode.transform.position.y, 0f), Quaternion.identity);
                newMissile.SetActive(false);
            }
            else
            {
                newMissile = ObjectPoolManager.instance.GetUsableMissileFromObjectPool();
                newMissile.transform.position = new Vector3(launcherNode.transform.position.x, launcherNode.transform.position.y, 0f);
            }
            return newMissile;
        }

        /// <summary>
        /// Generate a new missile either from the object pool, or by instantiating a new one, and place it at the transform position.
        /// </summary>
        /// <param name="parentTransform"></param>
        /// <returns></returns>
        private GameObject FetchAndPositionAMissile(Transform parentTransform)
        {
            GameObject newMissile;
            // Create the missile GameObject according to whether we are instantiating new missiles, or using the ObjectPool to spawn missiles.
            if (!useObjectPoolToSpawnMissiles)
            {
                newMissile = (GameObject)Instantiate(missilePrefab, new Vector3(parentTransform.position.x, parentTransform.position.y, 0f), Quaternion.identity);
                newMissile.SetActive(false);
            }
            else
            {
                newMissile = ObjectPoolManager.instance.GetUsableMissileFromObjectPool();
                newMissile.transform.position = new Vector3(parentTransform.position.x, parentTransform.position.y, 0f);
                newMissile.SetActive(true);
            }
            return newMissile;
        }

        /// <summary>
        /// Launch missile(s) from the GameObject that this script is attached to.
        /// </summary>
        /// <param name="targetGameObjects">Array of GameObjects to target with missiles (if using launchernodes) if not using launcherNodes then the first target will be targeted with a single missile.</param>
        /// <param name="randomTargeting">If randomTargeting is false, then fires missile(s) at target GameObject array uniformly. i.e. first missile goes to first target GameObject, second goes to second target GameObject etc... If there are more missiles to fire than there are target GameObjects to target, then the excess missiles will target starting at the beginning of the target GameObject array again... If randomTargeting is true then missiles will fire at random targets when launched.</param>
        /// <param name="swarmMissilesOutward">If swarmMissilesOutward is true then missiles are initially launched to target an area just above, or just below their launcher nodes (only works when you are using launcherNodes to fire missiles). The missiles travel toward these first, in an outward motion, and then shortly afterward switch to track their designated targets instead. This gives a "nice" arcing missile effect.</param>
        public void LaunchMissiles(GameObject[] targetGameObjects, bool randomTargeting, bool swarmMissilesOutward)
        {
            if (targetGameObjects.Length <= 0) return; // Return if there are no valid target game objects passed in.

            if (randomTargeting)
            {
                // Target random targets
                if (useLauncherNodes)
                {
                    StartCoroutine(FireMissileRandomTargeting(targetGameObjects, launcherNodes, stagedLaunchDelay, swarmMissilesOutward));
                }
                else
                {
                    // Grab a missile and set target to random target in the target array that was passed into this method.
                    var newMissile = FetchAndPositionAMissile(transform);
                    var missileScriptReference = SetMissileControllerParameters(newMissile);
                    var selectedIndex = Random.Range(0, targetGameObjects.Length);
                    missileScriptReference.target = targetGameObjects[selectedIndex]; //Random.Range(0, targetGameObjects.Length)];
                    newMissile.SetActive(true);
                }
            }
            else
            {
                // Uniform targeting (not random)
                // Need to launch at initial target nodes first, and also set main target to be correct target index item for the launcher node usage here...
                var launcherNodeCount = launcherNodes.Count;

                if (stagedLaunch)
                {
                    StartCoroutine(FireMissileUniformTargeting(targetGameObjects, launcherNodes, launcherNodeCount, stagedLaunchDelay, swarmMissilesOutward));
                }
                else
                {
                    // No delay in firing if no staged launch
                    StartCoroutine(FireMissileUniformTargeting(targetGameObjects, launcherNodes, launcherNodeCount, 0f, swarmMissilesOutward));
                }
            }
        }

        private MissileController SetMissileControllerParameters(GameObject newMissile)
        {
            var missileScriptReference = newMissile.GetComponent<MissileController>();

            if (missileScriptReference == null)
            {
                Debug.LogError("The missile GameObject queried has not got a valid MissileController script on it.");
                return null;
            }

            missileScriptReference.kProportionalConst = missileProportionalConst;
            missileScriptReference.maxSpeed = missileSpeed;
            missileScriptReference.usingObjectPool = useObjectPoolToSpawnMissiles;
            missileScriptReference.initialTargetChangeTimer = initialTargetChangeTimer;
            missileScriptReference.isTrackingInitialTarget = true;

            return missileScriptReference;
        }

        private Vector2 GetRandomOffsetVector2(Vector2 input, float xAmount, float yAmount)
        {
            var absXAmount = Mathf.Abs(xAmount);
            var absYAmount = Mathf.Abs(yAmount);
            return new Vector2(input.x + Random.Range(-absXAmount, absXAmount),
                input.y + Random.Range(-absYAmount, absYAmount));
        }
        private IEnumerator wait()
        {
            MultiTargets t = TargetPosition0.GetComponent<MultiTargets>();
            MultiTargets t1 = TargetPosition1.GetComponent<MultiTargets>();
            MultiTargets t2 = TargetPosition2.GetComponent<MultiTargets>();

            yield return new WaitUntil(() => t.getDone() && t1.getDone() && t2.getDone());
            
            InvokeRepeating("AutoFireMissiles", 0f, autoFireInterval);
            yield return new WaitForSeconds(5f);
            CancelInvoke("AutoFireMissiles");
            yield return new WaitForSeconds(5f);
            t.setDone(false);
            t1.setDone(false);
            t2.setDone(false);


           t.setSet(true);
           t1.setSet(true);
           t2.setSet(true);



        }

        private void StartInvokes()
        {

            Vector2 min1 = Camera.main.ViewportToWorldPoint(new Vector2(.3f, .3f));
            Vector2 max1 = Camera.main.ViewportToWorldPoint(new Vector2(.6f, .6f));

            MultiTargets t = TargetPosition0.GetComponent<MultiTargets>();
            MultiTargets t1 = TargetPosition1.GetComponent<MultiTargets>();
            MultiTargets t2 = TargetPosition2.GetComponent<MultiTargets>();
            t.setSet(false);
            t1.setSet(false);
            t2.setSet(false);
            t.setDone(false);
            t1.setDone(false);
            t2.setDone(false);

            t.setTargetVector(new Vector2(playerPos.transform.position.x + 3f, Random.Range(min1.y, max1.y)));
            t1.setTargetVector(new Vector2(Random.Range(min1.x, max1.x), playerPos.transform.position.y + 3f));
            t2.setTargetVector(new Vector2(playerPos.transform.position.x + 4.5f, playerPos.transform.position.x + 1.5f));

            StartCoroutine(wait());
           
        }
    }

   

}