using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovelDownloader.Plugin
{
	/// <summary>
	/// 释放非托管资源的委托。
	/// </summary>
	/// <param name="unmanagedResourceHandle">非托管资源的句柄。</param>
	public delegate void UnmanagedResourceReleaser(IntPtr unmanagedResourceHandle);
}
