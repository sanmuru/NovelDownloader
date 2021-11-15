using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader
{
    public interface IVolume
    {
        string Title { get; }
        IChapter[] Chapters { get; }

        IBook Book { get; }
    }
}
