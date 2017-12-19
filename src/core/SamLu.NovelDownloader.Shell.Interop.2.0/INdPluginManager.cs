using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader.Shell.Interop
{
    [Guid("6E023AB2-9FD9-40D6-BB7E-891F6EE1E889")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface INdPluginManager
    {
        [return: MarshalAs(UnmanagedType.Error)]
        Constants.HRESULT SearchIn(
            [MarshalAs(UnmanagedType.LPWStr)]
            [In] string pszFilename,
            [MarshalAs(UnmanagedType.Interface)]
            [Out] out INdEnumPluginCLSSIDs ppenums);

        [return: MarshalAs(UnmanagedType.Error)]
        Constants.HRESULT LoadFrom(
            [MarshalAs(UnmanagedType.LPWStr)]
            [In] string pszFilename,
            [In] Guid pClassID,
            [MarshalAs(UnmanagedType.Interface)]
            [Out] out INdPlugin plugin);

        [return: MarshalAs(UnmanagedType.Error)]
        Constants.HRESULT ReleaseInstance(
            [MarshalAs(UnmanagedType.LPWStr)]
            [In] string pszFilename,
            [MarshalAs(UnmanagedType.Interface)]
            [In] INdPlugin plugin);
    }
}
