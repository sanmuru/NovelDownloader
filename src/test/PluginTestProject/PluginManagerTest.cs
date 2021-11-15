using Microsoft.VisualStudio.TestTools.UnitTesting;
using SamLu.NovelDownloader.Plugin;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace PluginTestProject
{
    [TestClass]
    public class PluginManagerTest
    {
        [TestMethod]
        public void LoadExistingPluginDirectory()
        {
            string[] urls = new[] {
                "https://book.qidian.com/info/1030223496/",
            };

            var manager = new PluginManager();
            var plugins = manager.Load("plugins");
            foreach (var url in urls) {
                Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);
                var books =
                    from plugin in plugins
                    where plugin.CompatibleHosts.Any(host => Wildcard.IsMatch(uri.Host, host))
                    select plugin.CreateBook(uri);
                foreach (var book in books) {

                }
                if (!books.Any()) Console.WriteLine("无适配插件可下载“{0}”", url);
            }
        }

        [TestMethod]
        public void DisplayTestWin32Plugin()
        {
            var manager = new PluginManager();
            var plugins = manager.Load("SimpleWin32Plugin.dll");
            foreach (var plugin in plugins) {
                Console.WriteLine("Plugin[{0}]", string.Join(";", plugin.CompatibleHosts.Select(host => $"\"{host}\"")));
                var book = plugin.CreateBook(new Uri("https://www.samlu.com", UriKind.Absolute));
                Console.WriteLine("    Title: {0}", book.Title);
                Console.WriteLine("    Author: {0}", book.Author);
                Console.WriteLine("    Tags: {0}", string.Join(" ", book.Tags));
                Console.WriteLine("    Description: {0}", book.Description);
                Console.WriteLine("    Volumes: [{0}]", book.Volumes.Length);
            }
        }
    }
}
