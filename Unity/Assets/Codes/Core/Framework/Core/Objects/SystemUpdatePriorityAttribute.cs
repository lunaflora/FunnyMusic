using System;

namespace Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SystemUpdatePriorityAttribute : Attribute
    {
        public byte UpdatePriority;

        public SystemUpdatePriorityAttribute()
        {
            UpdatePriority = 0;
        }

    }
}