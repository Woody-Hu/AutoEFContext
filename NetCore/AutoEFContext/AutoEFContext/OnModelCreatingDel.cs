using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoEFContext
{
    /// <summary>
    /// 模型建立委托
    /// </summary>
    /// <param name="modelBuilder"></param>
    public delegate void OnModelCreatingDel(ModelBuilder modelBuilder);
}
