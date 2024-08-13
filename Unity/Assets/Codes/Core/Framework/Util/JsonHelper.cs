using System;
using CatJson;

namespace Framework
{
    public static class JsonHelper
    {
        public static string ToJson<T>(T obj)
        {
            return  JsonParser.Default.ToJson<T>(obj);
        }

        public static T FromJson<T>(string str)
        {
            T t = JsonParser.Default.ParseJson<T>(str);
            ISupportInitialize iSupportInitialize = t as ISupportInitialize;
            if (iSupportInitialize == null)
            {
                return t;
            }

            iSupportInitialize.EndInit();
            return t;
        }

        /// <summary>
        /// 这个是直接返回jsondata,自己解析
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static JsonObject FromJson(string str)
        {
            JsonObject t = JsonParser.Default.ParseJson<JsonObject>(str);
          
            return t;
        }

        public static T Clone<T>(T t)
        {
            return FromJson<T>(ToJson(t));
        }
    }
}