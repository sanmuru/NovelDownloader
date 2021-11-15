using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader
{
    [AttributeUsage(AttributeTargets.Constructor, Inherited = true, AllowMultiple = false)]
    public sealed class BookActivatorAttribute : Attribute
    {
        public BookActivatorAttribute() { }
    }
}
