using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

namespace SamLu.NovelDownloader.Plugin.qidian.com
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
			if (doc.DocumentNode.SelectSingleNode("//div[@class='vip-limit-wrap']") != null)
			{
#warning 插入支付代码。
#if false
				System.Diagnostics.Process.Start(string.Format("http://vipreader.qidian.com/chapter/{0}/{1}", this.BookUnicode, this.ChapterUnicode));
#endif
                HtmlWeb web = new HtmlWeb();
                doc = web.Load(this.ChapterUrl); // 重新加载文档。
				
				if (doc.DocumentNode.SelectSingleNode("//div[@class='vip-limit-wrap']") == null) return true;
				else
				{
					HtmlNode contentElement = doc.DocumentNode.SelectSingleNode("//div[@class='read-content j_readContent']");
					if (contentElement != null)
					{
						this.enumerator =
							HttpUtility.HtmlDecode(contentElement.InnerHtml).Split(new string[] { "<p>" }, StringSplitOptions.RemoveEmptyEntries)
							.Select(p => p.Trim())
							.Where(line => !string.IsNullOrEmpty(line))
							.Concat(new[] { string.Empty, "这是VIP章节，需要订阅后阅读。" })
							.GetEnumerator();
					}

					return false;
				}
			}
			else return true;
		}
	}
}
