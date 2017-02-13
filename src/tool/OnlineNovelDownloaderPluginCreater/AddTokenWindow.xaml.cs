using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NovelDownloader.Tool.OnlineNovelDownloaderPluginCreater.PropertyNodeItem;

namespace NovelDownloader.Tool.OnlineNovelDownloaderPluginCreater
{
	/// <summary>
	/// AddTokenWindow.xaml 的交互逻辑
	/// </summary>
	public partial class AddTokenWindow : Window
	{
		public string TokenName
		{
			get
			{
				return this.txtName.Text;
			}
		}
		internal TokenType Type { get; private set; }
		
		public AddTokenWindow()
		{
			InitializeComponent();
		}
		
		internal AddTokenWindow(TokenType type) : this()
		{
			this.Type = type;

			TokenTemplateInfoPropertyNodeItem item = typeof(TokenTemplateInfoPropertyNodeItem)?.GetField(type.ToString() + "TokenTemplate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)?.GetValue(typeof(TokenTemplateInfoPropertyNodeItem)) as TokenTemplateInfoPropertyNodeItem;
			if (item != null)
			{
				if (item.DefaultName.Contains("{0}"))
				{
					this.txtName.IsEnabled = false;
					this.setName("(Auto)");
				}
				else
				{
					this.txtName.IsEnabled = true;
					this.setName(item.DefaultName);
				}
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.btnOK.Focus();
		}

		private void btnOK_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}

		bool isNameChanged = false;
		private void txtName_KeyUp(object sender, KeyEventArgs e)
		{
			isNameChanged = true;
		}

		private void txtName_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox txt = (TextBox)sender;
			if (string.IsNullOrEmpty(txt.Text))
			{
				this.btnOK.IsEnabled = false;
			}
			else
			{
				this.btnOK.IsEnabled = true;
			}
		}

		private void lbTokenTemplates_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			ListBox lb = (ListBox)sender;
			TokenTemplateInfoPropertyNodeItem selectedItem = lb?.SelectedItem as TokenTemplateInfoPropertyNodeItem;
			if (selectedItem == null) return;

			this.btnOK_Click(this.btnOK, null);
		}

		private void lbTokenTemplates_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ListBox lb = (ListBox)sender;
			TokenTemplateInfoPropertyNodeItem selectedItem = lb?.SelectedItem as TokenTemplateInfoPropertyNodeItem;
			if (selectedItem == null) return;

			if (!this.isNameChanged) this.setName(selectedItem.DefaultName);
			this.tbDescription.Text = selectedItem.Description;
		}

		private void lbTokenTemplates_Loaded(object sender, RoutedEventArgs e)
		{
			ListBox lb = (ListBox)sender;
			lb.ItemsSource = TokenTemplateInfoPropertyNodeItem.GetTemplateItemsSource();

			if (this.Type != TokenType.Unknown)
			{
				for (int i = 0; i < this.lbTokenTemplates.Items.Count; i++)
				{
					TokenTemplateInfoPropertyNodeItem template = this.lbTokenTemplates.Items[i] as TokenTemplateInfoPropertyNodeItem;
					if (template == null) continue;

					if (this.Type == template.Type)
					{
						this.lbTokenTemplates.SelectedIndex = i;
						break;
					}
				}
			}
		}

		private void setName(string name)
		{
			this.txtName.Text = name;
			this.txtName.SelectionStart = name.Length;
		}
	}
}
