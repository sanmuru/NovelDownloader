using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NovelDownloader.Plugin
{
	public class Win32PluginLoader : IWin32PluginLoader
	{
		/// <summary>
		/// 已加载的插件字典。
		/// </summary>
		public IDictionary<Guid, IPlugin> Plugins { get; private set; } = new Dictionary<Guid, IPlugin>();

		[DllImport("kernal32.dll")]
		private static extern IntPtr LoadLibrary(string lpFileName);
		
		[DllImport("kernal32.dll")]
		private static extern void FreeLibrary(IntPtr hModule);

		[DllImport("kernal32.dll")]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string lpFileName);
		
		public LoadLibrary LoadLibraryFunc
		{
			get
			{
				return Win32PluginLoader.LoadLibrary;
			}
		}

		public FreeLibrary FreeLibraryFunc
		{
			get
			{
				return Win32PluginLoader.FreeLibrary;
			}
		}

		public GetProcAddress GetProcAddressFunc
		{
			get
			{
				return Win32PluginLoader.GetProcAddress;
			}
		}

		public IEnumerable<IPlugin> Load(string pluginFileName)
		{
			IntPtr hModule = this.LoadLibraryFunc(pluginFileName);
			if (hModule == IntPtr.Zero) throw new Win32Exception(string.Format("无法加载\"{0}\"。", pluginFileName));
			
			DPluginLoadReturnsGuidArray getPluginListFunc;
			Win32Utility.MarshalDelegateFromFunctionPointer(out getPluginListFunc, this.GetProcAddressFunc, hModule, "GetPluginList", "无法获取插件列表。");

			DPluginLoad loadPluginFunc;
			Win32Utility.MarshalDelegateFromFunctionPointer(out loadPluginFunc, this.GetProcAddressFunc, hModule, "LoadPlugin");
			DPluginRelease releasePluginFunc;
			Win32Utility.MarshalDelegateFromFunctionPointer(out releasePluginFunc, this.GetProcAddressFunc, hModule, "ReleasePlugin");

			foreach (Guid pluginGuid in getPluginListFunc())
			{
				IWin32Plugin plugin = new Win32Plugin(this, hModule)
				{
					LoadPlugin = loadPluginFunc,
					ReleasePlugin = releasePluginFunc
				};
				this.Plugins.Add(pluginGuid, plugin);
				yield return plugin;
			}
		}

		private unsafe IEnumerable<Guid> getPluginGuids(IntPtr p_guids)
		{
			throw new Win32Exception();
		}

		public void Release(IPlugin plugin)
		{
			if (this.Plugins.ContainsKey(plugin.Guid))
				this.Plugins.Remove(plugin.Guid);
			
			if (plugin is IWin32Plugin)
			{
				((IWin32Plugin)plugin).Dispose();
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
		// ~Win32PluginLoader() {
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
