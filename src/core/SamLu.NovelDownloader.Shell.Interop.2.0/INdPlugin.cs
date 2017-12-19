using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader.Shell.Interop
{
    [Guid("5C542655-3285-4C71-AD77-2A875ADBEA00")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INdPlugin
    {
        int GetName(out string ppszName);
        int GetDisplayName(out string ppszDisplayName);
        int GetVersion(out uint pnMajor, out uint pnMinor, out uint pnRevison, out string ppszDate, out string ppszPeriod);
        int GetMinVersion(out uint pnMajor, out uint pnMinor, out uint pnRevison, out string ppszDate, out string ppszPeriod);
        int GetDescription(out string ppszDescription);
        int GetClassID(out Guid pClassID);
    }
}
