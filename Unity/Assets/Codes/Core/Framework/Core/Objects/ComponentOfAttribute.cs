using System;

namespace Framework
{
    /// <summary>
    /// Component父类型约束,不标记则没有约束
    /// 用法: [ComponentOf(typeof(parentComponent))]
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentOfAttribute : Attribute
    {
        public Type type;

        public ComponentOfAttribute(Type type)
        {
            this.type = type;
        }
    }
}