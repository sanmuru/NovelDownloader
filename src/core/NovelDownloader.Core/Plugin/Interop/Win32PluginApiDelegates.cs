using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader.Plugin.Interop
{
    #region 功能委托
    /// <summary>
    /// 枚举指针或句柄的回调函数。
    /// </summary>
    /// <param name="value">返回的指针或句柄。</param>
    /// <returns>为False时中止枚举。</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
    public delegate bool IntPtrIterator(IntPtr value);

    /// <summary>
    /// 枚举字符串的回调函数。
    /// </summary>
    /// <param name="value">返回的字符串。</param>
    /// <returns>为False时中止枚举。</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true, ThrowOnUnmappableChar = true)]
    public delegate bool StringIterator(string value);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
    public delegate long ApiGetInt64(IntPtr handle);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true, ThrowOnUnmappableChar = true)]
    public delegate IntPtr ApiGetString(IntPtr handle);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
    public delegate IntPtr ApiGetPointer(IntPtr handle);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
    public delegate int ApiGetArray(IntPtr handle, IntPtrIterator iterator);
    #endregion

    #region 基本API
    /// <summary>
    /// 获取所有插件对象。
    /// </summary>
    /// <param name="host">插件适配的主机地址。</param>
    /// <param name="iterator">返回插件对象句柄的回调函数。</param>
    /// <returns>返回插件对象句柄的数量。</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true, ThrowOnUnmappableChar = true)]
    public delegate int GetAllPlugins(string host, IntPtrIterator iterator);

    /// <summary>
    /// 获取指定插件对象适配的所有主机地址。
    /// </summary>
    /// <param name="pluginHandle">插件对象句柄。</param>
    /// <param name="iterator">返回主机地址的回调函数。</param>
    /// <returns>返回主机地址的数量。</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
    public delegate int GetCompatibleHosts(IntPtr pluginHandle, StringIterator iterator);

    /// <summary>
    /// 使用指定插件对象创建一个书籍对象。
    /// </summary>
    /// <param name="pluginHandle">插件对象句柄</param>
    /// <param name="uri">书籍对象加载的地址。</param>
    /// <returns>使用指定插件对象创建，并加载指定地址后的书籍对象句柄。</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true, ThrowOnUnmappableChar = true)]
    public delegate IntPtr Activate(IntPtr pluginHandle, string uri);

    /// <summary>
    /// 销毁指定插件对象。
    /// </summary>
    /// <param name="pluginHandle"></param>
    /// <returns>指示函数是否执行成功。</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool Deactivate(IntPtr pluginHandle);
    #endregion
}
