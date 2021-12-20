using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "VRMTalkClip",menuName = "VRMTalk/CreateVRMTalkClip")]
public class VRMTalkClip : ScriptableObject
{
    [SerializeField] public float clipBegin = 0f;
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

