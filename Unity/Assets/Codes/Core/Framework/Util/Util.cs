using System;
using System.Collections.Generic;

namespace Framework
{
    public static class Util
    {
        #region parse config to fixnumber

          public static int FastParseInteger(string text, int offset, int count)
        {
            int flag = 1;
            int v = 0;
            for (var i = 0; i < count; ++i)
            {
                var t = text[offset + i];
                if (t == '-' && v == 0)
                {
                    flag = -1;
                    continue;
                }

                if (t == '.')
                {
                    break;
                }

                if (t < '0' || t > '9')
                {
                    continue;
                }
                v = v * 10 + (t - '0');
            }
            v *= flag;
            return v;
        }

        public static int FastParseInteger2(string text, int offset, int count)
        {
            int flag = 1;
            int v = 0;
            for (var i = 0; i < count; ++i)
            {
                var t = text[offset + i];
                if (t == '-' && v == 0)
                {
                    flag = -1;
                    continue;
                }

                if (t < '0' || t > '9')
                {
                    break;
                }
                v = v * 10 + (t - '0');
            }
            v *= flag;
            return v;
        }

        public static bool FastParseBool(string text, int offset, int count)
        {
            bool v = false;
            if (count == 4 && string.Compare("true", 0, text, offset, count, StringComparison.OrdinalIgnoreCase) == 0)
            {
                v = true;
            }
            return v;
        }

        public static sfloat FastParseFixedNumber(string text, int offset, int count)
        {
            return (sfloat)((float)FastParseInteger(text, offset, count) / 10000.0f);
        }

        public static int[] FaseParseIntegerArray(string text, int offset, int count)
        {
            var arrayCount = 0;
            for (var i = 0; i < count; ++i)
            {
                if (text[offset + i] == '_')
                {
                    arrayCount++;
                }
            }
            var result = new int[arrayCount + 1];
            var startPos = 0;
            arrayCount = 0;
            for (var i = 0; i < count; ++i)
            {
                if (text[offset + i] == '_')
                {
                    result[arrayCount++] = Util.FastParseInteger(text, offset + startPos, i - startPos);
                    startPos = i + 1;
                }
            }
            result[arrayCount++] = Util.FastParseInteger(text, offset + startPos, count - startPos);
            return result;
        }

        public static int[][] FaseParseIntegerArrayArray(string text, int offset, int count)
        {
            var arrayCount = 0;
            for (var i = 0; i < count; ++i)
            {
                if (text[offset + i] == '|')
                {
                    arrayCount++;
                }
            }
            var result = new int[arrayCount + 1][];
            var startPos = 0;
            arrayCount = 0;
            for (var i = 0; i < count; ++i)
            {
                if (text[offset + i] == '|')
                {
                    result[arrayCount++] = Util.FaseParseIntegerArray(text, offset + startPos, i - startPos);
                    startPos = i + 1;
                }
            }
            result[arrayCount++] = Util.FaseParseIntegerArray(text, offset + startPos, count - startPos);
            return result;
        }


        public static bool[] FaseParseBoolArray(string text, int offset, int count)
        {
            var arrayCount = 0;
            for (var i = 0; i < count; ++i)
            {
                if (text[offset + i] == '_')
                {
                    arrayCount++;
                }
            }
            var result = new bool[arrayCount + 1];
            var startPos = 0;
            arrayCount = 0;
            for (var i = 0; i < count; ++i)
            {
                if (text[offset + i] == '_')
                {
                    result[arrayCount++] = Util.FastParseBool(text, offset + startPos, i - startPos);
                    startPos = i + 1;
                }
            }
            result[arrayCount++] = Util.FastParseBool(text, offset + startPos, count - startPos);
            return result;
        }

        public static bool[][] FaseParseBoolArrayArray(string text, int offset, int count)
        {
            var arrayCount = 0;
            for (var i = 0; i < count; ++i)
            {
                if (text[offset + i] == '|')
                {
                    arrayCount++;
                }
            }
            var result = new bool[arrayCount + 1][];
            var startPos = 0;
            arrayCount = 0;
            for (var i = 0; i < count; ++i)
            {
                if (text[offset + i] == '|')
                {
                    result[arrayCount++] = Util.FaseParseBoolArray(text, offset + startPos, i - startPos);
                    startPos = i + 1;
                }
            }
            result[arrayCount++] = Util.FaseParseBoolArray(text, offset + startPos, count - startPos);
            return result;
        }

