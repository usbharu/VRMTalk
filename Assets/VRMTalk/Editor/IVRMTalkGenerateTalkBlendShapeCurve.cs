using UnityEngine;

namespace VRMTalk.Editor
{
    
    public interface IVRMTalkGenerateTalkBlendShapeCurve
    { 
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
         * <param name="vowel">生成する母音</param>
         */
        void GenerateTalkBlendShapeCurve(AnimationCurve[] animationCurves, string vowel);
    }
}