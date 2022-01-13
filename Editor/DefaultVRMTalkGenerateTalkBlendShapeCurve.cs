using UnityEditor;
using UnityEngine;

namespace VRMTalk.Editor
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
         * <param name="scripts">生成する台本</param>
         */
        public void GenerateTalkBlendShapeCurve(AnimationCurve[] animationCurves, string scripts)
        {
            string vowel = VRMTalkUtility.ConvertFromHiraganaToVowels(VRMTalkUtility.StringUnification(scripts));
            float baseTime = 0.5f;
            float gapTime = 0.2f;
            float nowTime = 0.1f;
            for (int i = 0; i < vowel.Length; i++)
            {
                int vowelIndex = VRMTalkUtility.VowelIndexOf(vowel[i]);

                if (i>1 && VRMTalkUtility.VowelIndexOf(vowel[i-1])!=vowelIndex)
                {
                    animationCurves[vowelIndex].AddKey(nowTime , 0f);
                }

                nowTime += baseTime;
                animationCurves[vowelIndex].AddKey(nowTime,100f);
                nowTime += gapTime;

                if (i<vowel.Length-1 &&VRMTalkUtility.VowelIndexOf(vowel[++i])!=vowelIndex)
                {
                    animationCurves[vowelIndex].AddKey(nowTime , 0f);
                    i--;
                }
            }

            //すべてのKeyFrameのTangentModeをClampedAutoに変更
            foreach (var animationCurve in animationCurves)
            {
                VRMTalkUtility.ChangeAllTangentMode(animationCurve,AnimationUtility.TangentMode.ClampedAuto);
            }
        }
    }
}