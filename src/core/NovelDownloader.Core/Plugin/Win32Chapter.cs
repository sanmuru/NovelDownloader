using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader.Plugin
{
    internal class Win32Chapter : IChapter
    {
        public string Title => throw new NotImplementedException();

        public DateTime UpdateAt => throw new NotImplementedException();

        public IVolume Volume => throw new NotImplementedException();

        public IBook Book => throw new NotImplementedException();
    }
}
