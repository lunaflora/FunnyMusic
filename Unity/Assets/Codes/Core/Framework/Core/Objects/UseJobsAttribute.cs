using System;

namespace Framework
{
    /// <summary>
    ///  用于实体Entity和Component,允许其可以使用jobs调用其逻辑
    /// 一般用于渲染层逻辑
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,Inherited = false)]
    public class UseJobsAttribute : Attribute
    {
        
    }
}