using System;

namespace Framework
{
    /// <summary>
    /// 用于实体Entity和Component,允许其声明method
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AllowMethodAttribute : Attribute
    {
        
    }
}