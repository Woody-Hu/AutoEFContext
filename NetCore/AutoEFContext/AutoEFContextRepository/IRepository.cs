using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AutoEFContextRepository
{
    /// <summary>
    /// 储存层接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T>
        where T:class
    {
        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="useWhere"></param>
        /// <returns></returns>
        List<T> GetAll(Expression<Func<T, bool>> useWhere = null);

        /// <summary>
        /// 获取第一个
        /// </summary>
        /// <param name="useWhere"></param>
        /// <returns></returns>
        T FindFirst(Expression<Func<T, bool>> useWhere = null);

        /// <summary>
        /// 添加一个
        /// </summary>
        /// <param name="input"></param>
        void Add(T input);

        /// <summary>
        /// 添加一组
        /// </summary>
        /// <param name="input"></param>
        void AddRange(IEnumerable<T> input);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="input"></param>
        void Update(T input);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="input"></param>
        void Delete(T input);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="usePage"></param>
        /// <param name="pageSize"></param>
        /// <param name="useWhere"></param>
        /// <returns></returns>
        PagePacker<T> GetPage(int usePage, int pageSize, Expression<Func<T, bool>> useWhere = null);
    }
}
