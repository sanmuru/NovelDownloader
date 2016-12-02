using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NovelDownloader;
using NovelDownloader.Plugin;
using NovelDownloader.Token;

namespace ExampleNovelDownloader
{
	class Program
	{
		static readonly PluginLoader pluginLoader = new PluginLoader();
		
		static void Main(string[] args)
		{
			var plugins = pluginLoader.Load("luoqiu.com.dll");
			INovelDownloadPlugin plugin = plugins.First() as INovelDownloadPlugin;
			
			foreach (string arg in args)
			{
				StringBuilder sb = new StringBuilder();

				NDTBook bookToken = plugin.GetBookToken(new Uri(arg));
				Console.WriteLine("{0} - {1}", bookToken.Type, bookToken.Title);



				bookToken.CreepStarted += (sender, e) => record(string.Format("\t开始下载书籍……"), sb);
				bookToken.CreepFetched += (sender, e) => record(string.Format("\t获取 - {0}", e.Data), sb);
				bookToken.CreepFetched += (sender, e) => record(string.Format("\t下载书籍完成。"), sb);
				bookToken.StartCreep();

				foreach (NDTChapter chapterToken in bookToken.Children)
				{
					chapterToken.CreepStarted += (sender, e) => record(string.Format("\t开始下载章节……"), sb);
					chapterToken.CreepFetched += (sender, e) => record(string.Format("{0}", e.Data), sb);
					chapterToken.CreepFinished += (sender, e) => record(string.Format("\t下载章节完成。"), sb);
				}
			}
		}

		private static void record(string str, StringBuilder sb)
		{
			Console.WriteLine(str);
			sb.AppendLine(str);
		}
	}
}
