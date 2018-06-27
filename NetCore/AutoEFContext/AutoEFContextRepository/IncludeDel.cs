using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoEFContextRepository
{
    /// <summary>
    /// 使用的Include机制委托
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="inputSource"></param>
    /// <returns></returns>
    public delegate IQueryable<T> IncludeDel<T>(IQueryable<T> inputSource);
}
