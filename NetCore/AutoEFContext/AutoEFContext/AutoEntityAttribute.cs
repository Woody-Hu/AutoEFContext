using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AutoEFContext
{
    /// <summary>
    /// 自动Entity特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AutoEntityAttribute : Attribute
    {
        /// <summary>
        /// 使用的过滤Key
        /// </summary>
        public string Key { set; get; }
    }
}