using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

namespace TwoDHomingMissiles
{
    [RequireComponent(typeof (MissileLaunchScript))]
    public class DemoMissileLaunchOptions : MonoBehaviour
    {
        public GameObject[] TargetGameObjects;
        public Sprite LauncherNodeOffsetSprite;
        // Make sure this is hooked up to your player's WeaponSystem script for the demo GUI to work!
        public MissileLaunchScript LaunchScriptRef;
        public bool missileLaunchOptionsVisible, missilePerformanceControlsVisible;
        public bool useObjectPool, useLauncherNodes, stagedMissileLaunch, swarmMissilesOutward;
        public float missileSpeedSliderValue = 0.35f;
        public float missileProportionalConstSliderValue = 0.55f;
        public float stagedMissileLaunchDelay = 0.2f;
        public float launchRandomnessX = 2f;
        public float launchRandomnessY = 0f;
        public GUIText propConstLabel, maxSpeedLabel, propConstInfoLabel, maxSpeedInfoLabel;
        
        private bool _nodeOffsetsVisible;
        private float screenHeight;
        private float screenWidth;
        private float timeToTrackInitialTargetNode = 0.5f;
        private Rect guiControlsLeftRect, guiControlsTopRect;
        private bool mouseOverGui;
        private bool _missileRandomnessOnLaunch;

        public bool NodeOffsetsVisible
        {
            get { return _nodeOffsetsVisible; }
            set
            {
                if (_nodeOffsetsVisible == value) return;
                _nodeOffsetsVisible = value;
                if (LaunchScriptRef != null)
                {
                    if (_nodeOffsetsVisible)
                    {
                        foreach (var node in LaunchScriptRef.launcherNodes)
                        {
                            var nodeOffsetGameObject = node.transform.GetChild(0);
                            
                            if (nodeOffsetGameObject == null) continue;
                            if (LauncherNodeOffsetSprite == null) continue;

                            var sr = nodeOffsetGameObject.gameObject.GetComponent<SpriteRenderer>();
                            if (sr == null)
                            {
                                sr = nodeOffsetGameObject.gameObject.AddComponent<SpriteRenderer>();
                                sr.sprite = LauncherNodeOffsetSprite;
                                sr.sortingLayerName = "Default";
                                sr.sortingOrder = 10;
                            }
                            else
                            {
                                sr.enabled = true;
                            }
                            
                        }
                    }
                    else
                    {
                        foreach (var node in LaunchScriptRef.launcherNodes)
                        {
                            var nodeOffsetGameObject = node.transform.GetChild(0);

                            if (nodeOffsetGameObject == null) continue;
                            if (LauncherNodeOffsetSprite == null) continue;

                            var sr = nodeOffsetGameObject.gameObject.GetComponent<SpriteRenderer>();
                            if (sr != null) sr.enabled = false;
                        }
                    }
                }
            }
        }

        public bool MissileRandomnessOnLaunch
        {
            get
            {
                return _missileRandomnessOnLaunch;
            }
            set
            {
                _missileRandomnessOnLaunch = value;
                foreach (var node in LaunchScriptRef.launcherNodes)
                {
                    var launcherNode = node.GetComponent<MissileLauncherNode>();
                    if (launcherNode != null)
                    {
                        launcherNode.applyLauncherNodeRandomness = _missileRandomnessOnLaunch;
                    }
                }
            }
        }

        // Use this for initialization
        private void Start()
        {
            LaunchScriptRef = gameObject.GetComponent<MissileLaunchScript>();
            screenHeight = Screen.height;
            screenWidth = Screen.width;

            guiControlsLeftRect = new Rect(10, 10, 250, screenHeight);
            guiControlsTopRect = new Rect(screenWidth / 4, 5, 500, 60);

            // Set up some demo scene defaults
            swarmMissilesOutward = true;
            useLauncherNodes = true;
            stagedMissileLaunch = true;
            MissileRandomnessOnLaunch = true;
        }

