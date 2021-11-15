using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader.Plugin
{
    internal class Win32Book : IBook
    {
        protected readonly Win32Plugin plugin;
        protected readonly IntPtr handle;

        protected readonly Lazy<Win32Volume[]> volumes;

        public Win32Book(Uri uri, Win32Plugin plugin)
        {
            this.plugin = plugin;
            this.handle = plugin.wrapper.Activate(plugin.handle, uri.ToString());

            this.volumes = new Lazy<Win32Volume[]>(() =>
                this.plugin.wrapper.Book_GetVolumes(this.handle)
                    .Select(volumeHandle => new Win32Volume(this.handle, this, plugin))
                    .ToArray()
            );
        }

        public string Title => this.plugin.wrapper.Book_GetTitle(this.handle);

        public string Author => this.plugin.wrapper.Book_GetAuthor(this.handle);

        public string[] Tags => this.plugin.wrapper.Book_GetTags(this.handle);

        public string Description => this.plugin.wrapper.Book_GetDescription(this.handle);

        public IVolume[] Volumes => this.volumes.Value;
    }
}
