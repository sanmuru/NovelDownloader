using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamLu.NovelDownloader;

namespace YueWen.Qidian
{
    public class Chapter : IChapter
    {
        private readonly Book book;
        internal readonly string id;

        public string Title => throw new NotImplementedException();

        public DateTime UpdateAt => throw new NotImplementedException();

        public IVolume Volume => throw new NotImplementedException();

        public IBook Book => this.book;

        public Chapter(Book book)
        {
            this.book = book ?? throw new ArgumentNullException(nameof(book));
        }

        internal static bool TryGetBookID(string idStr, string chapterIDStr, out ulong bookID)
        {
            throw new NotImplementedException();
        }
    }
}
