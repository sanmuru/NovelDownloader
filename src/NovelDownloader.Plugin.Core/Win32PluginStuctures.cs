using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NovelDownloader.Plugin
{
    /// <summary>
    /// 表示 Win32 插件版本的 C++ 结构 VERSION 的映射。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VERSION
    {
        [MarshalAs(UnmanagedType.U4)]
        private uint major;
        [MarshalAs(UnmanagedType.U4)]
        private uint minor;
        [MarshalAs(UnmanagedType.U4)]
        private uint revision;
        private IntPtr date;
        private IntPtr period;

        public uint Major { get => this.major; }
        public uint Minor { get => this.minor; }
        public uint Revision { get => this.revision; }
        public string Date { get => Marshal.PtrToStringUni(this.date); }
        public string Period { get => Marshal.PtrToStringUni(this.period); }
    }
}
