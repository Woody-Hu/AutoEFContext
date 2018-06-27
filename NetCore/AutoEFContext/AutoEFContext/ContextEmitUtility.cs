using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace AutoEFContext
{
    /// <summary>
    /// 上下文代理类射出工具
    /// </summary>
    internal class ContextEmitUtility
    {
        #region 私有字段
        /// <summary>
        /// 使用的上下文名称
        /// </summary>
        private string m_useContextName;

        /// <summary>
        /// 使用的Entity类型
        /// </summary>
        private List<Type> m_useEntityTypies;

        /// <summary>
        /// 使用的程序集名称
        /// </summary>
        private string m_useAssemblyName;

        /// <summary>
        /// 上下文基础类
        /// </summary>
        private static Type m_useBaseType = typeof(AutoContext);

        /// <summary>
        /// 字段前缀
        /// </summary>
        private const string m_filedAppendStr = "m_";

        /// <summary>
        /// DB后缀
        /// </summary>
        private const string m_DBStr = "DB";

        /// <summary>
        /// Set前缀
        /// </summary>
        private const string m_SetStr = "Set";

        /// <summary>
        /// Get前缀
        /// </summary>
        private const string m_GetStr = "Get";

        /// <summary>
        /// 泛型Db容器类型
        /// </summary>
        private static Type m_baseDBSetType = typeof(DbSet<>);
        #endregion

        internal ContextEmitUtility(string inputAssemblyName, string inputContexName, List<Type> inputLstEntities)
        {
            m_useAssemblyName = inputAssemblyName;
            m_useContextName = inputContexName;
            m_useEntityTypies = inputLstEntities;
        }

        /// <summary>
        /// 生成射出类型
        /// </summary>
        /// <returns></returns>
        internal Type EmiteContextType(Type inputBaseType = null)
        {
            Type returnValue = null;

            AppDomain useAppDomain = AppDomain.CurrentDomain;

            Type useBaseType = null;

            //若没输入基类则使用默认类
            if (null == inputBaseType)
            {
                useBaseType = m_useBaseType;
            }
            else
            {
                useBaseType = inputBaseType;
            }

            AssemblyBuilder useAssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(m_useAssemblyName), AssemblyBuilderAccess.Run);

            ModuleBuilder useModuleBuilder = useAssemblyBuilder.DefineDynamicModule(m_useAssemblyName);

            TypeBuilder useTypeBuilder = useModuleBuilder.DefineType(m_useContextName, TypeAttributes.Public, useBaseType);

            PrepareConstr(useTypeBuilder, useBaseType);

            foreach (var oneType in m_useEntityTypies)
            {
                PrepareProperty(useTypeBuilder, oneType);
            }


            //创建类型
            returnValue = useTypeBuilder.CreateType();


            return returnValue;
        }

        /// <summary>
        /// 准备构造方法
        /// </summary>
        /// <param name="inputTypeBuilder"></param>
        private void PrepareConstr(TypeBuilder inputTypeBuilder,Type inputBaseType)
        {
            //循环基类的构造方法
            foreach (var onCons in inputBaseType.GetConstructors())
            {
                var tempArgParam = onCons.GetParameters();
                var tempArgTypes = (from n in tempArgParam select n.ParameterType).ToArray();
                //设置构造方法
                var tempCon = inputTypeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, tempArgTypes);

                var tempIl = tempCon.GetILGenerator();

                //加载参数调度构造方法
                tempIl.Emit(OpCodes.Ldarg_0);

                for (int i = 0; i < tempArgTypes.Length; i++)
                {
                    tempIl.Emit(OpCodes.Ldarg, i + 1);
                }

                //发起基类构造方法
                tempIl.Emit(OpCodes.Call, onCons);
                tempIl.Emit(OpCodes.Ret);
            }
        }

        /// <summary>
        /// 制作属性
        /// </summary>
        /// <param name="inputTypeBuilder"></param>
        /// <param name="inputType"></param>
        private void PrepareProperty(TypeBuilder inputTypeBuilder, Type inputType)
        {
            //制作泛型参数
            Type tempType = m_baseDBSetType.MakeGenericType(inputType);
            //制作字段
            var tempFiled = inputTypeBuilder.DefineField(m_filedAppendStr + inputType.Name + m_DBStr, tempType, FieldAttributes.Private);

            //制作get/set方法
            var methodGet = inputTypeBuilder.DefineMethod(m_GetStr + inputType.Name + m_DBStr, MethodAttributes.Public, tempType, null);
            var methodSet = inputTypeBuilder.DefineMethod(m_SetStr + inputType.Name + m_DBStr, MethodAttributes.Public, null, new Type[] { tempType });

            var getMethodIl = methodGet.GetILGenerator();
            //加载参数0;此实例
            getMethodIl.Emit(OpCodes.Ldarg_0);
            //加载读取字段行为
            getMethodIl.Emit(OpCodes.Ldfld, tempFiled);
            //返回
            getMethodIl.Emit(OpCodes.Ret);

            var setMethodIl = methodSet.GetILGenerator();
            //加载参数0：此实例
            setMethodIl.Emit(OpCodes.Ldarg_0);
            //加载参数1：输入对象
            setMethodIl.Emit(OpCodes.Ldarg_1);
            //加载设置字段行为
            setMethodIl.Emit(OpCodes.Stfld, tempFiled);
            //返回
            setMethodIl.Emit(OpCodes.Ret);

            //声明属性
            var tempProperty = inputTypeBuilder.DefineProperty(inputType.Name + m_DBStr, PropertyAttributes.None, tempType, null);

            //绑定方法
            tempProperty.SetGetMethod(methodGet);
            tempProperty.SetSetMethod(methodSet);
        }
    }
}