using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovelDownloader.Plugin
{
	public interface IPluginLoader
	{
		IEnumerable<IPlugin> Load(string pluginFileName);
	}
}
