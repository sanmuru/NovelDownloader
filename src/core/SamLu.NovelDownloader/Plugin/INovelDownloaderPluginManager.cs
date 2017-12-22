using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamLu.NovelDownloader.Token;

namespace SamLu.NovelDownloader.Plugin
{
    /// <summary>
    /// 定义小说下载插件管理器需要实现的接口。
    /// </summary>
    public interface INovelDownloaderPluginManager : IPluginManager
    {
        /// <summary>
        /// 为指定的小说下载插件注册书籍输出器。
        /// </summary>
        /// <param name="plugin">小说下载插件。</param>
        /// <param name="bookWriter">书籍输出器。</param>
        void RegisterBookWriter(INovelDownloadPlugin plugin, IBookWriter bookWriter);

        /// <summary>
        /// 寻找合适的书籍输出器，并保存书籍到文件。
        /// </summary>
        /// <param name="bookToken">书籍节点。</param>
        /// <param name="outputDir">输出目录。</param>
        void SaveTo(NDTBook bookToken, string outputDir);

        /// <summary>
        /// 寻找合适的书籍输出器，并异步保存书籍到文件。
        /// </summary>
        /// <param name="bookToken">书籍节点。</param>
        /// <param name="outputDir">输出目录。</param>
        Task SaveToAsync(NDTBook bookToken, string outputDir);
    }
}
