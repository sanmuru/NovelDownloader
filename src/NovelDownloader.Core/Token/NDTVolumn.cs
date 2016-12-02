using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovelDownloader.Token
{
	/// <summary>
	/// 表示卷的<see cref="NDToken"/>子类的虚拟基类。
	/// </summary>
	public abstract class NDTVolumn : NDToken
	{
		/// <summary>
		/// 使用指定的统一资源标识符初始化<see cref="NDTVolumn"/>对象。
		/// </summary>
		/// <param name="uri"></param>
		protected NDTVolumn(Uri uri) : base(uri) { }

		/// <summary>
		/// 使用指定的标题和说明初始化<see cref="NDTVolumn"/>对象。
		/// </summary>
		/// <param name="title">指定的标题。</param>
		/// <param name="description">指定的说明。</param>
		protected NDTVolumn(string title, string description) : this(nameof(NDTVolumn), title, description) { }

		private NDTVolumn(string type, string title, string description) : base(type, title, description) { }
	}
}
