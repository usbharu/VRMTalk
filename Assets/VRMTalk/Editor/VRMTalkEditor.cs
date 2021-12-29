using System;
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

        private string[] BlendShapeKeys;
        
        private AnimationCurvePair[] _animationCurvePairs = Array.Empty<AnimationCurvePair>();
        //test
        private AnimationClip _animationClip; 
        //test
        
        private TalkBlendShapeKeyName _talkBlendShapeKeyName;
        
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

                    selectedBlendShapeIndex =
                        EditorGUILayout.Popup("Preview BlendShape", selectedBlendShapeIndex, BlendShapeKeys);
                    _skinnedMeshRenderer.SetBlendShapeWeight(selectedBlendShapeIndex,EditorGUILayout.Slider("BlendShapeWeight",_skinnedMeshRenderer.GetBlendShapeWeight(selectedBlendShapeIndex),0f,100f));
                }

                GUILayout.Box(thumbnail,GUILayout.Width(150f),GUILayout.Height(150f));
            }

            VRMTalkEditorUtillity.Separator();
            
            initClip();
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

            _vrmTalkClip = EditorGUILayout.ObjectField("Clip", _vrmTalkClip, typeof(VRMTalkClip), true)as VRMTalkClip;

            //test
            _animationClip = EditorGUILayout.ObjectField("Animation Clip",_animationClip,typeof(AnimationClip),false)as AnimationClip;
            //test
            
            _talkBlendShapeKeyName = EditorGUILayout.ObjectField("TalkBlendShapeKeyName",_talkBlendShapeKeyName,typeof(TalkBlendShapeKeyName),false)as TalkBlendShapeKeyName;
            
            VRMTalkEditorUtillity.Separator();
            if (_vrmTalkClip !=null)
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    _vrmTalkClip.talkScript = EditorGUILayout.TextField("talk script", _vrmTalkClip.talkScript);
                    EditorGUILayout.LabelField(VRMTalkUtility.ConvertFromHiraganaToVowels(VRMTalkUtility.StringUnification(_vrmTalkClip.talkScript)));
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
                        Logging.Log(_vrmTalkClip.clipEnd-_vrmTalkClip.clipBegin,"VRMTalk");
                        GUILayoutUtility.GetRect((_vrmTalkClip.clipEnd - _vrmTalkClip.clipBegin)*90, 1);
                    }
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

            _skinnedMeshRenderer = vrm.GetComponentInChildren<SkinnedMeshRenderer>();
            BlendShapeKeys = new string[_skinnedMeshRenderer.sharedMesh.blendShapeCount];
            for (var i = 0; i < BlendShapeKeys.Length; i++)
            {
                BlendShapeKeys[i] = _skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i);
            }
        }

        void initClip()
        {
            if (_vrmTalkClip==null)
            {
                return;
            }

            _animationCurvePairs = _vrmTalkClip.animationCurveList.ToArray();
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
            Logging.Log("save asset : "+ _vrmTalkClip,"VRMTalk");
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
            Logging.Log("write clip : "+ _animationClip,"VRMTalk");
        }

        void GenerationBlendShape()
        {
            
            
            VRMTalk.GenerateTalkBlendShapeCurve(_vrmTalkClip,_talkBlendShapeKeyName,_vrmTalkClip.talkScript);
            Logging.Log("Generation BlendShape","VRMTalk");
        }

    }
}