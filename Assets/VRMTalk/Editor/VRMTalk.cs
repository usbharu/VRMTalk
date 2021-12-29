using System;
using UnityEditor;
using UnityEngine;
using UnityLogging;
using VRM;

namespace VRMTalk.Editor
{
    public class VRMTalk
    {

        public static IVRMTalkGenerateTalkBlendShapeCurve VrmTalkGenerateTalkBlendShapeCurve;
        
        public static void WriteBlendShapeToClip(VRMTalkClip vrmTalkClip, VRMMeta vrm)
        {
            if (vrmTalkClip==null||vrm==null)
            {
                return;
            }

            vrmTalkClip.animationCurveList.Clear();
            string[] blendShapeNames = VRMTalkUtility.GetBlendShapeNames(vrm);
            foreach (string blendShapeName in blendShapeNames)
            {
                AnimationCurvePair pair = new AnimationCurvePair();
                pair.key = blendShapeName;
                pair.animationCurve = AnimationCurve.Linear(vrmTalkClip.clipBegin,0f,vrmTalkClip.clipEnd,0f);
                vrmTalkClip.animationCurveList.Add(pair);
            }
        }
        
        public static void WriteAnimationCurveToAnimationClip(AnimationCurve animationCurve,string propertyName,AnimationClip animationClip){
            animationClip.SetCurve("Face",typeof(SkinnedMeshRenderer),propertyName,animationCurve);
        }

        public static void WriteVRMTalkClipToAnimationClip(VRMTalkClip vrmTalkClip, AnimationClip animationClip)
        {
            foreach (var t in vrmTalkClip.animationCurveList)
            {
                WriteAnimationCurveToAnimationClip(t.animationCurve,"blendShape."+t.key,animationClip);
            }
        }

        /**
         * <summary>母音から口の<c>BlendShape</c>の<c>AnimationCurve</c>を生成する</summary>
         * <param name="animationCurves">
         * [0] BlendShape a
         * [1] BlendShape i
         * [2] BlendShape u
         * [3] BlendShape e
         * [4] BlendShape o
         * [5] BlendShape Neutral
         * </param>
         * <param name="scripts">生成する台本</param>
         */
        public static void GenerationTalkBlendShapeAnimationCurve(AnimationCurve[] animationCurves,string scripts)
        {
            if (VrmTalkGenerateTalkBlendShapeCurve==null)
            {
                VrmTalkGenerateTalkBlendShapeCurve = new DefaultVRMTalkGenerateTalkBlendShapeCurve();
            }
            VrmTalkGenerateTalkBlendShapeCurve.GenerateTalkBlendShapeCurve(animationCurves,scripts);
        }

        public static void GenerateTalkBlendShapeCurve(VRMTalkClip vrmTalkClip,TalkBlendShapeKeyName talkBlendShapeKeyName,string talkScripts)
        {

            AnimationCurvePair[] animationCurvePairs = vrmTalkClip.animationCurveList.ToArray();
            
            AnimationCurve[] animationCurves = new AnimationCurve[6];
            animationCurves[0] = VRMTalkUtility.KeyOfAnimationCurvePair(animationCurvePairs,talkBlendShapeKeyName.key_a);
            animationCurves[1] = VRMTalkUtility.KeyOfAnimationCurvePair(animationCurvePairs,talkBlendShapeKeyName.key_i);
            animationCurves[2] = VRMTalkUtility.KeyOfAnimationCurvePair(animationCurvePairs,talkBlendShapeKeyName.key_u);
            animationCurves[3] = VRMTalkUtility.KeyOfAnimationCurvePair(animationCurvePairs,talkBlendShapeKeyName.key_e);
            animationCurves[4] = VRMTalkUtility.KeyOfAnimationCurvePair(animationCurvePairs,talkBlendShapeKeyName.key_o);
            animationCurves[5] = VRMTalkUtility.KeyOfAnimationCurvePair(animationCurvePairs,talkBlendShapeKeyName.key_Neutral);
            
            GenerationTalkBlendShapeAnimationCurve(animationCurves,talkScripts);

            AnimationCurve animationCurve = VRMTalkUtility.GetLongestAnimationCurvePair(animationCurvePairs).animationCurve;
            float maxTime = animationCurve[animationCurve.length-1].time;
            vrmTalkClip.clipEnd = vrmTalkClip.clipBegin + maxTime;
            Logging.Log("max time : "+maxTime,"VRMTalk");
            
            
            //AnimationCurveの長さを統一
            foreach (var animationCurvePair in vrmTalkClip.animationCurveList)
            {
                float lastKeyValue =
                    animationCurvePair.animationCurve.keys[animationCurvePair.animationCurve.keys.Length-1].value;
                animationCurvePair.animationCurve.AddKey(maxTime, lastKeyValue);
            }
            
        }
    }
}