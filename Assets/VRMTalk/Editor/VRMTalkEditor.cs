using System;
using UnityEditor;
using UnityEngine;
using VRM;

namespace VRMTalk
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
        private float selectedBlendShapeWeigth;

        private string[] BlendShapeKeys;
        
        private AnimationCurvePair[] _animationCurvePair = Array.Empty<AnimationCurvePair>();
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

        void GenerationBlendShape()
        {
            AnimationCurve[] animationCurves = new AnimationCurve[6];
            animationCurves[0] = VRMTalkUtility.AnimationCurvePair(_animationCurvePair,_talkBlendShapeKeyName.key_a);
            animationCurves[1] = VRMTalkUtility.AnimationCurvePair(_animationCurvePair,_talkBlendShapeKeyName.key_i);
            animationCurves[2] = VRMTalkUtility.AnimationCurvePair(_animationCurvePair,_talkBlendShapeKeyName.key_u);
            animationCurves[3] = VRMTalkUtility.AnimationCurvePair(_animationCurvePair,_talkBlendShapeKeyName.key_e);
            animationCurves[4] = VRMTalkUtility.AnimationCurvePair(_animationCurvePair,_talkBlendShapeKeyName.key_o);
            animationCurves[5] = VRMTalkUtility.AnimationCurvePair(_animationCurvePair,_talkBlendShapeKeyName.key_Neutral);
            string vowel =
                VRMTalkUtility.ConvertFromHiraganaToVowels(VRMTalkUtility.StringUnification(_vrmTalkClip.talkScript));
            new DefaultVRMTalkGenerateTalkBlendShapeCurve().GenerateTalkBlendShapeCurve(animationCurves,vowel);
            Debug.Log("Generation BlendShape");
        }

    }
}