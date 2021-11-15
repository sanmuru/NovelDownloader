using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.NovelDownloader.Plugin
{
    public class PluginManager
    {
        [DllImport("kernel32.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
        public static extern IntPtr LoadLibrary(string lpLibFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);

        public IEnumerable<IPlugin> Load(string path)
        {
            if (path is null) throw new ArgumentNullException(nameof(path));

            if (File.Exists(path)) {
                AssemblyName name;
                try {
                    name = AssemblyName.GetAssemblyName(path);
                }
                catch (BadImageFormatException) {
                    name = null;
                }

                IEnumerable<IPlugin> plugins;
                if (name is null) plugins = this.LoadWin32(path);
                else plugins = this.LoadDotNet(name);
                if (plugins is null) throw new InvalidOperationException($"无法识别的插件文件“{path}”。");
                
                foreach (var plugin in plugins) yield return plugin;
            }
            else if (Directory.Exists(path)) {
                foreach (var file in Directory.GetFiles(path, "*.dll")) {
                    foreach (var plugin in this.Load(file)) yield return plugin;
                }
            }
            else if (Path.HasExtension(path)) throw new FileNotFoundException("找不到插件文件。", path);
            else throw new DirectoryNotFoundException($"找不到插件文件夹“{path}”。");
        }

        protected virtual IEnumerable<IPlugin> LoadDotNet(AssemblyName name)
        {
            var assembly = Assembly.Load(name);
            return
                from type in assembly.GetExportedTypes()
                where typeof(IBook).IsAssignableFrom(type)
                select new Plugin(type);
        }

        protected virtual IEnumerable<IPlugin> LoadWin32(string path)
        {
            IntPtr hModule = LoadLibrary(path);
            var wrapper = new Win32Plugin.ApiWrapper(hModule);

            return wrapper.GetAllPlugins(null).Select(handle => new Win32Plugin(handle, wrapper));
        }
    }
}
