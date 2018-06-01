using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoEFContext
{
    /// <summary>
    /// 使用的Db上下文接口
    /// </summary>
    public interface IDbContext
    {
        /// <summary>
        /// 获取DbSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IDbSet<T> GetDb<T>()
            where T : class;


        /// <summary>
        /// 设置DbSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputValue"></param>
        void SetDb<T>(IDbSet<T> inputValue)
            where T : class;
       
    }
}
