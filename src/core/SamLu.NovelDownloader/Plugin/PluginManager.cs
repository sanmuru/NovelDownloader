using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SamLu.NovelDownloader.Token;

namespace SamLu.NovelDownloader.Plugin
{
	/// <summary>
	/// 插件管理器。
	/// </summary>
    [Export(PluginManager.CONTRACTNAME_PLUGINMANAGER, typeof(IPluginManager))]
    [Export(PluginManager.CONTRACTNAME_NOVELDOWNLOADERPLUGINMANAGER, typeof(INovelDownloaderPluginManager))]
	public class PluginManager : IPluginManager, INovelDownloaderPluginManager
	{
        /// <summary>
        /// <see cref="PluginManager"/> 输出为 <see cref="IPluginManager"/> 时的名称。
        /// </summary>
        public const string CONTRACTNAME_PLUGINMANAGER = "PluginManager";
        /// <summary>
        /// <see cref="PluginManager"/> 输出为 <see cref="INovelDownloaderPluginManager"/> 时的名称。
        /// </summary>
        public const string CONTRACTNAME_NOVELDOWNLOADERPLUGINMANAGER = "NovelDownloaderPluginManager";

		/// <summary>
		/// 已加载的插件字典。
		/// </summary>
		public IDictionary<Guid, IPlugin> Plugins { get; } = new Dictionary<Guid, IPlugin>();

        /// <summary>
        /// 已注册的书籍输出器。
        /// </summary>
        protected internal IDictionary<Guid, IBookWriter> BookWriters { get; } = new Dictionary<Guid, IBookWriter>();

		/// <summary>
		/// 初始化<see cref="PluginManager"/>对象。
		/// </summary>
		public PluginManager() { }

		/// <summary>
		/// 从指定路径的文件中加载插件，并返回所有加载的插件。
		/// </summary>
		/// <param name="pluginFileName">指定路径的文件。</param>
		/// <returns>
		/// <para>如果加载成功，则返回指定路径的文件中加载的插件。</para>
		/// <para>如果未加载成功，则返回 <see langword="null"/> 。</para>
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="pluginFileName"/> 的值为 <see langword="null"/> 。</exception>
		/// <exception cref="FileNotFoundException">参数 <paramref name="pluginFileName"/> 指定的文件路径非法或无效。</exception>
		/// <exception cref="InvalidOperationException">参数 <paramref name="pluginFileName"/> 指定的路径的文件不是 CLI 程序集。</exception>
		/// <exception cref="InvalidOperationException">无法获取插件的实例。</exception>
		public IEnumerable<IPlugin> Load(string pluginFileName)
		{
			if (pluginFileName == null) throw new ArgumentNullException(nameof(pluginFileName));
			if (!File.Exists(pluginFileName)) throw new FileNotFoundException("无法从指定文件中加载插件。", pluginFileName);

			Assembly pluginAssembly = Assembly.LoadFrom(pluginFileName);
			return this.LoadInternal(pluginAssembly).ToList();
		}

        private IEnumerable<IPlugin> LoadInternal(Assembly pluginAssembly)
        {
            var pluginTypes =
                from type in pluginAssembly.GetTypes()
                where typeof(IPlugin).IsAssignableFrom(type)
                where !type.IsAbstract
                select type;

            foreach (var pluginType in pluginTypes)
            {
                IPlugin plugin = Activator.CreateInstance(pluginType) as IPlugin;
                if (plugin == null) throw new InvalidOperationException("无法获取插件的实例。");

                if (!this.Plugins.ContainsKey(plugin.Guid))
                {
                    this.Plugins.Add(plugin.Guid, plugin);

                    // 组装组件。
                    var catalog = new TypeCatalog(typeof(PluginManager), pluginType);
                    var container = new CompositionContainer(catalog);
                    container.ComposeParts(this, plugin);

                    plugin.Initialize(); // 初始化。
                }
                yield return plugin;
            }
        }

		/// <summary>
		/// 释放指定的插件对象。
		/// </summary>
		/// <param name="plugin">指定的插件对象</param>
		public void Release(IPlugin plugin)
		{
			if (this.Plugins.ContainsKey(plugin.Guid))
				this.Plugins.Remove(plugin.Guid);
		}

