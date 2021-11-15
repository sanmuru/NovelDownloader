using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader.Plugin
{
    /// <summary>
    /// 描述小说下载器的插件所应提供的信息和操作方法。
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// 获取插件适配的所有主机地址。
        /// </summary>
        string[] CompatibleHosts { get; }

        /// <summary>
        /// 创建书籍的实例。
        /// </summary>
        /// <param name="uri">书籍获取信息的网址。</param>
        /// <returns>由插件创建的书籍实例。</returns>
        IBook CreateBook(Uri uri);
    }
}
