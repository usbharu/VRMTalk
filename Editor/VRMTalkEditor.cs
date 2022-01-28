﻿using System;
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
        private Texture thumbnail;
        private Vector2 scrollPos;
        private Vector2 scrollPos2;
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

        private bool isInitVrm;
        private bool isInitVRMClip;
        private bool isInitPreview;
        
        private bool isChangeVRM;
        
        private Rect rect;
        private bool resize;
        private Vector2 vrmScrollPosition;

        private GameObjectPreview gameObjectPreview;
        private GameObject previewGameObject;
        enum Tab
        {
            VRM,
            Talk,
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
                using (new EditorGUILayout.VerticalScope())
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
                    initPreview();
                    switch (_tab)
                    {
                        case Tab.VRM:
                            showVRM();
                            break;
                        case Tab.Talk:
                            showClip();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                
                showPreview();
            }
        }

        void showVRM()
        {
            if (_skinnedMeshRenderer==null)
            {
                isInitVrm=false;
                return;
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope())
                {
                    using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                    {
                        isChangeVRM = changeCheckScope.changed;
                        vrm = EditorGUILayout.ObjectField("VRM", vrm, typeof(VRMMeta), true) as VRMMeta;
                    }

                    VrmMeta = EditorGUILayout.ObjectField("VRMMeta", VrmMeta, typeof(VRMMetaObject), true) as VRMMetaObject;
                    if (GUILayout.Button("Write BlendShape"))
                    {
                        VRMTalk.WriteBlendShapeToClip(_vrmTalkClip, vrm);
                        SaveClip();
                    }
                    selectedBlendShapeIndex =
                        EditorGUILayout.Popup("Preview BlendShape", selectedBlendShapeIndex, BlendShapeKeys);
                    using (new GUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Add") && !selectedBlendShapeIndexList.Contains(selectedBlendShapeIndex))
                        {
                            selectedBlendShapeIndexList.Add(selectedBlendShapeIndex);
                        }

                        if (GUILayout.Button("Add all"))
                        {
                            for (var index = 0; index < BlendShapeKeys.Length; index++)
                            {
                                var i = BlendShapeKeys[index];
                                if (!selectedBlendShapeIndexList.Contains(index))
                                {
                                    selectedBlendShapeIndexList.Add(index);
                                }
                            }
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
                        using (var view = new GUILayout.ScrollViewScope(scrollPos2))
                        {
                            scrollPos2 = view.scrollPosition;
                            for (var i = 0; i < selectedBlendShapeIndexList.Count; i++)
                            {
                                using (new GUILayout.HorizontalScope())
                                {
                                    _skinnedMeshRenderer.SetBlendShapeWeight(selectedBlendShapeIndexList[i],
                                        EditorGUILayout.Slider(BlendShapeKeys[selectedBlendShapeIndexList[i]],
                                            _skinnedMeshRenderer.GetBlendShapeWeight(selectedBlendShapeIndexList[i]), 0f,
                                            100f));
                                    if (GUILayout.Button("Remove",GUILayout.ExpandWidth(false)))
                                    {
                                        selectedBlendShapeIndexList.Remove(selectedBlendShapeIndexList[i]);
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

        void showClip()
        {
            using (new EditorGUILayout.VerticalScope())
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
                VRMTalkEditorUtility.Separator();
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
        }

        void showPreview()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                if (GUILayout.Button("Reset"))
                {
                    gameObjectPreview.camera.transform.position = new Vector3(0, 1f, -2f);
                }

                if (gameObjectPreview==null)
                {
                    isInitPreview = false;
                    GUILayout.Box(thumbnail,GUILayout.Height(position.height),GUILayout.Width(position.width*0.3f));
                    return;
                }

                if (isChangeVRM|| (previewGameObject == null && vrm != null))
                {
                    DestroyImmediate(previewGameObject);
                    previewGameObject = Instantiate(vrm.gameObject);
                }

                Vector3 drag = Vector2.zero;
                if (previewGameObject!=null)
                {
                    if (Event.current.type == EventType.MouseDrag)
                    {
                        drag = new Vector3(-Event.current.delta.x, Event.current.delta.y);
                        gameObjectPreview.camera.transform.position += drag/500;
                        if (drag!=Vector3.zero)
                        {
                            Repaint();
                        }
                    }
                
                
                    gameObjectPreview.drawRect.width = position.width * 0.3f;
                    gameObjectPreview.drawRect.height = position.height;
                    thumbnail = gameObjectPreview.CreatePreviewTexture(previewGameObject);
                }

                GUILayout.Box(thumbnail,GUILayout.Height(position.height),GUILayout.Width(position.width*0.3f));
            }
        }
        
        void initVRM()
        {
            if (vrm == null)
            {
                return;
            }

            if (isInitVrm)
            {
                return;
            }

            isInitVrm = true;

            Logging.Log("Init VRM","VRMTalk");
            
            VrmMeta = vrm.GetComponent<VRMMeta>().Meta;

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

            if (isInitVRMClip)
            {
                return;
            }

            isInitVRMClip = true;
            _animationCurvePairs = _vrmTalkClip.animationCurveList.ToArray();
        }

        void initPreview()
        {
            if (isInitPreview)
            {
                return;
            }
            isInitPreview = true;
            gameObjectPreview = new GameObjectPreview();
            
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

        private void OnDisable()
        {
            gameObjectPreview.Cleanup();
        }
    }
}