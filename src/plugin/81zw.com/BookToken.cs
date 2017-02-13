using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using NovelDownloader.Token;
using SamLu.Web;

namespace NovelDownloader.Plugin._81zw.com
{
	public class BookToken:NDTBook
	{
		internal static readonly Regex BookUrlRegex = new Regex(@"http://www.81zw.com/book/(?<BookUnicode>\d*)/(index.html)?", RegexOptions.Compiled);

		private string source;
		private string bookCategoryHTML;

		public override string Type { get; protected set; } = "书籍";

		/// <summary>
		/// 书籍的统一码。
		/// </summary>
		internal ulong BookUnicode { get; private set; }

		/// <summary>
		/// 书籍的URL。
		/// </summary>
		internal string BookUrl { get; private set; }

		/// <summary>
		/// 初始化<see cref="BookToken"/>对象。
		/// </summary>
		public BookToken() : base() { }

		/// <summary>
		/// 使用指定的统一资源标识符初始化<see cref="BookToken"/>对象。
		/// </summary>
		/// <param name="uri">指定的统一资源标识符。</param>
		/// <exception cref="ArgumentNullException">
		/// 参数<paramref name="uri"/>的值为<see langword="null"/>。
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// 参数<paramref name="uri"/>不属于书籍URL或目录URL。
		/// </exception>
		public BookToken(Uri uri) : this(null, null)
		{
			string url = uri.ToString();
			if (url == null) throw new ArgumentNullException(nameof(url));

			Match m = BookToken.BookUrlRegex.Match(url);
			if (m.Success)
			{
				this.BookUnicode = ulong.Parse(m.Groups["BookUnicode"].Value);
				this.BookUrl = url;
			}
			else
				throw new InvalidOperationException(
					"无法解析URL。",
					new ArgumentOutOfRangeException(nameof(url), url, "URL不符合格式。"));



			this.source = HTML.GetSource(this.BookUrl, Encoding.GetEncoding("GBK"));

			Match head_match = Regex.Match(source, @"<head>(?<HeadContent>[\s\S]*?)</head>", RegexOptions.Compiled);
			if (!head_match.Success) throw new InvalidOperationException("无法抓取信息。");

			string head = head_match.Groups["HeadContent"].Value;

			MatchCollection meta_matches = Regex.Matches(head, @"<meta property=""(?<MetaProperty>[\s\S]*?)"" content=""(?<MetaContent>[\s\S]*?)""/>", RegexOptions.Compiled);
			if (meta_matches.Count == 0) throw new InvalidOperationException("无法抓取信息。");

			foreach (Match meta_match in meta_matches)
			{
				if (!meta_match.Success) throw new InvalidOperationException("无法抓取信息。");

				switch (meta_match.Groups["MetaProperty"].Value)
				{
					case "og:novel:book_name":
						this.Title = HttpUtility.HtmlDecode(meta_match.Groups["MetaContent"].Value);
						break;
					case "og:novel:author":
						this.Author = HttpUtility.HtmlDecode(meta_match.Groups["MetaContent"].Value);
						break;
					case "og:image":
						this.Cover = new Uri(HttpUtility.HtmlDecode(meta_match.Groups["MetaContent"].Value), UriKind.Absolute);
						break;
					case "og:description":
						this.Description = HttpUtility.HtmlDecode(meta_match.Groups["MetaContent"].Value.Replace("<br />", Environment.NewLine)).Trim();
						break;
				}
			}
		}

		/// <summary>
		/// 使用指定的URL初始化<see cref="BookToken"/>对象。
		/// </summary>
		/// <param name="url">指定的URL。</param>
		public BookToken(string url) : this(new Uri(url)) { }

		public BookToken(string title, string description) : base(title, description) { }

		public BookToken(string title, string description, string author) : this(title, description)
		{
			this.Author = author;
		}

		#region StartCreep
		protected override bool CanStartCreep()
		{
			if ((this.BookUrl == null) || !BookToken.BookUrlRegex.IsMatch(this.BookUrl))
				return false;

			try
			{
				Match m = Regex.Match(this.source, @"<div id=""list"">(?<BookCategoryHTML>[\s\S]*?)</div>", RegexOptions.Compiled);
				if (!m.Success) return false;

				this.bookCategoryHTML = m.Groups["BookCategoryHTML"].Value;
			}
			catch (Exception e)
			{
#if DEBUG
				throw;
#endif
				this.OnCreepErrored(this, e);
				return false;
			}

			return true;
		}

		protected override void StartCreepInternal()
		{
			const string REGEX = @"<a href=""(?<ChapterRelativeUrl>(?<ChapterUnicode>\d*?).html)"">(?<ChapterTitle>[\s\S]*?)</a>";
			this.chapterRegex = new Regex(REGEX, RegexOptions.Compiled);
		}
		#endregion

		#region Creep
		int index;
		Regex chapterRegex;
		Match nextMatch;
		private bool CanCreep(int index)
		{
			nextMatch = this.chapterRegex.Match(this.bookCategoryHTML, index);
			return nextMatch.Success;
		}

		protected override bool CanCreep<TData>(TData data)
		{
			if (typeof(TData).Equals(typeof(int)))
				return this.CanCreep((int)(object)data);
			else
				throw new NotSupportedException(
					string.Format("不支持的数据类型{0}", typeof(TData).FullName),
					new ArgumentException(
						string.Format("参数的类型为{1}。", typeof(TData).FullName),
						nameof(data)
					)
				);
		}

		private string[] Creep()
		{
			this.index = this.nextMatch.Index + this.nextMatch.Length;

			return new string[2]
			{
				this.nextMatch.Groups["ChapterTitle"].Value,
				new Uri(_81ZW_NovelDownloader.HostUri, new Uri(string.Format("book/{0}/{1}", this.BookUnicode, this.nextMatch.Groups["ChapterRelativeUrl"].Value), UriKind.Relative)).ToString()
			};
		}

		public override TFetch Creep<TData, TFetch>(TData data)
		{
			if (typeof(TFetch).Equals(typeof(string)))
			{
				string chapter_uri = this.Creep()[1];
				return (TFetch)(object)chapter_uri;
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
			if (!this.CanCreep(this.index)) return false;

			string[] data = this.Creep();
			if (data != null && data.Length == 2)
			{
				this.Add(new ChapterToken(new Uri(data[1])) { Title = data[0] });
				this.OnCreepFetched(this, data[0]);
			}
			return true;
		}
		#endregion
	}
}
