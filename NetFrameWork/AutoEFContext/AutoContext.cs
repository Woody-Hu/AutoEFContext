using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoEFContext
{
    /// <summary>
    /// 自动上下文基类
    /// </summary>
    public abstract class AutoContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var useDic = ConcurrentTypeDelDic.GetDic();
            var thisType = this.GetType();

            //使用委托
            if (useDic.Contains(thisType))
            {
                var tempPacker = useDic.Get(thisType);

                tempPacker.UseOnModelCreating?.Invoke(modelBuilder);
            }
        }
      

        #region 私有字段
        /// <summary>
        /// 使用的Set表达式字典
        /// </summary>
        private Dictionary<Type, object> m_useSetExpression = null;

        /// <summary>
        /// 使用的Get表达式字典
        /// </summary>
        private Dictionary<Type, object> m_useGetExpression = null;

        /// <summary>
        /// 使用的返回值类型泛型基础类
        /// </summary>
        private static Type m_useBaseReturnType = typeof(DbSet<>);
        #endregion

        /// <summary>
        /// 构造方法
        /// </summary>
        public AutoContext() : base()
        {
            Init();
        }

        /// <summary>
        /// 获取DbSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DbSet<T> GetDb<T>()
            where T : class
        {
            Type useType = typeof(T);

            if (!m_useGetExpression.ContainsKey(useType))
            {
                return null;
            }

            Func<AutoContext, DbSet<T>> useExpression = m_useGetExpression[useType] as Func<AutoContext, DbSet<T>>;
            if (null != useExpression)
            {
                //执行表达式树
                return useExpression(this);
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 设置DbSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputValue"></param>
        public void SetDb<T>(DbSet<T> inputValue)
            where T : class
        {
            Type useType = typeof(T);

            if (!m_useGetExpression.ContainsKey(useType))
            {
                return;
            }

            Action<AutoContext, DbSet<T>> useExpression = m_useSetExpression[useType] as Action<AutoContext, DbSet<T>>;
            //获取表达式树
            if (null != useExpression)
            {
                useExpression(this, inputValue);
            }
        }

        /// <summary>
        /// 初始化数据库结构
        /// </summary>
        /// <param name="connectionStr"></param>
        public void InitDB()
        {
            this.Database.Initialize(true);
        }


        #region 私有方法

        /// <summary>
        /// 初始化字段
        /// </summary>
        private void Init()
        {
            Type useType = this.GetType();

            m_useSetExpression = ExpressionUtility.GetSetActionDic(useType);

            m_useGetExpression = ExpressionUtility.GetGetFuncDic(useType);
        }
        #endregion
    }
}
