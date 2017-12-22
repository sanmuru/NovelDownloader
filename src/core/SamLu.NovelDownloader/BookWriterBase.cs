using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamLu.NovelDownloader.Token;
using System.IO;
using SamLu.NovelDownloader.Plugin;
using System.ComponentModel.Composition;

namespace SamLu.NovelDownloader
{
    public abstract class BookWriterBase : IBookWriter
    {
        private TextWriter writer;
        public TextWriter LogWriter => this.writer;

        [Import(NovelDownloadPluginBase.CONTRACTNAME_NOVELDOWNLOADPLUGIN, typeof(INovelDownloadPlugin))]
        private INovelDownloadPlugin plugin;
        protected INovelDownloadPlugin Plugin => this.plugin;

        protected BookWriterBase(TextWriter logWriter)
        {
            this.writer = logWriter ?? throw new ArgumentNullException(nameof(logWriter));
        }

        public virtual void Write(NDTBook bookToken, string outputDir)
        {
            if (bookToken == null) throw new ArgumentNullException(nameof(bookToken));
            if (outputDir == null) throw new ArgumentNullException(nameof(outputDir));

            var di = new DirectoryInfo(outputDir);
            if (!di.Exists) di.Create();

            this.WriteInternal(bookToken, outputDir);
        }
        
        /// <summary>
        /// 子类提供输出书籍的实现。
        /// </summary>
        /// <param name="bookToken">书籍节点。</param>
        /// <param name="outputDir">输出目录。</param>
        protected abstract void WriteInternal(NDTBook bookToken, string outputDir);

        /// <summary>
        /// 异步将书籍输出到文件。
        /// </summary>
        /// <param name="bookToken">书籍节点。</param>
        /// <param name="outputDir">输出目录。</param>
        public virtual Task WriteAsync(NDTBook bookToken, string outputDir)
        {
            return Task.Run(() => Write(bookToken, outputDir));
        }
    }
}
