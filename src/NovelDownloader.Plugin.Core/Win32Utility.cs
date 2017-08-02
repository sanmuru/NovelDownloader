using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NovelDownloader.Plugin
{
    internal static class Win32Utility
    {
        #region PtrToObjectOrDefault
        /// <summary>
        /// 将数据从非托管内存块经过指定封送处理到指定类型的托管对象。
        /// </summary>
        /// <typeparam name="T">指定的托管对象的类型。</typeparam>
        /// <param name="ptr">指向非托管内存块的指针。</param>
        /// <param name="marshalFunc">指定封送处理。</param>
        /// <param name="defaultValue">操作前提条件不满足时返回的默认值。此方法的前提条件是 <paramref name="ptr"/> 不为 <see cref="IntPtr.Zero"/> 。</param>
        /// <returns>当 <paramref name="ptr"/> 为 <see cref="IntPtr.Zero"/> 时，返回 <paramref name="defaultValue"/> ；否则返回数据从非托管内存块经过指定封送处理到指定类型的托管对象。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="marshalFunc"/> 的值为 null 。</exception>
        public static T PtrToObjectOrDefault<T>(IntPtr ptr, Func<IntPtr, T> marshalFunc, T defaultValue)
        {
            if (marshalFunc == null) throw new ArgumentNullException(nameof(marshalFunc));

            return Win32Utility.PtrToObjectOrDefault(ptr, (_ptr => _ptr != IntPtr.Zero), marshalFunc, defaultValue);
        }

        /// <summary>
        /// 将数据从非托管内存块经过指定封送处理到指定类型的托管对象。
        /// </summary>
        /// <typeparam name="T">指定的托管对象的类型。</typeparam>
        /// <param name="ptr">指向非托管内存块的指针。</param>
        /// <param name="predicate">检测操作前提条件是否满足。</param>
        /// <param name="marshalFunc">指定封送处理。</param>
        /// <param name="defaultValue">操作前提条件不满足时返回的默认值。</param>
        /// <returns>当检测操作前提条件满足时，返回 <paramref name="defaultValue"/> ；否则返回数据从非托管内存块经过指定封送处理到指定类型的托管对象。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> 的值为 null 。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="marshalFunc"/> 的值为 null 。</exception>
        public static T PtrToObjectOrDefault<T>(IntPtr ptr, Predicate<IntPtr> predicate, Func<IntPtr, T> marshalFunc, T defaultValue)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (marshalFunc == null) throw new ArgumentNullException(nameof(marshalFunc));

            if (predicate(ptr))
                return marshalFunc(ptr);
            else
                return defaultValue;
        }
        #endregion

        #region MarshalDelegateFromFunctionPointer
        /// <summary>
        /// 将导出函数封装到对应委托对象中。
        /// </summary>
        /// <typeparam name="T">对应委托类型。</typeparam>
        /// <param name="func">对应委托对象。</param>
        /// <param name="funcHandle">Win32Dll中对应导出函数的地址。</param>
        /// <param name="errorMessage">当参数<paramref name="funcHandle"/>指定的导出函数的函数指针封装为托管委托对象失败时抛出的异常的信息。</param>
        /// <exception cref="ArgumentException">
        /// <para>泛型类型<typeparamref name="T"/>不是从<see cref="Delegate"/>基类派生而来的委托类型。</para>
        /// <para>泛型类型<typeparamref name="T"/>是<see cref="Delegate"/>基类自身。</para>
        /// </exception>
        /// <exception cref="Win32Exception">
        /// 将参数<paramref name="funcHandle"/>指定的导出函数的函数指针封装为托管委托对象时失败。
        /// </exception>
        public static void MarshalDelegateFromFunctionPointer<T>(out T func, IntPtr funcHandle, string errorMessage = null) where T : class
        {
            if (!typeof(Delegate).IsAssignableFrom(typeof(T)) || typeof(T).Equals(typeof(Delegate)))
                throw new ArgumentException(string.Format("泛型类型{0}必须为委托类型且不能为{1}", typeof(T).FullName, typeof(Delegate).FullName), nameof(T));

            T dFunc = Marshal.GetDelegateForFunctionPointer(funcHandle, typeof(T)) as T;
            if (dFunc == null) throw new Win32Exception(errorMessage ?? "导出函数的函数指针封装为托管委托对象失败。");

            func = dFunc;
        }

        /// <summary>
        /// 将导出函数封装到对应委托对象中。
        /// </summary>
        /// <typeparam name="T">对应委托类型。</typeparam>
        /// <param name="func">对应委托对象。</param>
        /// <param name="funcName">导出函数名称。</param>
        /// <param name="funcHandle">Win32Dll中对应导出函数的地址。</param>
        /// <param name="errorMessage">当参数<paramref name="funcHandle"/>指定的导出函数的函数指针封装为托管委托对象失败时抛出的异常的信息。</param>
        /// <exception cref="ArgumentException">
        /// <para>泛型类型<typeparamref name="T"/>不是从<see cref="Delegate"/>基类派生而来的委托类型。</para>
        /// <para>泛型类型<typeparamref name="T"/>是<see cref="Delegate"/>基类自身。</para>
        /// </exception>
        internal static void MarshalDelegateFromFunctionPointer<T>(out T func, string funcName, IntPtr funcHandle, string errorMessage = null) where T : class
        {
            if (!typeof(Delegate).IsAssignableFrom(typeof(T)) || typeof(T).Equals(typeof(Delegate)))
                throw new ArgumentException(string.Format("泛型类型{0}必须为委托类型且不能为{1}", typeof(T).FullName, typeof(Delegate).FullName), nameof(T));

            Win32Utility.MarshalDelegateFromFunctionPointer(out func, funcHandle, errorMessage ?? string.Format("导出函数{0}的函数指针封装为托管委托对象失败。", funcName));
        }

        /// <summary>
        /// 将导出函数封装到对应委托对象中。
        /// </summary>
        /// <typeparam name="T">对应委托类型。</typeparam>
        /// <param name="func">对应委托对象。</param>
        /// <param name="getProcAddressFunc">封装GetProcAddress系统API的委托对象。</param>
        /// <param name="moduleHandle">导出函数所属的DLL模块句柄。</param>
        /// <param name="funcName">导出函数名称。</param>
        /// <param name="errorMessage">当无法获取导出函数地址时抛出的异常的信息。</param>
        /// <exception cref="ArgumentException">
        /// <para>泛型类型<typeparamref name="T"/>不是从<see cref="Delegate"/>基类派生而来的委托类型。</para>
        /// <para>泛型类型<typeparamref name="T"/>是<see cref="Delegate"/>基类自身。</para>
        /// </exception>
        /// <exception cref="Win32Exception">
        /// <para>无法获取参数<paramref name="funcName"/>指定的导出函数的地址。</para>
        /// <param>若指定了参数<paramref name="errorMessage"/>的值且不为<see langword="null"/>，则异常的<see cref="Exception.Message"/>属性值为参数<paramref name="errorMessage"/>的值。否则，为默认值。</param>
        /// </exception>
        public static void MarshalDelegateFromFunctionPointer<T>(out T func, GetProcAddress getProcAddressFunc, IntPtr moduleHandle, string funcName, string errorMessage = null) where T : class
        {
            if (!typeof(Delegate).IsAssignableFrom(typeof(T)) || typeof(T).Equals(typeof(Delegate)))
                throw new ArgumentException(string.Format("泛型类型{0}必须为委托类型且不能为{1}", typeof(T).FullName, typeof(Delegate).FullName), nameof(T));

            IntPtr pFunc = getProcAddressFunc(moduleHandle, funcName);
            if (pFunc == IntPtr.Zero) throw new Win32Exception(errorMessage ?? string.Format("无法获取导出函数{0}的地址。", funcName));

            Win32Utility.MarshalDelegateFromFunctionPointer(out func, funcName, pFunc);
        }
        #endregion
    }
}
