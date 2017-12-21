using SamLu.NovelDownloader.Plugin;
using SamLu.NovelDownloader.Token;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NovelDownloaderPluginTest
{
    public class Program
    {
        internal static readonly PluginManager manager = new PluginManager();

        static void Main(string[] args)
        {
            const string plugins_directory = @".\";

            if (!Directory.Exists(plugins_directory)) return;

            new string[]
            {
                "sfacg.com.dll"
            }
                .Select(file => Path.Combine(plugins_directory, file))
                .Where(file => Path.GetExtension(file).ToLower() == ".dll")
                .ToList()
                .ForEach(file =>
                {
                    try
                    {
                        manager.Load(file);
                    }
                    catch (Exception) { }
                });
            INovelDownloadPlugin[] plugins = manager.Plugins.Values.OfType<INovelDownloadPlugin>().ToArray();

            while (true) {
                string url = Console.ReadLine();
                foreach (var plugin in plugins)
                {
                    if (plugin.TryGetBookToken(new Uri(url, UriKind.RelativeOrAbsolute), out NDTBook bookToken))
                    {
                        manager.SaveTo(bookToken, @"downloads\");
                    }
                }
            }
        }
    }
}
