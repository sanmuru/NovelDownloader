using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using NovelDownloader.Token;
using SamLu.Web;

namespace NovelDownloader.Plugin.qidian.com
{
	public abstract class ChapterToken : NDTChapter
	{
		internal static readonly Regex FreeChapterUrlRegex = new Regex(@"http://read.qidian.com/chapter/(?<BookUnicode>\w*)/(?<ChapterUnicode>\w*)", RegexOptions.Compiled);
		internal static readonly Regex VipChapterUrlRegex = new Regex(@"http://vipreader.qidian.com/chapter/(?<BookUnicode>\d*)/(?<ChapterUnicode>\d*)", RegexOptions.Compiled);

		/// <summary>
		/// 获取匹配章节URL的正则表达式。
		/// </summary>
		protected abstract Regex ChapterUrlRegex { get; }

		public override string Type { get; protected set; } = "章";
		
		/// <summary>
		/// 书籍的统一码。
		/// </summary>
		internal string BookUnicode { get; private set; }
		/// <summary>
		/// 章节的统一码。
		/// </summary>
		internal string ChapterUnicode { get; private set; }

		/// <summary>
		/// 使用指定的统一资源标识符初始化<see cref="ChapterToken"/>对象。
		/// </summary>
		/// <param name="uri">指定的统一资源标识符。</param>
		/// <exception cref="ArgumentNullException">
		/// 参数<paramref name="uri"/>的值为<see langword="null"/>。
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// 参数<paramref name="uri"/>不属于免费章节URL或付费章节URL。
		/// </exception>
		protected ChapterToken(Uri uri) : base(uri)
		{
			if (uri == null) throw new ArgumentNullException(nameof(uri));
			string url = uri.ToString();

			Match m = this.ChapterUrlRegex.Match(url);
			if (m.Success)
			{
				this.BookUnicode = m.Groups["BookUnicode"].Value;
				this.ChapterUnicode = m.Groups["ChapterUnicode"].Value;
				this.ChapterUrl = url;
			}
			else
				throw new InvalidOperationException(
					"无法解析URL。",
					new ArgumentOutOfRangeException(nameof(url), url, "URL不符合格式。"));
		}

		/// <summary>
		/// 初始化<see cref="ChapterToken"/>对象。
		/// </summary>
		protected ChapterToken() : base() { }

		protected ChapterToken(string url) : this(new Uri(url)) { }

		protected ChapterToken(string title, string description) : base(title, description) { }

		internal string ChapterUrl { get; set; }

		#region StartCreep
		protected sealed override bool CanStartCreep()
		{
			if ((this.ChapterUrl == null) || !this.ChapterUrlRegex.IsMatch(this.ChapterUrl)) return false;

			try
			{
				HtmlDocument doc = new HtmlDocument();
				doc.LoadHtml(HTML.GetSource(this.ChapterUrl, Encoding.UTF8));

				if (this.CanStartCreepInternal(doc))
				{
					HtmlNode contentElement = doc.DocumentNode.SelectSingleNode("//div[@class='read-content j_readContent']");
					if (contentElement == null) return false;

					this.enumerator =
						HttpUtility.HtmlDecode(contentElement.InnerHtml).Split(new string[] { "<p>" }, StringSplitOptions.RemoveEmptyEntries)
						.Select(p => p.Trim())
						.Where(line => !string.IsNullOrEmpty(line))
						.GetEnumerator();
				}
			}
			catch (Exception e)
			{
				this.OnCreepErrored(this, e);
				return false;
			}

			return this.enumerator != null;
		}

		protected abstract bool CanStartCreepInternal(HtmlDocument doc);

		protected override void StartCreepInternal()
		{
			Match m = this.ChapterUrlRegex.Match(this.ChapterUrl);
			this.BookUnicode = m.Groups["BookUnicode"].Value;
			this.ChapterUnicode = m.Groups["ChapterUnicode"].Value;

			this.hasNext = this.enumerator.MoveNext();
		}
		#endregion

		#region Creep
		protected IEnumerator<string> enumerator;
		private bool hasNext;

		private bool CanCreep()
		{
			return this.hasNext;
		}

		protected override bool CanCreep<TData>(TData data)
		{
			return this.CanCreep();
		}

		private string Creep()
		{
			string line = this.enumerator.Current;
			this.hasNext = this.enumerator.MoveNext();

			return line;
		}

		public override TFetch Creep<TData, TFetch>(TData data)
		{
			if (typeof(TFetch).Equals(typeof(string)))
			{
				return (TFetch)(object)this.Creep();
			}
			else
			{
				if (!typeof(TFetch).Equals(typeof(string)))
					throw new NotSupportedException(
						string.Format("不支持的数据类型{0}", typeof(TFetch).FullName)
					);
			}

			throw new InvalidOperationException();
		}

		protected override bool CreepInternal()
		{
			if (!this.CanCreep()) return false;

			string data = this.Creep();
			if (ImageToken.ImageUrlRegex.IsMatch(data))
			{
				this.Add(new ImageToken(data));
				this.OnCreepFetched(this, string.Format("[插图({0})]", data));
			}
			else
			{
				this.Add(new TextToken(data));
				this.OnCreepFetched(this, data);
			}

			return true;
		}
		#endregion
	}
}
