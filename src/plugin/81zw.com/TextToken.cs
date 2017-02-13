using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NovelDownloader.Token;

namespace NovelDownloader.Plugin._81zw.com
{
	public class TextToken : NDTText
	{

		/// <summary>
		/// 初始化<see cref="NDTChapter"/>对象。
		/// </summary>
		public TextToken() : base() { }

		/// <summary>
		/// 使用指定的内容初始化<see cref="TextToken"/>对象。
		/// </summary>
		/// <param name="content">指定的内容</param>
		public TextToken(string content) : base(content) { }
	}
}
