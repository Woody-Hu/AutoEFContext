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
    public abstract class AutoContext:DbContext
    {
        /// <summary>
        /// 使用的实际类型
        /// </summary>
        private static Type m_realUseContextType;

        /// <summary>
        /// 使用的属性字典
        /// </summary>
        private Dictionary<Type, PropertyInfo> m_useDicPropertyInfo = null;

        /// <summary>
        /// 使用的返回值类型泛型基础类
        /// </summary>
        private static Type m_useBaseReturnType = typeof(IDbSet<>);

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
        public AutoContext():base()
        {
            Init();
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="connectionStr">数据库连接字符串</param>
        public AutoContext(string connectionStr):base(connectionStr)
        {
            Init();
        }

        public IDbSet<T> GetDb<T>()
            where T:class
        {

            Type useType = typeof(T);
            return m_useDicPropertyInfo[useType].GetValue(this) as IDbSet<T>;
        }

        /// <summary>
        /// 获得数据库连接对象
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <returns></returns>
        public static AutoContext GetContext(string connectionStr)
        {
            if (!string.IsNullOrWhiteSpace(connectionStr))
            {
                return Activator.CreateInstance(m_realUseContextType, new object[] { connectionStr }) as AutoContext;
            }
            else
            {
                return Activator.CreateInstance(m_realUseContextType) as AutoContext;
            }
        }

        /// <summary>
        /// 初始化数据库结构
        /// </summary>
        /// <param name="connectionStr"></param>
        public static void InitDB(string connectionStr)
        {
            using (var useContext = GetContext(connectionStr))
            {
                useContext.Init();
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
