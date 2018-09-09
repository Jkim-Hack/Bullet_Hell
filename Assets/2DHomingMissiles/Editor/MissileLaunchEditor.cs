using System.Diagnostics;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using Debug = UnityEngine.Debug;

namespace TwoDHomingMissiles
{
    [Flags]
    public enum EditorListOption
    {
        None = 0,
        ListSize = 1,
        ListLabel = 2,
        ElementLabels = 4,
        Buttons = 8,
        Default = ListSize | ListLabel | ElementLabels,
        NoElementLabels = ListSize | ListLabel,
        All = Default | Buttons
    }

    /// <summary>
    /// Custom inspector/editor for WeaponSystem component script
    /// </summary>
    [CustomEditor(typeof (MissileLaunchScript)), CanEditMultipleObjects]
    public class MissileLaunchEditor : Editor
    {
        private static readonly Color LightGreenColour = new Color(0.565f, 0.933f, 0.565f, 1.0f);

        private void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
        }

        /// <summary>
        /// Draws a basic separator texture in the custom inspector.
        /// </summary>
        public static void DrawSeparator()
        {
            GUILayout.Space(12f);

            if (Event.current.type == EventType.Repaint)
            {
                Texture2D tex = EditorGUIUtility.whiteTexture;

                Rect rect = GUILayoutUtility.GetLastRect();

                var savedColor = GUI.color;
                GUI.color = new Color(0f, 0f, 0f, 0.25f);

                GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 4f), tex);
                GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 1f), tex);
                GUI.DrawTexture(new Rect(0f, rect.yMin + 9f, Screen.width, 1f), tex);

                GUI.color = savedColor;
            }

        }

        /// <summary>
        /// All the custom editor inspector handling is done here.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawSeparator();

            EditorGUILayout.Space();
            var enableRichTextStyle = new GUIStyle {richText = true};
            EditorGUILayout.LabelField("<size=12><b>Missile launch options</b></size>", enableRichTextStyle);
            EditorGUILayout.Space();

            var missileLaunchScriptRef = (MissileLaunchScript) target;

            var useObjectPoolToSpawnMissilesTooltip = new GUIContent("Use object pool for missiles",
                "Specify you want to use the included object pool manager script to spawn missiles from a " +
                "pre-initialised object pool. Note: you should have an ObjectPoolManager script attached to a " +
                "gameobject in your scene, set up with a missile prefab, and number of initial missiles to spawn when using this option.");
            var useObjectPoolToSpawnMissilesProp = serializedObject.FindProperty("useObjectPoolToSpawnMissiles");

            useObjectPoolToSpawnMissilesProp.boolValue = EditorGUILayout.Toggle(useObjectPoolToSpawnMissilesTooltip,
                useObjectPoolToSpawnMissilesProp.boolValue);

            #region MissilePrefab

            if (!useObjectPoolToSpawnMissilesProp.boolValue)
            {
                var missilePrefabTooltip = new GUIContent("Missile prefab",
                    "Specify a prefab to use for missiles that are fired.");

                var missilePrefabProp = serializedObject.FindProperty("missilePrefab");

                if (missilePrefabProp.objectReferenceValue == null)
                {
                    GUI.color = Color.white;
                    EditorGUILayout.HelpBox("Choose a suitable prefab to use for missiles fired.", MessageType.Info,
                        true);
                    GUI.color = Color.white;
                }

                EditorGUILayout.PropertyField(missilePrefabProp, missilePrefabTooltip);
            }
            else
            {
                var objectPoolManager = FindObjectOfType<ObjectPoolManager>();
                if (objectPoolManager == null || objectPoolManager.enabled == false)
                {
                    GUI.color = Color.red;
                    EditorGUILayout.HelpBox(
                        "You do not have an ObjectPoolManager script instance in your scene (or you have one and it is not enabled). This is required in order to fire missiles from an Object Pool. Create an empty GameObject and attach the ObjectPoolManager.cs Monobehaviour script to it, then set it up with a suitable missile prefab to use, along with a number to initially spawn.",
                        MessageType.Error, true);
                    GUI.color = Color.white;
                }
                else
                {
                    GUI.color = LightGreenColour;
                    EditorGUILayout.HelpBox(
                        "An enabled ObjectPoolManager instance was detected in your scene. This will be used to get missiles from the pool when they are fired.",
                        MessageType.Info, true);
                    GUI.color = Color.white;
                }

                EditorGUILayout.Space();
            }

            #endregion

            var missileSpeedTooltip = new GUIContent("Missile Speed", "Controls how fast missiles are able to move.");
            var missilePropConstTooltip = new GUIContent("Missile Proportional Constant",
                "Used to tweak performance of missile. Lower is more 'swingy' and higher is more 'direct/concise'");


            var missileSpeedProp = serializedObject.FindProperty("missileSpeed");
            var missileProportionalConstProp = serializedObject.FindProperty("missileProportionalConst");


            EditorGUILayout.Slider(missileSpeedProp, 0f, 1f, missileSpeedTooltip);
            EditorGUILayout.Slider(missileProportionalConstProp, 0f, 1f, missilePropConstTooltip);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("<size=12><b>Multiple missile options</b> (enable for more options)</size>",
                enableRichTextStyle);
            EditorGUILayout.Space();

            #region LauncherNodes

            var useLauncherNodesTooltip = new GUIContent("Use launcher nodes",
                "Allows you to specify individual launch node positions on your GameObject for firing multiple missiles, and enables use of more " +
                "properties relating to multiple missile usage.");

            var useLauncherNodesProp = serializedObject.FindProperty("useLauncherNodes");


            useLauncherNodesProp.boolValue = EditorGUILayout.Toggle(useLauncherNodesTooltip,
                useLauncherNodesProp.boolValue);

            // Show more details if useLauncherNodes is enabled.
            if (useLauncherNodesProp.boolValue)
            {
                EditorGUI.indentLevel++;

                var applyLauncherNodeRandomnessTooltip = new GUIContent("Randomness on launches",
                    "Applies a specified amount of randomness to the X/Y position of the initial " +
                    "target that a missile travels toward when initially launched (before it re-targets it's actual target). " +
                    "This setting makes for some interesting swarming launch patterns when firing multiple missiles, and prevents " +
                    "'uniform/boring' looking launches. Enter a positive value, which will be turned into a random number between this " +
                    "value and it's negative version. E.g. 2.5 will be random value between -2.5 and 2.5");
                var launcherNodeOffsetRandomXAmountTooltip = new GUIContent("Random X offset",
                    "Value to use for random X offset applied to missiles initial target direction when fired. (Gives a nice random look to initially launched missiles).");
                var launcherNodeOffsetRandomYAmountTooltip = new GUIContent("Random Y offset",
                    "Value to use for random Y offset applied to missiles initial target direction when fired. (Gives a nice random look to initially launched missiles).");
                var launcherNodeOffsetDistanceTooltip = new GUIContent("Node offset distance",
                    "Value to offset the spawn distance of the launcher node initial targets when the game starts. These initial targets are what the missiles initially track when launched to give them a 'swarm' looking effect. After the initial target timer runs out, they re-target their 'true' targets.");
                var launcherNodeSpriteTooltip = new GUIContent("Launcher node sprite",
                    "A Sprite for the launcher node to use when created. It's layer order will be to 1 above that of the parent object's sprite if it has one.");
                var stagedLaunchTooltip = new GUIContent("Staged launch",
                    "Allows you to specify if missiles should be fired from launcher nodes one at a time in succession. If enabled, you can also set the delay between firing from each launcher node.");
                var stagedLaunchDelayTooltip = new GUIContent("Staged launch delay", "Allows you to specify the delay between missiles firing from each launcher node.");
                var initialTargetChangeTimerTooltip = new GUIContent("Initial node targeting time", "Set how long you want missiles that swarm out to 'fake' target nodes to track the fake nodes for. Note you'll need a longer time to be set here the further you make your 'fake' target nodes from the missile origin point.");

                var launcherNodeSpriteProp = serializedObject.FindProperty("launcherNodeSprite");
                var launcherNodeOffsetRandomXAmountProp =
                    serializedObject.FindProperty("launcherNodeOffsetRandomXAmount");
                var launcherNodeOffsetRandomYAmountProp =
                    serializedObject.FindProperty("launcherNodeOffsetRandomYAmount");
                var applyLauncherNodeRandomnessProp = serializedObject.FindProperty("applyLauncherNodeRandomness");
                var launcherNodeOffsetDistanceProp = serializedObject.FindProperty("launcherNodeOffsetDistance");
                var stagedLaunchProp = serializedObject.FindProperty("stagedLaunch");
                var stagedLaunchDelayProp = serializedObject.FindProperty("stagedLaunchDelay");
                var initialTargetChangeTimerProp = serializedObject.FindProperty("initialTargetChangeTimer");

                EditorGUILayout.PropertyField(launcherNodeSpriteProp, launcherNodeSpriteTooltip);
                EditorGUILayout.Slider(launcherNodeOffsetDistanceProp, 0f, 10f, launcherNodeOffsetDistanceTooltip);

                applyLauncherNodeRandomnessProp.boolValue = EditorGUILayout.Toggle(applyLauncherNodeRandomnessTooltip,
                    applyLauncherNodeRandomnessProp.boolValue);

                // More options to show if launcher node randomness is enabled.
                if (applyLauncherNodeRandomnessProp.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.Slider(launcherNodeOffsetRandomXAmountProp, 0f, 10f,
                        launcherNodeOffsetRandomXAmountTooltip);
                    EditorGUILayout.Slider(launcherNodeOffsetRandomYAmountProp, 0f, 10f,
                        launcherNodeOffsetRandomYAmountTooltip);
                    EditorGUI.indentLevel--;
                }

                stagedLaunchProp.boolValue = EditorGUILayout.Toggle(stagedLaunchTooltip,
                    stagedLaunchProp.boolValue);

                // More options to show if staged launching is enabled.
                if (stagedLaunchProp.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.Slider(stagedLaunchDelayProp, 0.01f, 10f,
                        stagedLaunchDelayTooltip);
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Slider(initialTargetChangeTimerProp, 0.1f, 5f, initialTargetChangeTimerTooltip);

                #region launcher node management

                var launcherNodesProp = serializedObject.FindProperty("launcherNodes");

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<size=11><b>Create new launcher node</b></size>", enableRichTextStyle);

                GUI.color = Color.white;
                EditorGUILayout.HelpBox(
                    "Launcher nodes determine points from where missiles are fired. For every launcher node you have, a missile will be fired when using the LaunchMissiles method on the MissileLaunchScript component.",
                    MessageType.Info, true);
                GUI.color = Color.white;

                var createButtonTooltipTop = new GUIContent("Create launcher node (up)",
                    "Create a new launcher node point on the parent GameObject, which will fire missiles in an upward direction before they begin tracking target.");
                var createButtonTooltipBottom = new GUIContent("Create launcher node (down)",
                    "Create a new launcher node point on the parent GameObject, which will fire missiles in a downward direction before they begin tracking target.");
                var createButtonTooltipLeft = new GUIContent("Create launcher node (left)",
                    "Create a new launcher node point on the parent GameObject, which will fire missiles in a left direction before they begin tracking target.");
                var createButtonTooltipRight = new GUIContent("Create launcher node (right)",
                    "Create a new launcher node point on the parent GameObject, which will fire missiles in a right direction before they begin tracking target.");

                EditorGUILayout.Space();

                GUI.color = LightGreenColour;
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();

                var layoutOptions = new GUILayoutOption[] {GUILayout.Height(20), GUILayout.Width(170)};

                var applyRandomness = applyLauncherNodeRandomnessProp.boolValue;

                if (GUILayout.Button(createButtonTooltipTop, EditorStyles.miniButtonLeft, layoutOptions))
                {
                    var nodeGo = CreateAndConfigureMissileLauncherNode("LauncherNodeUp_" + launcherNodesProp.arraySize,
                        launcherNodeSpriteProp, missileLaunchScriptRef, MissileLauncherNode.MissileNodeDirection.Up,
                        launcherNodeOffsetDistanceProp.floatValue, launcherNodeOffsetRandomXAmountProp.floatValue,
                        launcherNodeOffsetRandomYAmountProp.floatValue, applyRandomness);
                    missileLaunchScriptRef.launcherNodes.Add(nodeGo);
                }

                if (GUILayout.Button(createButtonTooltipBottom, EditorStyles.miniButtonRight, layoutOptions))
                {
                    var nodeGo = CreateAndConfigureMissileLauncherNode(
                        "LauncherNodeDown_" + launcherNodesProp.arraySize, launcherNodeSpriteProp,
                        missileLaunchScriptRef, MissileLauncherNode.MissileNodeDirection.Down,
                        launcherNodeOffsetDistanceProp.floatValue, launcherNodeOffsetRandomXAmountProp.floatValue,
                        launcherNodeOffsetRandomYAmountProp.floatValue, applyRandomness);
                    missileLaunchScriptRef.launcherNodes.Add(nodeGo);
                }

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();

                if (GUILayout.Button(createButtonTooltipLeft, EditorStyles.miniButtonLeft, layoutOptions))
                {
                    var nodeGo = CreateAndConfigureMissileLauncherNode(
                        "LauncherNodeLeft_" + launcherNodesProp.arraySize, launcherNodeSpriteProp,
                        missileLaunchScriptRef, MissileLauncherNode.MissileNodeDirection.Left,
                        launcherNodeOffsetDistanceProp.floatValue, launcherNodeOffsetRandomXAmountProp.floatValue,
                        launcherNodeOffsetRandomYAmountProp.floatValue, applyRandomness);
                    missileLaunchScriptRef.launcherNodes.Add(nodeGo);
                }

                if (GUILayout.Button(createButtonTooltipRight, EditorStyles.miniButtonRight, layoutOptions))
                {
                    var nodeGo =
                        CreateAndConfigureMissileLauncherNode("LauncherNodeRight_" + launcherNodesProp.arraySize,
                            launcherNodeSpriteProp, missileLaunchScriptRef,
                            MissileLauncherNode.MissileNodeDirection.Right, launcherNodeOffsetDistanceProp.floatValue,
                            launcherNodeOffsetRandomXAmountProp.floatValue,
                            launcherNodeOffsetRandomYAmountProp.floatValue, applyRandomness);
                    missileLaunchScriptRef.launcherNodes.Add(nodeGo);
                }

                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                GUI.color = Color.white;

                #endregion

                MissileLaunchEditorList.Show(launcherNodesProp, EditorListOption.All);
                EditorGUI.indentLevel--;
            }

            #endregion

            DrawSeparator();

            #region Launch Control

            EditorGUILayout.LabelField("<size=12><b>Launch control</b></size>", enableRichTextStyle);
            EditorGUILayout.Space();

            var autoFireEnabledTooltip = new GUIContent("Auto fire", "Enable this to auto fire missiles on a set interval");
            var autoFireEnabledProp = serializedObject.FindProperty("autoFireEnabled");

            autoFireEnabledProp.boolValue = EditorGUILayout.Toggle(autoFireEnabledTooltip,
                autoFireEnabledProp.boolValue);

            if (autoFireEnabledProp.boolValue)
            {
                var autoFireAtClosestTargetsTooltip = new GUIContent("Target closest", "Enable this to get missiles to target the closest GameObjects. (Closest targeted GameObjects by tag specified, by distance).");
                var autoFireAtClosestTargetsProp = serializedObject.FindProperty("autoFireAtClosestTargets");

                autoFireAtClosestTargetsProp.boolValue = EditorGUILayout.Toggle(autoFireAtClosestTargetsTooltip,
                    autoFireAtClosestTargetsProp.boolValue);

                var autoFireIntervalTooltip = new GUIContent("Auto fire interval", "The time between missile launches.");
                var autoFireIntervalProp = serializedObject.FindProperty("autoFireInterval");
                EditorGUILayout.Slider(autoFireIntervalProp, 0.1f, 60f, autoFireIntervalTooltip);

                var autoFireTagToTargetTooltip = new GUIContent("Tag to target", "The tag used for the missiles to target. All active GameObjects in the scene with this tag will be targeted when missiles are launched.");
                var autoFireTagToTarget = serializedObject.FindProperty("autoFireTagToTarget");
                EditorGUILayout.PropertyField(autoFireTagToTarget, autoFireTagToTargetTooltip);
            }

            #endregion

            DrawSeparator();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(missileLaunchScriptRef);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static GameObject CreateAndConfigureMissileLauncherNode(string name, SerializedProperty launcherNodeSpriteProp, MissileLaunchScript launchScript, MissileLauncherNode.MissileNodeDirection direction, float offsetDistance,
            float launcherNodeOffsetRandomXAmount, float launcherNodeOffsetRandomYAmount, bool applyRandomness)
        {
            var launcherNodeGo = new GameObject(name);
            launcherNodeGo.transform.parent = launchScript.transform;

            var missileLauncherNodeRef = launcherNodeGo.AddComponent<MissileLauncherNode>();

            if (missileLauncherNodeRef != null)
            {
                missileLauncherNodeRef.MissileNodeDirectionSetting = direction;
                missileLauncherNodeRef.nodeMissileSwarmTargetOffsetDistance = offsetDistance;
                missileLauncherNodeRef.applyLauncherNodeRandomness = applyRandomness;
                missileLauncherNodeRef.launcherNodeOffsetRandomXAmount = launcherNodeOffsetRandomXAmount;
                missileLauncherNodeRef.launcherNodeOffsetRandomYAmount = launcherNodeOffsetRandomYAmount;
            }

            // Add sprite to the launcher node GameObject and set layer order.
            var spriteRef = launcherNodeGo.AddComponent<SpriteRenderer>();
            var nodeSprite = (Sprite) launcherNodeSpriteProp.objectReferenceValue;
            if (nodeSprite != null)
            {
                spriteRef.sprite = (Sprite) launcherNodeSpriteProp.objectReferenceValue;
                var parentSpriteRenderer = launchScript.GetComponent<SpriteRenderer>();
                if (parentSpriteRenderer != null)
                {
                    spriteRef.sortingLayerName = parentSpriteRenderer.sortingLayerName;
                    spriteRef.sortingOrder = parentSpriteRenderer.sortingOrder + 1;
                }
            }

            launcherNodeGo.transform.localPosition = Vector2.zero;

            return launcherNodeGo;
        }

        /// <summary>
        /// Custom GUILayout progress bar for ammo remaining display and other progress bar usage...
        /// </summary>
        /// <param name="value"></param>
        /// <param name="label"></param>
        private void ProgressBar(float value, string label)
        {
            // Get a rect for the progress bar using the same margins as a textfield:
            Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
            EditorGUI.ProgressBar(rect, value, label);
            EditorGUILayout.Space();
        }
    }
}