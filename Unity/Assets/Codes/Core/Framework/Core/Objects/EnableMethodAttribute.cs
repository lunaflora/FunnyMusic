using System;

namespace Framework
{
    /// <summary>
    /// 有些实体，需要有方法，标记以下
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EnableMethodAttribute : Attribute
    {
        
    }
}