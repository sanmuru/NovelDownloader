using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamLu.NovelDownloader.Token
{
	/// <summary>
	/// 表示图片的<see cref="NDToken"/>子类的虚拟基类。
	/// </summary>
	public abstract class NDTImage : NDToken
	{
		/// <summary>
        /// 获取子节点。
        /// </summary>
        /// <exception cref="NotSupportedException"><see cref="NDTImage"/> 节点不支持获取子节点。</exception>
        public sealed override ICollection<NDToken> Children
		{
			get
			{
				throw new NotSupportedException();
			}

			protected set
			{
				throw new NotSupportedException();
			}
		}
        /// <summary>
        /// 获取标题。
        /// </summary>
        /// <exception cref="NotSupportedException"><see cref="NDTImage"/> 节点不支持获取标题。</exception>
        public sealed override string Title
		{
			get
			{
				throw new NotSupportedException();
			}

			set
			{
				throw new NotSupportedException();
			}
		}
        /// <summary>
        /// 获取描述。
        /// </summary>
        /// <exception cref="NotSupportedException"><see cref="NDTImage"/> 节点不支持获取描述。</exception>
        public sealed override string Description
		{
			get
			{
				throw new NotSupportedException();
			}

			set
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// 初始化<see cref="NDTImage"/>对象。
		/// </summary>
		protected NDTImage() : base() { }

		/// <summary>
		/// 使用指定的统一资源标识符初始化<see cref="NDTImage"/>对象。
		/// </summary>
		/// <param name="uri">指定的统一资源标识符。</param>
		protected NDTImage(Uri uri) : base(uri) { }
		
		/// <summary>
		/// 使用指定的URL初始化<see cref="NDTImage"/>对象。
		/// </summary>
		/// <param name="url">指定的URL。</param>
		protected NDTImage(string url) : this(new Uri(url)) { }
	}
}
