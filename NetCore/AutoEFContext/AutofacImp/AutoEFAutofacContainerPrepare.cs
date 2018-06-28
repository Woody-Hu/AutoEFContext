using AutoEFContext;
using Autofac;
using AutofacMiddleware;
using System;

namespace AutofacImp
{
    /// <summary>
    /// 自动扫描框架Autofac配置器(以AutoContext为注册基类)
    /// </summary>
    public class AutoEFAutofacContainerPrepare : IAutofacContainerPrepare
    {
        private OnConfiguringDel m_useOnConfiguring = null;

        private OnModelCreatingDel m_useOneModelCreatingDel = null;

        private IKEYFilter m_useKeyFilter = null;

        /// <summary>
        /// 构造配置器
        /// </summary>
        /// <param name="inputConfiguringDel">使用的定义委托</param>
        /// <param name="inputOneMdelCreatingDel">使用的模型创建委托</param>
        public AutoEFAutofacContainerPrepare(OnConfiguringDel inputConfiguringDel = null, OnModelCreatingDel inputOneMdelCreatingDel = null, IKEYFilter inputKeyFilter = null)
        {
            m_useOnConfiguring = inputConfiguringDel;
            m_useOneModelCreatingDel = inputOneMdelCreatingDel;
            m_useKeyFilter = inputKeyFilter;
        }

        public void Prepare(ContainerBuilder builder)
        {
            //制作临时上下文对象
            var tempType = ContextTypeFactory.GetProxyType(m_useOnConfiguring, m_useOneModelCreatingDel, m_useKeyFilter);

            //初始化数据库结构
            using (var tempContext = Activator.CreateInstance(tempType) as AutoContext)
            {
                tempContext.InitDB();
            }

            //注册为请求实例
            builder.RegisterType(tempType).As(typeof(AutoContext)).InstancePerLifetimeScope();
        }
    }
}
