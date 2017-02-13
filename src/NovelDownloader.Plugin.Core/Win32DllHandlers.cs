using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovelDownloader.Plugin
{
	/// <summary>
	/// 封装LoadLibrary系统API的委托类型。
	/// </summary>
	/// <param name="lpDllName">Win32Dll文件路径。</param>
	/// <returns>Win32Dll文件的模块句柄。</returns>
	public delegate IntPtr LoadLibrary(string lpDllName);
	/// <summary>
	/// 封装FreeLibrary系统API的委托类型。
	/// </summary>
	/// <param name="hModule">Win32Dll文件的模块句柄。</param>
	public delegate void FreeLibrary(IntPtr hModule);
	/// <summary>
	/// 封装GetProcAddress系统API的委托类型。
	/// </summary>
	/// <param name="hModule">Win32Dll文件的模块句柄。</param>
	/// <param name="lpFuncName">Win32Dll中导出函数的名称。</param>
	/// <returns>Win32Dll中导出函数的地址。</returns>
	public delegate IntPtr GetProcAddress(IntPtr hModule, string lpFuncName);
}
