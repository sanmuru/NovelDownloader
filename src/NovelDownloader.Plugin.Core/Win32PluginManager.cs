﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NovelDownloader.Plugin
{
    /// <summary>
    /// Win32插件管理器。
    /// </summary>
    public class Win32PluginManager : IWin32PluginManager
    {
        /// <summary>
        /// 已加载的插件字典。
        /// </summary>
        public IDictionary<Guid, IPlugin> Plugins { get; private set; } = new Dictionary<Guid, IPlugin>();
        
        /// <summary>
        /// 加载插件。
        /// </summary>
        /// <param name="pluginFileName">插件所在文件路径。</param>
        /// <returns>指定文件中的所有插件对象的集合。</returns>
        /// <exception cref="ArgumentNullException">
        /// 参数<paramref name="pluginFileName"/>为<see langword="null"/>。
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// 参数<paramref name="pluginFileName"/>指定的文件路径非法或无效。
        /// </exception>
        public IEnumerable<IPlugin> Load(string pluginFileName)
        {
            if (pluginFileName == null) throw new ArgumentNullException(nameof(pluginFileName));
            if (!File.Exists(pluginFileName)) throw new FileNotFoundException("无法从指定文件中加载插件。", pluginFileName);

            IntPtr hModule = Win32Utility.LoadLibrary(pluginFileName);
            if (hModule == IntPtr.Zero) throw new Win32Exception(string.Format("无法加载\"{0}\"。", Path.GetFullPath(pluginFileName)), new Win32Exception(Marshal.GetLastWin32Error()));

            Win32Utility.MarshalDelegateFromFunctionPointer(out DPluginLoad loadPluginFunc, Win32Utility.GetProcAddress, hModule, "LoadPlugin");
            Win32Utility.MarshalDelegateFromFunctionPointer(out DPluginRelease releasePluginFunc, Win32Utility.GetProcAddress, hModule, "ReleasePlugin");

            Win32Utility.MarshalDelegateFromFunctionPointer(out DPluginGetPluginList getPluginListFunc, Win32Utility.GetProcAddress, hModule, "GetPluginList", "无法获取插件列表。");

            int count = getPluginListFunc(out IntPtr guid_array);
            foreach (Guid pluginGuid in PtrToStructureEnumerable<Guid>(guid_array, count))
            {
                if (!this.Plugins.ContainsKey(pluginGuid))
                {
                    IWin32Plugin plugin = new Win32Plugin(hModule)
                    {
                        LoadPlugin = loadPluginFunc,
                        ReleasePlugin = releasePluginFunc,
                        Guid = pluginGuid
                    };
                    plugin.Load();
                    Console.WriteLine(plugin.Name);
                    this.Plugins.Add(pluginGuid, plugin);
                    yield return plugin;
                }
            }
        }

        private IEnumerable<T> PtrToStructureEnumerable<T>(IntPtr ptr, int count)
            where T : struct
        {
            for (int i = 0; i < count; i++)
                yield return (T)Marshal.PtrToStructure(
                    new IntPtr(
                        (IntPtr.Size == Marshal.SizeOf(typeof(int)) ? ptr.ToInt32() : ptr.ToInt64()) +
                        (i * Marshal.SizeOf(typeof(T)))
                    ),
                    typeof(T)
                );
        }

        /// <summary>
        /// 释放指定的插件对象。
        /// </summary>
        /// <param name="plugin">指定的插件对象。</param>
        public void Release(IPlugin plugin)
        {
            if (this.Plugins.ContainsKey(plugin.Guid))
                this.Plugins.Remove(plugin.Guid);

            if (plugin is IWin32Plugin)
            {
                IWin32Plugin win32Plugin = (IWin32Plugin)plugin;
                win32Plugin.Release();
                win32Plugin.Dispose();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~Win32pluginManager() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}