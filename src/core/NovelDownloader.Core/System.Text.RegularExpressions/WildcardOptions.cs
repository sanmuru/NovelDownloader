using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Text.RegularExpressions
{
    /// <summary>
    /// 提供用于设置通配符选项的枚举值。
    /// </summary>
    [Flags]
    public enum WildcardOptions
    {
        /// <summary>
        /// 指定不设置任何选项。 
        /// </summary>
        None = 0,
        /// <summary>
        /// 指定将通配符编译为程序集。 这会产生更快的执行速度，但会增加启动时间。 
        /// </summary>
        Compiled = 1,
        /// <summary>
        /// 指定搜索从右向左而不是从左向右进行。
        /// </summary>
        RightToLeft = 2,
        /// <summary>
        /// 指定忽略语言中的区域性差异。 
        /// </summary>
        CultureInvariant = 4
    }
}
