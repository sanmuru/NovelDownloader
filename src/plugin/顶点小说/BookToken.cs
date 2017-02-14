using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using NovelDownloader.Token;
using SamLu.Web;

namespace NovelDownloader.Plugin.顶点小说
{
	public class BookToken : NDTBook
	{
		internal static readonly Regex BookUrlRegex = new Regex(@"http://www.23us.com/book/(?<BookUnicode>\d*)/", RegexOptions.Compiled);
		internal static readonly Regex CategoryUrlRegex = new Regex(@"http://www.23us.com/html/\d*/(?<BookUnicode>\d*)/(index.html)?", RegexOptions.Compiled);
		
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
		/// 目录的URL。
		/// </summary>
		internal string CategoryUrl { get; private set; }

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



			InvalidOperationException exception = new InvalidOperationException("无法抓取信息。");
			
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(HTML.GetSource(this.BookUrl, Encoding.GetEncoding("GBK")));

			string title = doc.DocumentNode.SelectSingleNode("/head/meta[@name='keywords']")?.GetAttributeValue("content", null);
			if (title == null) throw exception;
			else this.Title = title;

			HtmlNode contentDLElement = doc.GetElementbyId("content");
			if (contentDLElement == null) throw exception;

			string coverUrl = contentDLElement.SelectSingleNode("//img")?.GetAttributeValue("src", null);
			if (coverUrl == null) throw exception;
			else this.Cover = new Uri(coverUrl, UriKind.Absolute);

			HtmlNodeCollection table = contentDLElement.SelectSingleNode("//table")?.SelectNodes("//th | //td");
			if (table == null) throw exception;
			System.Diagnostics.Debug.Assert(table.Count % 2 == 0, "书籍信息不成对。");
			Dictionary<string, string> dic = new Dictionary<string, string>();
			for (int i = 0; i < table.Count; i += 2)
				dic.Add(table[i].InnerText, table[i + 1].InnerText);
			if (dic.ContainsKey("文章作者")) throw exception;
			else this.Author = dic["文章作者"];

			string description = contentDLElement.SelectSingleNode("//p[position()=2]")?.InnerText?.Trim();
			if (description == null) throw exception;
			else this.Description = description;
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
			)
				return false;
				
			try
			{
				HtmlDocument doc = new HtmlDocument();
				doc.LoadHtml(HTML.GetSource(this.CategoryUrl, Encoding.GetEncoding("GBK")));

				HtmlNode tableElement = doc.GetElementbyId("at");
				HtmlNodeCollection table = tableElement?.SelectNodes("//a");
				if (table == null) return false;

				this.enumerator = ((IEnumerable<HtmlNode>)table).GetEnumerator();
				if (this.enumerator == null) return false;
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
			this.hasNext = this.enumerator.MoveNext();
		}
		#endregion

		#region Creep
		private IEnumerator<HtmlNode> enumerator;
		private bool hasNext;

		private bool CanCreep()
		{
			return this.hasNext;
		}

		protected override bool CanCreep<TData>(TData data)
		{
			return this.CanCreep();
		}

		private string[] Creep()
		{
			HtmlNode current = this.enumerator.Current;
			string[] fetch = new string[]
			{
				current.InnerText,
				new Uri(DingDianXiaoShuo_NovelDownloader.HostUri,new Uri(current.GetAttributeValue("href", null), UriKind.Relative)).ToString()
			};

			this.hasNext = this.enumerator.MoveNext();
			return fetch;
		}

		public override TFetch Creep<TData, TFetch>(TData data)
		{
			if (typeof(TFetch).Equals(typeof(string[])))
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
			if (!this.CanCreep()) return false;

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