using UnityEditor;
using UnityEngine;

namespace TwoDLaserPack
{
    public static class EditorHelper
    {
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
        /// Custom GUILayout progress bar for future updates use...
        /// </summary>
        /// <param name="value"></param>
        /// <param name="label"></param>
        public static void ProgressBar(float value, string label)
        {
            // Get a rect for the progress bar using the same margins as a textfield:
            Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
            EditorGUI.ProgressBar(rect, value, label);
            EditorGUILayout.Space();
        }
    }
}