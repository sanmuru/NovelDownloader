using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace YueWen.Qidian
{
    public partial class Book
    {
        protected sealed class BookInfo
        {
            public BookInfo(Book book)
            {
                var web = new HtmlWeb();
                var document = web.Load($"https://book.qidian.com/info/{book.id}/");
                var metas =
                    (from eMeta in document.DocumentNode.Element("head").Elements("meta")
                     where eMeta.Attributes.Contains("property")
                     select eMeta)
                    .ToDictionary(
                        eMeta => eMeta.GetAttributeValue("property", null),
                        eMeta => eMeta.GetAttributeValue("content", null)
                    );
            }
        }
    }
}
