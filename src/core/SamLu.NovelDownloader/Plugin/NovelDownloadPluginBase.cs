using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamLu.NovelDownloader.Token;
using System.ComponentModel.Composition;

namespace SamLu.NovelDownloader.Plugin
{
    [Export(NovelDownloadPluginBase.CONTRACTNAME_NOVELDOWNLOADPLUGIN, typeof(INovelDownloadPlugin))]
    public abstract class NovelDownloadPluginBase : PluginBase, INovelDownloadPlugin
    {
        public const string CONTRACTNAME_NOVELDOWNLOADPLUGIN = "NovelDownloadPlugin";

        [Import(PluginManager.CONTRACTNAME_NOVELDOWNLOADERPLUGINMANAGER, typeof(INovelDownloaderPluginManager), AllowDefault = true)]
        private INovelDownloaderPluginManager manager;
        protected override IPluginManager Manager => this.manager;
        
        public abstract bool TryGetBookToken(Uri uri, out NDTBook bookToken);

        public void RegisterBookWriter(IBookWriter bookWriter)
        {
            this.manager.RegisterBookWriter(this, bookWriter);
        }

        public void SaveTo(NDTBook bookToken, string outputDir)
        {
            this.manager.SaveTo(bookToken, outputDir);
        }
    }
}
