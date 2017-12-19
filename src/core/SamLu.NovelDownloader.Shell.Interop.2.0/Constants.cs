using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader.Shell.Interop
{
    [ComVisible(false)]
    public static class Constants
    {
        [ComVisible(false)]
        public enum HRESULT : int
        {
            /// <summary>
            /// 操作成功。
            /// </summary>
            S_OK = 0x00000000,
            /// <summary>
            /// 操作成功，但是有问题。
            /// </summary>
            S_FALSE = 0x00000001,
            /// <summary>
            /// 操作中止。
            /// </summary>
            E_ABORT = unchecked((int)0x80004004),
            /// <summary>
            /// 拒绝访问。
            /// </summary>
            E_ACCESSDENIED = unchecked((int)0x80070005),
            /// <summary>
            /// 未知错误。
            /// </summary>
            E_FAIL = unchecked((int)0x80004005),
            /// <summary>
            /// 非法句柄。
            /// </summary>
            E_HANDLE = unchecked((int)0x80070006),
            /// <summary>
            /// 一个或多个参数非法。
            /// </summary>
            E_INVALIDARG = unchecked((int)0x80070057),
            /// <summary>
            /// 不支持指定接口。
            /// </summary>
            E_NOINTERFACE = unchecked((int)0x80004002),
            /// <summary>
            /// 非法指针。
            /// </summary>
            E_POINTER = unchecked((int)0x80004003),
            /// <summary>
            /// 未实现。
            /// </summary>
            E_NOTIMPL = unchecked((int)0x80004001)
        }
    }
}
