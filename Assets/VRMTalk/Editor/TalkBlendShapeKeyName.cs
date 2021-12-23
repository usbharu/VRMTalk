using UnityEngine;

[CreateAssetMenu(fileName = "TalkBlendShapeKeyName", menuName = "VRMTalk/CreateTalkBlendShapeKeyName")]
public class TalkBlendShapeKeyName : ScriptableObject
{
    [SerializeField] public string key_a = "Fcl_MTH_A";
    [SerializeField] public string key_i = "Fcl_MTH_I";
    [SerializeField] public string key_u = "Fcl_MTH_U";
    [SerializeField] public string key_e = "Fcl_MTH_e";
    [SerializeField] public string key_o = "Fcl_MTH_O";
    [SerializeField] public string key_Neutral = "Fcl_ALL_Neutral";
}