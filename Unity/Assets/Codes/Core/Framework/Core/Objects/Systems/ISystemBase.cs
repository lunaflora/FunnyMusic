using System;

namespace Framework
{
    public interface ISystemBase
    {
        Type Type();

        Type SystemType();
        
        public Byte SystemPriority
        {
            get;
            set;
        }
    }
}