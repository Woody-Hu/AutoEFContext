using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AutoEFContext
{
    /// <summary>
    /// 线程安全类型委托封装字典 单例模式
    /// </summary>
    internal class ConcurrentTypeDelDic
    {
        /// <summary>
        /// 内部读写锁
        /// </summary>
        private ReaderWriterLockSlim m_useLocker = new ReaderWriterLockSlim();

        /// <summary>
        /// 内部字典
        /// </summary>
        private Dictionary<Type, EFDelPacker> m_coreDic = new Dictionary<Type, EFDelPacker>();

        /// <summary>
        /// 私有构造
        /// </summary>
        private ConcurrentTypeDelDic()
        {

        }

        internal static ConcurrentTypeDelDic GetDic()
        {
            return InnerConcurrentTypeDelDic.m_singleTag;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="inputType"></param>
        /// <param name="inputPacker"></param>
        internal void Add(Type inputType,EFDelPacker inputPacker)
        {
            try
            {
                m_useLocker.EnterWriteLock();
                m_coreDic.Add(inputType, inputPacker);
            }
            finally
            {
                m_useLocker.ExitWriteLock();
            } 
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="inputKey"></param>
        /// <returns></returns>
        internal EFDelPacker Get(Type inputKey)
        {
            try
            {
                m_useLocker.EnterReadLock();
                return m_coreDic[inputKey];
            }
            finally
            {
                m_useLocker.ExitReadLock();
            }
        }

        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="inputKey"></param>
        /// <returns></returns>
        internal bool Contains(Type inputKey)
        {
            try
            {
                m_useLocker.EnterReadLock();
                return m_coreDic.ContainsKey(inputKey);
            }
            finally
            {
                m_useLocker.ExitReadLock();
            }
        }

        /// <summary>
        /// 内部类单例模式实现
        /// </summary>
        private class InnerConcurrentTypeDelDic
        {
            internal static ConcurrentTypeDelDic m_singleTag = new ConcurrentTypeDelDic();
        }

    }
}
