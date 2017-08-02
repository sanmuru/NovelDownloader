using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NovelDownloader.Plugin
{
    /// <summary>
    /// 封装 LoadPlugin 插件 API 的委托类型。
    /// </summary>
    /// <param name="pluginGuid">Win32 插件的全局唯一标识符。</param>
    /// <returns>Win32 插件的句柄。</returns>
    public delegate IntPtr DPluginLoad(Guid pluginGuid);
    /// <summary>
    /// 封装 ReleasePlugin 插件 API 的委托类型。
    /// </summary>
    /// <param name="pluginHandle">Win32 插件的句柄。</param>
    public delegate void DPluginRelease(IntPtr pluginHandle);
    /// <summary>
    /// 封装 GetPluginList 插件 API 的委托类型。
    /// </summary>
    /// <param name="plugins">指向所有 Win32 插件的全局唯一标识符的 C 形式数组首地址的指针。</param>
    /// <returns></returns>
    public delegate int DPluginGetPluginList(out IntPtr plugins);

    /// <summary>
    /// 封装接受一个句柄返回无符号的32位整数的插件 API 的委托类型。
    /// </summary>
    /// <param name="handle">指定的句柄。</param>
    /// <returns>一个无符号的32位整数。</returns>
    public delegate uint DPluginInvocationReturnsUInt32(IntPtr handle);
    /// <summary>
    /// 封装接受一个句柄返回指向字符串首地址的指针的插件 API 的委托类型。
    /// </summary>
    /// <param name="handle">指定的句柄。</param>
    /// <returns>指向字符串首地址的指针。</returns>
    public delegate IntPtr DPluginInvocationReturnsString(IntPtr handle);
    /// <summary>
    /// 封装接受一个句柄返回指向 <see cref="VERSION"/> 的指针的插件 API 的委托类型。
    /// </summary>
    /// <param name="handle">指定的句柄。</param>
    /// <returns>指向 <see cref="VERSION"/> 的指针。</returns>
    public delegate IntPtr DPluginInvocationReturnsVersion(IntPtr handle);
    /// <summary>
    /// 封装接受一个句柄返回全局唯一标识符的插件 API 的委托类型。
    /// </summary>
    /// <param name="handle">指定的句柄。</param>
    /// <returns>一个全局唯一标识符。</returns>
    public delegate Guid DPluginInvocationReturnsGuid(IntPtr handle);
}