        public static sfloat[] FaseParseFixedIntegerArray(string text, int offset, int count)
        {
            var arrayCount = 0;
            for (var i = 0; i < count; ++i)
            {
                if (text[offset + i] == '_')
                {
                    arrayCount++;
                }
            }
            var result = new sfloat[arrayCount + 1];
            var startPos = 0;
            arrayCount = 0;
            for (var i = 0; i < count; ++i)
            {
                if (text[offset + i] == '_')
                {
                    result[arrayCount++] = FastParseFixedNumber(text, offset + startPos, i - startPos);
                    startPos = i + 1;
                }
            }
            result[arrayCount++] = FastParseFixedNumber(text, offset + startPos, count - startPos);
            return result;
        }

        public static sfloat[][] FaseParseFixedIntegerArrayArray(string text, int offset, int count)
        {
            var arrayCount = 0;
            for (var i = 0; i < count; ++i)
            {
                if (text[offset + i] == '|')
                {
                    arrayCount++;
                }
            }
            var result = new sfloat[arrayCount + 1][];
            var startPos = 0;
            arrayCount = 0;
            for (var i = 0; i < count; ++i)
            {
                if (text[offset + i] == '|')
                {
                    result[arrayCount++] = Util.FaseParseFixedIntegerArray(text, offset + startPos, i - startPos);
                    startPos = i + 1;
                }
            }
            result[arrayCount++] = Util.FaseParseFixedIntegerArray(text, offset + startPos, count - startPos);
            return result;
        }

        public static float[] FaseParseFloatFixedIntegerArray(string text, int offset, int count)
        {
            var arrayCount = 0;
            for (var i = 0; i < count; ++i)
            {
                if (text[offset + i] == '_')
                {
                    arrayCount++;
                }
            }
            var result = new float[arrayCount + 1];
            var startPos = 0;
            arrayCount = 0;
            for (var i = 0; i < count; ++i)
            {
                if (text[offset + i] == '_')
                {
                    result[arrayCount++] = Util.FastParseInteger(text, offset + startPos, i - startPos) * 0.0001f;
                    startPos = i + 1;
                }
            }
            result[arrayCount++] = Util.FastParseInteger(text, offset + startPos, count - startPos) * 0.0001f;
            return result;
        }

        public static float[][] FaseParseFloatIntegerArrayArray(string text, int offset, int count)
        {
            var arrayCount = 0;
            for (var i = 0; i < count; ++i)
            {
                if (text[offset + i] == '|')
                {
                    arrayCount++;
                }
            }
            var result = new float[arrayCount + 1][];
            var startPos = 0;
            arrayCount = 0;
            for (var i = 0; i < count; ++i)
            {
                if (text[offset + i] == '|')
                {
                    result[arrayCount++] = Util.FaseParseFloatFixedIntegerArray(text, offset + startPos, i - startPos);
                    startPos = i + 1;
                }
            }
            result[arrayCount++] = Util.FaseParseFloatFixedIntegerArray(text, offset + startPos, count - startPos);
            return result;
        }

        public static string[] FaseParseStringArray(string text, int offset, int count)
        {
            var arrayCount = 0;
            for (var i = 0; i < count; ++i)
            {
                if (text[offset + i] == '_')
                {
                    arrayCount++;
                }
            }
            var result = new string[arrayCount + 1];
            var startPos = 0;
            arrayCount = 0;
            for (var i = 0; i < count; ++i)
            {
                if (text[offset + i] == '_')
                {
                    result[arrayCount++] = text.Substring(offset + startPos, i - startPos);
                    startPos = i + 1;
                }
            }
            result[arrayCount++] = text.Substring(offset + startPos, count - startPos);
            return result;
        }

        public static string[][] FaseParseStringArrayArray(string text, int offset, int count)
        {
            var arrayCount = 0;
            for (var i = 0; i < count; ++i)
            {
                if (text[offset + i] == '|')
                {
                    arrayCount++;
                }
            }
            var result = new string[arrayCount + 1][];
            var startPos = 0;
            arrayCount = 0;
            for (var i = 0; i < count; ++i)
            {
                if (text[offset + i] == '|')
                {
                    result[arrayCount++] = Util.FaseParseStringArray(text, offset + startPos, i - startPos);
                    startPos = i + 1;
                }
            }
            result[arrayCount++] = Util.FaseParseStringArray(text, offset + startPos, count - startPos);
            return result;
        }

