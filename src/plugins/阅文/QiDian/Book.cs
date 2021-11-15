using SamLu.NovelDownloader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YueWen.Qidian
{
    [CompatibleHost("book.qidian.com")]
    [CompatibleHost("read.qidian.com")]
    [CompatibleHost("vipreader.qidian.com")]
    public partial class Book : IBook
    {
        internal readonly ulong id;
        protected readonly Lazy<BookInfo> info;

        public string Title => throw new NotImplementedException();

        public string Author => throw new NotImplementedException();

        public string[] Tags => throw new NotImplementedException();

        public string Description => throw new NotImplementedException();

        public IVolume[] Volumes => throw new NotImplementedException();

        [BookActivator]
        public Book(Uri uri)
        {
            if (uri is null) throw new ArgumentNullException(nameof(uri));

            ulong bookID;
            if (uri.Host == "book.qidian.com" && uri.Segments.Length >= 3 && uri.Segments[1].Trim('/') == "info" && ulong.TryParse(uri.Segments[2].Trim('/'), out bookID))
                this.id = bookID;
            else if (uri.Host == "read.qidian.com" && uri.Segments.Length >= 4 && uri.Segments[1].Trim('/') == "chapter" && Chapter.TryGetBookID(uri.Segments[2].Trim('/'), uri.Segments[3].Trim('/'), out bookID))
                this.id = bookID;
            else if (uri.Host == "vipreader.qidian.com" && uri.Segments.Length >= 4 && uri.Segments[1].Trim('/') == "chapter" && ulong.TryParse(uri.Segments[2].Trim('/'), out bookID))
                this.id = bookID;
            else throw new BookNotFoundException($"无法从地址“{uri}”处获取书籍信息。");

            this.info = new Lazy<BookInfo>(() => new BookInfo(this), true);
        }
    }
}
