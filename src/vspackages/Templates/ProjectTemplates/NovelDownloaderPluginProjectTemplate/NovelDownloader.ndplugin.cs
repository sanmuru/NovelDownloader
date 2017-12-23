using SamLu.NovelDownloader;
using SamLu.NovelDownloader.Plugin;
using SamLu.NovelDownloader.Token;
using System;
using System.Collections.Generic;
$if$ ($targetframeworkversion$ >= 3.5)using System.Linq;
$endif$using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel.Composition;

namespace $safeprojectname$
{
    [Export(NovelDownloadPluginBase.CONTRACTNAME_NOVELDOWNLOADPLUGIN, typeof(INovelDownloadPlugin))]
    partial class NovelDownloader : NovelDownloadPluginBase
    {
        internal const string _guidStr = "$guid10$";
        internal static readonly Guid _guid = new Guid(NovelDownloader._guidStr);
        public override Guid Guid => NovelDownloader._guid;
    }
}
