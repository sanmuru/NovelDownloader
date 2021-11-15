using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader
{
    /// <summary>
    /// 表示找不到书籍时发生的错误。
    /// </summary>
    public class BookNotFoundException : Exception
    {
        /// <summary>
        /// 初始化 <see cref="BookNotFoundException"/> 类的实例。
        /// </summary>
        public BookNotFoundException() { }

        /// <summary>
        /// 用指定的错误消息初始化 <see cref="BookNotFoundException"/> 类的实例。
        /// </summary>
        /// <inheritdoc/>
        public BookNotFoundException(string message) : base(message) { }

        /// <summary>
        /// 使用指定的错误消息和对作为此异常原因的内部异常的引用来初始化 <see cref="BookNotFoundException"/> 类的实例。
        /// </summary>
        /// <inheritdoc/>
        public BookNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
