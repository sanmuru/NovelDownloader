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
    }
}
