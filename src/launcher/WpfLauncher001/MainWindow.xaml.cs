using NovelDownloader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfLauncher001
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            var testBookInfo = new BookInfo()
            {
                Title = "ドキドキ♡サンシャイン",
                Author = "Dd!!",
                Description = "SS#42 / a secret of After-school",
                Cover = new Uri("pack://application:,,,/yande.re 352035 drop_dead!! masturbation minase_shuu nopan pussy_juice seifuku.jpg", UriKind.Absolute)
            };
            foreach (var token in Enumerable.Range(1, 20).Select(i => new ChapterInfo() { Title = $"第{i}话　{string.Join(string.Empty, Enumerable.Repeat("哈", new Random().Next(5, 8)))}" }))
                testBookInfo.Chapters.Add(token);

            NovelDownloader.Plugin.wenku8.com.QingXiaoShuoWenKu_NovelDownloader downloader = new NovelDownloader.Plugin.wenku8.com.QingXiaoShuoWenKu_NovelDownloader();
            var novelBookInfo = new BookInfo(downloader.GetBookToken(@"http://www.wenku8.com/book/1695.htm"));
            this.DataContext = novelBookInfo;
        }

        private void chapter_Click(object sender, MouseButtonEventArgs e)
        {
            var token = ((Border)sender).Tag as NDToken;
            MessageBox.Show(token?.Uri?.ToString() ?? "<null>");
        }
    }
}
