using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using NovelDownloader.Token;
using SamLu.Web;

namespace NovelDownloader.Plugin.wenku8.com
{
	public class BookToken : NDTBook
	{
		internal static readonly Regex BookUrlRegex = new Regex(@"http://www.wenku8.com/book/(?<BookUnicode>\d*).htm", RegexOptions.Compiled);
		internal static readonly Regex CategoryUrlRegex = new Regex(@"http://www.wenku8.com/novel/\d*/(?<BookUnicode>\d*)/(index.htm)?", RegexOptions.Compiled);

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
		public BookToken(Uri uri) : base(uri)
		{
			string url = uri.ToString();
			if (url == null) throw new ArgumentNullException(nameof(url));

			Match bu_m = BookToken.BookUrlRegex.Match(url);
			Match cu_m = BookToken.CategoryUrlRegex.Match(url);
			if (bu_m.Success)
			{
				this.BookUnicode = ulong.Parse(bu_m.Groups["BookUnicode"].Value);
				this.BookUrl = url;
				this.CategoryUrl = string.Format(@"http://www.wenku8.com/novel/{0}/{1}/index.htm", this.BookUnicode.ToString().Remove(1), this.BookUnicode);
			}
			else if (cu_m.Success)
			{
				this.BookUnicode = ulong.Parse(cu_m.Groups["BookUnicode"].Value);
				this.BookUrl = string.Format(@"http://www.wenku8.com/book/{0}.htm", this.BookUnicode);
				this.CategoryUrl = url;
			}
			else
				throw new InvalidOperationException(
					"无法解析URL。",
					new ArgumentOutOfRangeException(nameof(url), url, "URL不符合格式。"));



			InvalidOperationException exception = new InvalidOperationException("无法抓取信息。");

			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(HTML.GetSource(this.BookUrl, Encoding.GetEncoding("GBK")));

			HtmlNode node;

			HtmlNode contentElement = doc.GetElementbyId("content");
			if (contentElement == null) throw exception;

			HtmlNode tableElement1 = contentElement.SelectSingleNode("div/table[position()=1]");
			if (tableElement1 == null) throw exception;

			node = tableElement1.SelectSingleNode("tr[position()=1]/td/table/tr/td/span/b");
			string title = node?.InnerText;
			if (title == null) throw exception;
			else this.Title = title;

			node = tableElement1.SelectSingleNode("tr[position()=2]");
			HtmlNodeCollection table = node?.SelectNodes("td");
			if (table == null) throw exception;
			Dictionary<string, string> dic = new Dictionary<string, string>();
			for (int i = 0; i < table.Count; i++)
			{
				string[] info = HttpUtility.HtmlDecode(table[i].InnerHtml).Trim().Split('：');
				System.Diagnostics.Debug.Assert(info.Length == 2, "书籍信息不成对。");
				dic.Add(info[0], info[1]);
			}
			if (!dic.ContainsKey("小说作者")) throw exception;
			else this.Author = dic["小说作者"];

			HtmlNode tableElement2 = contentElement.SelectSingleNode("div/table[position()=2]");
			node = tableElement2.SelectSingleNode("tr/td[position()=1]/img");
			string coverUrl = node?.GetAttributeValue("src", null);
			if (coverUrl == null) throw exception;
			else this.Cover = new Uri(coverUrl);

			node = tableElement2.SelectSingleNode("tr/td[position()=2]/span[last()]");
			string description = node?.InnerHtml;
			if (description == null) throw exception;
			else this.Description = Regex.Replace(HttpUtility.HtmlDecode(description).Replace("<br>", Environment.NewLine), string.Format("({0})", Environment.NewLine) + "{2,}", Environment.NewLine);
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
				
				HtmlNode tableElement = doc.DocumentNode.SelectSingleNode("//body/table");
				HtmlNodeCollection table = tableElement?.SelectNodes("tr/td");
				if (table == null) return false;

				Dictionary<string, List<HtmlNode>> dic = new Dictionary<string, List<HtmlNode>>();
				string volumeTitle = null;
				foreach (var node in table)
				{
					switch (node.GetAttributeValue("class", string.Empty))
					{
						case "vcss":
							volumeTitle = HttpUtility.HtmlDecode(node.InnerHtml);
							dic.Add(volumeTitle, new List<HtmlNode>());
							break;
						case "ccss":
							HtmlNode chapter_node = node.SelectSingleNode("a");
							if (chapter_node != null)
								dic[volumeTitle].Add(chapter_node);
							break;
						default:
							throw new InvalidOperationException("无法捕获节点。");
					}
				}

				this.enumerator = dic.GetEnumerator();
			}
			catch (WebException we)
			{
				if (we.Status == WebExceptionStatus.ProtocolError &&
					(((HttpWebResponse)we.Response).StatusCode == HttpStatusCode.NotFound))
					return false;
				else
				{
#if DEBUG
					throw;
#endif
					this.OnCreepErrored(this, we);
					return false;
				}
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
		private Dictionary<string, List<HtmlNode>>.Enumerator enumerator;
		private bool hasNext;

		private bool CanCreep()
		{
			return this.hasNext;
		}

		protected override bool CanCreep<TData>(TData data)
		{
			return this.CanCreep();
		}

		private KeyValuePair<string, List<HtmlNode>> Creep()
		{
			var current = this.enumerator.Current;
			this.hasNext = this.enumerator.MoveNext();

			return current;
		}

		public override TFetch Creep<TData, TFetch>(TData data)
		{
			if (typeof(TFetch).Equals(typeof(KeyValuePair<string, List<HtmlNode>>)))
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

			KeyValuePair<string, List<HtmlNode>> data = this.Creep();
			this.Add(new VolumeToken(data.Value) { Title = data.Key });
			this.OnCreepFetched(this, data.Key);

			return true;
		}
		#endregion
	}
}
