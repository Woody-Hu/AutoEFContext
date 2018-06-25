using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoEFContext
{
    /// <summary>
    /// 使用的Key过滤器
    /// </summary>
    public interface IKEYFilter
    {
        /// <summary>
        /// 是否使用相应的Key
        /// </summary>
        /// <param name="inputKey">输入的Key</param>
        /// <returns>是/否</returns>
        bool IfUse(string inputKey);
    }
}
