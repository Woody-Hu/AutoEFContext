using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoEFContext
{
    /// <summary>
    /// 自动Entity特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false,Inherited = false)]
    public class AutoEntityAttribute:Attribute
    {
    }
}
