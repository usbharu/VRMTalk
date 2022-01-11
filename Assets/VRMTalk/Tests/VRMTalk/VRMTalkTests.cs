using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using VRM;
using VRMTalk.Editor;
using Assert = UnityEngine.Assertions.Assert; 

namespace VRMTalk
{
    public class VRMTalkTests
    {
       
        [Test]
        public void GetBlendShapeNames_UseTestModel_IsNotNull()
        {
            var vrm = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/VRMTalk/Tests/VRMTalk/Resources/TestModel.prefab");
            var vrmMeta = vrm.GetComponent<VRMMeta>();
            Assert.IsNotNull(VRMTalkUtility.GetBlendShapeNames(vrmMeta));
        }
        
        [Test]
        public void CharacterUnification_SimpleCharacter_IsHiraganaChar()
        {
            Assert.AreEqual(VRMTalkUtility.CharacterUnification('ア'),'あ');
            Assert.AreEqual(VRMTalkUtility.CharacterUnification('ﾜ'),'わ');
            Assert.AreEqual(VRMTalkUtility.CharacterUnification('ヱ'),'ゑ');
        }
    }
}
