//==================={By Qcbf|qcbf@qq.com|7/13/2023 5:00:32 PM}===================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FLib
{
    public static class SlimToml
    {
        public const string BREAK_CHARS = "{}[],，\r\n\"";
        public const string DATE_TIME_FORMAT_STR = "yyyy-MM-dd_HH-mm-ss";

        public static Dictionary<Type, ICustomSerializer> CustomSerializers = new();

        public interface ICustomSerializer
        {
            string TomlSerialize(object value);
            object TomlDeserialize(string rawText, ref int index, StringBuilder strbuf, Type t);
        }

        public enum EToken
        {
            None,
            ArrayOpen,
            ObjectOpen,
            Close,
            Value,
            Comment,
        }




        #region deserializer
        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<string, object> Deserialize(ReadOnlySpan<char> text)
        {
            var result = new Dictionary<string, object>();
            Deserialize(text, result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public static object Deserialize(ReadOnlySpan<char> text, object instance = null)
        {
            var token = NextToken(ref text);
            return DeserializeValue(token, ref text, instance);
        }

        /// <summary>
        /// 
        /// </summary>
        private static EToken NextToken(ref ReadOnlySpan<char> text)
        {
            var token = EToken.None;
            text = text.TrimStart();

            var c = text[0];
            if (c == '-' || char.IsNumber(c))
            {
                token = EToken.Value;
            }
            else
            {
                switch (c)
                {
                    case '#': token = EToken.Comment; break;
                    case '[': token = EToken.ArrayOpen; break;
                    case '{': token = EToken.ObjectOpen; break;
                    case '}':
                    case ']': token = EToken.Close; break;
                    case '"': token = EToken.Value; break;
                }
                text = text[1..];
            }
            return token;
        }

        /// <summary>
        /// 
        /// </summary>
        private static ReadOnlySpan<char> NextWord(ref ReadOnlySpan<char> text)
        {
            var wordLength = 0;
            for (; wordLength < text.Length; wordLength++)
            {
                if (BREAK_CHARS.Contains(text[wordLength]))
                {
                    break;
                }
            }
            var word = text[0..wordLength];
            text = text[wordLength..];
            return word;
        }

        /// <summary>
        /// 
        /// </summary>
        private static object DeserializeValue(EToken token, ref ReadOnlySpan<char> text, object instance)
        {
            switch (token)
            {
                case EToken.Comment:
                    NextWord(ref text);
                    break;
                case EToken.ArrayOpen:
                    return DeserializeArray(ref text, instance);
                case EToken.ObjectOpen:
                    return DeserializeArray(ref text, instance);
                case EToken.Value:
                    return DeserializeArray(ref text, instance);
            }
            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        private static object DeserializeArray(ref ReadOnlySpan<char> text, object instance)
        {
            var token = NextToken(ref text);
            var list = new List<object>();

            //while (!text.IsEmpty)
            //{
            //    switch (token)
            //    {


            //    }
            //}


            return instance;
        }


        #endregion





    }
}
