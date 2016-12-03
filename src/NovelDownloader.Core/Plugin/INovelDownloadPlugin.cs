using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NovelDownloader.Token;

namespace NovelDownloader.Plugin
{
	public interface INovelDownloadPlugin : IPlugin
	{
		/// <summary>
		/// 获取位于指定的<see cref="Uri"/>位置的小说标签。
		/// </summary>
		/// <param name="uri">指定的统一资源标识符。</param>
		/// <returns></returns>
		NDTBook GetBookToken(Uri uri);
	}
}
