using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SamLu.NovelDownloader.Plugin.Interop;

namespace SamLu.NovelDownloader.Plugin
{
    /// <summary>
    /// 所有描述Win32插件的对象的基类。
    /// </summary>
    public partial class Win32Plugin : IPlugin
    {
        /// <summary>
        /// Win32插件的类文件的模块地址。
        /// </summary>
        protected internal readonly IntPtr handle;
        /// <summary>
        /// 包装Win32插件的类文件的API的包装器。
        /// </summary>
        protected internal readonly ApiWrapper wrapper;

        /// <inheritdoc/>
        public string[] CompatibleHosts => this.wrapper.GetCompatibleHosts(this.handle);

        protected internal Win32Plugin(IntPtr handle, ApiWrapper wrapper)
        {
            if (handle == IntPtr.Zero) throw new ArgumentOutOfRangeException(nameof(handle), handle, "不正确Win32插件对象句柄，因为使用了空指针。");
            if (wrapper is null) throw new ArgumentNullException(nameof(wrapper));

            this.handle = handle;
            this.wrapper = wrapper;
        }

        /// <inheritdoc/>
        public virtual IBook CreateBook(Uri uri)
        {
            if (uri is null) throw new ArgumentNullException(nameof(uri));

            return new Win32Book(uri, this);
        }
    }

