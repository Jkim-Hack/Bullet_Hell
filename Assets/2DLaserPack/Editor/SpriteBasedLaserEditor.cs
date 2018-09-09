using System;
using UnityEditor;
using UnityEngine;
using System.Collections;

namespace TwoDLaserPack
{
    /// <summary>
    /// Custom inspector/editor for Laser component script
    /// </summary>
    [CustomEditor(typeof(SpriteBasedLaser)), CanEditMultipleObjects]
    public class SpriteBasedLaserEditor : Editor
    {
        private void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
        }

        /// <summary>
        /// All the custom editor inspector handling is done here.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            #region Laser

            EditorGUILayout.Space();
            var enableRichTextStyle = new GUIStyle { richText = true };
            EditorGUILayout.LabelField("<size=12><b>Laser settings</b></size>", enableRichTextStyle);
            EditorGUILayout.Space();

            var spriteBasedLaserRef = (SpriteBasedLaser)target;

            // Laser sprite component pieces
            var laserStartPieceTooltip = new GUIContent("Laser start piece", "The starting laser component 'template' to use when assembling our laser dynamically.");
            var laserStartPieceProp = serializedObject.FindProperty("laserStartPiece");

            if (laserStartPieceProp.objectReferenceValue == null)
            {
                GUI.color = Color.white;
                EditorGUILayout.HelpBox("A starting laser component 'template' prefab reference is required.", MessageType.Error, true);
                GUI.color = Color.white;
            }

            EditorGUILayout.PropertyField(laserStartPieceProp, laserStartPieceTooltip);

            var laserMiddlePieceTooltip = new GUIContent("Laser middle piece", "The starting laser component 'template' to use when assembling our laser dynamically.");
            var laserMiddlePieceProp = serializedObject.FindProperty("laserMiddlePiece");

            if (laserMiddlePieceProp.objectReferenceValue == null)
            {
                GUI.color = Color.white;
                EditorGUILayout.HelpBox("A middle laser component 'template' prefab reference is required.", MessageType.Error, true);
                GUI.color = Color.white;
            }

            EditorGUILayout.PropertyField(laserMiddlePieceProp, laserMiddlePieceTooltip);

            var laserEndPieceTooltip = new GUIContent("Laser end piece", "The starting laser component 'template' to use when assembling our laser dynamically.");
            var laserEndPieceProp = serializedObject.FindProperty("laserEndPiece");

            if (laserEndPieceProp.objectReferenceValue == null)
            {
                GUI.color = Color.white;
                EditorGUILayout.HelpBox("An end laser component 'template' prefab reference is required.", MessageType.Error, true);
                GUI.color = Color.white;
            }

            EditorGUILayout.PropertyField(laserEndPieceProp, laserEndPieceTooltip);

            var hitSparkParticleTooltip = new GUIContent("Hit particle effect", "Reference to the particle effect gameobject to display on the laser hit location");
            var hitSparkParticleSystemProp = serializedObject.FindProperty("hitSparkParticleSystem");

            if (hitSparkParticleSystemProp.objectReferenceValue == null)
            {
                GUI.color = Color.white;
                EditorGUILayout.HelpBox("A particle effect gameobject to display on the laser hit location should be specified here.", MessageType.Warning, true);
                GUI.color = Color.white;
            }

            EditorGUILayout.PropertyField(hitSparkParticleSystemProp, hitSparkParticleTooltip);

            var useArcTooltip = new GUIContent("Use Arc Laser", "Adds a lightning arc laser effect to the laser");
            var useArcProp = serializedObject.FindProperty("useArc");

            useArcProp.boolValue = EditorGUILayout.Toggle(useArcTooltip, useArcProp.boolValue);

            if (useArcProp.boolValue)
            {
                EditorGUI.indentLevel++;
                var laserArcPrefabTooltip = new GUIContent("Laser Arc GameObject", "Reference to a suitable GameObject with LineRenderer component to use for the laser arc effect.");

                var laserArcPrefabProp = serializedObject.FindProperty("laserLineRendererArc");

                if (laserArcPrefabProp.objectReferenceValue == null)
                {
                    GUI.color = Color.white;
                    EditorGUILayout.HelpBox("Specify a suitable GameObject with LineRenderer component to use for the laser arc effect.", MessageType.Error,
                        true);
                    GUI.color = Color.white;
                }

                EditorGUILayout.PropertyField(laserArcPrefabProp, laserArcPrefabTooltip);

                var laserArcSegmentsTooltip = new GUIContent("Laser Arc Segments", "Specifies how many segments the arc section of the laser should use. Default is 20.");
                var laserArcSegmentsProp = serializedObject.FindProperty("laserArcSegments");
                EditorGUILayout.IntSlider(laserArcSegmentsProp, 0, 100, laserArcSegmentsTooltip);

                var laserArcMaxYDownTooltip = new GUIContent("Arc down Y clamp value", "The maximum clamp value that a laser arc is allowed to go up to on the negative Y scale.");
                var laserArcMaxYDownProp = serializedObject.FindProperty("laserArcMaxYDown");
                EditorGUILayout.Slider(laserArcMaxYDownProp, -1.2f, 1.2f, laserArcMaxYDownTooltip);

                var laserArcMaxYUpTooltip = new GUIContent("Arc up Y clamp value", "The maximum clamp value that a laser arc is allowed to go up to on the positive Y scale.");
                var laserArcMaxYUpProp = serializedObject.FindProperty("laserArcMaxYUp");
                EditorGUILayout.Slider(laserArcMaxYUpProp, -1.2f, 1.2f, laserArcMaxYUpTooltip);

                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUILayout.Space();
            }

            var laserActiveTooltip = new GUIContent("Laser Active", "Determines whether the laser is 'firing' or not.");
            var laserActiveProp = serializedObject.FindProperty("laserActive");

