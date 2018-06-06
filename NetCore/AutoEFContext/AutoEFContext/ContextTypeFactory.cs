using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AutoEFContext
{
    /// <summary>
    /// 代理上下文制造工厂
    /// </summary>
    internal class ContextTypeFactory
    {
        /// <summary>
        /// 默认的assembly名称
        /// </summary>
        private const string m_strUseContextAssembly = "ContextAssembly";

        /// <summary>
        /// 默认的Context名称
        /// </summary>
        private const string m_strUseContext = "UseContext";

        /// <summary>
        /// 制造代理上下文类型
        /// </summary>
        /// <returns></returns>
        internal static Type GetProxyType()
        {
            List<Type> lstEntityTypes = new List<Type>();

            List<Type> lstTempType;

            //加载当前应用程序域的所有程序集
            foreach (var oneAssembliy in AppDomain.CurrentDomain.GetAssemblies())
            {
                lstTempType = null;

                //尝试获取所有类型对象
                try
                {
                    lstTempType = oneAssembliy.GetTypes().ToList();
                }
                catch (Exception)
                {
                    lstTempType = new List<Type>();
                }

                //寻找public的设置自动Entity的类型
                foreach (var oneType in lstTempType)
                {
                    if (oneType.IsClass && oneType.IsPublic
                        && null != oneType.GetCustomAttributes(typeof(AutoEntityAttribute), false)
                        && 1 == oneType.GetCustomAttributes(typeof(AutoEntityAttribute), false).Count())
                    {
                        lstEntityTypes.Add(oneType);
                    }
                }
            }

            //制造context代理对象
            ContextEmitUtility tempUseEmitUtility = new ContextEmitUtility(m_strUseContextAssembly, m_strUseContext, lstEntityTypes);

            var returnValue = tempUseEmitUtility.EmiteContextType();

            return returnValue;
        }
    }
}