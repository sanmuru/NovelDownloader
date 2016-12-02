using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovelDownloader.Plugin
{
	/// <summary>
	/// 加载非托管资源的委托。
	/// </summary>
	/// <returns>非托管资源的句柄。</returns>
	public delegate IntPtr UnmanagedResourceLoader();
}
