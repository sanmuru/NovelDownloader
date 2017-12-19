using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader.Shell.Interop
{
    [Guid("8F553E9A-062D-42B8-A113-A2CF611A6090")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INdEnumPluginCLSSIDs
    {
        [return: MarshalAs(UnmanagedType.Error)]
        Constants.HRESULT Next(
            [In] uint celt,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)]
            [Out] Guid[] rgelt,
            [Out] out uint pceltFetched);

        [return: MarshalAs(UnmanagedType.Error)]
        Constants.HRESULT Skip([In] uint celt);

        [return: MarshalAs(UnmanagedType.Error)]
        Constants.HRESULT Reset();

        void Clone(
            [MarshalAs(UnmanagedType.Interface)]
            [Out] out INdEnumPluginCLSSIDs ppenum);
    }
}
