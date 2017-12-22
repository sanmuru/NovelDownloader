using SamLu.NovelDownloader.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader
{
    /// <summary>
    /// 定义书籍输出器应实现的接口。
    /// </summary>
    public interface IBookWriter
    {
        /// <summary>
        /// 将书籍输出到文件。
        /// </summary>
        /// <param name="bookToken">书籍节点。</param>
        /// <param name="outputDir">输出目录。</param>
        void Write(NDTBook bookToken, string outputDir);

        /// <summary>
        /// 异步将书籍输出到文件。
        /// </summary>
        /// <param name="bookToken">书籍节点。</param>
        /// <param name="outputDir">输出目录。</param>
        Task WriteAsync(NDTBook bookToken, string outputDir);
    }
}