        /// <summary>
        /// 为指定的小说下载插件注册书籍输出器。
        /// </summary>
        /// <param name="plugin">小说下载插件。</param>
        /// <param name="bookWriter">书籍输出器。</param>
        /// <exception cref="ArgumentNullException"><paramref name="plugin"/> 的值为 <see langword="null"/> 。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="bookWriter"/> 的值为 <see langword="null"/> 。</exception>
        /// <exception cref="InvalidOperationException">已为插件注册了书籍输出器。</exception>
        /// <exception cref="InvalidOperationException">插件未加载。</exception>
        public void RegisterBookWriter(INovelDownloadPlugin plugin, IBookWriter bookWriter)
        {
            if (plugin == null) throw new ArgumentNullException(nameof(plugin));
            if (bookWriter == null) throw new ArgumentNullException(nameof(bookWriter));

            if (this.Plugins.ContainsKey(plugin.Guid))
            {
                if (this.BookWriters.ContainsKey(plugin.Guid))
                    throw new InvalidOperationException("已为插件注册了书籍输出器。");
                else
                    this.BookWriters.Add(plugin.Guid, bookWriter);
            }
            else
                throw new InvalidOperationException("插件未加载。");
        }

        /// <summary>
        /// 寻找合适的书籍输出器，并保存书籍到文件。
        /// </summary>
        /// <param name="bookToken">书籍节点。</param>
        /// <param name="outputDir">输出目录。</param>
        /// <exception cref="ArgumentNullException"><paramref name="bookToken"/> 的值为 <see langword="null"/> 。</exception>
        /// <exception cref="NotSupportedException">未找到支持节点的小说下载插件。</exception>
        /// <exception cref="NotSupportedException">不支持的插件类型。</exception>
        public void SaveTo(NDTBook bookToken, string outputDir)
        {
            if (bookToken == null) throw new ArgumentNullException(nameof(bookToken));

            IBookWriter bookWriter = this.FindBookWriterForBookToken(bookToken);
            // 写入文件。
            bookWriter.Write(bookToken, outputDir);
        }

        /// <summary>
        /// 寻找合适的书籍输出器。
        /// </summary>
        /// <param name="bookToken">书籍节点。</param>
        /// <returns>支持指定书籍节点的书籍输出器。</returns>
        /// <exception cref="NotSupportedException">未找到支持节点的小说下载插件。</exception>
        /// <exception cref="NotSupportedException">不支持的插件类型。</exception>
        protected internal IBookWriter FindBookWriterForBookToken(NDTBook bookToken)
        {
            NovelDownLoadPluginBookTokenAttribute attribute = bookToken.GetType().GetCustomAttribute<NovelDownLoadPluginBookTokenAttribute>(true);
            if (attribute == null)
                throw new NotSupportedException("未找到支持节点的小说下载插件。");

            // 获取已注册的插件实例。
            if (this.Plugins.TryGetValue(attribute.PluginGuid, out IPlugin plugin) && plugin is INovelDownloadPlugin ndPlugin)
            {
                if (!this.BookWriters.TryGetValue(ndPlugin.Guid, out IBookWriter bookWriter)) // 尝试从已注册的书籍输出器中寻找。
                {
                    // 指定插件未注册书籍输出器，采用默认书籍输出器。
                    if (ndPlugin is ILightNovelDownloadPlugin)
                        bookWriter = new LightNovelBookWriter();
                    else
                        bookWriter = new NormalBookWriter();
                }

                // 组装组件。
                var catalog = new TypeCatalog(typeof(NovelDownloadPluginBase), typeof(BookWriterBase), typeof(PluginManager));
                var container = new CompositionContainer(catalog);
                container.ComposeParts(plugin, bookWriter, this);

                return bookWriter;
            }
            else
                throw new NotSupportedException("不支持的插件类型。");
        }

        /// <summary>
        /// 寻找合适的书籍输出器，并异步保存书籍到文件。
        /// </summary>
        /// <param name="bookToken">书籍节点。</param>
        /// <param name="outputDir">输出目录。</param>
        /// <exception cref="ArgumentNullException"><paramref name="bookToken"/> 的值为 <see langword="null"/> 。</exception>
        /// <exception cref="NotSupportedException">未找到支持节点的小说下载插件。</exception>
        /// <exception cref="NotSupportedException">不支持的插件类型。</exception>
        public Task SaveToAsync(NDTBook bookToken, string outputDir)
        {
            if (bookToken == null) throw new ArgumentNullException(nameof(bookToken));

            IBookWriter bookWriter = this.FindBookWriterForBookToken(bookToken);
            // 异步写入文件。
            return bookWriter.WriteAsync(bookToken, outputDir);
        }
    }
}
