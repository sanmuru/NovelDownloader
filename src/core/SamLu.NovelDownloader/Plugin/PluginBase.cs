using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamLu.NovelDownloader.Plugin
{
	/// <summary>
	/// 所有插件的虚拟基类。
	/// </summary>
	public abstract class PluginBase : IPlugin
    {
        /// <summary>
        /// 获取插件的名字。
        /// </summary>
        public abstract string Name { get; }
		/// <summary>
		/// 获取插件显示在插件管理器中的名字。
		/// </summary>
        public abstract string DisplayName { get; }
        /// <summary>
        /// 获取插件的描述。
        /// </summary>
        public abstract string Description { get; }
        /// <summary>
        /// 获取插件的版本。
        /// </summary>
        public abstract Version Version { get; }
        /// <summary>
        /// 获取插件支持的最小版本。
        /// </summary>
        public abstract Version MinVersion { get; }

        /// <summary>
        /// 获取插件的全局唯一标识符。
        /// </summary>
        public abstract Guid Guid { get; }
	}
}
