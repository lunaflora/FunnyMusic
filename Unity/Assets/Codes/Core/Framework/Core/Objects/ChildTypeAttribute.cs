using System;

namespace Framework
{
    /// <summary>
    /// Entity的Child类型约束,null为无约束
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ChildTypeAttribute : Attribute
    {
        public Type type;

        public ChildTypeAttribute(Type type = null)
        {
            this.type = type;
        }
    }
}