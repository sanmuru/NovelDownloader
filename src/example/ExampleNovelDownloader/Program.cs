using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NovelDownloader;
using NovelDownloader.Plugin;
using NovelDownloader.Token;

namespace ExampleNovelDownloader
{
	class Program
	{
		static readonly PluginManager pluginManager = new PluginManager();

		static void Main(string[] args)
		{
			IEnumerable<INovelDownloadPlugin> plugins = null;
			try
			{
				Exception ex = null;

				pluginManager.Load("luoqiu.com.dll");
				pluginManager.Load("81zw.com.dll");
				pluginManager.Load("顶点小说.dll");

				plugins = pluginManager.Plugins.Select(pair => pair.Value).OfType<INovelDownloadPlugin>();

				if (args.Length != 0)
				{
					foreach (string arg in args)
					{
						try
						{
							NDTBook bookToken;
							foreach (var plugin in plugins)
							{
								if (!plugin.TryGetBookToken(new Uri(arg), out bookToken)) continue;

								download(bookToken);
							}
						}
						catch (Exception e)
						{
							ex = ex ?? e;
						}
					}
				}
				else
				{
					string url;
					while (true)
					{
						Console.WriteLine();
						Console.WriteLine("输入小说主页面网址：");
						url = Console.ReadLine().Trim();
						if (string.IsNullOrEmpty(url)) break;

						try
						{
							NDTBook bookToken;
							foreach (var plugin in plugins)
							{
								if (!plugin.TryGetBookToken(new Uri(url, UriKind.RelativeOrAbsolute), out bookToken)) continue;

								download(bookToken);
							}
						}
						catch (Exception e)
						{
							ex = ex ?? e;
						}
					}
				}

				if (ex != null) throw ex;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				Console.ReadLine();
			}
			finally
			{
				if (plugins != null)
					foreach (var plugin in plugins) pluginManager.Release(plugin);
			}
		}

		private static void download(NDTBook bookToken)
		{
			new Thread(() =>
			{
				using (StreamWriter writer = new StreamWriter(string.Format("{0}.txt", bookToken.Title), false, Encoding.UTF8))
				{
					//StringBuilder sb = new StringBuilder();

					Console.WriteLine("{0} - {1}", bookToken.Type, bookToken.Title);
					writer.WriteLine(string.Format("书名：{0}", bookToken.Title));
					writer.WriteLine(string.Format("作者：{0}", bookToken.Author));
					writer.WriteLine();
					writer.WriteLine();

					bookToken.CreepStarted += (sender, e) => Console.WriteLine("\t开始下载书籍《{0}》", bookToken.Title);
					bookToken.CreepFetched += (sender, e) => Console.WriteLine("\t获取 “{0}”", e.Data);
					bookToken.CreepFinished += (sender, e) => Console.WriteLine("\t下载书籍《{0}》完成。", bookToken.Title);
					bookToken.StartCreep();
					foreach (NDTChapter chapterToken in bookToken.Children)
					{
						chapterToken.CreepStarted += (sender, e) =>
						{
							Console.WriteLine("\t开始下载章节“{0}”", chapterToken.Title);
							writer.WriteLine("--------------------");
							writer.WriteLine();
							writer.WriteLine(chapterToken.Title);
						};
						chapterToken.CreepFetched += (sender, e) =>
						{
							//Console.WriteLine("{0}", e.Data);
							writer.WriteLine(string.Format("　　{0}", e.Data));
						};
						chapterToken.CreepFinished += (sender, e) =>
						{
							Console.WriteLine("\t下载章节“{0}”完成。\n", chapterToken.Title);
							writer.WriteLine();
						};
						chapterToken.StartCreep();
					}

					//Console.WriteLine(sb.ToString());
				}
			}).Start();
		}

		private static void download(INovelDownloadPlugin plugin, string url)
		{
			NDTBook bookToken;
			if (!plugin.TryGetBookToken(new Uri(url, UriKind.RelativeOrAbsolute), out bookToken)) return;

			Console.WriteLine(string.Format("{0}({1}) v{2}\n{3}", plugin.Name, plugin.DisplayName, plugin.Version, plugin.Description));

			Console.WriteLine();

			download(bookToken);
		}
	}
}
