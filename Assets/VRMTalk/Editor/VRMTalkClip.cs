using System.Collections.Generic;
using UnityEngine;

namespace VRMTalk.Editor
{
    [CreateAssetMenu(fileName = "VRMTalkClip",menuName = "VRMTalk/CreateVRMTalkClip")]
    public class VRMTalkClip : ScriptableObject
    {
        [SerializeField] public float clipBegin;
        [SerializeField] public float clipEnd = 10f;
        [SerializeField] public string talkScript = "";
        [SerializeField] public List<AnimationCurvePair> animationCurveList = new List<AnimationCurvePair>();
    }

    [System.Serializable]
    public class AnimationCurvePair
    {
        public string key;
        public AnimationCurve animationCurve;
    }
}