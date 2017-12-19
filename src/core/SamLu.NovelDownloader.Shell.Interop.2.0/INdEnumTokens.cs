using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader.Shell.Interop
{
    [Guid("97346B38-1432-4C8B-9CF4-622C76DD571C")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INdEnumTokens
    {
        [return: MarshalAs(UnmanagedType.Error)]
        Constants.HRESULT Next(
            [In] uint celt,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.Interface, SizeParamIndex = 0)]
            [Out] INdToken[] rgelt,
            [Out] out uint pceltFetched);

        [return: MarshalAs(UnmanagedType.Error)]
        Constants.HRESULT Skip([In] uint celt);

        [return: MarshalAs(UnmanagedType.Error)]
        Constants.HRESULT Reset();

        void Clone(
            [MarshalAs(UnmanagedType.Interface)]
            [Out] out INdEnumTokens ppenum);
    }
}
