using NovelDownloader.Token;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfLauncher001
{
    class BookInfo
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        private Uri _pre_cover;
        public Uri Cover { get; set; }
        private ImageSource _coverImageSource;
        public ImageSource CoverImageSource
        {
            get
            {
                if (this._pre_cover != this.Cover)
                {
                    this._pre_cover = this.Cover;
                    this._coverImageSource = new BitmapImage(this._pre_cover);
                }

                return this._coverImageSource;
            }
        }
        public double gBookInfoHeight { get; } = 300;
        public double bCoverWidth
        {
            get
            {
                var source = this.CoverImageSource;

                return this.bCoverHeight * (source.Width / source.Height);
            }
        }
        public double bCoverHeight { get; } = 250;
        public ObservableCollection<ChapterInfo> Chapters { get; } = new ObservableCollection<ChapterInfo>();

        internal BookInfo() { }

        public BookInfo(NDTBook bookToken)
        {
            if (bookToken == null) throw new ArgumentNullException(nameof(bookToken));

            this.Title = bookToken.Title;
            this.Author = bookToken.Author;

            var description = bookToken.Description;
            var lines = description.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0) this.Description = string.Empty;
            else if (lines.Length == 1) this.Description = lines[0].Trim();
            else
                this.Description = string.Join(Environment.NewLine, new[] { string.Empty }.Concat(lines.Select(line => "　　" + line.Trim())));

            this.Cover = bookToken.Cover;

            bookToken.StartCreep();
            foreach (var token in bookToken.Children.OfType<NDTChapter>().Select(token => new ChapterInfo(token)))
                this.Chapters.Add(token);

            if (this.Chapters.Count == 0)
            {
                foreach (var token in bookToken.Children.OfType<NDTVolume>().SelectMany(token =>
                {
                    token.StartCreep();
                    return token.Children.OfType<NDTChapter>();
                }).Select(token => new ChapterInfo(token)))
                    this.Chapters.Add(token);
            }
        }
    }
}
