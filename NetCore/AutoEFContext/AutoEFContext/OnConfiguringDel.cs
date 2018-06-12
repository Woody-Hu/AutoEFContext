using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoEFContext
{
    /// <summary>
    /// 设置委托
    /// </summary>
    /// <param name="optionsBuilder"></param>
    public delegate void OnConfiguringDel(DbContextOptionsBuilder optionsBuilder);
}
