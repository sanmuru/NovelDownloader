using HtmlAgilityPack;
using SamLu.NovelDownloader.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SamLu.NovelDownloader.Plugin.sfacg.com
{
    [NovelDownLoadPluginBookToken(NovelDownloader._guidStr)]
    internal class BookToken : NDTBook
    {
        internal static readonly Regex BookUrlRegex = new Regex(@"^http://book.sfacg.com/Novel/(?<BookUnicode>\d*)(/)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        internal static readonly Regex CategoryUrlRegex = new Regex(@"^http://book.sfacg.com/Novel/(?<BookUnicode>\d*)/MainIndex(/)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        
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
        /// 获取横幅图片的统一资源标识符。
        /// </summary>
        public Uri Banner { get; private set; }

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
                this.CategoryUrl = string.Format(@"http://book.sfacg.com/Novel/{0}/MainIndex/", this.BookUnicode);
            }
            else if (cu_m.Success)
            {
                this.BookUnicode = ulong.Parse(cu_m.Groups["BookUnicode"].Value);
                this.BookUrl = string.Format(@"http://book.sfacg.com/Novel/{0}/", this.BookUnicode);
                this.CategoryUrl = url;
            }
            else
                throw new InvalidOperationException(
                    "无法解析URL。",
                    new ArgumentOutOfRangeException(nameof(url), url, "URL不符合格式。"));



            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(this.BookUrl);
            HtmlNode html = document.DocumentNode;

            HtmlNode banner = html.SelectNodes("//div").FirstOrDefault(node => node.HasClass("d-banner"));
            Match m = Regex.Match(banner.GetAttributeValue("style", string.Empty), @"background:url\((?<banner_background_url>[^\)]*)\)");
            if (m.Success)
                this.Banner = new Uri(m.Groups["banner_background_url"].Value);

            HtmlNode summary = banner.SelectNodes("//div").FirstOrDefault(node => node.HasClass("summary-content"));
            ;

            HtmlNode title = summary.Elements("h1").FirstOrDefault(node => node.HasClass("title"));
            this.Title = title.SelectNodes("span").FirstOrDefault(node => node.HasClass("text")).InnerText.Trim();

            HtmlNode author = summary.SelectNodes("//div").FirstOrDefault(node => node.HasClass("author-info"));
            Uri avatar; // 作者头像。
            avatar = new Uri(
                author.Elements("div")
                    .FirstOrDefault(node => node.HasClass("author-mask"))
                    .Element("img")
                    .GetAttributeValue("src", null),
                UriKind.Absolute);
            this.Author = author.Elements("div")
                .FirstOrDefault(node => node.HasClass("author-name"))
                .Element("span")
                .InnerText
                .Trim();
            ;

            HtmlNode introduce = summary.Elements("p").FirstOrDefault(node => node.HasClass("introduce"));
            this.Description = introduce.InnerText.Trim();
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

        #region StartCreep
        protected override bool CanStartCreep()
        {
            if (
                ((this.BookUrl == null) || !BookToken.BookUrlRegex.IsMatch(this.BookUrl)) ||
                ((this.CategoryUrl == null) || !BookToken.CategoryUrlRegex.IsMatch(this.CategoryUrl))
            )
                return false;

            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument document = web.Load(this.CategoryUrl);
                HtmlNode catelogNode = document.DocumentNode.SelectNodes("//div").FirstOrDefault(node => node.HasClass("s-list"));

                HtmlNodeCollection volumes = catelogNode.SelectNodes("div[@class='story-catalog']");

                var dic = new Dictionary<(string title, ulong unicode), IEnumerable<HtmlNode>>();
                foreach (var volume in volumes)
                {
                    Match m = Regex.Match(
                        volume.SelectSingleNode("div[@class='catalog-hd']/h3[@class='catalog-title']").InnerText.Trim(),
                        $"^【{this.Title}】\\s+(?<volume_title>(\\s|\\S)*)$"
                    );
                    if (!m.Success) continue;
                    string volume_title = m.Groups["volume_title"].Value;

                    HtmlNodeCollection volume_chapters = volume.SelectNodes("div[@class='catalog-list']/ul/li/a");

                    ulong volume_unicode = ulong.Parse((
                        from node in volume.SelectNodes("//div")
                            .FirstOrDefault(node => node.HasClass("downliad-box"))
                            .SelectNodes("p[@class='row']/a")
                        let href = node.GetAttributeValue("href", string.Empty)
                        let _m = Regex.Match(href, @"/txt/(?<BookUnicode>\d*)/(?<VolumeUnicode>\d*).txt")
                        where _m.Success
                        select _m.Groups["VolumeUnicode"].Value
                    ).FirstOrDefault());
                    dic.Add((volume_title, volume_unicode), volume_chapters);
                }

                this.enumerator = dic.GetEnumerator();
            }
            catch (Exception e)
            {
#if DEBUG
                throw;
#endif
                this.OnCreepErrored(this, e);
                return false;
            }

            return true;
        }

        protected override void StartCreepInternal()
        {
            this.hasNext = this.enumerator.MoveNext();
        }
        #endregion

        #region Creep
        private Dictionary<(string title, ulong unicode), IEnumerable<HtmlNode>>.Enumerator enumerator;
        private bool hasNext;

        private bool CanCreep()
        {
            return this.hasNext;
        }

        protected override bool CanCreep<TData>(TData data)
        {
            return this.CanCreep();
        }

        private KeyValuePair<(string title, ulong unicode), IEnumerable<HtmlNode>> Creep()
        {
            var current = this.enumerator.Current;
            this.hasNext = this.enumerator.MoveNext();

            return current;
        }

        public override TFetch Creep<TData, TFetch>(TData data)
        {
            if (typeof(TFetch).Equals(typeof(KeyValuePair<string, IEnumerable<HtmlNode>>)))
            {
                string volume_title = this.Creep().Key.title;
                return (TFetch)(object)volume_title;
            }
            else
            {
                if (!typeof(TFetch).Equals(typeof(string)))
                    throw new NotSupportedException(
                        string.Format("不支持的数据类型{0}", typeof(TFetch).FullName)
                    );
            }

            throw new InvalidOperationException();
        }

        protected override bool CreepInternal()
        {
            if (!this.CanCreep()) return false;

            KeyValuePair<(string title, ulong unicode), IEnumerable<HtmlNode>> data = this.Creep();
            this.Add(new VolumeToken(data.Value)
            {
                Title = data.Key.title,
                BookUnicode = this.BookUnicode,
                VolumeUnicode = data.Key.unicode
            });
            this.OnCreepFetched(this, data.Key.title);

            return true;
        }
        #endregion
    }
}
