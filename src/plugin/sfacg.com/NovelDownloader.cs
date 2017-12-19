using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamLu.NovelDownloader.Token;
using System.Text.RegularExpressions;

namespace SamLu.NovelDownloader.Plugin.sfacg.com
{
    internal sealed class NovelDownloader : INovelDownloadPlugin
    {
        internal static readonly Uri HostUri = new Uri("http://book.sfacg.com/", UriKind.Absolute);

        public string Name { get; } = "sfacg.com";

        public string DisplayName { get; } = "SF轻小说";

        public Version Version { get; } = new Version(1, 0, 0, DateTime.Now.ToString("yyyyMMdd"), Version.BetaVersion);

        public Version MinVersion { get; } = Version.MinVersion;

        public string Description { get; } = "国内最大原创轻小说网站|明日的轻小说新星从这里起步,振兴中国轻小说";

        internal static readonly Guid _guid = new Guid("798BA759-2A65-4587-ADB7-67F9D10216DB");
        public Guid Guid => NovelDownloader._guid;

        /// <summary>
        /// 获取指定书籍编号的<see cref="BookToken"/>对象。
        /// </summary>
        /// <param name="bookUnicode">指定的书籍编号。</param>
        /// <returns>指定书籍编号的<see cref="BookToken"/>对象。</returns>
        public NDTBook GetBookToken(ulong bookUnicode)
        {
            return this.GetBookToken(new Uri(string.Format(@"http://www.luoqiu.com/book/{0}.html", bookUnicode)));
        }

        /// <summary>
        /// 获取位于指定URL的<see cref="BookToken"/>对象。
        /// </summary>
        /// <param name="url">指定的URL。</param>
        /// <returns>位于指定URL的<see cref="BookToken"/>对象。</returns>
        public NDTBook GetBookToken(string url)
        {
            if (BookToken.BookUrlRegex.IsMatch(url))
                return this.GetBookToken(new Uri(url));
            else
            {
                Match m = BookToken.CategoryUrlRegex.Match(url);
                if (m.Success)
                {
                    ulong bookUnicode = ulong.Parse(m.Groups["BookUnicode"].Value);
                    return this.GetBookToken(bookUnicode);
                }
                else
                    throw new InvalidOperationException(
                     "无法解析URL。",
                     new ArgumentOutOfRangeException(nameof(url), url, "URL不符合格式。"));
            }
        }

        /// <summary>
        /// 获取指定统一资源标识符的 <see cref="BookToken"/> 对象。
        /// </summary>
        /// <param name="uri"> 指定的统一资源标识符。</param>
        /// <returns>指定统一资源标识符的 <see cref="BookToken"/> 对象。</returns>
        public NDTBook GetBookToken(Uri uri)
        {
            return new BookToken(uri);
        }

        /// <summary>
        /// 尝试获取位于指定的 <see cref="Uri"/> 位置的小说标签。
        /// </summary>
        /// <param name="uri">指定的统一资源标识符。</param>
        /// <param name="bookToken">位于指定的 <see cref="Uri"/> 位置的小说标签。</param>
        /// <returns>是否获取成功。</returns>
        public bool TryGetBookToken(Uri uri, out NDTBook bookToken)
        {
            try
            {
                bookToken = this.GetBookToken(uri);

                return (bookToken != null);
            }
            catch (Exception)
            {
                bookToken = null;

#if DEBUG
                throw;
#endif

                return false;
            }
        }
    }
}
