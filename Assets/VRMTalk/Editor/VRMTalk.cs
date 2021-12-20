using UnityEditor;
using UnityEngine;
using VRM;

namespace VRMTalk
{
    public class VRMTalk
    {
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
                pair.animationCurve = AnimationCurve.Linear(vrmTalkClip.clipBegin,1f,vrmTalkClip.clipEnd,1f);
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
    }
}