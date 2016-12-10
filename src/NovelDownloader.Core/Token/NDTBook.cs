using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovelDownloader.Token
{
	/// <summary>
	/// 表示书籍的<see cref="NDToken"/>子类的虚拟基类。
	/// </summary>
	public abstract class NDTBook : NDToken
	{
		/// <summary>
		/// 书籍的作者。
		/// </summary>
		public virtual string Author { get; protected set; } = null;

		/// <summary>
		/// 书籍的封面统一资源标识符。
		/// </summary>
		public virtual Uri Cover { get; protected set; } = null;

		/// <summary>
		/// 初始化<see cref="NDTBook"/>对象。
		/// </summary>
		protected NDTBook() : base() { }

		/// <summary>
		/// 使用指定的统一资源标识符初始化<see cref="NDTBook"/>对象。
		/// </summary>
		/// <param name="uri"></param>
		protected NDTBook(Uri uri) : base(uri) { }

		/// <summary>
		/// 使用指定的标题和说明初始化<see cref="NDTBook"/>对象。
		/// </summary>
		/// <param name="title">指定的标题。</param>
		/// <param name="description">指定的说明。</param>
		protected NDTBook(string title, string description) : this(nameof(NDTBook), title, description) { }

		private NDTBook(string type, string title, string description) : base(type, title, description) { }
	}
}
