using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityLogging;
using VRM;

namespace VRMTalk.Editor
{
    public class VRMTalkEditor : EditorWindow
    {
        private VRMTalkClip _vrmTalkClip;
        private VRMMeta vrm;
        private VRMMetaObject VrmMeta;
        private Texture2D thumbnail;
        private Vector2 scrollPos;
        private SkinnedMeshRenderer _skinnedMeshRenderer;

        private int selectedBlendShapeIndex;
        private float selectedBlendShapeWeight;

        private List<int> selectedBlendShapeIndexList = new List<int>();

        private string[] BlendShapeKeys;

        private AnimationCurvePair[] _animationCurvePairs = Array.Empty<AnimationCurvePair>();

        //test
        private AnimationClip _animationClip;
        //test

        private TalkBlendShapeKeyName _talkBlendShapeKeyName;

        enum Tab
        {
            VRM,
            Talk,
            Setting,
        }

        private Tab _tab = Tab.VRM;
        
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
            VrmMeta = vrm != null ? vrm.Meta : null;
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                _tab = (Tab) GUILayout.Toolbar((int) _tab, Styles.TabToggles, Styles.TabButtonStyle,
                    Styles.TabButtonSize);
                GUILayout.FlexibleSpace();
            }

            initVRM();
            initClip();
            switch (_tab)
            {
                case Tab.VRM:
                    showVRM();
                    break;
                case Tab.Talk:
                    showClip();
                    break;
                case Tab.Setting:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            
        }

