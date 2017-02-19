using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using NovelDownloader.Token;
using SamLu.Web;

namespace NovelDownloader.Plugin.qidian.com
{
	public class BookToken : NDTBook
	{
		internal static readonly Regex BookUrlRegex = new Regex(@"^http://book.qidian.com/info/(?<BookUnicode>\d*)$", RegexOptions.Compiled);
		internal static readonly Regex CategoryUrlRegex = new Regex(@"^http://book.qidian.com/info/(?<BookUnicode>\d*)#Catalog$", RegexOptions.Compiled);

		public override string Type { get; protected set; } = "书籍";

		/// <summary>
		/// 书籍的统一码。
		/// </summary>
		internal string BookUnicode { get; private set; }

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
		public BookToken(Uri uri) : base(uri)
		{
			if (uri == null) throw new ArgumentNullException(nameof(uri));
			string url = uri.ToString();

			Match bu_m = BookToken.BookUrlRegex.Match(url);
			Match cu_m = BookToken.CategoryUrlRegex.Match(url);
			if (bu_m.Success)
			{
				this.BookUnicode = bu_m.Groups["BookUnicode"].Value;
				this.BookUrl = url;
				this.CategoryUrl = string.Format(@"http://book.qidian.com/info/{0}#Catalog", this.BookUnicode);
			}
			else if (cu_m.Success)
			{
				this.BookUnicode = cu_m.Groups["BookUnicode"].Value;
				this.BookUrl = string.Format(@"http://book.qidian.com/info/{0}", this.BookUnicode);
				this.CategoryUrl = url;
			}
			else
				throw new InvalidOperationException(
					"无法解析URL。",
					new ArgumentOutOfRangeException(nameof(url), url, "URL不符合格式。"));



			InvalidOperationException exception = new InvalidOperationException("无法抓取信息。");

			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(HTML.GetSource(this.BookUrl, Encoding.UTF8));

			HtmlNode node;

			HtmlNode informationNode = doc.DocumentNode.SelectSingleNode("//div[@class='book-information cf']");
			if (informationNode == null) throw exception;

			node = informationNode.SelectSingleNode("div[@class='book-img']/a/img");
			string cover = node?.GetAttributeValue("src", null);
			if (cover == null) throw exception;
			else this.Cover = new Uri(cover);

			HtmlNode infoNode = informationNode.SelectSingleNode("div[@class='book-info ']");

			node = infoNode.SelectSingleNode("h1/em");
            string title = node?.InnerText;
			if (title == null) throw exception;
			else this.Title = title.Trim();

			node = infoNode.SelectSingleNode("h1/span/a");
			string author = node?.InnerText;
			if (author == null) throw exception;
			else this.Author = author;

			node = infoNode.SelectSingleNode("p[@class='intro']");
			string description = node?.InnerText;
			if (description == null) throw exception;
			else this.Description = description;

			this.catalogNode = doc.GetElementbyId("j-catalogWrap");
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
		HtmlNode catalogNode;
		protected override bool CanStartCreep()
		{
			if (
				((this.BookUrl == null) || !BookToken.BookUrlRegex.IsMatch(this.BookUrl)) ||
				((this.CategoryUrl == null) || !BookToken.CategoryUrlRegex.IsMatch(this.CategoryUrl))
			)
				return false;

			try
			{
				if (this.catalogNode == null) return false;

				HtmlNodeCollection volumes = this.catalogNode.SelectNodes("div[@class='volume-wrap']/div[@class='volume']");
				if (volumes == null) return false;

				Dictionary<string, IEnumerable<HtmlNode>> dic = new Dictionary<string, IEnumerable<HtmlNode>>();
				foreach (var volume in volumes)
				{
					string volume_title = Regex.Replace(System.Web.HttpUtility.HtmlDecode(volume.Element("h3").InnerText).Trim(), @"\s+|·", " ").Split()[1];
					HtmlNodeCollection volume_chapters = volume.SelectNodes("ul/li/a");

					if (dic.ContainsKey(volume_title))
						dic[volume_title] = dic[volume_title].Concat(volume_chapters);
					else
						dic.Add(volume_title, volume_chapters);
				}

				this.enumerator = dic.GetEnumerator();
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
		private Dictionary<string, IEnumerable<HtmlNode>>.Enumerator enumerator;
		private bool hasNext;

		private bool CanCreep()
		{
			return this.hasNext;
		}

		protected override bool CanCreep<TData>(TData data)
		{
			return this.CanCreep();
		}

		private KeyValuePair<string, IEnumerable<HtmlNode>> Creep()
		{
			var current = this.enumerator.Current;
			this.hasNext = this.enumerator.MoveNext();

			return current;
		}

		public override TFetch Creep<TData, TFetch>(TData data)
		{
			if (typeof(TFetch).Equals(typeof(KeyValuePair<string, IEnumerable<HtmlNode>>)))
			{
				string volume_title = this.Creep().Key;
				return (TFetch)(object)volume_title;
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

			KeyValuePair<string, IEnumerable<HtmlNode>> data = this.Creep();
			this.Add(new VolumeToken(data.Value) { Title = data.Key });
			this.OnCreepFetched(this, data.Key);

			return true;
		}
		#endregion
	}
}
