using System;
using System.Collections.Generic;
using System.IO;
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
				sb.AppendLine(string.Format("书名：{0}", bookToken.Title));
				sb.AppendLine(string.Format("作者：{0}", bookToken.Author));
				sb.AppendLine();
				sb.AppendLine();

				bookToken.CreepStarted += (sender, e) => record(string.Format("\t开始下载书籍《{0}》", bookToken.Title), sb);
				bookToken.CreepFetched += (sender, e) => record(string.Format("\t获取 “{0}”", e.Data), sb);
				bookToken.CreepFinished += (sender, e) => record(string.Format("\t下载书籍《{0}》完成。", bookToken.Title), sb);
				bookToken.StartCreep();
				foreach (NDTChapter chapterToken in bookToken.Children)
				{
					chapterToken.CreepStarted += (sender, e) =>
					{
						record(string.Format("\t开始下载章节“{0}”", chapterToken.Title), sb);
						sb.AppendLine("--------------------");
						sb.AppendLine();
						sb.AppendLine(chapterToken.Title);
					};
					chapterToken.CreepFetched += (sender, e) =>
					{
						//record(string.Format("{0}", e.Data), sb);
						sb.AppendLine(string.Format("　　{0}", e.Data));
					};
					chapterToken.CreepFinished += (sender, e) =>
					{
						record(string.Format("\t下载章节“{0}”完成。", chapterToken.Title), sb);
						sb.AppendLine();
					};
					chapterToken.StartCreep();
				}

				//Console.WriteLine(sb.ToString());
				File.WriteAllText(string.Format("{0}.txt", bookToken.Title), sb.ToString());
			}
		}

		private static void record(string str, StringBuilder sb)
		{
			Console.WriteLine(str);
			//sb.AppendLine(str);
		}
	}
}
