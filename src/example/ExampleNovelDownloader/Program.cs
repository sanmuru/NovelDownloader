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
				pluginManager.Load("wenku8.com.dll");

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

								download(plugin, arg);
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

								download(plugin, url);
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

		private static void downloadLight(INovelDownloadPlugin plugin, NDTBook bookToken)
		{
			new Thread(() =>
			{
				Console.WriteLine("{0} - {1}", bookToken.Type, bookToken.Title);

				DirectoryInfo directory = new DirectoryInfo(process(bookToken.Title));
				bookToken.CreepStarted += (sender, e) =>
				{
					Console.WriteLine("\t开始下载书籍《{0}》", bookToken.Title);

					if (!directory.Exists) directory.Create();
				};
				bookToken.CreepFetched += (sender, e) => Console.WriteLine("\t获取 （{0}）", e.Data);
				bookToken.CreepFinished += (sender, e) => Console.WriteLine("\t下载书籍《{0}》完成。", bookToken.Title);
				bookToken.StartCreep();
				foreach (NDTVolume volumeToken in bookToken.Children)
				{
					FileInfo file = new FileInfo(Path.Combine(directory.FullName, process(volumeToken.Title)));
					StreamWriter writer = null;
					volumeToken.CreepStarted += (sender, e) =>
					{
						Console.WriteLine("\t\t开始下载卷（{0}）", volumeToken.Title);

						writer = file.CreateText();
					};
					volumeToken.CreepFetched += (sender, e) => Console.WriteLine("\t\t获取 【{0}】", e.Data);
					volumeToken.CreepFinished += (sender, e) => Console.WriteLine("\t\t下载卷（{0}）完成。", volumeToken.Title);
					volumeToken.StartCreep();

					writer.WriteLine("[{0}][{1}][{2}]", bookToken.Author, bookToken.Title, volumeToken.Title);
					writer.WriteLine();
					writer.WriteLine("≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡");
					writer.WriteLine();
					writer.WriteLine("　　书名：{0}", bookToken.Title);
					writer.WriteLine("　　作者：{0}", bookToken.Author);
					writer.WriteLine("　　来源：{0}", bookToken.Uri.Host);
					writer.WriteLine();
					writer.WriteLine("　　本资源由小说下载插件 {0}({1}) v{2} 【{3}】 扫描提供。", plugin.DisplayName, plugin.Name, plugin.Version, plugin.Description);
					writer.WriteLine();
					writer.WriteLine("≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡");
					writer.WriteLine();
					writer.WriteLine();
					writer.WriteLine(volumeToken.Title);
					writer.WriteLine();
					writer.Flush();
					foreach (NDTChapter chapterToken in volumeToken.Children)
					{
						chapterToken.CreepStarted += (sender, e) =>
						{
							Console.WriteLine("\t\t\t开始下载章节【{0}】”", chapterToken.Title);
							writer.WriteLine();
							writer.WriteLine();
							writer.WriteLine(chapterToken.Title);
							writer.WriteLine();
						};
						chapterToken.CreepFetched += (sender, e) =>
						{
							writer.WriteLine("　　{0}", e.Data);
						};
						chapterToken.CreepFinished += (sender, e) =>
						{
							Console.WriteLine("\t\t\t下载章节【{0}】完成。\n", chapterToken.Title);
							writer.WriteLine();
						};
						chapterToken.StartCreep();
					}
					
					writer.Flush();
					writer.Close();
					writer.Dispose();
				}
			}).Start();
		}

		private static void downloadNormal(INovelDownloadPlugin plugin, NDTBook bookToken)
		{
			new Thread(() =>
			{
				using (StreamWriter writer = new StreamWriter(string.Format("{0}.txt", process(bookToken.Title)), false, Encoding.UTF8))
				{
					//StringBuilder sb = new StringBuilder();

					Console.WriteLine("{0} - {1}", bookToken.Type, bookToken.Title);
					writer.WriteLine(string.Format("书名：{0}", bookToken.Title));
					writer.WriteLine(string.Format("作者：{0}", bookToken.Author));
					writer.WriteLine();
					writer.WriteLine();

					bookToken.CreepStarted += (sender, e) => Console.WriteLine("\t开始下载书籍《{0}》", bookToken.Title);
					bookToken.CreepFetched += (sender, e) => Console.WriteLine("\t获取 【{0}】", e.Data);
					bookToken.CreepFinished += (sender, e) => Console.WriteLine("\t下载书籍《{0}》完成。", bookToken.Title);
					bookToken.StartCreep();
					foreach (NDTChapter chapterToken in bookToken.Children)
					{
						chapterToken.CreepStarted += (sender, e) =>
						{
							Console.WriteLine("\t开始下载章节【{0}】”", chapterToken.Title);
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
							Console.WriteLine("\t下载章节【{0}】完成。\n", chapterToken.Title);
							writer.WriteLine();
						};
						chapterToken.StartCreep();
					}

					//Console.WriteLine(sb.ToString());
				}
			}).Start();
		}

		private static void download(INovelDownloadPlugin plugin, string url, NDTBook bookToken = null)
		{
			if (bookToken == null && !plugin.TryGetBookToken(new Uri(url, UriKind.RelativeOrAbsolute), out bookToken)) return;

			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(string.Format("{0}({1}) v{2}\n{3}", plugin.DisplayName, plugin.Name, plugin.Version, plugin.Description));
			Console.ForegroundColor = ConsoleColor.Gray;

			Console.WriteLine();

			if (bookToken.Children.All(child => child is NDTVolume))
				downloadLight(plugin, bookToken);
			else
				downloadNormal(plugin, bookToken);
		}

		private static string process(string s)
		{
			return s.Replace(':', '：').Replace('.', '·');
        }
	}
}
