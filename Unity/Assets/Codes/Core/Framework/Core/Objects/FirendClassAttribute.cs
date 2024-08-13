using System;

namespace Framework
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true,Inherited = false)]
    public class FriendClassAttribute : Attribute
    {
        public Type type;

        public FriendClassAttribute(Type type)
        {
            this.type = type;
        }
    }
}