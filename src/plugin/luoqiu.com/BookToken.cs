using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NovelDownloader.Token;
using SamLu.Web;

namespace NovelDownloader.Plugin.luoqiu.com
{
	public class BookToken : NDTBook
	{
		internal static readonly Regex BookUrlRegex = new Regex(@"http://www.luoqiu.com/book/(?<BookUnicode>\d*).html", RegexOptions.Compiled);
		internal static readonly Regex CategoryUrlRegex = new Regex(@"http://www.luoqiu.com/read/(?<BookUnicode>\d*/", RegexOptions.Compiled);

		private string bookCategoryHTML;

		public override string Type { get; protected set; } = "书籍";

		/// <summary>
		/// 书籍的封面URL。
		/// </summary>
		public string Cover { get; private set; } = null;

		/// <summary>
		/// 书籍的统一码。
		/// </summary>
		internal ulong BookUnicode { get; private set; }

		/// <summary>
		/// 书籍的URL。
		/// </summary>
		internal string BookUrl { get; private set; }
		/// <summary>
		/// 目录的URL。
		/// </summary>
		internal string CategoryUrl { get; private set; }

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

			Match bu_m = BookToken.BookUrlRegex.Match(url);
			Match cu_m = BookToken.CategoryUrlRegex.Match(url);
			if (bu_m.Success)
			{
				this.BookUnicode = ulong.Parse(bu_m.Groups["BookUnicode"].Value);
				this.BookUrl = url;
				this.CategoryUrl = string.Format(@"http://www.luoqiu.com/read/{0}/", this.BookUnicode);
			}
			else if (cu_m.Success)
			{
				this.BookUnicode = ulong.Parse(cu_m.Groups["BookUnicode"].Value);
				this.BookUrl = string.Format(@"http://www.luoqiu.com/book/{0}.html", this.BookUnicode);
				this.CategoryUrl = url;
			}
			else
				throw new InvalidOperationException(
			  "无法解析URL。",
			  new ArgumentOutOfRangeException(nameof(url), url, "URL不符合格式。"));
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
			if (
				((this.BookUrl == null) || !BookToken.BookUrlRegex.IsMatch(this.BookUrl)) ||
				((this.CategoryUrl == null) || !BookToken.CategoryUrlRegex.IsMatch(this.CategoryUrl))
			) return false;

			try
			{
				string source = HTML.GetSource(this.BookUrl);

				Match head_match = Regex.Match(source, @"<head>(?<HeadContent>\.*?_</head>", RegexOptions.Compiled);
				if (!head_match.Success) throw new InvalidOperationException("无法抓取信息。");

				source = head_match.Groups["HeadContent"].Value;

				MatchCollection meta_matches = Regex.Matches(source, @"<meta property=""(?<MetaProperty>\.*?)"" content=""(?<MetaContent>\.*?)"">", RegexOptions.Compiled);
				if (meta_matches.Count == 0) throw new InvalidOperationException("无法抓取信息。");

				foreach (Match meta_match in meta_matches)
				{
					if (!meta_match.Success) throw new InvalidOperationException("无法抓取信息。");

					switch (meta_match.Groups["MetaProperty"].Value)
					{
						case "og:novel:book_name":
							this.Title = meta_match.Groups["MetaContent"].Value;
							break;
						case "og:novel:author":
							this.Author = meta_match.Groups["MetaContent"].Value;
							break;
						case "og:image":
							this.Cover = meta_match.Groups["MetaContent"].Value;
							break;
						case "og:description":
							this.Description = meta_match.Groups["MetaContent"].Value;
							break;
					}
				}
			}
			catch (Exception)
			{
				return false;
			}

			Match m = Regex.Match(@"<!--作品展示 start-->(?<BookCategoryHTML>\.*?)<!--作品展示 end-->", "", RegexOptions.Compiled);
			if (m.Success)
			{
				this.bookCategoryHTML = m.Groups["BookCategoryHTML"].Value;
				return true;
			}
			return false;
        }

		protected override void StartCreepInternal()
		{
			this.chapterRegex = new Regex(string.Format(@"<a href=""(?<ChapterRelativeUrl>/read/{0}/(?<ChapterUnicode>\d*?).html)""alt=""(?<AltContent>\.*?)"">(?<ChapterTitle>\.*?)</a>"), RegexOptions.Compiled);
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
			return new string[2]
			{
				this.nextMatch.Groups["ChapterTitle"].Value,
				new Uri(LuoQiu_NovelDownloader.HostUri, new Uri(this.nextMatch.Groups["ChapterRelativeUrl"].Value)).ToString()
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
				this.Add(new ChapterToken(new Uri(data[1])));
				this.OnCreepFetched(this, new DataEventArgs<object>(data[0]));
			}
			return true;
		}
		#endregion
	}
}
