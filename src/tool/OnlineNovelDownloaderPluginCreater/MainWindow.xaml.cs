using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HtmlAgilityPack;
using SamLu.Web;

namespace NovelDownloader.Tool.OnlineNovelDownloaderPluginCreater
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// 表示全部保存的<see cref="RoutedUICommand"/>对象。
		/// </summary>
		private static readonly RoutedUICommand SaveAllCommand = new RoutedUICommand("Save All", nameof(SaveAllCommand), typeof(MainWindow), 
			new InputGestureCollection()
			{
				new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift)
			}
		);
		
		public MainWindow()
		{
			InitializeComponent();
		}

		private void browse()
		{
			try
			{
				this.wb.Navigate(new Uri(this.txtUrl.Text, UriKind.Absolute));
			}
			catch (Exception e)
			{
				MessageBox.Show(string.Format("{0} : {1}\n{2}", e.GetType().FullName, e.Message, e.StackTrace));
			}
		}

		private void OnWebBrowserDownloaded()
		{
			try
			{
				this._source = this.GetHtmlSource(this.txtUrl.Text);
				this._document = new HtmlDocument();
				this._document.LoadHtml(this._source);
				this._DOM_loaded = false;
				this._SORTED_loaded = false;
				this._SOURCE_loaded = false;

				this.TabControl_SelectionChanged(this.tc, null);
			}
			catch (Exception e)
			{
				MessageBox.Show(string.Format("{0} : {1}\n{2}", e.GetType().FullName, e.Message, e.StackTrace));
			}
		}

		private string GetHtmlSource(string url)
		{
			Encoding encoding = Encoding.UTF8;
			try
			{
				string source = HTML.GetSource(url, encoding);
				Regex charsetRegex = new Regex(@"(charset=""(?<charset>[-\w]*?)"")|(charset=(?<charset>[-\w]*))", RegexOptions.IgnoreCase | RegexOptions.Compiled);
				Match m = charsetRegex.Match(source);
				if (m.Success)
				{
					string charset = m.Groups["charset"].Value.ToLower();
					if (encoding.WebName.ToLower() != charset)
						encoding = Encoding.GetEncoding(charset);
				}
			}
			catch (Exception e)
			{
#if DEBUG
				MessageBox.Show(string.Format("{0} : {1}\n{2}", e.GetType().FullName, e.Message, e.StackTrace));
#endif
				encoding = Encoding.UTF8;
			}

			this._encoding = encoding;
			return HTML.GetSource(url, _encoding);
		}

		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TabControl tc = (TabControl)sender;
			if (!tc.IsLoaded) return;

			TabItem item = tc.SelectedItem as TabItem;
			if (item != null)
			{
				if (item == this.tiHTML_DOM) this.load_DOM();
				else if (item == this.tiHTML_SORTED) this.load_SORTED();
				else if (item == this.tiHTML_SOURCE) this.load_SOURCE();
				else throw new NotSupportedException();
			}

			txtCommand_TextChanged(this.txtCommand, null);
		}

		private void WebBrowser_LoadCompleted(object sender, NavigationEventArgs e)
		{
			Uri uri = e.Uri;
			if (this.txtUrl.Text != uri.ToString()) this.txtUrl.Text = uri.ToString();

			this.OnWebBrowserDownloaded();
		}

		private void btnNavigate_Click(object sender, RoutedEventArgs e)
		{
			this.browse();
		}

		private void txtUrl_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				this.browse();
			}
		}

		HtmlNodeCollection previous_DOM_mapKeys;
		List<TextRange> previous_SORTED_textRanges = new List<TextRange>();
		List<TextRange> previous_SOURCE_textRanges = new List<TextRange>();
		private void txtCommand_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox txt = (TextBox)sender;
			txt.Foreground = getBrush(Colors.Black);

			TabItem item = this.tc.SelectedItem as TabItem;
			if (item == null) return;
			else if (item == this.tiHTML_DOM)
			{
				if (_document == null) return;

				#region tiHTML_DOM
				try
				{
					if (this.previous_DOM_mapKeys != null)
					{
						foreach (HtmlNode node in this.previous_DOM_mapKeys)
						{
							if (this._DOM_map.ContainsKey(node))
								this._DOM_map[node].Background = getBrush(Colors.Transparent);
						}
					}

					if (string.IsNullOrEmpty(txt.Text)) return;

					HtmlNodeCollection nodes = _document.DocumentNode.SelectNodes(txt.Text);

					foreach (HtmlNode node in nodes)
					{
						if (this._DOM_map.ContainsKey(node))
							this._DOM_map[node].Background = getBrush(0xFF, 0x7F, 0xFF);
					}
					this.previous_DOM_mapKeys = nodes;
				}
				catch (Exception)
				{
					txt.Foreground = getBrush(Colors.Red);
				}
				#endregion
			}
			else if (item == this.tiHTML_SORTED)
			{
				#region tiHTML_SOURTED
				try
				{
					foreach (TextRange range in previous_SORTED_textRanges)
					{
						range.ApplyPropertyValue(TextElement.BackgroundProperty, getBrush(Colors.Transparent));
					}

					if (string.IsNullOrEmpty(txt.Text)) return;

					TextRange contentRange = new TextRange(this.fdsvHTML_SORTED.Document.ContentStart, this.fdsvHTML_SORTED.Document.ContentEnd);
					string source = contentRange.Text;
					MatchCollection matches = Regex.Matches(source, txt.Text);

					foreach (Match match in matches)
					{
						int index = 0;
						TextPointer position = this.fdsvHTML_SORTED.Document.ContentStart;
						TextPointer start = null;
						TextPointer end = null;
						while (position != null)
						{
							if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
							{
								string text = position.GetTextInRun(LogicalDirection.Forward);
								if (index <= match.Index && (index + text.Length) > match.Index)
									start = position.GetPositionAtOffset(match.Index - index);
								if (index <= (match.Index + match.Length) && (index + text.Length) > (match.Index + match.Length))
									end = position.GetPositionAtOffset(match.Index + match.Length - index);
								index += text.Length;
							}

							if (start != null && end != null) break;

							position = position.GetNextContextPosition(LogicalDirection.Forward);
						}

						if (start != null && end != null)
						{
							TextRange range = new TextRange(start, end);
							range.ApplyPropertyValue(TextElement.BackgroundProperty, getBrush(0xFF, 0x7F, 0xFF));
							this.previous_SORTED_textRanges.Add(range);
						}
					}
				}
				catch (Exception)
				{
					txt.Foreground = getBrush(Colors.Red);
				}
				#endregion
			}
			else if (item == this.tiHTML_SOURCE)
			{
				if (this._source == null) return;

				#region tiHTML_SOURCE
				try
				{
					foreach (TextRange range in previous_SOURCE_textRanges)
					{
						range.ApplyPropertyValue(TextElement.BackgroundProperty, getBrush(Colors.Transparent));
					}

					if (string.IsNullOrEmpty(txt.Text)) return;

					MatchCollection matches = Regex.Matches(_source.Replace(Environment.NewLine, string.Empty), txt.Text);
					foreach (Match match in matches)
					{
						int index = 0;
						TextPointer position = this.rtbHTML_SOURCE.Document.ContentStart;
						TextPointer start = null;
						TextPointer end = null;
						while (position != null)
						{
							if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
							{
								string text = position.GetTextInRun(LogicalDirection.Forward);
								if (index <= match.Index && (index + text.Length) > match.Index)
									start = position.GetPositionAtOffset(match.Index - index);
								if (index <= (match.Index + match.Length) && (index + text.Length) > (match.Index + match.Length))
									end = position.GetPositionAtOffset(match.Index + match.Length - index);
								index += text.Length;
							}

							if (start != null && end != null) break;

							position = position.GetNextContextPosition(LogicalDirection.Forward);
						}

						if (start != null && end != null)
						{
							TextRange range = new TextRange(start, end);
							range.ApplyPropertyValue(TextElement.BackgroundProperty, getBrush(0xFF, 0x7F, 0xFF));
							this.previous_SOURCE_textRanges.Add(range);
						}
					}
				}
				catch (Exception)
				{
					txt.Foreground = getBrush(Colors.Red);
				}

				#endregion
			}
			else throw new NotSupportedException();
		}

		int xpathNO = 0;
		int regexNO = 0;
		int _DOM_fieldNO = 0;
		int _SORTED_fieldNO = 0;
		int _SOURCE_fieldNO = 0;
		private void txtCommand_KeyUp(object sender, KeyEventArgs e)
		{
			TextBox txt = (TextBox)sender;
			
			if (e.Key == Key.Enter)
			{
				if (string.IsNullOrEmpty(txt.Text)) return;
				
				TabItem item = this.tc.SelectedItem as TabItem;
				if (item == null) return;
				else if (item == this.tiHTML_DOM)
				{
					this.xpathNO++;
					this._DOM_fieldNO++;

					this.addField(string.Format("xpath{0}", this.xpathNO), typeof(string), txt.Text);
					this.addField(string.Format("_DOM_fragment{0}", this._DOM_fieldNO), typeof(HtmlNodeCollection), string.Format("xpath{0}", this.xpathNO));
				}
				else if (item == this.tiHTML_SORTED)
				{
					this.regexNO++;
					this._SORTED_fieldNO++;
					
					this.addField(string.Format("regex{0}", this.regexNO), typeof(string), txt.Text);
					this.addField(string.Format("_SORTED_fragment{0}", this._SORTED_fieldNO), typeof(MatchCollection), string.Format("regex{0}", this.regexNO));
				}
				else if (item == this.tiHTML_SOURCE)
				{
					this.regexNO++;
					this._SOURCE_fieldNO++;
					
					this.addField(string.Format("regex{0}", this.regexNO), typeof(string), txt.Text);
					this.addField(string.Format("_SOURCE_fragment{0}", this._SOURCE_fieldNO), typeof(MatchCollection), string.Format("regex{0}", this.regexNO));
				}
				else throw new NotSupportedException();
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			StartupPage page = new StartupPage();
			if (page.ShowDialog() == true)
			{
				this.Init(page.Name, page.Location);
			}
		}

		private void miBuild_Click(object sender, RoutedEventArgs e)
		{
			this.generateFiles();
		}
	}
}
