using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

namespace SamLu.NovelDownloader.Plugin.sfacg.com
{
	public class FreeChapterToken : ChapterToken
	{
		protected override Regex ChapterUrlRegex
		{
			get
			{
				return ChapterToken.FreeChapterUrlRegex;
			}
		}

		/// <summary>
		/// 使用指定的统一资源标识符初始化<see cref="FreeChapterToken"/>对象。
		/// </summary>
		/// <param name="uri">指定的统一资源标识符。</param>
		public FreeChapterToken(Uri uri) : base(uri) { }
		public FreeChapterToken() : base() { }
		public FreeChapterToken(string url) : base(url) { }
		public FreeChapterToken(string title, string description) : base(title, description) { }

		protected override bool CanStartCreepInternal(HtmlDocument doc)
		{
			return true;
		}
	}
}
