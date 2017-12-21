using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using SamLu.NovelDownloader.Token;

namespace SamLu.NovelDownloader.Plugin.sfacg.com
{
	public abstract class ChapterToken : NDTChapter
	{
		internal static readonly Regex FreeChapterUrlRegex = new Regex(@"http(s)?://book.sfacg.com/Novel/(?<BookUnicode>\d*)/(?<VolumeUnicode>\d*)/(?<ChapterUnicode>\d*)/", RegexOptions.Compiled);
		internal static readonly Regex VipChapterUrlRegex = new Regex(@"http(s)?://book.sfacg.com/vip/c/(?<ChapterUnicode>\d*)/", RegexOptions.Compiled);

		/// <summary>
		/// 获取匹配章节URL的正则表达式。
		/// </summary>
		protected abstract Regex ChapterUrlRegex { get; }

		public override string Type { get; protected set; } = "章";

        /// <summary>
        /// 书籍的统一码。
        /// </summary>
        internal ulong BookUnicode { get; set; }
        /// <summary>
        /// 书籍的统一码。
        /// </summary>
        internal ulong VolumeUnicode { get; set; }

        /// <summary>
        /// 章节的统一码。
        /// </summary>
        internal ulong ChapterUnicode { get; private set; }

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
				this.ChapterUnicode = ulong.Parse(m.Groups["ChapterUnicode"].Value);
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
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = web.Load(this.ChapterUrl);

				if (this.CanStartCreepInternal(document))
				{
                    HtmlNode contentElement = document.GetElementbyId("ChapterBody");
					if (contentElement == null) return false;

                    this.enumerator = contentElement.SelectNodes("img | p").AsEnumerable().GetEnumerator();
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
			this.hasNext = this.enumerator.MoveNext();
		}
		#endregion

		#region Creep
		protected IEnumerator<HtmlNode> enumerator;
		private bool hasNext;

		private bool CanCreep()
		{
			return this.hasNext;
		}

		protected override bool CanCreep<TData>(TData data)
		{
			return this.CanCreep();
		}

		private HtmlNode Creep()
		{
			HtmlNode line = this.enumerator.Current;
			this.hasNext = this.enumerator.MoveNext();

			return line;
		}

		public override TFetch Creep<TData, TFetch>(TData data)
		{
            if (typeof(TFetch).Equals(typeof(string)))
            {
                HtmlNode node = this.Creep();
                switch (node.Name)
                {
                    case "div":
                        return (TFetch)(object)node.InnerText.Trim();
                    case "img":
                        return (TFetch)(object)node.GetAttributeValue("href", string.Empty);
                    case "p":
                        return (TFetch)(object)node.InnerText.Trim();
                }
            }
            else if (typeof(TFetch).Equals(typeof(HtmlNode)))
            {
                return (TFetch)(object)this.Creep();
            }
            else
            {
                throw new NotSupportedException(
                    string.Format("不支持的数据类型{0}", typeof(TFetch).FullName)
                );
            }

			throw new InvalidOperationException();
		}

        protected override bool CreepInternal()
        {
            if (!this.CanCreep()) return false;

            HtmlNode node = this.Creep();
            switch (node.Name)
            {
                case "img":
                    string src = node.GetAttributeValue("src", null);
                    this.Add(new ImageToken(src));
                    this.OnCreepFetched(this, $"[插图({src})]");
                    break;
                case "div":
                case "p":
                    string line = node.InnerText.Trim();
                    this.Add(new TextToken(line));
                    this.OnCreepFetched(this, line);
                    break;
            }

			return true;
		}
		#endregion
	}
}
