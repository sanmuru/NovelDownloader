using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NovelDownloader.Tool.OnlineNovelDownloaderPluginCreater.Controls
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:NovelDownloader.Tool.OnlineNovelDownloaderPluginCreater.Controls"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:NovelDownloader.Tool.OnlineNovelDownloaderPluginCreater.Controls;assembly=NovelDownloader.Tool.OnlineNovelDownloaderPluginCreater.Controls"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误: 
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:HtmlDocumentViewer/>
    ///
    /// </summary>
    public class HtmlDocumentViewer : Control
    {
        #region Document 属性
        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register(
                nameof(Document),
                typeof(HtmlDocument),
                typeof(HtmlDocumentViewer),
                new PropertyMetadata(DocumentPropertyChangedCallback)
            );
        [Description("获取控件的HTML文档对象。")]
        [Category("通用")]
        public HtmlDocument Document
        {
            get => (HtmlDocument)this.GetValue(HtmlDocumentViewer.DocumentProperty);
            set => this.SetValue(HtmlDocumentViewer.DocumentProperty, value);
        }

        private static void DocumentPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HtmlDocumentViewer viewer)
            {
                viewer.OnDocumentUpdated((HtmlDocument)e.OldValue, (HtmlDocument)e.NewValue);
            }
        }

        private void OnDocumentUpdated(HtmlDocument oldValue, HtmlDocument newValue)
        {
            var args = new RoutedPropertyChangedEventArgs<HtmlDocument>(oldValue, newValue, HtmlDocumentViewer.DocumentUpdatedEvent);
            this.RaiseEvent(args);
        }
        #endregion

        #region DocumentUpdated Event
        public static readonly RoutedEvent DocumentUpdatedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(DocumentUpdated),
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<HtmlDocument>),
                typeof(HtmlDocumentViewer)
            );
        public event RoutedPropertyChangedEventHandler<HtmlDocument> DocumentUpdated
        {
            add => this.AddHandler(HtmlDocumentViewer.DocumentUpdatedEvent, value);
            remove => this.RemoveHandler(HtmlDocumentViewer.DocumentUpdatedEvent, value);
        }
        #endregion

        static HtmlDocumentViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HtmlDocumentViewer), new FrameworkPropertyMetadata(typeof(HtmlDocumentViewer)));
        }
    }
}
