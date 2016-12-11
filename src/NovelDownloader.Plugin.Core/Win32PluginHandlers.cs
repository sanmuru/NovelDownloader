using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovelDownloader.Plugin
{
	public delegate IntPtr DPluginLoad(Guid pluginGuid);
	public delegate void DPluginRelease(IntPtr pluginHandle);
	public delegate Guid[] DPluginLoadReturnsGuidArray();
	
	public delegate string DPluginInvocationReturnsString(IntPtr pluginHandle);
	public delegate Version DPluginInvocationReturnsVersion(IntPtr pluginHandle);
	public delegate Guid DPluginInvocationReturnsGuid(IntPtr pluginHandle);
}
