using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader.Plugin
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class NovelDownLoadPluginBookTokenAttribute : Attribute
    {
        private Guid guid;
        /// <summary>
        /// 所在插件的全局唯一标识符。
        /// </summary>
        public Guid PluginGuid => this.guid;

        public NovelDownLoadPluginBookTokenAttribute(string pluginGuid)
        {
            if (pluginGuid == null) throw new ArgumentNullException(nameof(pluginGuid));

            this.guid = new Guid(pluginGuid);
        }
    }
}
