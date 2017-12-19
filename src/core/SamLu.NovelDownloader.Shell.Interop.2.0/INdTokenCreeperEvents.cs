using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader.Shell.Interop
{
    [Guid("BCA72A13-583A-4BDC-9B93-FFA0823B6350")]
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INdTokenCreeperEvents
    {
        void OnStarted(
            [MarshalAs(UnmanagedType.Interface)]
            [In] INdToken ndToken);
        void OnFetched(
            [MarshalAs(UnmanagedType.Interface)]
            [In] INdToken ndToken,
            [In] IntPtr pFetched);
        void OnFinished(
            [MarshalAs(UnmanagedType.Interface)]
            [In] INdToken ndToken);
        void OnErrored(
            [MarshalAs(UnmanagedType.Interface)]
            [In] INdToken ndToken,
            [In] int hResult);
    }
}
