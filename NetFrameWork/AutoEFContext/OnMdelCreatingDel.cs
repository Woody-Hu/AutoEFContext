using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoEFContext
{
    /// <summary>
    /// 模型建立委托
    /// </summary>
    /// <param name="modelBuilder"></param>
    public delegate void OnModelCreatingDel(DbModelBuilder modelBuilder);
}
