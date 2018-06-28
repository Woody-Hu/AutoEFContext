using AutoEFContext;
using AutofacMiddleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutofacEFImp
{
    /// <summary>
    /// 自动化上下文 autofac对接 扫描机制
    /// </summary>
    public class AutoGenericAutoEFAutofacContainerPrepare
    {
        /// <summary>
        /// 使用的Keyfilter接口基类
        /// </summary>
        private static Type m_useIKeyfilterType = typeof(IKEYFilter);

        /// <summary>
        /// 使用的自动化上下文基类
        /// </summary>
        private static Type m_useBaseContextType = typeof(AutoContext);

        /// <summary>
        /// 泛型准备器基类
        /// </summary>
        private static Type m_useBasePrepareType = typeof(GenericAutoEFAutofacContainerPrepare<>);

        /// <summary>
        /// 使用的扫描特性
        /// </summary>
        private static Type m_useAttributeType = typeof(AutoContextAttribute);

        /// <summary>
        /// 扫描并获取当前系统的准备器
        /// </summary>
        /// <returns></returns>
        public static List<IAutofacContainerPrepare> GetPrepares()
        {
            List<IAutofacContainerPrepare> returnValue = new List<IAutofacContainerPrepare>();

            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            //循环程序集
            foreach (var oneAssemblies in allAssemblies)
            {
                List<Type> tempTypes = new List<Type>();

                //获取类型
                try
                {
                    tempTypes = oneAssemblies.GetTypes().ToList();
                }
                catch (Exception)
                {
                    continue;
                }

                //循环类型
                foreach (var oneType in tempTypes)
                {
                    //若派生体系不符合要求
                    if (!m_useBaseContextType.IsAssignableFrom(oneType))
                    {
                        continue;
                    }

                    if (null != oneType.GetCustomAttribute(m_useAttributeType))
                    {
                        var tempAttribute = oneType.GetCustomAttribute(m_useAttributeType) as AutoContextAttribute;

                        IKEYFilter tempFilter = null;

                        tempFilter = CreateKeyFilter(tempAttribute);

                        IAutofacContainerPrepare tempPrepare = MakePrepare(oneType, tempFilter);

                        if (null != tempPrepare)
                        {
                            returnValue.Add(tempPrepare);
                        }
                    }
                }
            }

            return returnValue;
        }

        /// <summary>
        /// 制作准备器
        /// </summary>
        /// <param name="oneType"></param>
        /// <param name="tempFilter"></param>
        /// <returns></returns>
        private static IAutofacContainerPrepare MakePrepare(Type oneType, IKEYFilter tempFilter)
        {
            IAutofacContainerPrepare tempPrepare = null;

            try
            {
                //制作泛型类型
                var tempType = m_useBasePrepareType.MakeGenericType(oneType);
                tempPrepare = Activator.CreateInstance(tempType, new object[] { null, null, tempFilter }) as IAutofacContainerPrepare;
            }
            catch (Exception)
            {
                tempPrepare = null;
            }

            return tempPrepare;
        }

        /// <summary>
        /// 制作键值过滤器
        /// </summary>
        /// <param name="tempAttribute"></param>
        /// <returns></returns>
        private static IKEYFilter CreateKeyFilter(AutoContextAttribute tempAttribute)
        {
            IKEYFilter tempFilter = null;
            //创建KeyFilter
            if (null != tempAttribute.UseKeyFilterType && m_useIKeyfilterType.IsAssignableFrom(tempAttribute.UseKeyFilterType))
            {
                try
                {
                    tempFilter = Activator.CreateInstance(tempAttribute.UseKeyFilterType) as IKEYFilter;
                }
                catch (Exception)
                {
                    tempFilter = null;
                }
            }

            return tempFilter;
        }
    }
}
