using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PluginTestProject
{
    internal static class GlobalSettings
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize()
        {
            Environment.CurrentDirectory = typeof(SamLu.NovelDownloader.Plugin.IPlugin).Assembly.CodeBase;
        }
    }
}
