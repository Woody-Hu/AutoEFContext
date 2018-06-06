using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AutoEFContext
{
    /// <summary>
    /// 自动上下文基类
    /// </summary>
    public abstract class AutoContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// 使用的实际类型
        /// </summary>
        private static Type m_realUseContextType;

        /// <summary>
        /// 使用的属性字典
        /// </summary>
        private Dictionary<Type, PropertyInfo> m_useDicPropertyInfo = null;

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

        /// <summary>
        /// 静态初始化
        /// </summary>
        static AutoContext()
        {
            //制作代理类
            m_realUseContextType = ContextTypeFactory.GetProxyType();
        }

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
            return m_useDicPropertyInfo[useType].GetValue(this) as DbSet<T>;
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
            m_useDicPropertyInfo[useType].SetValue(this, inputValue);
        }

        /// <summary>
        /// 获得数据库连接对象
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <returns></returns>
        public static AutoContext GetContext()
        {
            return Activator.CreateInstance(m_realUseContextType) as AutoContext;
        }

        /// <summary>
        /// 初始化数据库结构
        /// </summary>
        /// <param name="connectionStr"></param>
        public static void InitDB()
        {
            using (var useContext = GetContext())
            {
                useContext.Init();
                useContext.Database.EnsureCreated();
            }
        }

        /// <summary>
        /// 初始化字段
        /// </summary>
        private void Init()
        {
            m_useDicPropertyInfo = new Dictionary<Type, PropertyInfo>();

            Type useType = this.GetType();

            foreach (var oneProperty in useType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (oneProperty.PropertyType.IsGenericType && oneProperty.PropertyType.GetGenericTypeDefinition() == m_useBaseReturnType)
                {
                    m_useDicPropertyInfo.Add(oneProperty.PropertyType.GetGenericArguments()[0], oneProperty);
                }
            }
        }
    }
}
