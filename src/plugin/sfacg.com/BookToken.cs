using HtmlAgilityPack;
using SamLu.NovelDownloader.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader.Plugin.sfacg.com
{
    internal class BookToken : NDTBook
    {
        internal static readonly Regex BookUrlRegex = new Regex(@"http://book.sfacg.com/Novel/(?<BookUnicode>\d*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        internal static readonly Regex CategoryUrlRegex = new Regex(@"http://book.sfacg.com/Novel/(?<BookUnicode>\d*)/MainIndex", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        
        public override string Type { get; protected set; } = "书籍";

        /// <summary>
        /// 书籍的统一码。
        /// </summary>
        internal ulong BookUnicode { get; private set; }

        /// <summary>
        /// 书籍的URL。
        /// </summary>
        internal string BookUrl { get; private set; }
        /// <summary>
        /// 目录的URL。
        /// </summary>
        internal string CategoryUrl { get; private set; }

        /// <summary>
        /// 初始化<see cref="BookToken"/>对象。
        /// </summary>
        public BookToken() : base() { }

        /// <summary>
        /// 使用指定的统一资源标识符初始化<see cref="BookToken"/>对象。
        /// </summary>
        /// <param name="uri">指定的统一资源标识符。</param>
        /// <exception cref="ArgumentNullException">
        /// 参数<paramref name="uri"/>的值为<see langword="null"/>。
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// 参数<paramref name="uri"/>不属于书籍URL或目录URL。
        /// </exception>
        public BookToken(Uri uri) : base(uri)
        {
            string url = uri.ToString();
            if (url == null) throw new ArgumentNullException(nameof(url));

            Match bu_m = BookToken.BookUrlRegex.Match(url);
            Match cu_m = BookToken.CategoryUrlRegex.Match(url);
            if (bu_m.Success)
            {
                this.BookUnicode = ulong.Parse(bu_m.Groups["BookUnicode"].Value);
                this.BookUrl = url;
                this.CategoryUrl = string.Format(@"http://book.sfacg.com/Novel/{0}/MainIndex", this.BookUnicode);
            }
            else if (cu_m.Success)
            {
                this.BookUnicode = ulong.Parse(cu_m.Groups["BookUnicode"].Value);
                this.BookUrl = string.Format(@"http://book.sfacg.com/Novel/{0}", this.BookUnicode);
                this.CategoryUrl = url;
            }
            else
                throw new InvalidOperationException(
                    "无法解析URL。",
                    new ArgumentOutOfRangeException(nameof(url), url, "URL不符合格式。"));



            HtmlDocument document = new HtmlDocument();
            document.Load(this.BookUrl);
            ;
        }

        /// <summary>
        /// 使用指定的URL初始化<see cref="BookToken"/>对象。
        /// </summary>
        /// <param name="url">指定的URL。</param>
        public BookToken(string url) : this(new Uri(url)) { }

        public BookToken(string title, string description) : base(title, description) { }

        public BookToken(string title, string description, string author) : this(title, description)
        {
            this.Author = author;
        }

    }
}
