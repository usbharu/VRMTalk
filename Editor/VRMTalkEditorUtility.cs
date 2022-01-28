using System;
using UnityEditor;
using UnityEngine;

namespace VRMTalk.Editor
{
    public static class VRMTalkEditorUtility
    {
        public enum SeparatorDirection
        {
            Horizontal,
            Vertical
        }

        [Obsolete]
        public static void Separator(EditorWindow editorWindow)
        {
            Color color = GUI.color;
            GUI.color = Color.black;
            GUILayout.Box("", GUILayout.Width(editorWindow.position.width), GUILayout.Height(2));
            GUI.color = color;
        }

        public static void Separator(SeparatorDirection separatorDirection = SeparatorDirection.Horizontal)
        {
            Color color = GUI.color;
            GUI.color = Color.black;
            switch (separatorDirection)
            {
                case SeparatorDirection.Horizontal:
                    GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));
                    break;
                case SeparatorDirection.Vertical:
                    GUILayout.Box("", GUILayout.Width(2), GUILayout.ExpandHeight(true));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(separatorDirection), separatorDirection, null);
            }

            GUI.color = color;
        }
    }
}