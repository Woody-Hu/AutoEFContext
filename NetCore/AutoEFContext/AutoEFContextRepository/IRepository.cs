using AutoEFContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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


        #region 查询接口
        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="useWhere">使用的过滤条件</param>
        /// <param name="useInclude">使用的Include委托</param>
        /// <returns></returns>
        List<X> GetAll(Expression<Func<X, bool>> useWhere = null, IncludeDel<X> useInclude = null);

        /// <summary>
        /// 附带转换机制的获取全部
        /// </summary>
        /// <typeparam name="Y">转换后的类型</typeparam>
        /// <param name="useTransformer">使用的转换机制（如group操作）</param>
        /// <param name="useWhere">使用的过滤条件</param>
        /// <param name="useInclude">使用的Include委托</param>
        /// <returns></returns>
        List<Y> GetAll<Y>
            (Func<IQueryable<X>, IQueryable<Y>> useTransformer
            , Expression<Func<Y, bool>> useWhere = null, IncludeDel<Y> useInclude = null) where Y : class;

        /// <summary>
        /// 获取第一个
        /// </summary>
        /// <param name="useWhere">使用的过滤条件</param>
        /// <param name="useInclude">使用的Include委托</param>
        /// <returns></returns>
        X FindFirst(Expression<Func<X, bool>> useWhere = null, IncludeDel<X> useInclude = null);


        /// <summary>
        /// 附带转换机制的获取第一个
        /// </summary>
        /// <typeparam name="Y">转换后的类型</typeparam>
        /// <param name="useTransformer">使用的转换机制（如group操作）</param>
        /// <param name="useWhere">使用的过滤条件</param>
        /// <param name="useInclude">使用的Include委托</param>
        /// <returns></returns>
        Y FindFirst<Y>
            (Func<IQueryable<X>, IQueryable<Y>> useTransformer
            , Expression<Func<Y, bool>> useWhere = null, IncludeDel<Y> useInclude = null) where Y : class;

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="usePage">查询的页数</param>
        /// <param name="pageSize">每页的容量</param>
        /// <param name="useWhere">使用的过滤条件</param>
        /// <param name="useInclude">使用的Include委托</param>
        /// <returns></returns>
        PagePacker<X> GetPage(int usePage, int pageSize, Expression<Func<X, bool>> useWhere = null, IncludeDel<X> useInclude = null);

        /// <summary>
        /// 附带转换机制的分页查询
        /// </summary>
        /// <typeparam name="Y">转换后的类型</typeparam>
        /// <param name="useTransformer">使用的转换机制（如group操作）</param>
        /// <param name="usePage">查询的页数</param>
        /// <param name="pageSize">每页的容量</param>
        /// <param name="useWhere">使用的过滤条件</param>
        /// <param name="useInclude">使用的Include委托</param>
        /// <returns></returns>
        PagePacker<Y> GetPage<Y>(Func<IQueryable<X>, IQueryable<Y>> useTransformer,
            int usePage, int pageSize, Expression<Func<Y, bool>> useWhere = null, IncludeDel<Y> useInclude = null) where Y : class;
        #endregion


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
    }
}
