using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovelDownloader.Plugin
{
	/// <summary>
	/// 定义Win32插件管理器需要实现的接口。
	/// </summary>
	public interface IWin32PluginManager : IPluginManager, IDisposable
	{
		/// <summary>
		/// 封装LoadLibrary系统API的委托。
		/// </summary>
		LoadLibrary LoadLibraryFunc { get; }
		/// <summary>
		/// 封装FreeLibrary系统API的委托。
		/// </summary>
		FreeLibrary FreeLibraryFunc { get; }
		/// <summary>
		/// 封装GetProcAddress系统API的委托。
		/// </summary>
		GetProcAddress GetProcAddressFunc { get; }
	}
}
