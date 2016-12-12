using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovelDownloader.Plugin
{
	/// <summary>
	/// 定义Win32插件需要实现的接口。
	/// </summary>
	public interface IWin32Plugin : IPlugin, IDisposable
	{
		/// <summary>
		/// 加载插件。
		/// </summary>
		void Load();
		/// <summary>
		/// 释放插件。
		/// </summary>
		void Release();
	}
}
