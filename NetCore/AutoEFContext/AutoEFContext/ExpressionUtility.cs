using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AutoEFContext
{
    /// <summary>
    /// 表达式树工具
    /// </summary>
    public class ExpressionUtility
    {
        /// <summary>
        /// 使用的上下文类型
        /// </summary>
        private static Type m_useContextType = typeof(AutoContext);

        /// <summary>
        /// 获得一个类型的DbsetGet委托字典
        /// </summary>
        /// <param name="inputType"></param>
        /// <returns></returns>
        public static Dictionary<Type,object> GetGetFuncDic(Type inputType)
        {
            return null;
        }

        /// <summary>
        /// 获得一个类型的DbsetSet委托字典
        /// </summary>
        /// <param name="inputType"></param>
        /// <returns></returns>
        public static Dictionary<Type,object> GetSetActionDic(Type inputType)
        {
            return null;
        }

        /// <summary>
        /// 获得所有的Get委托
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputType"></param>
        /// <param name="inputPropertyDic"></param>
        /// <returns></returns>
        private static Func<AutoContext,DbSet<T>> GetGetFunc<T>(Type inputType,Dictionary<Type,PropertyInfo> inputPropertyDic)
            where T : class
        {
            //获取期望类型
            Type wantType = typeof(T);

            //若字典不含义Key
            if (!inputPropertyDic.ContainsKey(wantType))
            {
                return null;
            }

            //获得使用的属性
            PropertyInfo useProperty = inputPropertyDic[wantType];

            //获得参数类型
            var useType = typeof(DbSet<T>);

            //输入的数据库上下文参数
            ParameterExpression inputContextParameter = Expression.Parameter(m_useContextType);

            //上下文类型转换
            var realUseContextParameter = Expression.TypeAs(inputContextParameter, inputType);

            //获得使用的属性
            var tempProperty = Expression.Property(realUseContextParameter, useProperty);

            var useGetExpression = Expression.Call(realUseContextParameter, useProperty.GetMethod);

            return Expression.Lambda<Func<AutoContext, DbSet<T>>>(useGetExpression, inputContextParameter).Compile();

        }

        /// <summary>
        /// 获得所有的Set委托
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputType"></param>
        /// <param name="inputPropertyDic"></param>
        /// <returns></returns>
        private static Action<AutoContext, DbSet<T>> GetSetAction<T> (Type inputType, Dictionary<Type, PropertyInfo> inputPropertyDic)
            where T:class
        {
            //获取期望类型
            Type wantType = typeof(T);

            //若字典不含义Key
            if (!inputPropertyDic.ContainsKey(wantType))
            {
                return null;
            }

            //获得使用的属性
            PropertyInfo useProperty = inputPropertyDic[wantType];

            //获得参数类型
            var useType = typeof(DbSet<T>);

            //输入的数据库上下文参数
            ParameterExpression inputContextParameter = Expression.Parameter(m_useContextType);

            //上下文类型转换
            var realUseContextParameter = Expression.TypeAs(inputContextParameter, inputType);

            //输入Dbset参数
            ParameterExpression inputValueParameter = Expression.Parameter(useType);

            //获得使用的属性
            var tempProperty = Expression.Property(realUseContextParameter, useProperty);

            //制作set表达式
            var useSetExpression = Expression.Call(realUseContextParameter, useProperty.SetMethod, inputValueParameter);

            //编译表达式
            return Expression.Lambda<Action<AutoContext, DbSet<T>>>(useSetExpression, inputContextParameter, inputValueParameter).Compile();
        }
    }
}
