using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NovelDownloader.Plugin
{
	/// <summary>
	/// 插件管理器。
	/// </summary>
	public class PluginManager : IPluginManager
	{
		/// <summary>
		/// 已加载的插件字典。
		/// </summary>
		public IDictionary<Guid, IPlugin> Plugins { get; private set; } = new Dictionary<Guid, IPlugin>();

		/// <summary>
		/// 初始化<see cref="PluginManager"/>对象。
		/// </summary>
		public PluginManager() { }

		/// <summary>
		/// 从指定路径的文件中加载插件，并返回所有加载的插件。
		/// </summary>
		/// <param name="pluginFileName">指定路径的文件。</param>
		/// <returns>
		/// <para>如果加载成功，则返回指定路径的文件中加载的插件。</para>
		/// <para>如果未加载成功，则返回<see langword="null"/>。</para>
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// 参数<paramref name="pluginFileName"/>为<see langword="null"/>。
		/// </exception>
		/// <exception cref="FileNotFoundException">
		/// 参数<paramref name="pluginFileName"/>指定的文件路径非法或无效。
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// 参数<paramref name="pluginFileName"/>指定的路径的文件不是.NET程序集。
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// 无法获取插件的实例。
		/// </exception>
		public IEnumerable<IPlugin> Load(string pluginFileName)
		{
			if (pluginFileName == null) throw new ArgumentNullException(nameof(pluginFileName));
			if (!File.Exists(pluginFileName)) throw new FileNotFoundException("无法从指定文件中加载插件。", pluginFileName);

			Assembly pluginAssembly = Assembly.LoadFrom(pluginFileName);
			var pluginTypes =
				from type in pluginAssembly.GetTypes()
				where typeof(IPlugin).IsAssignableFrom(type)
				where !type.IsAbstract
				select type;

			foreach (var pluginType in pluginTypes)
			{
				IPlugin plugin = pluginAssembly.CreateInstance(pluginType.FullName) as IPlugin;
				if (plugin == null) throw new InvalidOperationException("无法获取插件的实例。");

				if (!this.Plugins.ContainsKey(plugin.Guid)) this.Plugins.Add(plugin.Guid, plugin);
				yield return plugin;
			}
		}

		/// <summary>
		/// 释放指定的插件对象。
		/// </summary>
		/// <param name="plugin">指定的插件对象</param>
		public void Release(IPlugin plugin)
		{
			if (this.Plugins.ContainsKey(plugin.Guid))
				this.Plugins.Remove(plugin.Guid);
		}
	}
}
