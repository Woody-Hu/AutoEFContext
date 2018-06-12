using System;
using System.Collections.Generic;
using System.Text;

namespace AutoEFContext
{
    /// <summary>
    /// EF执行委托封装
    /// </summary>
    internal class EFDelPacker
    {
        internal OnConfiguringDel UseOnConfig { set; get; }

        internal OnModelCreatingDel UseOnModelCreating { set; get; }
    }
}
