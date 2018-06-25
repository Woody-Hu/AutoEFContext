using System;
using System.Collections.Generic;
using System.Text;

namespace AutoEFContextRepository
{
    /// <summary>
    /// 分页封装
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagePacker<T>
    {
        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalCount { get; internal set; }

        /// <summary>
        /// 使用的对象列表
        /// </summary>
        public List<T> Values { get; internal set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage { get; internal set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage { get; internal set; }

        /// <summary>
        /// 页容量
        /// </summary>
        public int PageSize { get; internal set; }

    }
}
