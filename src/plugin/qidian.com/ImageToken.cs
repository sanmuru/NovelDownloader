using SamLu.NovelDownloader.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SamLu.NovelDownloader.Plugin.qidian.com
{
	public class ImageToken : NDTImage
	{
		internal static readonly Regex ImageUrlRegex = new Regex(@"http(s)?://pic.wenku8.com/pictures/\d*/(?<BookUnicode>\d*)/(?<ChapterUnicode>\d*)/(?<ImageFileName>(?<ImageUnicode>\d*).(\w*))", RegexOptions.Compiled);

		/// <summary>
		/// 图片的统一码。
		/// </summary>
		internal ulong ImageUnicode { get; private set; }

		/// <summary>
		/// 图片的文件名。
		/// </summary>
		internal string ImageFileName { get; private set; }

		/// <summary>
		/// 初始化<see cref="ImageToken"/>对象。
		/// </summary>
		public ImageToken() : base() { }

		/// <summary>
		/// 使用指定的统一资源标识符初始化<see cref="ImageToken"/>对象。
		/// </summary>
		/// <param name="uri">指定的统一资源标识符。</param>
		public ImageToken(Uri uri) : base(uri)
		{
			if (uri == null) throw new ArgumentNullException(nameof(uri));
			string url = uri.ToString();

			Match m = ImageToken.ImageUrlRegex.Match(url);
			if (m.Success)
			{
				this.ImageUnicode = ulong.Parse(m.Groups["ImageUnicode"].Value);
				this.ImageFileName = m.Groups["ImageFileName"].Value;
			}
			else
				throw new InvalidOperationException(
					"无法解析URL。",
					new ArgumentOutOfRangeException(nameof(url), url, "URL不符合格式。"));
		}

		/// <summary>
		/// 使用指定的URL初始化<see cref="ImageToken"/>对象。
		/// </summary>
		/// <param name="url">指定的URL。</param>
		public ImageToken(string url) : this(new Uri(url)) { }
	}
}
