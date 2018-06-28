using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacEFImp
{
    /// <summary>
    /// 自动化上下文特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = false, Inherited = false)]
    public class AutoContextAttribute:Attribute
    {
        /// <summary>
        /// 使用的KeyFilter类型
        /// </summary>
        public Type UseKeyFilterType { set; get; }
    }
}
