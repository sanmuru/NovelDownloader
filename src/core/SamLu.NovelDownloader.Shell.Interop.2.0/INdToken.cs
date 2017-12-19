using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader.Shell.Interop
{
    [Guid("5F9D0AB0-00BB-4E9E-8781-2F6C72DBC825")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INdToken
    {
        [return: MarshalAs(UnmanagedType.Error)]
        Constants.HRESULT GetParent(
            [MarshalAs(UnmanagedType.Interface)]
            [Out] out INdToken pParentToken);
        [return: MarshalAs(UnmanagedType.Error)]
        Constants.HRESULT GetChildren(
            [MarshalAs(UnmanagedType.Interface)]
            [Out] out INdEnumTokens ndTokens);
        [return: MarshalAs(UnmanagedType.Error)]
        Constants.HRESULT GetType([Out] out IntPtr ppszType);
        [return: MarshalAs(UnmanagedType.Error)]
        Constants.HRESULT GetTitle([Out] out IntPtr ppszTitle);
        [return: MarshalAs(UnmanagedType.Error)]
        Constants.HRESULT GetDescription([Out] out IntPtr ppszDescription);
        [return: MarshalAs(UnmanagedType.Error)]
        Constants.HRESULT GetUri(
            [Out] out IntPtr ppszUri,
            [MarshalAs(UnmanagedType.U1)] [Out] out bool pfIsUriAbsolute);

        [return: MarshalAs(UnmanagedType.Error)]
        Constants.HRESULT AddToken(
            [MarshalAs(UnmanagedType.Interface)]
            [In] INdToken ndToken);
        [return: MarshalAs(UnmanagedType.Error)]
        Constants.HRESULT RemoveToken(
            [MarshalAs(UnmanagedType.Interface)]
            [In] INdToken ndToken);

        [return: MarshalAs(UnmanagedType.Error)]
        Constants.HRESULT StartCreep();
        [return: MarshalAs(UnmanagedType.Error)]
        Constants.HRESULT Creep(object data, out IntPtr pFetched);
    }
}
