using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamLu.NovelDownloader.Token;
namespace SamLu.NovelDownloader
{
    /// <summary>
    /// 标准小说书籍输出器。
    /// </summary>
    public class NormalBookWriter : BookWriterBase
    {
        /// <summary>
        /// 初始化 <see cref="NormalBookWriter"/> 类的新实例，此实例使用 <see cref="Console.Out"/> 作为日志输出。
        /// </summary>
        public NormalBookWriter() : base(Console.Out) { }

        /// <summary>
        /// 提供输出书籍的实现。
        /// </summary>
        /// <param name="bookToken">书籍节点。</param>
        /// <param name="outputDir">输出目录。</param>
        protected override void WriteInternal(NDTBook bookToken, string outputDir)
        {
            using (StreamWriter writer = new StreamWriter(Path.Combine(outputDir, $"{bookToken.Title}.txt"), false, Encoding.UTF8))
            {
                //StringBuilder sb = new StringBuilder();

                this.LogWriter.WriteLine("{0} - {1}", bookToken.Type, bookToken.Title);
                writer.WriteLine(string.Format("书名：{0}", bookToken.Title));
                writer.WriteLine(string.Format("作者：{0}", bookToken.Author));
                writer.WriteLine();
                writer.WriteLine();

                bookToken.CreepStarted += (sender, e) => this.LogWriter.WriteLine("\t开始获取书籍《{0}》", bookToken.Title);
                bookToken.CreepFetched += (sender, e) => this.LogWriter.WriteLine("\t获取 【{0}】", e.Data);
                bookToken.CreepFinished += (sender, e) => this.LogWriter.WriteLine("\t书籍《{0}》获取完成。", bookToken.Title);
                bookToken.StartCreep();
                foreach (NDTChapter chapterToken in bookToken.Children)
                {
                    chapterToken.CreepStarted += (sender, e) =>
                    {
                        this.LogWriter.WriteLine("\t开始获取章节【{0}】”", chapterToken.Title);
                        writer.WriteLine("--------------------");
                        writer.WriteLine();
                        writer.WriteLine(chapterToken.Title);
                    };
                    chapterToken.CreepFetched += (sender, e) =>
                    {
                        //this.LogWriter.WriteLine("{0}", e.Data);
                        writer.WriteLine(string.Format("　　{0}", e.Data));
                    };
                    chapterToken.CreepFinished += (sender, e) =>
                    {
                        this.LogWriter.WriteLine("\t章节【{0}】获取完成。\n", chapterToken.Title);
                        writer.WriteLine();
                    };
                    chapterToken.StartCreep();
                }

                //this.LogWriter.WriteLine(sb.ToString());
            }
        }
    }
}
