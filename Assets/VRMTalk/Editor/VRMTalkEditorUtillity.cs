using System;
using UnityEditor;
using UnityEngine;

namespace VRMTalk.Editor
{
    public class VRMTalkEditorUtillity
    {
        [Obsolete]
        public static void Separator(EditorWindow editorWindow)
        {
            Color color = GUI.color;
            GUI.color = Color.black;
            GUILayout.Box("", GUILayout.Width(editorWindow.position.width), GUILayout.Height(2));
            GUI.color = color;
        }

        public static void Separator()
        {
            Color color = GUI.color;
            GUI.color = Color.black;
            GUILayout.Box("",GUILayout.ExpandWidth(true),GUILayout.Height(2));
            GUI.color = color;
        }
    }
}