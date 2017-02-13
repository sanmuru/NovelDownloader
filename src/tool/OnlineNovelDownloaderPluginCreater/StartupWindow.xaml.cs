using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NovelDownloader.Tool.OnlineNovelDownloaderPluginCreater
{
	/// <summary>
	/// StartupWindow.xaml 的交互逻辑
	/// </summary>
	public partial class StartupWindow : Window
	{
		public string ProjectName
		{
			get
			{
				return this.txtName.Text;
			}
		}

		public string ProjectLocation
		{
			get
			{
				return this.txtLocation.Text;
			}
		}

		public StartupWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.txtName.Focus();
			this.txtLocation.Text = Environment.CurrentDirectory;
		}

		private void btnBrowse_Click(object sender, RoutedEventArgs e)
		{
			using (FolderBrowserDialog fbd = new FolderBrowserDialog() { SelectedPath = Environment.CurrentDirectory })
			{
				if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					this.txtLocation.Text = fbd.SelectedPath;
				}
			}
		}

		private void btnOK_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}

		private void txtName_TextChanged(object sender, TextChangedEventArgs e)
		{
			System.Windows.Controls.TextBox txt = (System.Windows.Controls.TextBox)sender;
			if (string.IsNullOrEmpty(txt.Text))
			{
				this.btnOK.IsEnabled = false;
			}
			else
			{
				this.btnOK.IsEnabled = true;
			}
		}
	}
}
