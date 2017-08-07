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
	}
}
