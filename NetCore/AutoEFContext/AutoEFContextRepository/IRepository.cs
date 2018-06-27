using AutoEFContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AutoEFContextRepository
{
    /// <summary>
    /// 储存层接口
    /// </summary>
    /// <typeparam name="X"></typeparam>
    public interface IRepository<T,X>
        where X:class
        where T:AutoContext
    {
        /// <summary>
        /// 使用的DB
        /// </summary>
        DbSet<X> UseDB { get; }

        /// <summary>
        /// 使用的Context
        /// </summary>
        T UseContext { get; }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="useWhere"></param>
        /// <returns></returns>
        List<X> GetAll(Expression<Func<X, bool>> useWhere = null,IncludeDel<X> useInclude = null);

        /// <summary>
        /// 获取第一个
        /// </summary>
        /// <param name="useWhere"></param>
        /// <returns></returns>
        X FindFirst(Expression<Func<X, bool>> useWhere = null, IncludeDel<X> useInclude = null);

        /// <summary>
        /// 添加一个
        /// </summary>
        /// <param name="input"></param>
        void Add(X input);

        /// <summary>
        /// 添加一组
        /// </summary>
        /// <param name="input"></param>
        void AddRange(IEnumerable<X> input);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="input"></param>
        void Update(X input);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="input"></param>
        void Delete(X input);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="usePage"></param>
        /// <param name="pageSize"></param>
        /// <param name="useWhere"></param>
        /// <returns></returns>
        PagePacker<X> GetPage(int usePage, int pageSize, Expression<Func<X, bool>> useWhere = null, IncludeDel<X> useInclude = null);
    }
}
