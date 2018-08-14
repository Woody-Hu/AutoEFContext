using System.Collections.Generic;

namespace AutoEFContextRepository
{
    /// <summary>
    /// bootstrap用分页封装
    /// </summary>
    /// <typeparam name="T"></typeparam>
   public class PagePackerBoostrap<T>
    {
        /// <summary>
        /// 总数量
        /// </summary>
       public int Total { get; set; }

        /// <summary>
        /// 当前页数据
        /// </summary>
        public List<T> Rows { get; set; }
    }
}