            laserActiveProp.boolValue = EditorGUILayout.Toggle(laserActiveTooltip, laserActiveProp.boolValue);

            var oscillateLaserTooltip = new GUIContent("Oscillate laser", "If enabled, the laser will randomly oscillate up and down within the oscillation max thresholds.");
            var oscillateLaserProp = serializedObject.FindProperty("oscillateLaser");

            oscillateLaserProp.boolValue = EditorGUILayout.Toggle(oscillateLaserTooltip, oscillateLaserProp.boolValue);

            if (oscillateLaserProp.boolValue)
            {
                EditorGUI.indentLevel++;

                var laserOscillationPositionerScriptTooltip = new GUIContent("Positioner script", "Reference to the RandomPositionMover script - this script is used to generate random positions");

                var laserOscillationPositionerScriptProp = serializedObject.FindProperty("laserOscillationPositionerScript");

                if (laserOscillationPositionerScriptProp.objectReferenceValue == null)
                {
                    GUI.color = Color.white;
                    EditorGUILayout.HelpBox("Reference to the RandomPositionMover script is required", MessageType.Error, true);
                    GUI.color = Color.white;
                }

                EditorGUILayout.PropertyField(laserOscillationPositionerScriptProp, laserOscillationPositionerScriptTooltip);

                var oscillationSpeedTooltip = new GUIContent("Oscillation speed", "Specify the speed at which the laser beam oscillates up and down if oscillation is enabled. Default value is 1f.");
                var oscillationSpeedProp = serializedObject.FindProperty("oscillationSpeed");

                EditorGUILayout.Slider(oscillationSpeedProp, 0.01f, 15f, oscillationSpeedTooltip);

                var oscillationThresholdTooltip = new GUIContent("Oscillation threshold", "Sets the maximum radius used to find oscillation height if laser oscillation is enabled. This gets set on the referenced 'RandomPositionMover' script when this script initialises. Default is 0.2f. Note that this cannot be set via the editor at runtime, it must be set before initialisation.");
                var oscillationThresholdProp = serializedObject.FindProperty("oscillationThreshold");

                EditorGUILayout.Slider(oscillationThresholdProp, 0.01f, 3f, oscillationThresholdTooltip);

                EditorGUI.indentLevel--;
            }

            var ignoreCollisionsTooltip = new GUIContent("Ignore collisions", "Ignores any collisions the laser finds by use of it's raycast check. If this is enabled, lasers will not collide with any Collider2D objects.");
            var ignoreCollisionsProp = serializedObject.FindProperty("ignoreCollisions");

            ignoreCollisionsProp.boolValue = EditorGUILayout.Toggle(ignoreCollisionsTooltip, ignoreCollisionsProp.boolValue);

            var maskTooltip = new GUIContent("Layer Mask for Raycast detection", "Set a layer mask here to determine what the ray cast can collide with.");
            var maskProp = serializedObject.FindProperty("mask");

            EditorGUILayout.PropertyField(maskProp, maskTooltip);

            var laserRotationEnabledTooltip = new GUIContent("Laser rotation", "If enabled, sets lasers to face toward a designated target (targetGo). Lasers can then use lerpLaserRotation to 'lerp' track targets based on the set turningRate value, or to instantly track targets (true or false).");
            var laserRotationEnabledProp = serializedObject.FindProperty("laserRotationEnabled");

            laserRotationEnabledProp.boolValue = EditorGUILayout.Toggle(laserRotationEnabledTooltip, laserRotationEnabledProp.boolValue);

            if (laserRotationEnabledProp.boolValue)
            {
                EditorGUI.indentLevel++;
                var targetGoTooltip = new GUIContent("Laser target", "The target GameObject for the laser to track if tracking is enabled.");

                var targetGoProp = serializedObject.FindProperty("targetGo");

                if (targetGoProp.objectReferenceValue == null)
                {
                    GUI.color = Color.white;
                    EditorGUILayout.HelpBox("A target GameObject for the laser to track is required", MessageType.Error,
                        true);
                    GUI.color = Color.white;
                }

                EditorGUILayout.PropertyField(targetGoProp, targetGoTooltip);

                var lerpLaserRotationTooltip = new GUIContent("Lerp laser rotation", "If enabled, this will slowly lerp the laser rotation to face it's target, rather than instantly facing the target. (Requires laser rotation to be enabled too).");
                var lerpLaserRotationProp = serializedObject.FindProperty("lerpLaserRotation");

                lerpLaserRotationProp.boolValue = EditorGUILayout.Toggle(lerpLaserRotationTooltip, lerpLaserRotationProp.boolValue);

                if (lerpLaserRotationProp.boolValue)
                {
                    EditorGUI.indentLevel++;

                    var turningRateTooltip = new GUIContent("Laser turning rate", "Determines the rate at which lasers track their designated target gameobjects. (targetGo). Default is 3f.");
                    var turningRateProp = serializedObject.FindProperty("turningRate");

                    EditorGUILayout.Slider(turningRateProp, 0.05f, 15f, turningRateTooltip);

                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUILayout.Space();
            }

            var maxLaserRaycastDistanceTooltip = new GUIContent("Laser raycast distance", "The maximum distance to raycast when checking for targets that the laser can collide with. Ideally this should be large enough for the laser to go 'off screen' Default is 20f.");
            var maxLaserRaycastDistanceProp = serializedObject.FindProperty("maxLaserRaycastDistance");

            EditorGUILayout.Slider(maxLaserRaycastDistanceProp, 0f, 50f, maxLaserRaycastDistanceTooltip);

            #endregion

            if (GUI.changed)
            {
                EditorUtility.SetDirty(spriteBasedLaserRef);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}