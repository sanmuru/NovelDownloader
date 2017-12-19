using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader.Shell.Interop
{
    [Guid("00A2D56E-E352-4967-BF19-1E5C38B93F4F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INdNovelDownloadPlugin : INdPlugin
    {
        /// <summary>
        /// 获取位于指定的 URI 位置的小说标签。
        /// </summary>
        /// <param name="pszUri">URI 字符串。</param>
        /// <param name="fIsUriAbsolute">一个值，指示 <paramref name="pszUri"/> 的 URI 是否为绝对路径。</param>
        /// <param name="bookToken">获得的书籍节点。</param>
        /// <returns>如果执行成功，返回 <see cref="Constants.HRESULT.S_OK"/> ，否则返回 <see cref="Constants.HRESULT.S_FALSE"/> 。</returns>
        [return: MarshalAs(UnmanagedType.Error)]
        Constants.HRESULT GetBookToken(
            [In] string pszUri,
            [MarshalAs(UnmanagedType.U1)] [In] bool fIsUriAbsolute,
            [MarshalAs(UnmanagedType.Interface)] [Out] out INdToken bookToken);
    }
}
