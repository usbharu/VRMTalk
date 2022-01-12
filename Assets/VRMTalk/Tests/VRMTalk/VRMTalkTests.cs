using System;
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
    public class VRMTalkUtilityTests
    {
        [Test]
        public void GetBlendShapeNames_UseTestModel_IsNotNull()
        {
            var vrm = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/VRMTalk/Tests/VRMTalk/Resources/TestModel.prefab");
            var vrmMeta = vrm.GetComponent<VRMMeta>();
            Assert.IsNotNull(VRMTalkUtility.GetBlendShapeNames(vrmMeta));
        }

        [Test]
        public void CharacterUnification_SimpleCharacter_IsHiraganaChar()
        {
            Assert.AreEqual(VRMTalkUtility.CharacterUnification('ア'), 'あ');
            Assert.AreEqual(VRMTalkUtility.CharacterUnification('ﾜ'), 'わ');
            Assert.AreEqual(VRMTalkUtility.CharacterUnification('ヱ'), 'ゑ');
            Assert.AreEqual(VRMTalkUtility.CharacterUnification('1'), '1');
        }

        [Test]
        public void StringUnification_SimpleString_IsHiraganaString()
        {
            Assert.AreEqual(VRMTalkUtility.StringUnification("テスト"), "てすと");
            Assert.AreEqual(VRMTalkUtility.StringUnification("オﾜり"), "おわり");
            Assert.AreEqual(VRMTalkUtility.StringUnification("漢字テスト"), "漢字てすと");
        }

        [Test]
        public void ConvertFromHiraganaToVowels_SimpleCharacter_IsVowel()
        {
            Assert.AreEqual(VRMTalkUtility.ConvertFromHiraganaToVowels('か'), 'あ');
            Assert.AreEqual(VRMTalkUtility.ConvertFromHiraganaToVowels('ん'), 'ん');
        }

        [Test]
        public void ConvertFromHiraganaToVowels_NotHiraganaCharacter_ThrowArgumentException()
        {
            NUnit.Framework.Assert.Catch(typeof(ArgumentException),
                () => VRMTalkUtility.ConvertFromHiraganaToVowels('!'));
        }

        [Test]
        public void ConvertFromHiraganaToVowels_SimpleString_IsVowel()
        {
            Assert.AreEqual(VRMTalkUtility.ConvertFromHiraganaToVowels("てすと"), "えうお");
            Assert.AreEqual(VRMTalkUtility.ConvertFromHiraganaToVowels("かきくけこさしすせそ"), "あいうえおあいうえお");
        }

        [Test]
        public void ConvertFromHiraganaToVowels_NotHiraganaString_LogWarning()
        {
            LogAssert.Expect(LogType.Warning, "Contains characters that cannot be used.");
            VRMTalkUtility.ConvertFromHiraganaToVowels("漢字");
        }

        private static char[] VowelCharacters = {'あ', 'い', 'う', 'え', 'お', 'ん'};

        [Test]
        public void IsVowel_VowelCharacters_IsTrue([ValueSource(nameof(VowelCharacters))] char value)
        {
            Assert.IsTrue(VRMTalkUtility.IsVowel(value));
        }


        private static char[] NotVowelCharacters = {'か', '!', 'a'};

        [Test]
        public void IsVowel_NotVowelCharacters_IsFalse([ValueSource(nameof(NotVowelCharacters))] char value)
        {
            Assert.IsFalse(VRMTalkUtility.IsVowel(value));
        }

        [Test]
        public void VowelIndexOf_VowelCharacter_Between0And5([ValueSource(nameof(VowelCharacters))] char value)
        {
            int index = VRMTalkUtility.VowelIndexOf(value);
            Assert.IsTrue(0 <= index && 5 >= index);
        }

        //VowelIndex is {あ,い,う,え,お,ん}'s index 
        [Test]
        public void VowelIndexOf_VowelCharacter_IsVowelIndex()
        {
            Assert.AreEqual(VRMTalkUtility.VowelIndexOf('あ'),0);
            Assert.AreEqual(VRMTalkUtility.VowelIndexOf('い'),1);
            Assert.AreEqual(VRMTalkUtility.VowelIndexOf('う'),2);
            Assert.AreEqual(VRMTalkUtility.VowelIndexOf('え'),3);
            Assert.AreEqual(VRMTalkUtility.VowelIndexOf('お'),4);
            Assert.AreEqual(VRMTalkUtility.VowelIndexOf('ん'),5);
        }

        [Test]
        public void KeyOfAnimationCurvePair_SimpleKey_IsPairAnimationCurve()
        {
            var key = "Test";
            AnimationCurvePair[] pair = {new AnimationCurvePair()}; 
            pair[0].key = key;
            pair[0].animationCurve = AnimationCurve.Linear(0f,1f,1f,1f);

            Assert.AreEqual(VRMTalkUtility.KeyOfAnimationCurvePair(pair, key),pair[0].animationCurve);
        }

        [Test]
        public void KeyOfAnimationCurvePair_NoneExistentKey_IsNull()
        {
            var key = "Test";
            AnimationCurvePair[] pair = {new AnimationCurvePair()}; 
            pair[0].key = key;
            pair[0].animationCurve = AnimationCurve.Linear(0f,1f,1f,1f);
  
            Assert.IsNull(VRMTalkUtility.KeyOfAnimationCurvePair(pair,"NoneExistentKey"));
        }

        [Test]
        public void ChangeAllTangentMode_ClampedAuto_IsClampedAnyTime()
        {
            var curve = new AnimationCurve();
            curve = new AnimationCurve();
            curve.AddKey(0f, 0f);
            curve.AddKey(0.1f, 100f);
            curve.AddKey(0.5f, 0f);
            curve.AddKey(1f, 100f);
            
            VRMTalkUtility.ChangeAllTangentMode(curve,AnimationUtility.TangentMode.ClampedAuto);

            float length = curve[curve.length - 1].time;
            float detail = 0.001f;

            float fixedLength = length / detail;
            
            for (float i = 0; i < fixedLength; i++)
            {
                float value = curve.Evaluate(i*detail);
                Assert.IsTrue(0f<=value&&100f>=value);
            }
        }

        [Test]
        public void GetLongestAnimationCurvePair_RandomLengthAnimationCurvePairs_IsLongestAnimationCurvePair()
        {
            AnimationCurvePair[] animationCurvePairs =
            {
                new AnimationCurvePair(),new AnimationCurvePair(),new AnimationCurvePair(),
                new AnimationCurvePair(),new AnimationCurvePair(),new AnimationCurvePair()
            };
            for (int i = 0; i < animationCurvePairs.Length; i++)
            {
                animationCurvePairs[i].key = i.ToString();
                animationCurvePairs[i].animationCurve = AnimationCurve.Linear(0f,1f,i,1f);
            }
            
            Assert.AreEqual(VRMTalkUtility.GetLongestAnimationCurvePair(animationCurvePairs),animationCurvePairs[animationCurvePairs.Length-1]);
        }
    }
}