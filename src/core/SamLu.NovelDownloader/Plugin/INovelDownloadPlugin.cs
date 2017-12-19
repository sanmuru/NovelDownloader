using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SamLu.NovelDownloader.Token;

namespace SamLu.NovelDownloader.Plugin
{
	/// <summary>
    /// 定义小说下载插件需要实现的接口。
    /// </summary>
    public interface INovelDownloadPlugin : IPlugin
	{
		/// <summary>
		/// 尝试获取位于指定的<see cref="Uri"/>位置的小说标签。
		/// </summary>
		/// <param name="uri">指定的统一资源标识符。</param>
		/// <param name="bookToken">位于指定的<see cref="Uri"/>位置的小说标签。</param>
		/// <returns>是否获取成功。</returns>
		bool TryGetBookToken(Uri uri, out NDTBook bookToken);
	}
}
