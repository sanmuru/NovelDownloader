using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovelDownloader.Plugin
{
	/// <summary>
	/// 所有插件的虚拟基类。
	/// </summary>
	public abstract class PluginBase : IPlugin
	{
		public abstract string Description { get; }
		public abstract string DisplayName { get; }
		public abstract Guid Guid { get; }
		public abstract Version MinVersion { get; }
		public abstract string Name { get; }
		public abstract Version Version { get; }
	}
}
