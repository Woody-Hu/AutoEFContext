using AutoEFContext;
using Autofac;
using AutofacMiddleware;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AutofacEFImp
{
    /// <summary>
    /// 泛型自动化上下文准备器
    /// </summary>
    /// <typeparam name="T">使用的自动化上下文基类</typeparam>
    public class GenericAutoEFAutofacContainerPrepare<T> : IAutofacContainerPrepare
        where T:AutoContext
    {
        private OnConfiguringDel m_useOnConfiguring = null;

        private OnModelCreatingDel m_useOneModelCreatingDel = null;

        /// <summary>
        /// 构造自动化上下文准备器 以T为注册源
        /// </summary>
        /// <param name="inputConfiguringDel">使用的定义委托</param>
        /// <param name="inputOneMdelCreatingDel">使用的模型创建委托</param>
        public GenericAutoEFAutofacContainerPrepare(OnConfiguringDel inputConfiguringDel = null, OnModelCreatingDel inputOneMdelCreatingDel = null)
        {
            m_useOnConfiguring = inputConfiguringDel;
            m_useOneModelCreatingDel = inputOneMdelCreatingDel;
        }

        public void Prepare(ContainerBuilder builder)
        {
            //制作临时上下文对象
            var tempType = ContextTypeFactory.GetProxyType<T>(m_useOnConfiguring, m_useOneModelCreatingDel);

            //初始化数据库结构
            using (var tempContext = Activator.CreateInstance(tempType) as AutoContext)
            {
                tempContext.InitDB();
            }

            //注册为请求实例
            builder.RegisterType(tempType).As(typeof(T)).InstancePerLifetimeScope();
        }
    }
}