using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamLu.NovelDownloader.Plugin
{
	/// <summary>
	/// 定义插件需要实现的接口。
	/// </summary>
	public interface IPlugin
	{
		/// <summary>
		/// 获取插件的名字。
		/// </summary>
		string Name { get; }
		/// <summary>
		/// 获取插件显示在插件管理器中的名字。
		/// </summary>
		string DisplayName { get; }
		/// <summary>
		/// 获取插件的版本。
		/// </summary>
		Version Version { get; }
		/// <summary>
		/// 获取插件支持的最小版本。
		/// </summary>
		Version MinVersion { get; }
		/// <summary>
		/// 获取插件的说明。
		/// </summary>
		string Description { get; }

		/// <summary>
		/// 获取插件的全局唯一标识符。
		/// </summary>
		Guid Guid { get; }

        /// <summary>
        /// 插件初始化。
        /// </summary>
        void Initialize();
	}
}
