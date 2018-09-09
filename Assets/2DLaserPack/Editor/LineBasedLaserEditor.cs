using System.Diagnostics;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using Debug = UnityEngine.Debug;

namespace TwoDLaserPack
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
    /// Custom inspector/editor for Laser component script
    /// </summary>
    [CustomEditor(typeof(LineBasedLaser)), CanEditMultipleObjects]
    public class LineBasedLaserEditor : Editor
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

            var lineBasedLaserRef = (LineBasedLaser)target;

            var laserTooltip = new GUIContent("Laser GameObject", "Reference to a suitable GameObject with LineRenderer component to use for the main laser.");
            var laserProp = serializedObject.FindProperty("laserLineRenderer");

            if (laserProp.objectReferenceValue == null)
            {
                GUI.color = Color.white;
                EditorGUILayout.HelpBox("A suitable GameObject with LineRenderer component to use for the main laser is required.", MessageType.Error,
                    true);
                GUI.color = Color.white;
            }

            EditorGUILayout.PropertyField(laserProp, laserTooltip);

            var hitSparkParticleTooltip = new GUIContent("Hit particle effect", "Reference to the particle effect gameobject to display on the laser hit location");
            var hitSparkParticleSystemProp = serializedObject.FindProperty("hitSparkParticleSystem");

            if (hitSparkParticleSystemProp.objectReferenceValue == null)
            {
                GUI.color = Color.white;
                EditorGUILayout.HelpBox("A particle effect gameobject to display on the laser hit location should be specified here.", MessageType.Warning,
                    true);
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

            var ignoreCollisionsTooltip = new GUIContent("Ignore collisions", "Ignores any collisions the laser finds by use of it's raycast check. If this is enabled, lasers will not collide with any Collider2D objects.");
            var ignoreCollisionsProp = serializedObject.FindProperty("ignoreCollisions");

            ignoreCollisionsProp.boolValue = EditorGUILayout.Toggle(ignoreCollisionsTooltip, ignoreCollisionsProp.boolValue);

            var maskTooltip = new GUIContent("Layer Mask for Raycast detection", "Set a layer mask here to determine what the ray cast can collide with.");
            var maskProp = serializedObject.FindProperty("mask");

            EditorGUILayout.PropertyField(maskProp, maskTooltip);

            var sortLayerTooltip = new GUIContent("Sorting layer", "The sorting layer to use for the laser (and optional laser arc) to render on. Make sure this string exactly matches the name of the actual sorting layer you want to use.");
            var sortLayerProp = serializedObject.FindProperty("sortLayer");

            EditorGUILayout.PropertyField(sortLayerProp, sortLayerTooltip);

            var sortOrderTooltip = new GUIContent("Sort order in layer", "The sorting order to use on the sorting layer for the laser (and optional laser arc) to render on.");
            var sortOrderProp = serializedObject.FindProperty("sortOrder");

            EditorGUILayout.PropertyField(sortOrderProp, sortOrderTooltip);

            if (!ignoreCollisionsProp.boolValue)
            {
                EditorGUI.indentLevel++;

                var collisionTriggerIntervalTooltip = new GUIContent("Collision trigger interval", "The time between collision trigger events. If the laser is colliding with an object, this interval is used to fire the laser hit trigger event which other objects can subscribe to for custom functionality. Default value is 0.25f");
                var collisionTriggerIntervalProp = serializedObject.FindProperty("collisionTriggerInterval");
                EditorGUILayout.Slider(collisionTriggerIntervalProp, 0.05f, 5f, collisionTriggerIntervalTooltip);

                EditorGUI.indentLevel--;
            }

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

            var laserTexOffsetSpeedTooltip = new GUIContent("Texture X offset move speed", "Rate at which the laser's texture offset is moved to give an 'animation' effect to the laser stream. Default is 1f.");
            var laserTexOffsetSpeedProp = serializedObject.FindProperty("laserTexOffsetSpeed");

            EditorGUILayout.Slider(laserTexOffsetSpeedProp, 0f, 5f, laserTexOffsetSpeedTooltip);

            var maxLaserRaycastDistanceTooltip = new GUIContent("Laser raycast distance", "The maximum distance to raycast when checking for targets that the laser can collide with. Ideally this should be large enough for the laser to go 'off screen' Default is 20f.");
            var maxLaserRaycastDistanceProp = serializedObject.FindProperty("maxLaserRaycastDistance");

            EditorGUILayout.Slider(maxLaserRaycastDistanceProp, 0f, 50f, maxLaserRaycastDistanceTooltip);

            #endregion

            if (GUI.changed)
            {
                EditorUtility.SetDirty(lineBasedLaserRef);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}