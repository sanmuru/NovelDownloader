using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NovelDownloader.Plugin;

namespace ShowPlugins
{
    class Program
    {
        static readonly IPluginManager pluginManager = new PluginManager();
        static readonly IPluginManager win32PluginManager = new Win32PluginManager();

        static void Main(string[] args)
        {
            foreach (string arg in args)
            {
                showPluginInfo(arg);
            }

            while (true)
            {
                string path = Console.ReadLine();
                if (string.IsNullOrEmpty(path)) break;

                showPluginInfo(path);
            }
        }

        private static void showPluginInfo(string path)
        {
            try
            {
                showPluginInfo(pluginManager, path);
            }
            catch (BadImageFormatException)
            {
                showPluginInfo(win32PluginManager, path);
            }
        }

        private static void showPluginInfo(IPluginManager pluginManager, string path)
        {
            try
            {
                var plugins = pluginManager.Load(path).ToArray();
                Console.WriteLine("\"{0}\" : ", path);
                foreach (var plugin in plugins)
                {
                    Console.WriteLine(string.Format("[{0}]\n{1}({2}) (v{3}~)v{4}\n{5}\n", plugin.Guid, plugin.Name, plugin.DisplayName, plugin.MinVersion, plugin.Version, plugin.Description));
                }
            }
            catch (BadImageFormatException)
            {
                throw;
            }
            catch (Exception e)
            {
#if DEBUG
                throw;
#endif
                Console.WriteLine("{0} : {1}\n{2}\n", e.GetType().FullName, e.Message, e.StackTrace);
            }
        }
    }
}
