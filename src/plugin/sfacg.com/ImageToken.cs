using SamLu.NovelDownloader.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SamLu.NovelDownloader.Plugin.sfacg.com
{
	public class ImageToken : NDTImage
    {
		/// <summary>
		/// 图片的文件名。
		/// </summary>
		internal Uri ImageUri { get; private set; }

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

            this.ImageUri = uri;
		}

		/// <summary>
		/// 使用指定的URL初始化<see cref="ImageToken"/>对象。
		/// </summary>
		/// <param name="url">指定的URL。</param>
		public ImageToken(string url) : this(new Uri(url)) { }
	}
}
