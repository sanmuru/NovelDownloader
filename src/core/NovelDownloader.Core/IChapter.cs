using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader
{
    public interface IChapter
    {
        string Title { get; }
        DateTime UpdateAt { get; }

        IVolume Volume { get; }
        IBook Book { get; }
    }
}
