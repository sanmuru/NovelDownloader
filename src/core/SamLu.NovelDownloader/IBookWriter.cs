using SamLu.NovelDownloader.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader
{
    public interface IBookWriter
    {
        void Write(NDTBook bookToken, string outputDir);
    }
}