        void OnGUI()
        {
            GUI.Label(new Rect(Screen.width/4 - 65, Screen.height - 60, 750, 25),
                "Left click to fire missiles. Missiles will target 3 x objects flying on the right side of the screen.");

            GUI.Label(new Rect(Screen.width/4 - 65, Screen.height - 40, 700, 50),
                "Use the sliders and controls to adjust the missile launch and performance options! Check the MissileLaunchScript.cs script in-editor for even more customisation settings.");

            if (LaunchScriptRef != null)
            {
                if (Application.loadedLevelName != "WeaponConfigurationAndInventoryDemoScene")
                {
                    missileLaunchOptionsVisible = GUI.Toggle(new Rect(10, 5, 200, 20), missileLaunchOptionsVisible, "Show missile launch options");

                    missilePerformanceControlsVisible = GUI.Toggle(new Rect(Screen.width / 2 - 125, 5, 230, 20), missilePerformanceControlsVisible, "Show missile performance controls");
                }

                if (missileLaunchOptionsVisible)
                {
                    useObjectPool = GUI.Toggle(new Rect(10, 50, 200, 20), useObjectPool, "Use missile object pool");
                    useLauncherNodes = GUI.Toggle(new Rect(10, 90, 200, 20), useLauncherNodes, "Use launcher nodes");

                    if (useLauncherNodes)
                    {
                        swarmMissilesOutward = GUI.Toggle(new Rect(10, 130, 200, 20), swarmMissilesOutward, "Missiles swarm outward at first");
                        NodeOffsetsVisible = GUI.Toggle(new Rect(10, 150, 200, 20), NodeOffsetsVisible, "Initial tracking node offsets visible");
                        stagedMissileLaunch = GUI.Toggle(new Rect(10, 170, 200, 20), stagedMissileLaunch, "Staged launch");

                        if (stagedMissileLaunch)
                        {
                            GUI.Label(new Rect(15, 200, 230, 25), "Stage launch delay: " + Math.Round(stagedMissileLaunchDelay, 2));
                            stagedMissileLaunchDelay = GUI.HorizontalSlider(new Rect(15, 225, 200, 25), stagedMissileLaunchDelay, 0.00F, 3F);
                        }

                        MissileRandomnessOnLaunch = GUI.Toggle(new Rect(10, 260, 200, 20), MissileRandomnessOnLaunch, "Missile Randomness on launch");

                        if (MissileRandomnessOnLaunch)
                        {
                            GUI.Label(new Rect(15, 285, 230, 25), "launch Randomness X to add: " + Math.Round(launchRandomnessX, 2));
                            launchRandomnessX = GUI.HorizontalSlider(new Rect(15, 305, 200, 25), launchRandomnessX, 0.00F, 5F);
                            GUI.Label(new Rect(15, 330, 230, 25), "launch Randomness Y to add: " + Math.Round(launchRandomnessY, 2));
                            launchRandomnessY = GUI.HorizontalSlider(new Rect(15, 350, 200, 25), launchRandomnessY, 0.00F, 5F);
                        }

                        GUI.Label(new Rect(15, 385, 250, 25), "Time to track initial 'fake' nodes: " + Math.Round(timeToTrackInitialTargetNode, 2));
                        timeToTrackInitialTargetNode = GUI.HorizontalSlider(new Rect(15, 410, 200, 25), timeToTrackInitialTargetNode, 0.1F, 5F);
                    }
                }

                if (missilePerformanceControlsVisible)
                {
                    maxSpeedLabel.enabled = true;
                    propConstLabel.enabled = true;
                    propConstInfoLabel.enabled = true;
                    maxSpeedInfoLabel.enabled = true;

                    missileSpeedSliderValue = GUI.HorizontalSlider(new Rect(Screen.width / 4, 45, 200, 20), missileSpeedSliderValue, 0.05F, 1.5F);
                    missileProportionalConstSliderValue = GUI.HorizontalSlider(new Rect(Screen.width / 2, 45, 200, 20), missileProportionalConstSliderValue, 0.05F, 1.0F);

                    maxSpeedLabel.text = Math.Round(missileSpeedSliderValue, 2).ToString();
                    propConstLabel.text = Math.Round(missileProportionalConstSliderValue, 2).ToString();
                }
                else
                {
                    maxSpeedLabel.enabled = false;
                    propConstLabel.enabled = false;
                    propConstInfoLabel.enabled = false;
                    maxSpeedInfoLabel.enabled = false;
                }
            }

            if (GUI.Button(new Rect(Screen.width - 420, Screen.height - 90, 140, 30), "Next demo scene >>"))
            {
                var currentLevelIndex = Application.loadedLevel;
                if (currentLevelIndex < Application.levelCount - 1)
                {
                    Application.LoadLevel(currentLevelIndex + 1);
                }
                else
                {
                    Application.LoadLevel(0);
                }
            }
            if (GUI.Button(new Rect(280, Screen.height - 90, 140, 30), "<< Prev demo scene"))
            {
                var currentLevelIndex = Application.loadedLevel;
                if (currentLevelIndex > 0)
                {
                    Application.LoadLevel(currentLevelIndex - 1);
                }
                else
                {
                    Application.LoadLevel(Application.levelCount - 1);
                }
            }

        }

