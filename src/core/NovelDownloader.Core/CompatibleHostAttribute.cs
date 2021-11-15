using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class CompatibleHostAttribute : Attribute
    {
        public string Host { get; }

        public CompatibleHostAttribute(string host) => this.Host = host ?? throw new ArgumentNullException(nameof(host));
    }
}
