using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovelDownloader.Plugin
{
	public delegate IntPtr DPluginLoad(Guid pluginGuid);
	public delegate void DPluginRelease(IntPtr pluginHandle);
	public delegate int DPluginGetPluginList(out IntPtr plugins);
	
	public delegate IntPtr DPluginInvocationReturnsString(IntPtr pluginHandle);
	public delegate IntPtr DPluginInvocationReturnsVersion(IntPtr pluginHandle);
	public delegate Guid DPluginInvocationReturnsGuid(IntPtr pluginHandle);
}
