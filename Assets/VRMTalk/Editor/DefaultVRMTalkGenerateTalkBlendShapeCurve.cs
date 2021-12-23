using UnityEditor;
using UnityEngine;

namespace VRMTalk
{
    public class DefaultVRMTalkGenerateTalkBlendShapeCurve : IVRMTalkGenerateTalkBlendShapeCurve
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
        public void GenerateTalkBlendShapeCurve(AnimationCurve[] animationCurves, string vowel)
        {
            float baseTime = 0.5f;
            float gapTime = 0.2f;
            float nowTime = 0.1f;
            for (int i = 0; i < vowel.Length; i++)
            {
                int vowelIndex = VRMTalkUtility.VowelIndexOf(vowel[i]);

                animationCurves[vowelIndex].AddKey(nowTime , 0f);
                nowTime += baseTime;
                animationCurves[vowelIndex].AddKey(nowTime,100f);
                nowTime += gapTime;
                animationCurves[vowelIndex].AddKey(nowTime , 0f);
            }

            //すべてのKeyFrameのTangentModeをClampedAutoに変更
            foreach (var animationCurve in animationCurves)
            {
                VRMTalkUtility.ChangeAllTangentMode(animationCurve,AnimationUtility.TangentMode.ClampedAuto);
            }
        }
    }
}