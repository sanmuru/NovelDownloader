using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamLu.NovelDownloader.Token
{
	/// <summary>
	/// 表示章节的<see cref="NDToken"/>子类的虚拟基类。
	/// </summary>
	public abstract class NDTChapter : NDToken
	{
		/// <summary>
		/// 初始化<see cref="NDTChapter"/>对象。
		/// </summary>
		protected NDTChapter() : base() { }
		
		/// <summary>
		/// 使用指定的统一资源标识符初始化<see cref="NDTChapter"/>对象。
		/// </summary>
		/// <param name="uri"></param>
		protected NDTChapter(Uri uri) : base(uri) { }

		/// <summary>
		/// 使用指定的标题和说明初始化<see cref="NDTChapter"/>对象。
		/// </summary>
		/// <param name="title">指定的标题。</param>
		/// <param name="description">指定的说明。</param>
		protected NDTChapter(string title, string description) : this(nameof(NDTChapter), title, description) { }

		private NDTChapter(string type, string title, string description) : base(type, title, description) { }
	}
}
