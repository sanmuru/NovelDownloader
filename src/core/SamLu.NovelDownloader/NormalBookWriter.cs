using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamLu.NovelDownloader.Token;

namespace SamLu.NovelDownloader
{
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
            throw new NotImplementedException();
        }
    }
}