        void showVRM()
        {
            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope())
                {
                    vrm = EditorGUILayout.ObjectField("VRM", vrm, typeof(VRMMeta), true) as VRMMeta;
                    VrmMeta = EditorGUILayout.ObjectField("VRMMeta", VrmMeta, typeof(VRMMeta), true) as VRMMetaObject;
                    if (GUILayout.Button("Write BlendShape"))
                    {
                        VRMTalk.WriteBlendShapeToClip(_vrmTalkClip, vrm);
                        SaveClip();
                    }

                    using (new GUILayout.HorizontalScope())
                    {
                        selectedBlendShapeIndex =
                            EditorGUILayout.Popup("Preview BlendShape", selectedBlendShapeIndex, BlendShapeKeys);
                        if (GUILayout.Button("Add") && !selectedBlendShapeIndexList.Contains(selectedBlendShapeIndex))
                        {
                            selectedBlendShapeIndexList.Add(selectedBlendShapeIndex);
                        }

                        if (GUILayout.Button("Clear"))
                        {
                            selectedBlendShapeIndexList.Clear();
                        }

                        if (GUILayout.Button("Reset"))
                        {
                            for (int i = 0; i < _skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
                            {
                                _skinnedMeshRenderer.SetBlendShapeWeight(i,0f);
                            }
                        }
                    }

                    using (new EditorGUI.IndentLevelScope())
                    {
                        for (var i = 0; i < selectedBlendShapeIndexList.Count; i++)
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                _skinnedMeshRenderer.SetBlendShapeWeight(selectedBlendShapeIndexList[i],
                                    EditorGUILayout.Slider(BlendShapeKeys[selectedBlendShapeIndexList[i]],
                                        _skinnedMeshRenderer.GetBlendShapeWeight(selectedBlendShapeIndexList[i]), 0f,
                                        100f));
                                if (GUILayout.Button("Remove"))
                                {
                                    selectedBlendShapeIndexList.Remove(selectedBlendShapeIndexList[i]);
                                }
                            }
                        }
                    }
                }

                GUILayout.Box(thumbnail, GUILayout.Width(150f), GUILayout.Height(150f));
            }

        }

        void showClip()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("New Clip (Disabled)"))
                {
                }

                if (GUILayout.Button("Save Clip"))
                {
                    SaveClip();
                }

                if (GUILayout.Button("Save As Clip (Disabled)"))
                {
                }

                if (GUILayout.Button("WriteClip"))
                {
                    WriteClip();
                }
            }

            _vrmTalkClip = EditorGUILayout.ObjectField("Clip", _vrmTalkClip, typeof(VRMTalkClip), true) as VRMTalkClip;

            //test
            _animationClip =
                EditorGUILayout.ObjectField("Animation Clip", _animationClip, typeof(AnimationClip), false) as
                    AnimationClip;
            //test

            _talkBlendShapeKeyName =
                EditorGUILayout.ObjectField("TalkBlendShapeKeyName", _talkBlendShapeKeyName,
                    typeof(TalkBlendShapeKeyName), false) as TalkBlendShapeKeyName;

            VRMTalkEditorUtillity.Separator();
            if (_vrmTalkClip != null)
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    _vrmTalkClip.talkScript = EditorGUILayout.TextField("talk script", _vrmTalkClip.talkScript);
                    EditorGUILayout.LabelField(
                        VRMTalkUtility.ConvertFromHiraganaToVowels(
                            VRMTalkUtility.StringUnification(_vrmTalkClip.talkScript)));
                    if (GUILayout.Button("Generation"))
                    {
                        GenerationBlendShape();
                    }
                }
            }

            using (new EditorGUILayout.VerticalScope())
            {
                using (EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(scrollPos))
                {
                    scrollPos = scrollView.scrollPosition;
                    foreach (var t in _animationCurvePairs)
                    {
                        t.animationCurve = EditorGUILayout.CurveField(t.key,
                            t.animationCurve);
                    }

                    if (_vrmTalkClip != null)
                    {
                        Logging.Log(_vrmTalkClip.clipEnd - _vrmTalkClip.clipBegin, "VRMTalk");
                        GUILayoutUtility.GetRect((_vrmTalkClip.clipEnd - _vrmTalkClip.clipBegin) * 90, 1);
                    }
                }
            }
        }
        
        void initVRM()
        {
            if (vrm == null)
            {
                return;
            }

            VrmMeta = vrm.GetComponent<VRMMeta>().Meta;
            thumbnail = VrmMeta.Thumbnail;

            _skinnedMeshRenderer = vrm.GetComponentInChildren<SkinnedMeshRenderer>();
            BlendShapeKeys = new string[_skinnedMeshRenderer.sharedMesh.blendShapeCount];
            for (var i = 0; i < BlendShapeKeys.Length; i++)
            {
                BlendShapeKeys[i] = _skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i);
            }
        }

        void initClip()
        {
            if (_vrmTalkClip == null)
            {
                return;
            }

            _animationCurvePairs = _vrmTalkClip.animationCurveList.ToArray();
        }

        void SaveClip()
        {
            if (_vrmTalkClip == null)
            {
                Debug.LogWarning("Clip is not set");
                return;
            }

            EditorUtility.SetDirty(_vrmTalkClip);
            AssetDatabase.SaveAssets();
            Logging.Log("save asset : " + _vrmTalkClip, "VRMTalk");
        }

        void WriteClip()
        {
            if (_vrmTalkClip == null)
            {
                Debug.LogWarning("Clip is not set");
                return;
            }

            if (_animationClip == null)
            {
                Debug.LogWarning("Animation Clip is not set");
                return;
            }

            VRMTalk.WriteVRMTalkClipToAnimationClip(_vrmTalkClip, _animationClip);
            Logging.Log("write clip : " + _animationClip, "VRMTalk");
        }

        void GenerationBlendShape()
        {
            VRMTalk.GenerateTalkBlendShapeCurve(_vrmTalkClip, _talkBlendShapeKeyName, _vrmTalkClip.talkScript);
            Logging.Log("Generation BlendShape", "VRMTalk");
        }

        private static class Styles
        {
            private static GUIContent[] _tabToggles = null;

            public static GUIContent[] TabToggles
            {
                get
                {
                    if (_tabToggles==null)
                    {
                        _tabToggles = System.Enum.GetNames(typeof(Tab)).Select(x => new GUIContent(x)).ToArray();
                    }

                    return _tabToggles;
                }
            }
            public static GUIStyle TabButtonStyle = "LargeButton";
                
            public static readonly GUI.ToolbarButtonSize TabButtonSize = GUI. ToolbarButtonSize.Fixed;
        }
    }
}