using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NovelDownloader.Token;

namespace NovelDownloader.Plugin.qidian.com
{
	public class QiDian_NovelDownloader : INovelDownloadPlugin
	{
		/// <summary>
		/// 起点中文网的主域名。
		/// </summary>
		public static readonly Uri HostUri = new Uri("http://www.qidian.com", UriKind.Absolute);
		public static readonly Uri BookHostUri = new Uri("http://book.qidian.com", UriKind.Absolute);
		public static readonly Uri ReadHostUri = new Uri("http://read.qidian.com", UriKind.Absolute);
		public static readonly Uri VipReaderHostUri = new Uri("http://vipreader.qidian.com", UriKind.Absolute);

		public string Name { get; private set; } = "qidian.com";

		public string DisplayName { get; private set; } = "起点中文网";

		public Version MinVersion { get; private set; } = Version.MinVersion;

		public Version Version { get; private set; } = new Version(1, 0, 0, DateTime.Now.ToString("yyyyMMdd"), Version.BetaVersion);

		public string Description { get; private set; } = "提供起点中文网(qidian.com)小说下载。";

		internal static readonly Guid _guid = new Guid("b00222b7-75b4-4914-9452-c9d8bfc279f3");
		public Guid Guid
		{
			get
			{
				return QiDian_NovelDownloader._guid;
			}
		}

		/// <summary>
		/// 获取指定书籍编号的<see cref="BookToken"/>对象。
		/// </summary>
		/// <param name="bookUnicode">指定的书籍编号。</param>
		/// <returns>指定书籍编号的<see cref="BookToken"/>对象。</returns>
		public NDTBook GetBookToken(ulong bookUnicode)
		{
			return this.GetBookToken(new Uri(string.Format(@"http://www.wenku8.com/book/{0}.htm", bookUnicode)));
		}

		/// <summary>
		/// 获取位于指定URL的<see cref="BookToken"/>对象。
		/// </summary>
		/// <param name="url">指定的URL。</param>
		/// <returns>位于指定URL的<see cref="BookToken"/>对象。</returns>
		public NDTBook GetBookToken(string url)
		{
			if (BookToken.BookUrlRegex.IsMatch(url))
				return this.GetBookToken(new Uri(url));
			else
			{
				throw new InvalidOperationException(
					 "无法解析URL。",
					 new ArgumentOutOfRangeException(nameof(url), url, "URL不符合格式。"));
			}
		}

		/// <summary>
		/// 获取指定统一资源标识符的<see cref="BookToken"/>对象。
		/// </summary>
		/// <param name="uri">指定的统一资源标识符。</param>
		/// <returns>指定统一资源标识符的<see cref="BookToken"/>对象。</returns>
		public NDTBook GetBookToken(Uri uri)
		{
			return new BookToken(uri);
		}

		/// <summary>
		/// 尝试获取位于指定的<see cref="Uri"/>位置的小说标签。
		/// </summary>
		/// <param name="uri">指定的统一资源标识符。</param>
		/// <param name="bookToken">位于指定的<see cref="Uri"/>位置的小说标签。</param>
		/// <returns>是否获取成功。</returns>
		public bool TryGetBookToken(Uri uri, out NDTBook bookToken)
		{
			try
			{
				bookToken = this.GetBookToken(uri);

				return (bookToken != null);
			}
			catch (Exception)
			{
				bookToken = null;

				return false;
			}
		}
	}
}
