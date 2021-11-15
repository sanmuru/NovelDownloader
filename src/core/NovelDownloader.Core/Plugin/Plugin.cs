using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader.Plugin
{
    public class Plugin : IPlugin
    {
        protected readonly Type tBook;

        public string[] CompatibleHosts =>
            (from CompatibleHostAttribute attribute in this.tBook.GetCustomAttributes(typeof(CompatibleHostAttribute), false)
             select attribute.Host).ToArray();

        public Plugin(Type tBook)
        {
            if (tBook is null) throw new ArgumentNullException(nameof(tBook));

            if (!typeof(IBook).IsAssignableFrom(tBook)) throw new ArgumentException($"类型“{tBook}”不属于书籍类型。");

            this.tBook = tBook;
        }

        public virtual IBook CreateBook(Uri uri)
        {
            if (uri is null) throw new ArgumentNullException(nameof(uri));

            var ctors = this.tBook.GetConstructors().Where(ci => ci.GetCustomAttributes(typeof(BookActivatorAttribute), true).Any());
            var ctor = (ctors.Any() ?
                ctors.FirstOrDefault(ci => {
                    var parameters = ci.GetParameters();
                    return parameters.Length == 1 &&
                        (parameters[0].ParameterType == typeof(Uri) || parameters[0].ParameterType == typeof(string));
                }) : null) ??
                    this.tBook.GetConstructor(new[] { typeof(Uri) }) ??
                    this.tBook.GetConstructor(new[] { typeof(string) });
            if (ctor is null) throw new InvalidOperationException("找不到适合的书籍构造器。");

            var tParam = ctor.GetParameters()[0].ParameterType;
            if (tParam == typeof(Uri))
                return (IBook)ctor.Invoke(new object[] { uri });
            else if (tParam == typeof(string))
                return (IBook)ctor.Invoke(new object[] { uri.ToString() });
            else throw new InvalidOperationException("无法接受的激活参数。");
        }
    }

    public class Plugin<TBook> : Plugin
        where TBook : IBook
    {
        public Plugin() : base(typeof(TBook)) { }
    }
}