        public static int ParseInteger(string str)
        {
            int result = 0;
            int.TryParse(str, out result);
            return result;
        }

        public static List<int> ParseStringToIntList(string text, char[] separators)
        {
            text = text.Trim();
            List<int> result = new List<int>();
            if (!string.IsNullOrEmpty(text) && null != separators)
            {
                var tokens = text.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);
                result = new List<int>(tokens.Length);
                for (int index = 0; index < tokens.Length; ++index)
                {
                    int value = ParseInteger(tokens[index]);
                    if (0 != value)
                    {
                        result.Add(value);
                    }
                }
            }
            return result;
        }


        public static sfloat[] ParseStrToFixedArray(string text, char[] seprators, int denominator)
        {
            sfloat[] result = null;
            text = text.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                seprators = seprators ?? new char[] { ',' };
                var tokens = text.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length < 1)
                {
                    tokens = new string[] { text };
                }
                for (int tokenIndex = 0; tokenIndex < tokens.Length; ++tokenIndex)
                {
                    var token = tokens[tokenIndex];
                    var values = token.Split(seprators, System.StringSplitOptions.RemoveEmptyEntries);
                    if (null == result)
                    {
                        result = new sfloat[tokens.Length * values.Length];
                    }
                    for (int valueIndex = 0; valueIndex < values.Length; ++valueIndex)
                    {
                        int index = tokenIndex * values.Length + valueIndex;
                        int value = ParseInteger(values[valueIndex]);
                        result[index] = (sfloat)((float)value / denominator);
                    }
                }
            }
            return result;
        }

        public static sfloat[,] ParseStrToFixedArray2(string text, char[] seprators, int denominator)
        {
            sfloat[,] result = null;
            text = text.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                var tokens = text.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length < 1)
                {
                    tokens = new string[] { text, text, text };
                }
                for (int tokenIndex = 0; tokenIndex < tokens.Length; tokenIndex++)
                {
                    string[] values = tokens[tokenIndex].Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                    for (int valueIndex = 0; valueIndex < values.Length; valueIndex++)
                    {
                        if (null == result)
                        {
                            result = new sfloat[tokens.Length, values.Length];
                        }
                        int value = ParseInteger(values[valueIndex]);
                        result[tokenIndex, valueIndex] =(sfloat)((float)value / denominator);;
                    }
                }
            }

            return result;
        }

        private readonly static string[] _defaultResult = new string[0];
        private static string[] _splitResult = _defaultResult;
        public static void SplitString(string text, char separator, out string[] result, out int length)
        {
            if (string.IsNullOrEmpty(text))
            {
                result = _defaultResult;
                length = 0;
            }
            else
            {
                length = 1;
                for (var i = 0; i < text.Length; ++i)
                {
                    if (text[i] == separator)
                    {
                        ++length;
                    }
                }
                if (_splitResult.Length < length)
                {
                    _splitResult = new string[length];
                }

                result = _splitResult;
                var startPos = 0;
                var arrayIndex = 0;
                for (var i = 0; i < text.Length; ++i)
                {
                    if (text[i] == separator)
                    {
                        result[arrayIndex++] = text.Substring(startPos, i - startPos);
                        startPos = i + 1;
                    }
                }
                result[arrayIndex++] = text.Substring(startPos, text.Length - startPos);
            }
        }

        #endregion
        
    }
    
    static class TypeSwitch {
        public class CaseInfo {
            public bool IsDefault { get; set; }
            public Type Target { get; set; }
            public Action<object> Action { get; set; }
        }

        public static void Do(object source, params CaseInfo[] cases) {
            var type = source.GetType();
            foreach (var entry in cases) {
                if (entry.IsDefault || type == entry.Target) {
                    entry.Action(source);
                    break;
                }
            }
        }

        public static CaseInfo Case<T>(Action action) {
            return new CaseInfo() {
                Action = x => action(),
                Target = typeof(T)
            };
        }

        public static CaseInfo Case<T>(Action<T> action) {
            return new CaseInfo() {
                Action = (x) => action((T)x),
                Target = typeof(T)
            };
        }

        public static CaseInfo Default(Action action) {
            return new CaseInfo() {
                Action = x => action(),
                IsDefault = true
            };
        }
    }
}