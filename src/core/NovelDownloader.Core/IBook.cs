using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader
{
    public interface IBook
    {
        string Title { get; }
        string Author { get; }
        string[] Tags { get; }
        string Description { get; }

        IVolume[] Volumes { get; }
    }
}
