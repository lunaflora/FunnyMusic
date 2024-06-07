//==================={By Qcbf|qcbf@qq.com|10/8/2022 11:40:53 AM}===================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace FLib
{
    public static class SlimJson
    {
        public const string BREAK_CHARS = "{}[],，:";
        public const string DATE_TIME_FORMAT_STR = "yyyy-MM-dd_HH-mm-ss";

        public static Dictionary<Type, ICustomSerializer> CustomSerializers = new();

        public interface ICustomSerializer
        {
            string JsonSerialize(object value);
            object JsonDeserialize(string json, ref int index, StringBuilder strbuf, Type t);
        }

        public enum EToken
        {
            ArrOpen,
            ObjOpen,
            Close,
            Value,
            Skip,
            Comment,
        }

        public struct SerializeData
        {
            /// <summary>
            /// Object如果实现ToString将不序列化每个字段直接用ToString的字符串
            /// 忽略字符串转义字符
            /// </summary>
            public bool IsLog;

            /// <summary>
            /// 是否禁用添加双引号(IsLog,IsPolymorphic会强制为true)
            /// </summary>
            public bool IsDisableQuotationChar;

            /// <summary>
            /// 在各种地方添加换行 TODO
            /// </summary>
            public bool IsPretty;

            /// <summary>
            /// 仅序列化标记Serializable的字段
            /// </summary>
            public bool IsOnlySerializableFields;

            /// <summary>
            /// 支持多态序列化
            /// </summary>
            public bool IsPolymorphic;

            /// <summary>
            /// 包含空字符串字段
            /// </summary>
            public bool IncludeEmptyStringField;

            public StringBuilder Buffer;
        }

        public struct DeserializeData
        {
            public Type ToType;
            public Func<object, object> ParseObjectKeyHook;
            public bool IsStrictMatchObjectField;
        }

        public static string SerializeToLog(object value, SerializeData data = default)
        {
            data.IsLog = true;
            return Serialize(value, data);
        }

        public static string Serialize(object value, SerializeData data = default)
        {
            data.Buffer ??= StringFLibUtility.GetStrBuf();
            if (data.IsLog || data.IsPolymorphic)
            {
                data.IsDisableQuotationChar = true;
            }

            SerializeValue(value, data);
            return StringFLibUtility.ReleaseStrBufAndResult(data.Buffer);
        }

        public static T Deserialize<T>(string json, DeserializeData data = default)
        {
            data.ToType = typeof(T);
            return (T)Deserialize(json, data);
        }

        public static object Deserialize(string json, in DeserializeData data = default)
        {
            if (data.ToType == typeof(string))
            {
                return json;
            }

            var strbuf = StringFLibUtility.GetStrBuf();
            try
            {
                var index = 0;
                EToken token;
                if (data.ToType.IsArray && json[0] != '[')
                {
                    token = EToken.ArrOpen;
                }
                else
                {
                    token = NextToken(json, ref index);
                }

                while (token == EToken.Comment)
                {
                    NextComment(json, strbuf, ref index);
                    //Options.CommentHandler?.Invoke(strbuf.ToString(), null, null);
                    token = NextToken(json, ref index);
                }

                return Parse(data.ToType, json, strbuf, ref index, token, data);
            }
            finally
            {
                StringFLibUtility.ReleaseStrBuf(strbuf);
            }
        }


        #region serialize
        private static void SerializeValue(object value, in SerializeData data, bool isAppendType = false)
        {
            switch (value)
            {
                case null:
                    data.Buffer.Append('"').Append('"');
                    break;
                case string v:
                    SerializeString(v, data);
                    break;
                case IDictionary v:
                    SerializeDict(v, data);
                    break;
                case IEnumerable v:
                    SerializeList(v, data);
                    break;
                case float v:
                    data.Buffer.Append(v.ToString("0.#####"));
                    break;
                case double v:
                    data.Buffer.Append(v.ToString("0.########"));
                    break;
                case DateTime v:
                    if (!data.IsDisableQuotationChar) data.Buffer.Append('"');
                    data.Buffer.Append(v.ToString(DATE_TIME_FORMAT_STR));
                    if (!data.IsDisableQuotationChar) data.Buffer.Append('"');
                    break;
                default:
                    if (value is int || value is uint || value is long || value is sbyte || value is byte || value is short ||
                        value is ushort || value is ulong)
                    {
                        data.Buffer.Append(value);
                    }
                    else if (value is bool)
                    {
                        data.Buffer.Append(value.ToString().ToLowerInvariant());
                    }
                    else if (value is Enum)
                    {
                        if (data.IsLog)
                        {
                            data.Buffer.Append(value);
                        }
                        else
                        {
                            data.Buffer.Append(Convert.ToInt32(value));
                        }
                    }
                    else
                    {
                        SerializeObject(value, isAppendType, data);
                    }

                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void SerializeString(string str, in SerializeData data)
        {
            if (data.IsLog)
            {
                data.Buffer.Append(str);
            }
            else
            {
                if (!data.IsDisableQuotationChar && !data.IsPolymorphic) data.Buffer.Append('"');
                foreach (var c in str)
                {
                    switch (c)
                    {
                        case '\r':
                            break;
                        case '"':
                            data.Buffer.Append("\\\"");
                            break;
                        case '\b':
                            data.Buffer.Append("\\b");
                            break;
                        case '\f':
                            data.Buffer.Append("\\f");
                            break;
                        case '\n':
                            data.Buffer.Append("\\n");
                            break;
                        case '\t':
                            data.Buffer.Append("\\t");
                            break;
                        default:
                            data.Buffer.Append(c);
                            break;
                    }
                }

                if (!data.IsDisableQuotationChar && !data.IsPolymorphic) data.Buffer.Append('"');
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void SerializeList(IEnumerable list, in SerializeData data)
        {
            Type elementType = null;
            if (data.IsPolymorphic)
            {
                elementType = list.GetType();
                if (elementType.IsArray)
                {
                    elementType = elementType.GetElementType();
                }
                else if (elementType.IsGenericType)
                {
                    elementType = elementType.GetGenericArguments()[0];
                }
            }

            data.Buffer.Append('[');

            //var indentCountCached = IndentCount;
            //if (data.IsPretty && IndentCount == 0)
            //{
            //    data.StringBuffer.Append('\n').Append('\t', ++IndentCount);
            //}

            var isFirst = true;
            foreach (var item in list)
            {
                if (!isFirst)
                {
                    data.Buffer.Append(',');
                }
                else isFirst = false;

                SerializeValue(item, data, data.IsPolymorphic && item != null && elementType != item.GetType());
            }

            //if (data.IsPretty && indentCountCached == 0)
            //{
            //    data.StringBuffer.Append('\n');
            //    IndentCount--;
            //}

            data.Buffer.Append(']');
        }

        /// <summary>
        /// 
        /// </summary>
        private static void SerializeDict(IDictionary dict, in SerializeData data)
        {
            data.Buffer.Append('{');
            var isFirst = true;
            var valueBaseType = data.IsPolymorphic ? dict.GetType().GetGenericArguments()[1] : null;

            //var indentCountCached = IndentCount;
            //if (data.IsPretty && IndentCount == 0)
            //{
            //    data.StringBuffer.Append('\n').Append('\t', ++IndentCount);
            //}

            foreach (DictionaryEntry item in dict)
            {
                SerializeKV(ref isFirst, item.Key, item.Value, data.IsPolymorphic && item.Value != null && valueBaseType != item.Value.GetType(), data);
            }

            //if (data.IsPretty && indentCountCached == 0)
            //{
            //    data.StringBuffer.Append('\n');
            //    IndentCount--;
            //}

            data.Buffer.Append('}');
        }

        /// <summary>
        /// 
        /// </summary>
        private static void SerializeObject(object obj, bool isAppendType, in SerializeData data)
        {
            var t = obj.GetType();
            var declaringType = !data.IsLog ? null : t.GetMethod(nameof(ToString), BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), null).DeclaringType;
            if (declaringType != null && declaringType != typeof(object) && declaringType != typeof(ValueType))
            {
                data.Buffer.Append(obj);
            }
            else
            {
                if (CustomSerializers.TryGetValue(t, out var serializer))
                {
                    data.Buffer.Append(serializer.JsonSerialize(obj));
                }
                else if (obj is ICustomSerializer pluginSelf)
                {
                    data.Buffer.Append(pluginSelf.JsonSerialize(obj));
                }
                else
                {
                    var isFirst = true;
                    if (isAppendType)
                    {
                        data.Buffer.Append("${").Append(TypeAssistant.GetTypeName(obj.GetType())).Append('}');
                    }

                    data.Buffer.Append('{');
                    //var indentCountCached = IndentCount;
                    //if (data.IsPretty && IndentCount == 0)
                    //{
                    //    data.StringBuffer.Append('\n').Append('\t', ++IndentCount);
                    //}
                    foreach (var field in t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase))
                    {
                        if (field.IsInitOnly || field.IsLiteral || (!data.IsPolymorphic && field.FieldType.IsAssignableFrom(t)) ||
                            field.IsDefined(typeof(NonSerializedAttribute)) ||
                            (data.IsOnlySerializableFields && !field.IsDefined(typeof(SerializableAttribute))) ||
                            field.FieldType.IsSubclassOf(typeof(Delegate)))
                            continue;

                        var fieldName = field.Name;
                        var v = field.GetValue(obj);
                        if (v != null && (data.IncludeEmptyStringField || v is not string str || str.Length > 0))
                        {
                            SerializeKV(ref isFirst, fieldName, v, data.IsPolymorphic && field.FieldType != v.GetType(), data);
                        }
                    }

                    //if (data.IsPretty && indentCountCached == 0)
                    //{
                    //    data.StringBuffer.Append('\n');
                    //    IndentCount--;
                    //}
                    data.Buffer.Append('}');
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void SerializeKV(ref bool isFirst, object k, object v, bool isAppendValueType, in SerializeData data)
        {
            if (!isFirst)
            {
                data.Buffer.Append(',');
            }
            else
            {
                isFirst = false;
            }

            SerializeValue(k, data);
            data.Buffer.Append(':');
            SerializeValue(v, data, isAppendValueType);
        }
        #endregion


        #region deserialize
        /// <summary>
        /// 
        /// </summary>
        public static Type NextOverridePolymorphicType(StringBuilder strbuf, string json, ref int index)
        {
            if (json.Length < index + 1 || json[index] != '$' || json[index + 1] != '{')
            {
                return null;
            }

            strbuf.Clear();
            for (index += 2; index < json.Length; index++)
            {
                var c = json[index];
                if (c == '}')
                {
                    ++index;
                    break;
                }

                strbuf.Append(c);
            }

            return TypeAssistant.GetType(strbuf.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        public static EToken NextToken(string json, ref int index)
        {
            var count = json.Length;
            while (index < count)
            {
                var c = json[index++];
                if (char.IsWhiteSpace(c))
                {
                    continue;
                }

                switch (c)
                {
                    case '{':
                        return EToken.ObjOpen;
                    case '[':
                        return EToken.ArrOpen;
                    case '}':
                    case ']':
                        return EToken.Close;
                    case ':':
                    case '，':
                    case ',':
                        return EToken.Skip;
                    case '\\':
                        return EToken.Value;
                    default:
                        if (index < count)
                        {
                            var nextChar = json[index];
                            if ((c == '/' && nextChar == '/') || (c == '/' && nextChar == '*'))
                            {
                                index += 1;
                                return EToken.Comment;
                            }
                        }

                        index--;
                        return EToken.Value;
                }
            }

            return EToken.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void NextWord(bool isEnum, string json, StringBuilder strbuf, ref int index, string breaks = BREAK_CHARS)
        {
            strbuf.Clear();
            var count = json.Length;
            var isBeginQuotes = false;
            var spaceCount = 0;
            while (index < count)
            {
                var c = json[index++];
                if (char.IsWhiteSpace(c))
                {
                    ++spaceCount;
                    continue;
                }

                if (!isBeginQuotes && (breaks?.Contains(c) == true))
                {
                    --index;
                    break;
                }

                while (spaceCount > 0)
                {
                    --spaceCount;
                    strbuf.Append(' ');
                }

                switch (c)
                {
                    case '\\':
                        strbuf.Append(json[index++]);
                        break;
                    case '\'':
                    case '"':
                        isBeginQuotes = !isBeginQuotes;
                        break;
                    default:
                        strbuf.Append(isEnum && c == '|' || c == '+' ? ',' : c);
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void NextComment(string json, StringBuilder strbuf, ref int index)
        {
            strbuf?.Clear();
            var count = json.Length;
            while (index < count)
            {
                var c = json[index++];
                if (index < count)
                {
                    var nextChar = json[index];
                    if (c == '\n' || (c == '\r' && nextChar != '\n')) // "\n"和"\r"结束符
                    {
                        break;
                    }
                    else if (c == '\r' || (c == '*' && nextChar == '/')) // "\r\n"和 "*/" 结束符
                    {
                        index++;
                        break;
                    }
                    else
                    {
                        strbuf?.Append(c);
                    }
                }
                else
                {
                    strbuf?.Append(c);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static object Parse(Type t, string json, StringBuilder strbuf, ref int index, EToken token, in DeserializeData data = default)
        {
            t = Nullable.GetUnderlyingType(t) ?? t;
            if (CustomSerializers.TryGetValue(t, out var plugin))
            {
                strbuf.Clear();
                return plugin.JsonDeserialize(json, ref index, strbuf, t);
            }
            else if (typeof(ICustomSerializer).IsAssignableFrom(t))
            {
                strbuf.Clear();
                plugin = (ICustomSerializer)TypeAssistant.New(t);
                return plugin.JsonDeserialize(json, ref index, strbuf, t) ?? plugin;
            }

            return token switch
            {
                EToken.Value => ParseValue(t, json, strbuf, ref index, data),
                EToken.ArrOpen => ParseArray(t, json, strbuf, ref index, data),
                EToken.ObjOpen => ParseObject(t, json, strbuf, ref index, default, data),
                _ => null,
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public static object ParseValue(Type t, string json, StringBuilder strbuf, ref int index, in DeserializeData data = default)
        {
            try
            {
                NextWord(t.IsEnum, json, strbuf, ref index);
                if (strbuf.Length == 0)
                {
                    return t.DefaultValue();
                }

                object parsed;
                if (t == typeof(object))
                {
                    parsed = strbuf.ToString();
                    //var str = strbuf.ToString();
                    //if (long.TryParse(str, out var integer)) parsed = integer;
                    //else if (double.TryParse(str, out var number)) parsed = number;
                    //else parsed = str;
                }
                else if (t == typeof(string))
                {
                    parsed = strbuf.ToString();
                }
                else if (t.IsEnum)
                {
                    var str = strbuf.ToString();
                    parsed = int.TryParse(str, out var integer) ? Enum.ToObject(t, integer) : Enum.ToObject(t, Enum.Parse(t, str.Replace('|', ',')));
                }
                else if (t == typeof(bool))
                {
                    if (strbuf.Length <= 1)
                    {
                        parsed = strbuf[0] != '0';
                    }
                    else
                    {
                        parsed = strbuf.ToString().ToLowerInvariant() != "false";
                    }
                }
                else if (t == typeof(DateTime))
                {
                    parsed = DateTime.ParseExact(strbuf.ToString(), DATE_TIME_FORMAT_STR, CultureInfo.InvariantCulture);
                }
                else
                {
                    parsed = Convert.ChangeType(strbuf.ToString(), Nullable.GetUnderlyingType(t) ?? t);
                }

                return parsed;
            }
            catch (Exception ex)
            {
                throw new Exception($"Parse Value Error: {t} {(index >= 0 && index < json.Length ? json[index].ToString() : json)} {strbuf}", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static object ParseArray(Type t, string json, StringBuilder strbuf, ref int index, in DeserializeData data = default)
        {
            // list, array, collection
            byte typeCode = 1;
            var et = t;
            IList list;
            if (t == typeof(object))
            {
                list = new List<object>();
            }
            else if (t.IsArray)
            {
                et = t.GetElementType();
                list = (IList)TypeAssistant.New(typeof(List<>).MakeGenericType(et));
            }
            else
            {
                et = t.GetGenericArguments()[0];
                if (typeof(IList).IsAssignableFrom(t))
                {
                    typeCode = 0;
                    list = (IList)TypeAssistant.New(t);
                }
                else
                {
                    typeCode = 2;
                    list = (IList)TypeAssistant.New(typeof(List<>).MakeGenericType(et));
                }
            }

            var isParsing = true;
            //string comment = null;
            while (isParsing)
            {
                var overrideElementType = NextOverridePolymorphicType(strbuf, json, ref index);
                var token = NextToken(json, ref index);
                switch (token)
                {
                    case EToken.Close:
                        isParsing = false;
                        break;
                    case EToken.Skip:
                        break;
                    case EToken.Comment:
                        NextComment(json, strbuf, ref index);
                        //if (Options.CommentHandler != null)
                        //{
                        //    comment = strbuf.ToString();
                        //}
                        break;
                    default:
                        var obj = Parse(overrideElementType ?? et, json, strbuf, ref index, token, data);
                        list.Add(obj);
                        //if (comment != null)
                        //{
                        //    Options.CommentHandler(comment, list.Count - 1, obj);
                        //    comment = null;
                        //}
                        if (index >= json.Length)
                        {
                            isParsing = false;
                        }

                        break;
                }
            }

            if (typeCode == 1)
            {
                var result = Array.CreateInstance(et, list.Count);
                list.CopyTo(result, 0);
                return result;
            }
            else if (typeCode == 2)
            {
                return TypeAssistant.New(t, list);
            }

            return list;
        }


        /// <summary>
        /// 
        /// </summary>
        public static object ParseObject(Type t, string json, StringBuilder strbuf, ref int index, Dictionary<string, object> buffer = null, in DeserializeData data = default)
        {
            object result = buffer;
            if (result == null)
            {
                if (t == typeof(object))
                {
                    result = new Dictionary<string, object>();
                }
                else if (!t.IsStatic())
                {
                    result = TypeAssistant.New(t);
                }
            }

            var dictResult = result as IDictionary;
            var kvTypes = dictResult != null ? result.GetType().GetGenericArguments() : new[] { typeof(string), null };

            var isParsing = true;
            //string comment = null;
            while (isParsing)
            {
                var token = NextToken(json, ref index);
                switch (token)
                {
                    case EToken.Close:
                        isParsing = false;
                        break;
                    case EToken.Skip: break;
                    case EToken.Comment:
                        NextComment(json, strbuf, ref index);
                        //if (Options.CommentHandler != null || Options.ObjectFieldValueHook != null)
                        //{
                        //    comment = strbuf.ToString();
                        //}
                        break;
                    default:
                        var k = Parse(kvTypes[0], json, strbuf, ref index, token, data);
                        if (data.ParseObjectKeyHook != null)
                            k = data.ParseObjectKeyHook(k);
                        ++index; // : 号
                        var overrideValueType = NextOverridePolymorphicType(strbuf, json, ref index);
                        token = NextToken(json, ref index);

                        object v;
                        if (dictResult == null)
                        {
                            var fieldName = (string)k;
                            Type fieldType = null;
                            if (fieldName[0] == '$' && fieldName[1] == '{')
                                fieldType = TypeAssistant.GetType(ParseAppendTypeStr(ref fieldName, strbuf));

                            var field = t.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.IgnoreCase);
                            if (field == null)
                            {
                                if (data.IsStrictMatchObjectField)
                                    throw new Exception("not found field: " + fieldName + "\n" + t);
                                Parse(typeof(object), json, strbuf, ref index, token, data);
                            }
                            else
                            {
                                fieldType ??= field.FieldType;
                                //Options.ObjectFieldValueHook?.Invoke(ref k, ref fieldType, ref index, ref json, ref token, comment);
                                //field.SetValue(result, v = Parse(fieldType, json, strbuf, ref index, token));
                                field.SetValue(result, Parse(fieldType, json, strbuf, ref index, token, data));
                            }
                        }
                        else
                        {
                            var fieldType = overrideValueType ?? kvTypes[1];
                            //Options.ObjectFieldValueHook?.Invoke(ref k, ref fieldType, ref index, ref json, ref token, comment);
                            dictResult[k] = v = Parse(fieldType, json, strbuf, ref index, token, data);
                        }

                        //if (comment != null)
                        //{
                        //    Options.CommentHandler?.Invoke(comment, k, v);
                        //}
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// 解析序列化时 填写了 IsAppendType = true 的数据
        /// </summary>
        private static string ParseAppendTypeStr(ref string raw, StringBuilder strbuf)
        {
            strbuf.Clear();
            var i = 2;
            for (; i < raw.Length; i++)
            {
                var c = raw[i];
                if (c == '}')
                {
                    break;
                }

                strbuf.Append(c);
            }

            raw = raw[(i + 1)..];
            return strbuf.ToString();
        }
        #endregion
    }
}