    public partial class Win32Plugin
    {
        [DllImport("kernel32.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
        protected static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        /// <summary>
        /// 包装Win32插件的类文件的API的包装器。
        /// </summary>
        protected internal class ApiWrapper : IEquatable<ApiWrapper>, IDisposable
        {
            /// <summary>
            /// Win32插件的模块地址。
            /// </summary>
            protected IntPtr hModule { get; private set; }

            /// <summary>
            /// 获取所有插件对象。
            /// </summary>
            protected readonly GetAllPlugins f_GetAllPlugins;
            /// <summary>
            /// 获取指定插件对象适配的所有主机地址。
            /// </summary>
            protected readonly GetCompatibleHosts f_GetCompatibleHosts;

            /// <summary>
            /// 使用指定插件对象创建一个书籍对象。
            /// </summary>
            protected readonly Activate f_Activate;
            /// <summary>
            /// 销毁指定插件对象。
            /// </summary>
            protected readonly Deactivate f_Deactivate;

            protected readonly ApiGetString f_Book_GetTitle;
            protected readonly ApiGetString f_Book_GetAuthor;
            protected readonly ApiGetArray f_Book_GetTags;
            protected readonly ApiGetString f_Book_GetDescription;
            protected readonly ApiGetArray f_Book_GetVolumes;

            protected readonly ApiGetString f_Volume_GetTitle;
            protected readonly ApiGetArray f_Volume_GetChapters;

            protected readonly ApiGetString f_Chapter_GetTitle;
            protected readonly ApiGetInt64 f_Chapter_GetUpdatedAt;

            public ApiWrapper(IntPtr hModule)
            {
                if (hModule == IntPtr.Zero) throw new ArgumentOutOfRangeException(nameof(hModule), hModule, "无法初始化Win32插件，因为使用了空指针。");

                this.hModule = hModule;

                this.f_GetAllPlugins = Marshal.GetDelegateForFunctionPointer<GetAllPlugins>(GetProcAddress(hModule, "GetAllPlugins"));
                this.f_GetCompatibleHosts = Marshal.GetDelegateForFunctionPointer<GetCompatibleHosts>(GetProcAddress(hModule, "GetCompatibleHosts"));

                this.f_Activate = Marshal.GetDelegateForFunctionPointer<Activate>(GetProcAddress(hModule, "Activate"));
                this.f_Deactivate = Marshal.GetDelegateForFunctionPointer<Deactivate>(GetProcAddress(hModule, "Deactivate"));

                this.f_Book_GetTitle = Marshal.GetDelegateForFunctionPointer<ApiGetString>(GetProcAddress(hModule, "Book_GetTitle"));
                this.f_Book_GetAuthor = Marshal.GetDelegateForFunctionPointer<ApiGetString>(GetProcAddress(hModule, "Book_GetAuthor"));
                this.f_Book_GetTags = Marshal.GetDelegateForFunctionPointer<ApiGetArray>(GetProcAddress(hModule, "Book_GetTags"));
                this.f_Book_GetDescription = Marshal.GetDelegateForFunctionPointer<ApiGetString>(GetProcAddress(hModule, "Book_GetDescription"));
                this.f_Book_GetVolumes = Marshal.GetDelegateForFunctionPointer<ApiGetArray>(GetProcAddress(hModule, "Book_GetVolumes"));

                this.f_Volume_GetTitle = Marshal.GetDelegateForFunctionPointer<ApiGetString>(GetProcAddress(hModule, "Volume_GetTitle"));
                this.f_Volume_GetChapters = Marshal.GetDelegateForFunctionPointer<ApiGetArray>(GetProcAddress(hModule, "Volume_GetChapters"));

                this.f_Chapter_GetTitle = Marshal.GetDelegateForFunctionPointer<ApiGetString>(GetProcAddress(hModule, "Chapter_GetTitle"));
                this.f_Chapter_GetUpdatedAt = Marshal.GetDelegateForFunctionPointer<ApiGetInt64>(GetProcAddress(hModule, "Chapter_GetUpdatedAt"));
            }

            /// <inheritdoc cref="Interop.GetAllPlugins"/>
            public virtual IntPtr[] GetAllPlugins(string host)
            {
                this.Validate(throwException: true);
                List<IntPtr> list = new List<IntPtr>();
                this.f_GetAllPlugins(host, handle => {
                    list.Add(handle);
                    return true;
                });
                return list.ToArray();
            }
            /// <inheritdoc cref="Interop.GetCompatibleHosts"/>
            public virtual string[] GetCompatibleHosts(IntPtr pluginHandle)
            {
                this.Validate(throwException: true);
                List<string> list = new List<string>();
                this.f_GetCompatibleHosts(pluginHandle, host => {
                    list.Add(host);
                    return true;
                });
                return list.ToArray();
            }
            /// <inheritdoc cref="Interop.Activate"/>
            public virtual IntPtr Activate(IntPtr pluginHandle, string uri)
            {
                this.Validate(throwException: true);
                return this.f_Activate(pluginHandle, uri);
            }
            /// <inheritdoc cref="Interop.Deactivate"/>
            public virtual bool Deactivate(IntPtr pluginHandle) => this.Validate(throwException: true) && this.f_Deactivate(pluginHandle);
            public virtual string Book_GetTitle(IntPtr bookHandle)
            {
                this.Validate(throwException: true);
                return Marshal.PtrToStringAuto(this.f_Book_GetTitle(bookHandle));
            }
            public virtual string Book_GetAuthor(IntPtr bookHandle)
            {
                this.Validate(throwException: true);
                return Marshal.PtrToStringAuto(this.f_Book_GetAuthor(bookHandle));
            }
            public virtual string[] Book_GetTags(IntPtr bookHandle)
            {
                this.Validate(throwException: true);
                List<string> list = new List<string>();
                this.f_Book_GetTags(bookHandle, handle => {
                    string tag = Marshal.PtrToStringUni(handle);
                    list.Add(tag);
                    return true;
                });
                return list.ToArray();
            }
            public virtual string Book_GetDescription(IntPtr bookHandle)
            {
                this.Validate(throwException: true);
                return Marshal.PtrToStringAuto(this.f_Book_GetDescription(bookHandle));
            }
            public virtual IntPtr[] Book_GetVolumes(IntPtr bookHandle)
            {
                this.Validate(throwException: true);
                List<IntPtr> list = new List<IntPtr>();
                this.f_Book_GetVolumes(bookHandle, handle => {
                    list.Add(handle);
                    return true;
                });
                return list.ToArray();
            }

            public virtual string Volume_GetTitle(IntPtr volumeHandle)
            {
                this.Validate(throwException: true);
                return Marshal.PtrToStringAuto(this.f_Volume_GetTitle(volumeHandle));
            }
            public virtual IntPtr[] Volume_GetChapters(IntPtr volumeHandle)
            {
                this.Validate(throwException: true);
                List<IntPtr> list = new List<IntPtr>();
                this.f_Volume_GetChapters(volumeHandle, handle => {
                    list.Add(handle);
                    return true;
                });
                return list.ToArray();
            }

            public virtual string Chapter_GetTitle(IntPtr chapterHandle)
            {
                this.Validate(throwException: true);
                return Marshal.PtrToStringAuto(this.f_Chapter_GetTitle(chapterHandle));
            }
            public virtual DateTime Chapter_GetUpdatedAt(IntPtr chapterHandle)
            {
                this.Validate(throwException: true);
                long utcTimestamp = this.f_Chapter_GetUpdatedAt(chapterHandle);
                TimeSpan ts = TimeSpan.FromSeconds(utcTimestamp);
                DateTime start = new DateTime(1970, 1, 1);
                return start + ts;
            }

            /// <inheritdoc/>
            public override bool Equals(object obj) => obj is ApiWrapper wrapper && this.Equals(wrapper);
            /// <inheritdoc/>
            public virtual bool Equals(ApiWrapper other) => this.hModule == other.hModule;

            /// <inheritdoc/>
            public override int GetHashCode() => this.hModule.GetHashCode();

            protected bool Validate(bool throwException = false)
            {
                if (this.hModule == IntPtr.Zero)
                {
                    if (throwException) throw new InvalidOperationException("Win32插件资源已被释放，无法继续操作。");
                    else return false;
                }
                else return true;
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                if (this.Validate(throwException: false)) {
                    if (PluginManager.FreeLibrary(this.hModule)) // 释放成功。
                    {
                        this.hModule = IntPtr.Zero;
                        GC.SuppressFinalize(this);
                    }
                }
            }
        }
    }
}
