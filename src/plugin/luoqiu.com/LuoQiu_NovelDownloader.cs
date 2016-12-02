using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NovelDownloader.Token;

namespace NovelDownloader.Plugin.luoqiu.com
{
	public class LuoQiu_NovelDownloader : INovelDownloadPlugin
	{
		/// <summary>
		/// 落秋中文小说网的主域名。
		/// </summary>
		public static readonly Uri HostUri = new Uri("http://www.luoqiu.com");

		public string Name { get; private set; } = "luoqiu.com";

		public string DisplayName { get; private set; } = "落秋中文小说下载";

		public Version MinVersion { get; private set; } = Version.MinVersion;

		public Version Version { get; private set; } = new Version(0, 0);

		public string Description { get; private set; } = "提供落秋中文(luoqiu.com)小说下载。";

		public Guid Guid
		{
			get
			{
				return new Guid("ef191ee4-fbb1-4a0c-8a54-37b829107cbf");
			}
		}
		
		public NDTBook GetBookToken(Uri uri)
		{
			return new BookToken(uri);
		}
	}
}
