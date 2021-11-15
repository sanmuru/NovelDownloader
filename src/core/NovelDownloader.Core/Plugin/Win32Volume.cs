using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader.Plugin
{
    internal class Win32Volume : IVolume
    {
        protected readonly IntPtr handle;
        protected readonly Win32Book book;
        protected readonly Win32Plugin plugin;

        protected readonly Lazy<Win32Chapter[]> volumes;

        public Win32Volume(IntPtr handle, Win32Book book, Win32Plugin plugin)
        {
            this.handle = handle;
            this.book = book;
            this.plugin = plugin;
        }

        public string Title => throw new NotImplementedException();

        public IChapter[] Chapters => throw new NotImplementedException();

        public IBook Book => throw new NotImplementedException();
    }
}
