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
    public class DefaultGenericRepository<T, X> : IRespository<T,X>
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

        /// <summary>
        /// 使用的迭代类型
        /// </summary>
        private static readonly Type m_useIEnumableType = typeof(IEnumerable<>);
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
        /// 获取第一个
        /// </summary>
        /// <param name="useWhere">使用的过滤条件</param>
        /// <param name="useInclude">使用的Include委托</param>
        /// <returns></returns>
        public X FindFirst(Expression<Func<X, bool>> useWhere = null, IncludeDel<X> useInclude = null)
        {
            return FindFirstMethod(useWhere, useInclude).Result;
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="useWhere">使用的过滤条件</param>
        /// <param name="useInclude">使用的Include委托</param>
        /// <returns></returns>
        public List<X> GetAll(Expression<Func<X, bool>> useWhere = null, IncludeDel<X> useInclude = null, string inputPropertyName = null
            , bool ifASC = true)
        {
            return GetAllMethod(useWhere, useInclude,inputPropertyName,ifASC).Result;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="usePage">查询的页数</param>
        /// <param name="pageSize">每页的容量</param>
        /// <param name="useWhere">使用的过滤条件</param>
        /// <param name="useInclude">使用的Include委托</param>
        /// <returns></returns>
        public PagePacker<X> GetPage(int usePage, int pageSize, Expression<Func<X, bool>> useWhere = null, IncludeDel<X> useInclude = null, string inputPropertyName = null
            , bool ifASC = true)
        {
            return GetPage(m_useDB, usePage, pageSize, useWhere, useInclude,inputPropertyName,ifASC);

        }

        /// <summary>
        /// 附带转换机制的获取全部
        /// </summary>
        /// <typeparam name="Y">转换后的类型</typeparam>
        /// <param name="useTransformer">使用的转换机制（如group操作）</param>
        /// <param name="useWhere">使用的过滤条件</param>
        /// <param name="useInclude">使用的Include委托</param>
        /// <returns></returns>
        public List<Y> GetAll<Y>(Func<IQueryable<X>, IQueryable<Y>> useTransformer, Expression<Func<Y, bool>> useWhere = null, IncludeDel<Y> useInclude = null, string inputPropertyName = null
            , bool ifASC = true)
            where Y : class
        {
            return GetAllMethodGeneric(useTransformer(m_useDB), useWhere, useInclude,inputPropertyName,ifASC).Result;
        }

        /// <summary>
        /// 附带转换机制的获取第一个
        /// </summary>
        /// <typeparam name="Y">转换后的类型</typeparam>
        /// <param name="useTransformer">使用的转换机制（如group操作）</param>
        /// <param name="useWhere">使用的过滤条件</param>
        /// <param name="useInclude">使用的Include委托</param>
        /// <returns></returns>
        public Y FindFirst<Y>(Func<IQueryable<X>, IQueryable<Y>> useTransformer, Expression<Func<Y, bool>> useWhere = null, IncludeDel<Y> useInclude = null)
            where Y : class
        {
            return FindFirstMethodGeneric(useTransformer(m_useDB), useWhere, useInclude).Result;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="usePage">查询的页数</param>
        /// <param name="pageSize">每页的容量</param>
        /// <param name="useWhere">使用的过滤条件</param>
        /// <param name="useInclude">使用的Include委托</param>
        /// <returns></returns>
        public PagePacker<Y> GetPage<Y>(Func<IQueryable<X>, IQueryable<Y>> useTransformer, int usePage, int pageSize, Expression<Func<Y, bool>> useWhere = null, IncludeDel<Y> useInclude = null, string inputPropertyName = null
            , bool ifASC = true)
            where Y : class
        {
            return GetPage(useTransformer(m_useDB), usePage, pageSize, useWhere, useInclude, inputPropertyName, ifASC);
        }

        /// <summary>
        /// 分页查询 用于boostrap
        /// </summary>
        /// <param name="usePage">查询的页数</param>
        /// <param name="pageSize">每页的容量</param>
        /// <param name="useWhere">使用的过滤条件</param>
        /// <param name="useInclude">使用的Include委托</param>
        public PagePackerBoostrap<X> GetPageBootstrap(int usePage, int pageSize, Expression<Func<X, bool>> useWhere = null, IncludeDel<X> useInclude = null, string inputPropertyName = null
            , bool ifASC = true)
        {
            var tempPage = GetPage(m_useDB, usePage, pageSize, useWhere, useInclude, inputPropertyName, ifASC);

            return new PagePackerBoostrap<X>() { Total = tempPage.TotalCount, Rows = tempPage.Values };
        }

        /// <summary>
        /// 附带转换机制的分页查询 用于boostrap
        /// </summary>
        /// <typeparam name="Y">转换后的类型</typeparam>
        /// <param name="useTransformer">使用的转换机制（如group操作）</param>
        /// <param name="usePage">查询的页数</param>
        /// <param name="pageSize">每页的容量</param>
        /// <param name="useWhere">使用的过滤条件</param>
        /// <param name="useInclude">使用的Include委托</param>
        public PagePackerBoostrap<Y> GetPageBootstrap<Y>(Func<IQueryable<X>, IQueryable<Y>> useTransformer, int usePage, int pageSize, Expression<Func<Y, bool>> useWhere = null, IncludeDel<Y> useInclude = null, string inputPropertyName = null
            , bool ifASC = true) where Y : class
        {
            var tempPage = GetPage(useTransformer(m_useDB), usePage, pageSize, useWhere, useInclude, inputPropertyName, ifASC);

            return new PagePackerBoostrap<Y>() { Total = tempPage.TotalCount, Rows = tempPage.Values };
        }

        /// <summary>
        /// 更新一个
        /// </summary>
        /// <param name="input"></param>
        public void Update(X input)
        {
            m_useDB.Update(input);
        }

        public void Load<Y>(X input, Expression<Func<X, IEnumerable<Y>>> propertyExpression)
             where Y : class
        {

            LoadMethod(input, propertyExpression).Wait();
        }

        public void Load<Y>(X input, Expression<Func<X, Y>> propertyExpression)
              where Y : class
        {

            LoadMethod(input, propertyExpression).Wait();
        }

        #region 私有方法

        /// <summary>
        /// 加载方法NIO
        /// </summary>
        /// <typeparam name="Y"></typeparam>
        /// <param name="input"></param>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        private async Task LoadMethod<Y>(X input, Expression<Func<X, IEnumerable<Y>>> propertyExpression)
              where Y : class
        {
            await UseContext.Entry(input).Collection(propertyExpression).LoadAsync();
        }

        /// <summary>
        /// 加载方法NIO
        /// </summary>
        /// <typeparam name="Y"></typeparam>
        /// <param name="input"></param>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        private async Task LoadMethod<Y>(X input, Expression<Func<X, Y>> propertyExpression)
           where Y : class
        {
            await UseContext.Entry(input).Reference(propertyExpression).LoadAsync();
        }

        /// <summary>
        /// 使用Include委托实现Include机制
        /// </summary>
        /// <param name="useInclude"></param>
        /// <returns></returns>
        private IQueryable<Y> Include<Y>(IQueryable<Y> inputSource,IncludeDel<Y> useInclude = null)
            where Y:class
        {

            if (null == useInclude)
            {
                return inputSource;
            }
            else
            {
                return useInclude(inputSource);
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
            return await FindFirstMethodGeneric(m_useDB, useWhere, useInclude);
        }

        /// <summary>
        /// GetAllNio
        /// </summary>
        /// <param name="useWhere"></param>
        /// <returns></returns>
        private async Task<List<X>> GetAllMethod(Expression<Func<X, bool>> useWhere = null, IncludeDel<X> useInclude = null, string inputPropertyName = null, bool ifASC = true)
        {
            return await GetAllMethodGeneric(m_useDB, useWhere, useInclude,inputPropertyName,ifASC);
        }

        /// <summary>
        /// FindFirstNio泛型方式
        /// </summary>
        /// <param name="useWhere"></param>
        /// <returns></returns>
        private async Task<Y> FindFirstMethodGeneric<Y>(IQueryable<Y> inputSource, Expression<Func<Y, bool>> useWhere = null, IncludeDel<Y> useInclude = null)
            where Y : class
        {
            if (null == useWhere)
            {
                return await Include(inputSource, useInclude).FirstOrDefaultAsync();
            }
            else
            {
                return await Include(inputSource, useInclude).FirstOrDefaultAsync(useWhere);
            }
        }

        /// <summary>
        /// GetAllNio
        /// </summary>
        /// <param name="useWhere"></param>
        /// <returns></returns>
        private async Task<List<Y>> GetAllMethodGeneric<Y>
            (IQueryable<Y> inputSource, Expression<Func<Y, bool>> useWhere = null, IncludeDel<Y> useInclude = null
            , string inputPropertyName = null, bool ifASC = true)
            where Y : class
        {
            var useValue = Include(inputSource, useInclude);

            if (null != useWhere)
            {
                useValue = useValue.Where(useWhere);
            }

            //获取排序
            var tempOrderFunc = ExpressionUtility.GetOrderByFunc<Y>(inputPropertyName, ifASC);

            if (null != tempOrderFunc)
            {
                useValue = tempOrderFunc(useValue);
            }

            return await useValue.ToListAsync();
        }

        /// <summary>
        /// 获取数量NIO
        /// </summary>
        /// <param name="useWhere"></param>
        /// <returns></returns>
        private async Task<int> GetCountMethodGeneric<Y>(IQueryable<Y> inputSource, Expression<Func<Y, bool>> useWhere = null)
            where Y:class
        {
            if (null == useWhere)
            {
                return await inputSource.CountAsync();
            }
            else
            {
                return await inputSource.CountAsync(useWhere);
            }
        }

        /// <summary>
        /// 获得分页数据NIO方法 泛型方式
        /// </summary>
        /// <param name="skipValue"></param>
        /// <param name="takeValue"></param>
        /// <param name="useWhere"></param>
        /// <returns></returns>
        private async Task<List<Y>> GetPageValueMethodGeneric<Y>
            (IQueryable<Y> inputSource, int skipValue, int takeValue, Expression<Func<Y, bool>> useWhere = null, IncludeDel<Y> useInclude = null
            ,string inputPropertyName = null,bool ifASC = true)
            where Y : class
        {
            var useValue = Include(inputSource, useInclude);

            if (null != useWhere)
            {
                useValue = useValue.Where(useWhere);
            }

            //获取排序
            var tempOrderFunc = ExpressionUtility.GetOrderByFunc<Y>(inputPropertyName, ifASC);

            if (null != tempOrderFunc)
            {
                useValue = tempOrderFunc(useValue);
            }

            //Nio
            return await useValue.Skip(skipValue).Take(takeValue).ToListAsync();
        }

        /// <summary>
        /// 泛型分页查询
        /// </summary>
        /// <typeparam name="Y"></typeparam>
        /// <param name="inputSource"></param>
        /// <param name="usePage"></param>
        /// <param name="pageSize"></param>
        /// <param name="useWhere"></param>
        /// <param name="useInclude"></param>
        /// <returns></returns>
        private PagePacker<Y> GetPage<Y>
            (IQueryable<Y> inputSource, int usePage, int pageSize,
            Expression<Func<Y, bool>> useWhere = null, IncludeDel<Y> useInclude = null
            , string inputPropertyName = null, bool ifASC = true)
            where Y : class
        {
            //输入检查
            if (usePage <= 0 || pageSize <= 0)
            {
                return null;
            }

            PagePacker<Y> returnValue = new PagePacker<Y>();

            //计算总数量
            int tempTotalCount = GetCountMethodGeneric(inputSource, useWhere).Result;

            //计算总页数
            int tempTotalPage = 0 == tempTotalCount % pageSize ? tempTotalCount / pageSize :
                tempTotalCount / pageSize + 1;

            //校验页索引
            if (usePage > tempTotalPage)
            {
                usePage = tempTotalPage;
            }


            //若没有数据
            if (usePage == 0)
            {
                returnValue.TotalCount = 0;
                returnValue.TotalPage = 0;
                returnValue.Values = new List<Y>();
                returnValue.CurrentPage = 0;
                returnValue.PageSize = 0;
                return returnValue;
            }

            //获取数据
            var tempValue = GetPageValueMethodGeneric
                (inputSource, (usePage - 1) * pageSize, pageSize, useWhere, useInclude, inputPropertyName,ifASC).Result;

            //数值回写
            returnValue.TotalCount = tempTotalCount;
            returnValue.TotalPage = tempTotalPage;
            returnValue.Values = tempValue;
            returnValue.CurrentPage = usePage;
            returnValue.PageSize = pageSize;

            return returnValue;
        }
        #endregion
    }
}
