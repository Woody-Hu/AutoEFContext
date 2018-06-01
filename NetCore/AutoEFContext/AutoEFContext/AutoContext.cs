using Microsoft.EntityFrameworkCore;
using System;

namespace AutoEFContext
{
    public abstract class AutoContext:DbContext
    {


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
