using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovelDownloader.Plugin
{
	/// <summary>
	/// 定义插件管理器需要实现的接口。
	/// </summary>
	public interface IPluginManager
	{
		/// <summary>
		/// 加载插件。
		/// </summary>
		/// <param name="pluginFileName">插件所在文件路径。</param>
		/// <returns>指定文件中的所有插件对象的集合。</returns>
		IEnumerable<IPlugin> Load(string pluginFileName);
		/// <summary>
		/// 释放指定的插件对象。
		/// </summary>
		/// <param name="plugin">指定的插件对象</param>
		void Release(IPlugin plugin);
	}
}
