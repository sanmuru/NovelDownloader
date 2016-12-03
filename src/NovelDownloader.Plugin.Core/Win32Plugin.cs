using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NovelDownloader.Plugin
{
	internal class Win32Plugin : IWin32Plugin
	{
		/// <summary>
		/// 插件句柄。
		/// </summary>
		protected IntPtr PluginHandle;

		/// <summary>
		/// 加载插件句柄的委托对象。
		/// </summary>
		protected internal UnmanagedResourceLoader LoadPlugin;

		/// <summary>
		/// 释放插件句柄的委托对象。
		/// </summary>
		protected internal UnmanagedResourceReleaser ReleasePlugin;

		public delegate string DPluginInvocationReturnsString(IntPtr pluginHandle);
		/// <summary>
		/// 封装了Win32Dll中获取插件名称的函数。
		/// </summary>
		protected internal DPluginInvocationReturnsString PluginNameFunc;
		/// <summary>
		/// 封装了Win32Dll中获取插件显示在插件管理器中的名称的函数。
		/// </summary>
		protected internal DPluginInvocationReturnsString PluginDisplayNameFunc;

		public delegate Version DPluginInvocationReturnsVersion(IntPtr pluginHandle);
		/// <summary>
		/// 封装了Win32Dll中获取插件版本的函数。
		/// </summary>
		protected internal DPluginInvocationReturnsVersion PluginVersionFunc;
		/// <summary>
		/// 封装了Win32Dll中获取插件支持的最小版本的函数。
		/// </summary>
		protected internal DPluginInvocationReturnsVersion PluginMinVersionFunc;

		/// <summary>
		/// 封装了Win32Dll中获取插件描述的函数。
		/// </summary>
		protected internal DPluginInvocationReturnsString PluginDescriptionFunc;

		protected internal DPluginInvocationReturnsString PluginGuidFunc;

		/// <summary>
		/// 获取插件的名字。
		/// </summary>
		public string Name { get; protected set; }

		/// <summary>
		/// 获取插件显示在插件管理器中的名字
		/// </summary>
		public string DisplayName { get; protected set; }

		/// <summary>
		/// 获取插件的版本
		/// </summary>
		public Version Version { get; protected set; }

		/// <summary>
		/// 获取插件支持的最小版本
		/// </summary>
		public Version MinVersion { get; protected set; }

		/// <summary>
		/// 获取插件的说明
		/// </summary>
		public string Description { get; protected set; }

		/// <summary>
		/// 获取插件的全局唯一标识符。
		/// </summary>
		public Guid Guid { get; protected set; }

		/// <summary>
		/// 初始化<see cref="Win32Plugin"/>对象。
		/// </summary>
		protected internal virtual void Init()
		{
			this.PluginHandle = this.LoadPlugin();

			this.Name = this.PluginNameFunc(this.PluginHandle);
			this.DisplayName = this.PluginDisplayNameFunc(this.PluginHandle);
			this.Version = this.PluginVersionFunc(this.PluginHandle);
			this.MinVersion = this.PluginMinVersionFunc(this.PluginHandle);
			this.Description = this.PluginDescriptionFunc(this.PluginHandle);
			this.Guid = new Guid(this.PluginGuidFunc(this.PluginHandle));
		}

		protected internal Win32Plugin(IWin32PluginLoader pluginLoader, IntPtr moduleHandle)
		{
			const string PluginNameFuncName = "Plugin_Name";
			const string PluginDisplayNameFuncName = "Plugin_DisplayName";
			const string PluginVersionFuncName = "Plugin_Version";
			const string PluginMinVersionFuncName = "Plugin_MinVersion";
			const string PluginDescriptionFuncName = "Plugin_Description";
			const string PluginGuidFuncName = "Plugin_Guid";

			this.setFunc(ref this.PluginNameFunc, pluginLoader, moduleHandle, PluginNameFuncName);
			this.setFunc(ref this.PluginDisplayNameFunc, pluginLoader, moduleHandle, PluginDisplayNameFuncName);
			this.setFunc(ref this.PluginVersionFunc, pluginLoader, moduleHandle, PluginVersionFuncName);
			this.setFunc(ref this.PluginMinVersionFunc, pluginLoader, moduleHandle, PluginMinVersionFuncName);
			this.setFunc(ref this.PluginDescriptionFunc, pluginLoader, moduleHandle, PluginDescriptionFuncName);
			this.setFunc(ref this.PluginGuidFunc, pluginLoader, moduleHandle, PluginGuidFuncName);
		}

		/// <summary>
		/// 将导出函数封装到对应委托字段中。
		/// </summary>
		/// <typeparam name="T">对应委托类型</typeparam>
		/// <param name="func">对应委托字段。</param>
		/// <param name="pluginLoader">插件加载器。</param>
		/// <param name="moduleHandle">导出函数所属的DLL模块句柄。</param>
		/// <param name="funcName">导出函数名称。</param>
		private void setFunc<T>(ref T func, IWin32PluginLoader pluginLoader, IntPtr moduleHandle, string funcName) where T : class
		{
			IntPtr pFunc = pluginLoader.GetProcAddress(funcName);
			if (pFunc == IntPtr.Zero) throw new Win32Exception(string.Format("无法获取导出函数{0}的地址。", funcName));

			T dFunc = Marshal.GetDelegateForFunctionPointer(pFunc, typeof(T)) as T;
			if (dFunc == null) throw new Win32Exception(string.Format("将导出函数{0}的函数指针封装为托管委托对象失败。", funcName));

			func = dFunc;
		}

		#region IDisposable Support
		private bool disposedValue = false; // 要检测冗余调用

		protected internal virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: 释放托管状态(托管对象)。
				}

				// TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
				// TODO: 将大型字段设置为 null。
				this.ReleasePlugin(this.PluginHandle);
				this.PluginHandle = IntPtr.Zero;

				disposedValue = true;
			}
		}

		// TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
		// ~Win32Plugin() {
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
