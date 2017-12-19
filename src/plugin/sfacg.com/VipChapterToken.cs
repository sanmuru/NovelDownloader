using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

namespace SamLu.NovelDownloader.Plugin.sfacg.com
{
	public class VipChapterToken : ChapterToken
	{
		protected override Regex ChapterUrlRegex
		{
			get
			{
				return ChapterToken.VipChapterUrlRegex;
			}
		}

		/// <summary>
		/// 使用指定的统一资源标识符初始化<see cref="VipChapterToken"/>对象。
		/// </summary>
		/// <param name="uri">指定的统一资源标识符。</param>
		public VipChapterToken(Uri uri) : base(uri) { }
		public VipChapterToken() : base() { }
		public VipChapterToken(string url) : base(url) { }
		public VipChapterToken(string title, string description) : base(title, description) { }

		protected override bool CanStartCreepInternal(HtmlDocument doc)
		{
			if (doc.GetElementbyId("ChapterBody").SelectNodes("img | p").Count == 0)
			{
#warning 插入支付代码。
#if false
				System.Diagnostics.Process.Start(string.Format("http://book.sfacg.com/vip/c/{0}/", this.ChapterUnicode));
#endif
                HtmlWeb web = new HtmlWeb();
                doc = web.Load(this.ChapterUrl); // 重新加载文档。

                HtmlNode contentElement = doc.GetElementbyId("ChapterBody");
                if (contentElement.SelectNodes("img | p").Count != 0) return true;
                else
                {
                    this.enumerator = new[]
                    {
                        contentElement,
                        contentElement.SelectSingleNode("//div[@class='pay-bar']/p[@class='text']")
                    }
                        .AsEnumerable()
                        .GetEnumerator();

                    return false;
                }
			}
			else return true;
		}
	}
}
