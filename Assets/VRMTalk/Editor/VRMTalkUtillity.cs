﻿using System;
using System.Text;
using UnityEngine;
using VRM;

namespace VRMTalk
{
    public class VRMTalkUtility
    {
        public static string[] GetBlendShapeNames(VRMMeta vrm)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = vrm.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            int blendShapeCount = skinnedMeshRenderer.sharedMesh.blendShapeCount;
            string[] blendShape = new string[blendShapeCount];
            for (var i = 0; i < blendShape.Length; i++)
            {
                blendShape[i] = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i);
            }

            return blendShape;
        }


        public static readonly char[] HiraganaChars =
        {
            'あ','い','う','え','お',
            'か','き','く','け','こ',
            'さ','し','す','せ','そ',
            'た','ち','つ','て','と',
            'な','に','ぬ','ね','の',
            'は','ひ','ふ','へ','ほ',
            'ま','み','む','め','も',
            'や','ぃ','ゆ','ぇ','よ',
            'ら','り','る','れ','ろ',
            'わ','ゐ','ぅ','ゑ','を',
            'ん'
        };

        public static readonly char[] FullWidthKanaChars =
        {
            'ア','イ','ウ','エ','オ',
            'カ','キ','ク','ケ','コ',
            'サ','シ','ス','セ','ソ',
            'タ','チ','ツ','テ','ト',
            'ナ','ニ','ヌ','ネ','ノ',
            'ハ','ヒ','フ','ヘ','ホ',
            'マ','ミ','ム','メ','モ',
            'ヤ','ィ','ユ','ェ','ヨ',
            'ラ','リ','ル','レ','ロ',
            'ワ','ヰ','ゥ','ヱ','ヲ',
            'ン',
        };

        public static readonly char[] HalfWidthKanaChars =
        {
            'ｱ', 'ｲ', 'ｳ', 'ｴ', 'ｵ',
            'ｶ', 'ｷ', 'ｸ', 'ｹ', 'ｺ',
            'ｻ', 'ｼ', 'ｽ', 'ｾ', 'ｿ',
            'ﾀ', 'ﾁ', 'ﾂ', 'ﾃ', 'ﾄ',
            'ﾅ', 'ﾆ', 'ﾇ', 'ﾈ', 'ﾉ',
            'ﾊ', 'ﾋ', 'ﾌ', 'ﾍ', 'ﾎ',
            'ﾏ', 'ﾐ', 'ﾑ', 'ﾒ', 'ﾓ',
            'ﾔ', 'ｨ', 'ﾕ', 'ｪ', 'ﾖ',
            'ﾗ', 'ﾘ', 'ﾙ', 'ﾚ', 'ﾛ',
            'ﾜ', 'ｲ', 'ｩ', 'ｴ', 'ｦ',
            'ﾝ',
        };

        public static readonly char[] VowelsChars =
        {
            'あ', 'い', 'う', 'え', 'お', 'ん',
        };

        public static char CharacterUnification(char character)
        {
            for (int i = 0; i < FullWidthKanaChars.Length; i++)
            {
                if (FullWidthKanaChars[i] == character||HalfWidthKanaChars[i]==character||HiraganaChars[i]==character)
                {
                    return HiraganaChars[i];
                }
            }

            return character;
        }

        public static string StringUnification(string s)
        {
            StringBuilder returnValue= new StringBuilder();
            foreach (var c in s)
            {
                returnValue.Append(CharacterUnification(c));
            }

            return returnValue.ToString();
        }

        public static char ConvertFromHiraganaToVowels(char character)
        {
            if (character == 'ん')
            {
                return 'ん';
            }
            for (int i = 0; i < HiraganaChars.Length; i++)
            {
                if (HiraganaChars[i]==character)
                {
                    return VowelsChars[i % 5];
                }
            }

            throw new ArgumentException();
        }

        public static string ConvertFromHiraganaToVowels(string s)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var t in s)
            {
                try
                {
                    stringBuilder.Append(ConvertFromHiraganaToVowels(t));
                }
                catch (ArgumentException e)
                {
                    Debug.LogWarning("Contains characters that cannot be used.");
                }
            }

            return stringBuilder.ToString();
        }

        public static bool IsVowel(char character)
        {
            character = CharacterUnification(character);
            foreach (char c in VowelsChars)
            {
                if (character == c)
                {
                    return true;
                }
            }

            return false;
        }

        public static int VowelIndexOf(char character)
        {
            for (int i = 0; i < VowelsChars.Length; i++)
            {
                if (VowelsChars[i]==character)
                {
                    return i;
                }
            }

            throw new ArgumentException(character+" is not vowel");
        }

        public static AnimationCurve AnimationCurvePair(AnimationCurvePair[] animationCurvePairs, string key)
        {
            foreach (var animationCurvePair in animationCurvePairs)
            {
                if (animationCurvePair.key.Equals(key))
                {
                    return animationCurvePair.animationCurve;
                }
            }

            return null;
        }
    }
}