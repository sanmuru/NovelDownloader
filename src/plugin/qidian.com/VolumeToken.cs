using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using SamLu.NovelDownloader.Token;

namespace SamLu.NovelDownloader.Plugin.qidian.com
{
	public class VolumeToken : NDTVolume
	{
		public override string Type { get; protected set; } = "卷";

		internal IList<HtmlNode> Nodes { get; private set; } = new List<HtmlNode>();

		/// <summary>
		/// 初始化<see cref="VolumeToken"/>对象。
		/// </summary>
		public VolumeToken() : base() { }

		internal VolumeToken(IEnumerable<HtmlNode> nodes) : this(null, null, nodes) { }

		public VolumeToken(string title, string description) : base(title, description) { }

		internal VolumeToken(string title, string description, IEnumerable<HtmlNode> nodes) : this(title, description)
		{
			if (nodes == null) throw new ArgumentNullException(nameof(nodes));

			foreach (var node in nodes) this.Nodes.Add(node);
		}

		#region StartCreep
		protected override bool CanStartCreep()
		{
			return this.Nodes != null;
		}

		protected override void StartCreepInternal()
		{
			this.enumerator = this.Nodes.GetEnumerator();
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
			this.hasNext = this.enumerator.MoveNext();

			return new string[]
			{
				current.InnerText,
				new Uri("http:" + current.GetAttributeValue("href", null)).ToString()
			};
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
				Uri uri = new Uri(data[1]);
				ChapterToken chapterToken;
				if (ChapterToken.FreeChapterUrlRegex.IsMatch(data[1]))
					chapterToken = new FreeChapterToken(uri) { Title = data[0] };
				else
					chapterToken = new VipChapterToken(uri) { Title = data[0] };
				
				this.Add(chapterToken);
				this.OnCreepFetched(this, data[0]);
			}
			return true;
		}
		#endregion
	}
}
