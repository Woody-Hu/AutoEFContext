using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AutoEFContextRepository
{
    /// <summary>
    /// 表达式工具
    /// </summary>
    internal static class ExpressionUtility
    {
        /// <summary>
        /// 线程安全字典
        /// </summary>
        private static ConcurrentDictionary<string, object> useFuncDic = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// 排序字符串
        /// </summary>
        private const string m_OrderByStr = "OrderBy";

        /// <summary>
        /// 逆序排序字符串
        /// </summary>
        private const string m_OrderByDescendingStr = "OrderByDescending";

        /// <summary>
        /// 可查询类型
        /// </summary>
        private static Type m_UseQueryableType = typeof(Queryable);

        /// <summary>
        /// 获取Order表达式
        /// </summary>
        /// <typeparam name="Y">使用的泛型类型</typeparam>
        /// <param name="inputPropertyName">使用的属性名称</param>
        /// <param name="ifASC">是否升序</param>
        /// <returns>使用的表达式</returns>
        internal static Func<IQueryable<Y>, IOrderedQueryable<Y>> GetOrderByFunc<Y>(string inputPropertyName, bool ifASC = true)
            where Y : class
        {
            if (string.IsNullOrWhiteSpace(inputPropertyName))
            {
                return null;
            }

            //获得键
            string useKeyString = typeof(Y).FullName + "." + inputPropertyName + "." + ifASC.ToString();

            object useValue = null;

            //若没有出现过
            if (false == useFuncDic.TryGetValue(useKeyString, out useValue))
            {
                useValue = MakeOrderByFunc<Y>(inputPropertyName, ifASC);
                useFuncDic.TryAdd(useKeyString, useValue);
            }

            return useValue as Func<IQueryable<Y>, IOrderedQueryable<Y>>;

        }

        /// <summary>
        /// 制作一个排序表单式树
        /// </summary>
        /// <typeparam name="Y"></typeparam>
        /// <param name="inputPropertyName"></param>
        /// <param name="ifASC"></param>
        /// <returns></returns>
        private static Func<IQueryable<Y>, IOrderedQueryable<Y>> MakeOrderByFunc<Y>(string inputPropertyName, bool ifASC = true)
        {
            if (string.IsNullOrWhiteSpace(inputPropertyName))
            {
                return null;
            }

            var tempType = typeof(Y);

            var allProperties = tempType.GetProperties();

            PropertyInfo useProperty = null;

            //寻找属性名相同的
            foreach (var oneProperty in allProperties)
            {
                if (inputPropertyName.ToLower().Equals(oneProperty.Name.ToLower()))
                {
                    useProperty = oneProperty;
                    break;
                }
            }

            if (null == useProperty)
            {
                return null;
            }

            //获取输入的参数

            ParameterExpression useLambdaInputParameter = Expression.Parameter(typeof(Y));


            var usePropertyExpression = Expression.Property(useLambdaInputParameter, useProperty);

            //获取属性执行表达式
            var useSelector = Expression.Lambda(usePropertyExpression, useLambdaInputParameter);

            //获取OrderBy表单式
            ParameterExpression useInputExpression = Expression.Parameter(typeof(IQueryable<Y>));

            var useType = m_UseQueryableType;

            //获取方法名
            var useMethodName = ifASC ? m_OrderByStr : m_OrderByDescendingStr;

            //获取泛型方法
            var useDefineMethod = useType.GetMethods().Where(m => m.Name == useMethodName && m.IsGenericMethodDefinition)
                 .Where(m =>
                 {
                     var parameters = m.GetParameters().ToList();
                     return parameters.Count == 2;
                 }).Single();

            //制备泛型方法
            var useMethod = useDefineMethod.MakeGenericMethod(tempType, useProperty.PropertyType);

            var returnValue = Expression.Call(useMethod, useInputExpression, useSelector);

            return Expression.Lambda<Func<IQueryable<Y>, IOrderedQueryable<Y>>>(returnValue, useInputExpression).Compile();
        }
    }
}
