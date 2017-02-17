using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using NovelDownloader.Token;
using SamLu.Web;

namespace NovelDownloader.Plugin.wenku8.com
{
	public class ChapterToken : NDTChapter
	{
		internal static readonly Regex ChapterUrlRegex = new Regex(@"http://www.wenku8.com/novel/\d*/(?<BookUnicode>\d*)/(?<ChapterUnicode>\d*).htm", RegexOptions.Compiled);

		public override string Type { get; protected set; } = "章";

		public ulong ChapterUnicode { get; private set; }

		public ChapterToken(Uri uri) : base(uri)
		{
			if (uri == null) throw new ArgumentNullException(nameof(uri));
			string url = uri.ToString();

			Match m = ChapterToken.ChapterUrlRegex.Match(url);
			if (m.Success)
			{
				this.ChapterUnicode = ulong.Parse(m.Groups["ChapterUnicode"].Value);
				this.ChapterUrl = url;
			}
		}

		/// <summary>
		/// 初始化<see cref="ChapterToken"/>对象。
		/// </summary>
		public ChapterToken() : base() { }

		public ChapterToken(string url) : this(new Uri(url)) { }

		public ChapterToken(string title, string description) : base(title, description) { }

		internal string ChapterUrl { get; set; }

		#region StartCreep
		protected override bool CanStartCreep()
		{
			if ((this.ChapterUrl == null) || !ChapterToken.ChapterUrlRegex.IsMatch(this.ChapterUrl)) return false;

			try
			{
				HtmlDocument doc = new HtmlDocument();
				doc.LoadHtml(HTML.GetSource(this.ChapterUrl, Encoding.GetEncoding("GBK")));

				HtmlNode contentNode = doc.GetElementbyId("content");
				if (contentNode == null) return false;

				this.enumerator = HttpUtility.HtmlDecode(contentNode.InnerText).Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(line => line.Trim()).Where(line => line != string.Empty).GetEnumerator();
			}
			catch (Exception e)
			{
				this.OnCreepErrored(this, e);
				return false;
			}

			return true;
		}

		protected override void StartCreepInternal()
		{
			this.ChapterUnicode = ulong.Parse(ChapterToken.ChapterUrlRegex.Match(this.ChapterUrl).Groups["ChapterUnicode"].Value);

			this.hasNext = this.enumerator.MoveNext();
		}
		#endregion

		#region Creep
		private IEnumerator<string> enumerator;
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
			this.Add(new TextToken(data));
			this.OnCreepFetched(this, data);

			return true;
		}
		#endregion
	}
}
