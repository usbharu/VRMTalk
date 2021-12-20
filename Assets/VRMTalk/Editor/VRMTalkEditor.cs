using System;
using System.Reflection;
// using Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using VRM;
using Button = UnityEngine.UI.Button;
using Object = UnityEngine.Object;

namespace VRMTalk
{
    
    public class VRMTalkEditor : EditorWindow
    {
        private VRMTalkClip _vrmTalkClip;
        private VRMMeta vrm;
        private VRMMetaObject VrmMeta;
        private Texture2D thumbnail;
        private Vector2 scrollPos;
        
        private AnimationCurvePair[] _animationCurvePair = Array.Empty<AnimationCurvePair>();
        //test
        private AnimationClip _animationClip; 
        //test
        [MenuItem("VRMTalk/TalkEditor")]
        private static void ShowWindow()
        {
            var window = GetWindow<VRMTalkEditor>();
            window.titleContent = new GUIContent("VRMTalk");
            window.Show();
        }

        private void Awake()
        {
            vrm = FindObjectOfType<VRMMeta>();
            VrmMeta = vrm?.Meta;
        }

        private void OnGUI()
        {
            
            initVRM();
            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope())
                {
                    vrm = EditorGUILayout.ObjectField("VRM",vrm,typeof(VRMMeta),true)as VRMMeta;
                    VrmMeta = EditorGUILayout.ObjectField("VRMMeta", VrmMeta, typeof(VRMMeta), true)as VRMMetaObject;
                    if (GUILayout.Button("Write BlendShape"))
                    {
                        VRMTalk.WriteBlendShapeToClip(_vrmTalkClip,vrm);
                        SaveClip();
                    }
                }

                GUILayout.Box(thumbnail,GUILayout.Width(150f),GUILayout.Height(150f));
            }

            VRMTalkEditorUtillity.Separator();
            
            initClip();
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("New Clip"))
                {
                    
                }

                if (GUILayout.Button("Save Clip"))
                {
                    SaveClip();
                }

                if (GUILayout.Button("Save As Clip"))
                {
                    
                }

                if (GUILayout.Button("WriteClip"))
                {
                    WriteClip();
                }
            }

            _vrmTalkClip = EditorGUILayout.ObjectField("Clip", _vrmTalkClip, typeof(VRMTalkClip), true)as VRMTalkClip;

            //test
            _animationClip = EditorGUILayout.ObjectField("Animation Clip",_animationClip,typeof(AnimationClip),false)as AnimationClip;
            //test
            VRMTalkEditorUtillity.Separator();
            if (_vrmTalkClip !=null)
            {
                _vrmTalkClip.talkScript = EditorGUILayout.TextField("talk script", _vrmTalkClip.talkScript);
            }

            using (new EditorGUILayout.VerticalScope())
            {
                using (EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(scrollPos))
                {
                    scrollPos = scrollView.scrollPosition;
                    foreach (var t in _animationCurvePair)
                    {
                        t.animationCurve = EditorGUILayout.CurveField(t.key,
                            t.animationCurve);
                    }

                    GUILayoutUtility.GetRect(_vrmTalkClip?.clipEnd - _vrmTalkClip?.clipBegin ?? 0, 1);
                }
            }
        }

        void initVRM()
        {
            if (vrm==null)
            {
                return;
            }

            VrmMeta = vrm.GetComponent<VRMMeta>().Meta;
            thumbnail = VrmMeta.Thumbnail;
        }

        void initClip()
        {
            if (_vrmTalkClip==null)
            {
                return;
            }

            _animationCurvePair = _vrmTalkClip.animationCurveList.ToArray();
        }

        void SaveClip()
        {
            if (_vrmTalkClip==null)
            {
                Debug.LogWarning("Clip is not set");
                return;
            }

            EditorUtility.SetDirty(_vrmTalkClip);
            AssetDatabase.SaveAssets();
            Debug.Log("save asset : "+ _vrmTalkClip);
        }

        void WriteClip()
        {
            if (_vrmTalkClip==null)
            {
                Debug.LogWarning("Clip is not set");
                return;
            }
            if (_animationClip==null)
            {
                Debug.LogWarning("Animation Clip is not set");
                return;
            }
            VRMTalk.WriteVRMTalkClipToAnimationClip(_vrmTalkClip,_animationClip);
            Debug.Log("write clip : "+ _animationClip);
        }

    }
}