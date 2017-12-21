using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamLu.NovelDownloader.Token;
using System.IO;

namespace SamLu.NovelDownloader
{
    /// <summary>
    /// 轻小说书籍输出器。
    /// </summary>
    public class LightNovelBookWriter : BookWriterBase
    {
        /// <summary>
        /// 初始化 <see cref="LightNovelBookWriter"/> 类的新实例，此实例使用 <see cref="Console.Out"/> 作为日志输出。
        /// </summary>
        public LightNovelBookWriter() : base(Console.Out) { }

        /// <summary>
        /// 提供输出书籍的实现。
        /// </summary>
        /// <param name="bookToken">书籍节点。</param>
        /// <param name="outputDir">输出目录。</param>
        protected override void WriteInternal(NDTBook bookToken, string outputDir)
        {
            this.LogWriter.WriteLine("{0} - {1}", bookToken.Type, bookToken.Title);
            DirectoryInfo book = null;
            bookToken.CreepStarted += (sender, e) =>
            {
                this.LogWriter.WriteLine("\t开始获取书籍《{0}》", bookToken.Title);

                // 创建书籍根目录。
                book = new DirectoryInfo(outputDir).CreateSubdirectory(bookToken.Title);
            };
            bookToken.CreepFetched += (sender, e) => this.LogWriter.WriteLine("\t获取（{0}）", e.Data);
            bookToken.CreepFinished += (sender, e) => this.LogWriter.WriteLine("\t书籍《{0}》获取完成。", bookToken.Title);
            bookToken.StartCreep();
            foreach (NDTVolume volumeToken in bookToken.Children)
            {
                FileInfo file = new FileInfo(Path.Combine(book.FullName, volumeToken.Title + ".txt"));
                StreamWriter writer = null;
                volumeToken.CreepStarted += (sender, e) =>
                {
                    this.LogWriter.WriteLine("\t\t开始获取卷（{0}）", volumeToken.Title);

                    writer = file.CreateText();
                };
                volumeToken.CreepFetched += (sender, e) => this.LogWriter.WriteLine("\t\t获取 【{0}】", e.Data);
                volumeToken.CreepFinished += (sender, e) => this.LogWriter.WriteLine("\t\t卷（{0}）获取完成。", volumeToken.Title);
                volumeToken.StartCreep();

                writer.WriteLine("[{0}][{1}][{2}]", bookToken.Author, bookToken.Title, volumeToken.Title);
                writer.WriteLine();
                writer.WriteLine("≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡≡");
                writer.WriteLine();
                writer.WriteLine("　　书名：{0}", bookToken.Title);
                writer.WriteLine("　　作者：{0}", bookToken.Author);
                writer.WriteLine("　　来源：{0}", bookToken.Uri.Host);
                writer.WriteLine();
                writer.WriteLine("　　本资源由小说下载插件 {0}({1}) v{2} 【{3}】 扫描提供。", this.Plugin.DisplayName, this.Plugin.Name, this.Plugin.Version, this.Plugin.Description);
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
                        this.LogWriter.WriteLine("\t\t\t开始下载章节【{0}】”", chapterToken.Title);
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
                        this.LogWriter.WriteLine("\t\t\t章节【{0}】下载完成。\n", chapterToken.Title);
                        writer.WriteLine();
                    };
                    chapterToken.StartCreep();
                }

                writer.Flush();
                writer.Close();
                writer.Dispose();
            }
        }
    }
}
