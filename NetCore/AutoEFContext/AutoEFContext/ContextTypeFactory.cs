using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoEFContext
{
    /// <summary>
    /// 代理上下文制造工厂
    /// </summary>
    public class ContextTypeFactory
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
        /// 制造代理上下文类型（使用AutoContext作为基类）
        /// </summary>
        /// <typeparam name="T">使用的基类类型（需从AutoContext继承）</typeparam>
        /// <param name="useOneConfiguringDle">使用的定义委托</param>
        /// <param name="useOneMdoleCreatingDel">使用的模型定义委托</param>
        /// <param name="useKeyFilter">使用的Key值过滤器</param>
        /// <returns>创建的上下文类型</returns>
        public static Type GetProxyType
            (OnConfiguringDel useOneConfiguringDle = null, OnModelCreatingDel useOneMdoleCreatingDel = null,IKEYFilter useKeyFilter = null)
        {
            Type useBaseType = null;

            return CreatContextType(useOneConfiguringDle, useOneMdoleCreatingDel, useKeyFilter, useBaseType);
        }

        /// <summary>
        /// 制造代理上下文类型（可指定使用基类）
        /// </summary>
        /// <typeparam name="T">使用的基类类型（需从AutoContext继承）</typeparam>
        /// <param name="useOneConfiguringDle">使用的定义委托</param>
        /// <param name="useOneMdoleCreatingDel">使用的模型定义委托</param>
        /// <param name="useKeyFilter">使用的Key值过滤器</param>
        /// <returns>创建的上下文类型</returns>
        public static Type GetProxyType<T>
            (OnConfiguringDel useOneConfiguringDle = null, OnModelCreatingDel useOneMdoleCreatingDel = null, IKEYFilter useKeyFilter = null)
            where T:AutoContext
        {
            Type useBaseType = typeof(T);

            return CreatContextType(useOneConfiguringDle, useOneMdoleCreatingDel, useKeyFilter, useBaseType);
        }

        /// <summary>
        /// 制造上下文类型底层方法
        /// </summary>
        /// <param name="useOneConfiguringDle">使用的定义委托</param>
        /// <param name="useOneMdoleCreatingDel">使用的模型定义委托</param>
        /// <param name="useKeyFilter">使用的Key值过滤器</param>
        /// <param name="useBaseType">使用的代理类基类</param>
        /// <returns>>创建的上下文类型</returns>
        private static Type CreatContextType
            (OnConfiguringDel useOneConfiguringDle, OnModelCreatingDel useOneMdoleCreatingDel, IKEYFilter useKeyFilter, Type useBaseType)
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
                    AutoEntityAttribute tempAttribute = oneType.GetCustomAttribute(typeof(AutoEntityAttribute), false) as AutoEntityAttribute;
                    if (oneType.IsClass && oneType.IsPublic
                        && null != tempAttribute)
                    {
                        //使用Key过滤器过滤
                        if (null != useKeyFilter)
                        {
                            var tempString = tempAttribute.Key;
                            //若需过滤则跳过
                            if (false == useKeyFilter.IfUse(tempString))
                            {
                                continue;
                            }
                        }

                        lstEntityTypes.Add(oneType);
                    }
                }
            }

            //制造context代理对象
            ContextEmitUtility tempUseEmitUtility = new ContextEmitUtility(m_strUseContextAssembly, m_strUseContext, lstEntityTypes);

            var returnValue = tempUseEmitUtility.EmiteContextType(useBaseType);

            if (null != returnValue)
            {
                //获取全局字典
                ConcurrentTypeDelDic useDic = ConcurrentTypeDelDic.GetDic();

                //制作临时封装
                EFDelPacker tempPacker = new EFDelPacker() { UseOnConfig = useOneConfiguringDle, UseOnModelCreating = useOneMdoleCreatingDel };

                //添加到字典
                useDic.Add(returnValue, tempPacker);

            }

            return returnValue;
        }
    }
}