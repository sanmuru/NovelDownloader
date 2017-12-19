using NovelDownloader.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfLauncher001
{
    class ChapterInfo
    {
        public string Title { get; set; }
        public NDTChapter ChapterToken { get; set; }

        internal ChapterInfo() { }

        public ChapterInfo(NDTChapter chapterToken)
        {
            if (chapterToken == null) throw new ArgumentNullException(nameof(chapterToken));

            this.Title = chapterToken.Title;
            this.ChapterToken = chapterToken;
        }
    }
}
