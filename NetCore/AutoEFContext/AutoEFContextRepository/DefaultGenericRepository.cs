using AutoEFContext;
using AutofacMiddleware;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoEFContextRepository
{
    [Component(IfByClass = false,LifeScope = LifeScopeKind.Transient)]
    /// <summary>
    /// 默认的泛型操作机制类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="X"></typeparam>
    public class DefaultGenericRepository<T, X> : IRepository<T,X>
        where T:AutoContext
        where X:class
    {
        #region 私有字段
        /// <summary>
        /// 使用的自动化上下文对象
        /// </summary>
        private T m_useContext;

        /// <summary>
        /// 使用的对应DbSet对象
        /// </summary>
        private DbSet<X> m_useDB;
        #endregion

        /// <summary>
        /// 使用的DbSet对象
        /// </summary>
        public DbSet<X> UseDB => m_useDB;

        /// <summary>
        /// 使用的DbContext
        /// </summary>
        public T UseContext => m_useContext;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="inputContext"></param>
        public DefaultGenericRepository(T inputContext)
        {
            m_useContext = inputContext;
            m_useDB = inputContext.GetDb<X>();
        }

        /// <summary>
        /// 添加一个
        /// </summary>
        /// <param name="input"></param>
        public void Add(X input)
        {
            AddMethod(input).Wait();
        }

        /// <summary>
        /// 添加一组
        /// </summary>
        /// <param name="input"></param>
        public void AddRange(IEnumerable<X> input)
        {
            AddRangeMethod(input).Wait();
        }

        /// <summary>
        /// 删除一个
        /// </summary>
        /// <param name="input"></param>
        public void Delete(X input)
        {
            m_useDB.Remove(input);
        }

        /// <summary>
        /// 寻找符合条件的第一个
        /// </summary>
        /// <param name="useWhere">使用的过滤条件</param>
        /// <returns></returns>
        public X FindFirst(Expression<Func<X, bool>> useWhere = null, IncludeDel<X> useInclude = null)
        {
            return FindFirstMethod(useWhere, useInclude).Result;
        }

        /// <summary>
        /// 寻找所有
        /// </summary>
        /// <param name="useWhere">使用的过滤条件</param>
        /// <returns></returns>
        public List<X> GetAll(Expression<Func<X, bool>> useWhere = null, IncludeDel<X> useInclude = null)
        {
            return GetAllMethod(useWhere, useInclude).Result;
   
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="usePage">当前页</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="useWhere">使用的过滤条件</param>
        /// <returns></returns>
        public PagePacker<X> GetPage(int usePage, int pageSize, Expression<Func<X, bool>> useWhere = null, IncludeDel<X> useInclude = null)
        {
            //输入检查
            if (usePage <= 0 || pageSize <=0)
            {
                return null;
            }

            PagePacker<X> returnValue = new PagePacker<X>();

            //计算总数量
            int tempTotalCount = GetCountMethod(useWhere).Result;

            //计算总页数
            int tempTotalPage = 0 == tempTotalCount % pageSize ? tempTotalCount / pageSize :
                tempTotalCount / pageSize + 1;

            //校验页索引
            if (usePage > tempTotalPage)
            {
                usePage = tempTotalPage;
            }

            //获取数据
            var tempValue = GetPageValueMethod((usePage - 1) * pageSize, pageSize, useWhere,useInclude).Result;

            //数值回写
            returnValue.TotalCount = tempTotalCount;
            returnValue.TotalPage = tempTotalPage;
            returnValue.Values = tempValue;
            returnValue.CurrentPage = usePage;
            returnValue.PageSize = pageSize;

            return returnValue;

        }

        /// <summary>
        /// 更新一个
        /// </summary>
        /// <param name="input"></param>
        public void Update(X input)
        {
            m_useDB.Update(input);
        }

        #region 私有方法

        /// <summary>
        /// 使用Include委托实现Include机制
        /// </summary>
        /// <param name="useInclude"></param>
        /// <returns></returns>
        private IQueryable<X> Include(IncludeDel<X> useInclude = null)
        {
            if (null == useInclude)
            {
                return m_useDB;
            }
            else
            {
                return useInclude(m_useDB);
            }

        }

        /// <summary>
        /// Add NIO
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task AddMethod(X input)
        {
            await m_useDB.AddAsync(input);
        }

        /// <summary>
        /// AddRangeNIO
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task AddRangeMethod(IEnumerable<X> input)
        {
            await m_useDB.AddRangeAsync(input);
        }

        /// <summary>
        /// FindFirstNio
        /// </summary>
        /// <param name="useWhere"></param>
        /// <returns></returns>
        private async Task<X> FindFirstMethod(Expression<Func<X, bool>> useWhere = null, IncludeDel<X> useInclude = null)
        {
            if (null == useWhere)
            {
                return await Include(useInclude).FirstAsync();
            }
            else
            {
                return await Include(useInclude).FirstAsync(useWhere);
            }
        }

        /// <summary>
        /// GetAllNio
        /// </summary>
        /// <param name="useWhere"></param>
        /// <returns></returns>
        private async Task<List<X>> GetAllMethod(Expression<Func<X, bool>> useWhere = null, IncludeDel<X> useInclude = null)
        {
            if (null == useWhere)
            {
                return await Include(useInclude).ToListAsync();
            }
            else
            {
                return await Include(useInclude).Where(useWhere).ToListAsync();
            }
        }

        /// <summary>
        /// 获得分页数据NIO方法
        /// </summary>
        /// <param name="skipValue"></param>
        /// <param name="takeValue"></param>
        /// <param name="useWhere"></param>
        /// <returns></returns>
        private async Task<List<X>> GetPageValueMethod(int skipValue, int takeValue, Expression<Func<X, bool>> useWhere = null, IncludeDel<X> useInclude = null)
        {
            if (null == useWhere)
            {
                return await Include(useInclude).Skip(skipValue).Take(takeValue).ToListAsync();
            }
            else
            {
                return await Include(useInclude).Where(useWhere).Skip(skipValue).Take(takeValue).ToListAsync();
            }
        }

        /// <summary>
        /// 获取数量NIO
        /// </summary>
        /// <param name="useWhere"></param>
        /// <returns></returns>
        private async Task<int> GetCountMethod(Expression<Func<X, bool>> useWhere = null)
        {
            if (null == useWhere)
            {
                return await m_useDB.CountAsync();
            }
            else
            {
                return await m_useDB.CountAsync(useWhere);
            }
        } 
        #endregion
    }
}
