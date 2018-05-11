using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoEFContext
{
    /// <summary>
    /// 代理上下文制造工厂
    /// </summary>
    internal class ContextTypeFactory
    {
        private const string m_strUseContextAssembly = "ContextAssembly";

        private const string m_strUseContext = "UseContext";

        /// <summary>
        /// 制造代理上下文类型
        /// </summary>
        /// <returns></returns>
        internal static Type GetProxyType()
        {
            List<Type> lstEntityTypes = new List<Type>();

            List<Type> lstTempType;

            foreach (var oneAssembliy in AppDomain.CurrentDomain.GetAssemblies())
            {
                lstTempType = null;

                try
                {
                    lstTempType = oneAssembliy.GetTypes().ToList();
                }
                catch (Exception)
                {
                    lstTempType = new List<Type>();
                }

                foreach (var oneType in lstTempType)
                {
                    if (oneType.IsClass && oneType.IsPublic
                        && null != oneType.GetCustomAttributes(typeof(AutoEntityAttribute),false) 
                        && 1 == oneType.GetCustomAttributes(typeof(AutoEntityAttribute), false).Count())
                    {
                        lstEntityTypes.Add(oneType);
                    }
                }
            }

            ContextEmitUtility tempUseEmitUtility = new ContextEmitUtility(m_strUseContextAssembly, m_strUseContext, lstEntityTypes);

            var returnValue = tempUseEmitUtility.EmiteContextType();

            return returnValue;
        }
    }
}
