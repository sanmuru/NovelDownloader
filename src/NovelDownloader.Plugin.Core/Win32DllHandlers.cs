using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovelDownloader.Plugin
{
	/// <summary>
	/// 封装 LoadLibrary 系统 API 的委托类型。
	/// </summary>
	/// <param name="lpDllName">Win32 Dll 文件路径。</param>
	/// <returns>Win32 Dll 文件的模块句柄。</returns>
	public delegate IntPtr LoadLibrary(string lpDllName);
	/// <summary>
	/// 封装 FreeLibrary 系统 API 的委托类型。
	/// </summary>
	/// <param name="hModule">Win32 Dll 文件的模块句柄。</param>
	public delegate void FreeLibrary(IntPtr hModule);
	/// <summary>
	/// 封装 GetProcAddress 系统 API 的委托类型。
	/// </summary>
	/// <param name="hModule">Win32 Dll 文件的模块句柄。</param>
	/// <param name="lpFuncName">Win32 Dll 中导出函数的名称。</param>
	/// <returns>Win32 Dll 中导出函数的地址。</returns>
	public delegate IntPtr GetProcAddress(IntPtr hModule, string lpFuncName);
}
