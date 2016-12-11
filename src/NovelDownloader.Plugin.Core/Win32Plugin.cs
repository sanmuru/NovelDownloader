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
		protected internal DPluginLoad LoadPlugin;
		/// <summary>
		/// 释放插件句柄的委托对象。
		/// </summary>
		protected internal DPluginRelease ReleasePlugin;

		/// <summary>
		/// 封装了Win32Dll中获取插件名称的函数。
		/// </summary>
		protected internal DPluginInvocationReturnsString PluginNameFunc;
		/// <summary>
		/// 封装了Win32Dll中获取插件显示在插件管理器中的名称的函数。
		/// </summary>
		protected internal DPluginInvocationReturnsString PluginDisplayNameFunc;
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
		/// <summary>
		/// 封装了Win32Dll中获取插件全局唯一标识符的函数。
		/// </summary>
		protected internal DPluginInvocationReturnsGuid PluginGuidFunc;

		/// <summary>
		/// 获取插件的名字。
		/// </summary>
		public string Name
		{
			get
			{
				return this.PluginNameFunc(this.PluginHandle);
			}
		}

		/// <summary>
		/// 获取插件显示在插件管理器中的名字
		/// </summary>
		public string DisplayName
		{
			get
			{
				return this.PluginDisplayNameFunc(this.PluginHandle);
			}
		}

		/// <summary>
		/// 获取插件的版本
		/// </summary>
		public Version Version
		{
			get
			{
				return this.PluginVersionFunc(this.PluginHandle);
			}
		}

		/// <summary>
		/// 获取插件支持的最小版本
		/// </summary>
		public Version MinVersion
		{
			get
			{
				return this.PluginMinVersionFunc(this.PluginHandle);
			}
		}

		/// <summary>
		/// 获取插件的说明
		/// </summary>
		public string Description
		{
			get
			{
				return this.PluginDescriptionFunc(this.PluginHandle);
			}
		}

		/// <summary>
		/// 获取插件的全局唯一标识符。
		/// </summary>
		public Guid Guid
		{
			get
			{
				return this.PluginGuidFunc(this.PluginHandle);
			}
		}

		/// <summary>
		/// 初始化<see cref="Win32Plugin"/>对象。
		/// </summary>
		protected internal virtual void Init()
		{
			this.PluginHandle = this.LoadPlugin(this.Guid);
		}

		protected internal Win32Plugin(IWin32PluginLoader pluginLoader, IntPtr moduleHandle)
		{
			const string PluginNameFuncName = "Plugin_Name";
			const string PluginDisplayNameFuncName = "Plugin_DisplayName";
			const string PluginVersionFuncName = "Plugin_Version";
			const string PluginMinVersionFuncName = "Plugin_MinVersion";
			const string PluginDescriptionFuncName = "Plugin_Description";
			const string PluginGuidFuncName = "Plugin_Guid";

			Win32Utility.MarshalDelegateFromFunctionPointer(out this.PluginNameFunc, pluginLoader.GetProcAddressFunc, moduleHandle, PluginNameFuncName);
			Win32Utility.MarshalDelegateFromFunctionPointer(out this.PluginDisplayNameFunc, pluginLoader.GetProcAddressFunc, moduleHandle, PluginDisplayNameFuncName);
			Win32Utility.MarshalDelegateFromFunctionPointer(out this.PluginVersionFunc, pluginLoader.GetProcAddressFunc, moduleHandle, PluginVersionFuncName);
			Win32Utility.MarshalDelegateFromFunctionPointer(out this.PluginMinVersionFunc, pluginLoader.GetProcAddressFunc, moduleHandle, PluginMinVersionFuncName);
			Win32Utility.MarshalDelegateFromFunctionPointer(out this.PluginDescriptionFunc, pluginLoader.GetProcAddressFunc, moduleHandle, PluginDescriptionFuncName);
			Win32Utility.MarshalDelegateFromFunctionPointer(out this.PluginGuidFunc, pluginLoader.GetProcAddressFunc, moduleHandle, PluginGuidFuncName);

			this.Init();
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

				this.LoadPlugin = null;
				this.ReleasePlugin = null;
				this.PluginNameFunc = null;
				this.PluginDisplayNameFunc = null;
				this.PluginVersionFunc = null;
				this.PluginMinVersionFunc = null;
				this.PluginDescriptionFunc = null;
				this.PluginGuidFunc = null;

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