        // Update is called once per frame
        private void Update()
        {
            var mousePosInGUICoords = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            if (guiControlsLeftRect.Contains(mousePosInGUICoords) ||
                guiControlsTopRect.Contains(mousePosInGUICoords))
            {
                mouseOverGui = true;
            }
            else
            {
                mouseOverGui = false;
            }

            if (Input.GetMouseButtonDown(1))
            {
                ScreenCapture.CaptureScreenshot(Path.Combine(@"C:\Temp", Guid.NewGuid().ToString() + ".png"));
            }

            if (Input.GetMouseButtonUp(0) && !mouseOverGui)
            {
                // Set options based on selection in the demo scene using the GUI controls
                LaunchScriptRef.missileSpeed = missileSpeedSliderValue;
                LaunchScriptRef.missileProportionalConst = missileProportionalConstSliderValue;
                LaunchScriptRef.useObjectPoolToSpawnMissiles = useObjectPool;
                LaunchScriptRef.useLauncherNodes = useLauncherNodes;
                LaunchScriptRef.stagedLaunch = stagedMissileLaunch;
                LaunchScriptRef.stagedLaunchDelay = stagedMissileLaunchDelay;

                LaunchScriptRef.applyLauncherNodeRandomness = MissileRandomnessOnLaunch;
                LaunchScriptRef.launcherNodeOffsetRandomXAmount = launchRandomnessX;
                LaunchScriptRef.launcherNodeOffsetRandomYAmount = launchRandomnessY;
                LaunchScriptRef.initialTargetChangeTimer = timeToTrackInitialTargetNode;

                if (LaunchScriptRef.useLauncherNodes)
                {
                    // Get targets and highlight them, then fire missiles at them.
                    HighlightTargets(TargetGameObjects);

                    if (LaunchScriptRef.launcherNodes.Any(ln => ln == null))
                    {
                        Debug.LogError("'useLauncherNodes' is enabled, but there are no launcher nodes added to your object or one or more launcher nodes are null/not-defined. You require launcher nodes in order to use this option. Use the MissileLaunchScript custom inspector to add some launcher nodes to fire missiles from.");
                    }
                    else
                    {
                        // Uniform targeting
                        LaunchScriptRef.LaunchMissiles(TargetGameObjects, true, swarmMissilesOutward);
                    }
                }
                else
                {
                    // Uniform targeting
                    // Get Single random target and highlight it, then fire missiles at it.

                    var randomIndex = Random.Range(0, TargetGameObjects.Length);
                    var randomTarget = TargetGameObjects[randomIndex];
                    var randomSingleTargetArray = new[] { randomTarget };

                    HighlightTargets(randomSingleTargetArray);
                    LaunchScriptRef.LaunchMissiles(randomSingleTargetArray, true, swarmMissilesOutward);
                }
            }
        }

        private void HighlightTargets(IEnumerable<GameObject> targetsArray)
        {
            foreach (var targetGameObject in targetsArray)
            {
                var highlightSpriteGo = targetGameObject.transform.GetChild(0);
                if (highlightSpriteGo == null) continue;
                var highlightScriptRef = (DemoHighlightTarget) highlightSpriteGo.GetComponent(typeof (DemoHighlightTarget));
                if (highlightScriptRef != null) StartCoroutine(highlightScriptRef.HighlightTarget());
            }
        }
    }
}