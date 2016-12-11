using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovelDownloader.Plugin
{
	public interface IWin32PluginLoader : IPluginLoader, IDisposable
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
