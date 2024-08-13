using System;
using Framework;

namespace Game
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigAttribute: BaseAttribute
    {
        public string Path { get; }

        public ConfigAttribute(string path)
        {
            this.Path = path;
        }
    }
}