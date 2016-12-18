using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using HtmlAgilityPack;

namespace OnlineNovelDownloaderPluginCreater
{
	public partial class MainWindow
	{
		private Encoding _encoding = Encoding.UTF8;
		private string _source = null;
		private HtmlDocument _document = null;
		private bool _DOM_loaded = false;
		private bool _SORTED_loaded = false;
		private bool _SOURCE_loaded = false;

		#region LOAD_DOM
		private readonly Dictionary<HtmlNode, TreeViewItem> _DOM_map = new Dictionary<HtmlNode, TreeViewItem>();
		private void load_DOM()
		{
			if (_DOM_loaded) return;
			if (_document == null) return;

			this.tvHTML_DOM.BeginInit();

			this.tvHTML_DOM.FontFamily = new FontFamily("Consolas");
			this.tvHTML_DOM.FontSize = 12F;
			try
			{
				this.load_DOMInternal(_document.DocumentNode, null);
			}
			catch (Exception e)
			{
#if DEBUG
				MessageBox.Show(string.Format("{0} : {1}\n{2}", e.GetType().FullName, e.Message, e.StackTrace));
#endif
				throw;
			}

			this.tvHTML_DOM.EndInit();

			this._DOM_loaded = true;
		}

		private void load_DOMInternal(HtmlNode node, TreeViewItem tvi)
		{
			TreeViewItem new_tvi = new TreeViewItem();

			switch (node.NodeType)
			{
				case HtmlNodeType.Comment:
					HtmlCommentNode commentNode = (HtmlCommentNode)node;
					string outerHtml = commentNode.OuterHtml;

					Color foreColor;
					if (outerHtml.ToUpper().Contains("DOCTYPE"))
					{
						foreColor = Colors.Gray;
						this.tvHTML_DOM.Items.Add(new_tvi);
						this._DOM_map.Add(node, new_tvi);
					}
					else
					{
						return; // DOM树不包含注释。
					}

					new_tvi.Header = new Run(outerHtml) { Foreground = getBrush(foreColor) };
					return;
				case HtmlNodeType.Document:
				case HtmlNodeType.Element:
					break;
				case HtmlNodeType.Text:
					if (tvi == null) return; // DOM树文档不支持文本。

					HtmlTextNode textNode = (HtmlTextNode)node;
					string text = textNode.Text.Trim();
					if (text == string.Empty) return;
					
					new_tvi.Header = text;
					tvi.Items.Add(new_tvi);
					this._DOM_map.Add(node, new_tvi);
					return;
				default:
					throw new InvalidOperationException();
			}

			if (node.NodeType == HtmlNodeType.Element)
			{
				TextBlock tb = new TextBlock();
				tb.Inlines.Add(new Run(string.Format("<{0}", node.Name)) { Foreground = getBrush(Colors.Blue) });

				HtmlAttributeCollection attributes = node.Attributes;

				foreach (HtmlAttribute attribute in attributes)
				{
					tb.Inlines.Add(" ");
					tb.Inlines.Add(new Run(attribute.Name) { Foreground = getBrush(Colors.Red) });
					tb.Inlines.Add("=");
					tb.Inlines.Add(new Run(string.Format("\"{0}\"", HttpUtility.HtmlEncode(attribute.Value))) { Foreground = getBrush(Colors.Purple) });
				}

				tb.Inlines.Add(new Run(">") { Foreground = getBrush(Colors.Blue) });

				new_tvi.Header = tb;

				if (tvi == null)
					this.tvHTML_DOM.Items.Add(new_tvi);
				else
					tvi.Items.Add(new_tvi);
				this._DOM_map.Add(node, new_tvi);
			}

			TreeViewItem childItem = (node.NodeType == HtmlNodeType.Document) ? null : new_tvi;
			foreach (HtmlNode childNode in node.ChildNodes)
				this.load_DOMInternal(childNode, childItem);

			if (node.NodeType == HtmlNodeType.Element)
			{
				TreeViewItem new_tvi_open = new_tvi;
				TreeViewItem new_tvi_close = new TreeViewItem() { Visibility = Visibility.Collapsed };
				if (tvi == null)
					this.tvHTML_DOM.Items.Add(new_tvi_close);
				else
					tvi.Items.Add(new_tvi_close);
				new_tvi_open.Expanded += (sender, e) =>
				{
					TreeViewItem item = (TreeViewItem)sender;
					if (item.Items.Count == 0) return;

					new_tvi_close.Visibility = Visibility.Visible;
				};
				new_tvi_open.Collapsed += (sender, e) =>
				{
					TreeViewItem item = (TreeViewItem)sender;
					if (item.Items.Count == 0) return;

					new_tvi_close.Visibility = Visibility.Collapsed;
				};
				new_tvi_close.SetBinding(
					TreeViewItem.BackgroundProperty,
					new Binding()
					{
						Source = new_tvi_open,
						Path = new PropertyPath(TreeViewItem.BackgroundProperty)
					}
				);
				new_tvi_close.Header = new Run(string.Format("</{0}>", node.Name)) { Foreground = getBrush(Colors.Blue) };

				new_tvi_open.IsExpanded = true;
			}
		}
		#endregion

		#region LOAD_SORTED
		private void load_SORTED()
		{
			if (_SORTED_loaded) return;
			if (_document == null) return;

			this.fdsvHTML_SORTED.BeginInit();
			FlowDocument doc = new FlowDocument();
			this.fdsvHTML_SORTED.Document = doc;
			doc.FontFamily = new FontFamily("Consolas");
			doc.FontSize = 12F;

			doc.BeginInit();
			Paragraph p = new Paragraph();
			doc.Blocks.Add(p);

			this.load_SORTEDInternal(_document.DocumentNode, p);

			doc.EndInit();
			this.fdsvHTML_SORTED.EndInit();

			this._SORTED_loaded = true;
		}

		private void load_SORTEDInternal(HtmlNode node, Paragraph p)
		{
			switch (node.NodeType)
			{
				case HtmlNodeType.Comment:
					HtmlCommentNode commentNode = (HtmlCommentNode)node;
					string outerHtml = commentNode.OuterHtml;
					Color foreColor;
					if (outerHtml.ToUpper().Contains("DOCTYPE"))
						foreColor = Colors.Gray;
					else
						foreColor = Colors.Green;

					p.Inlines.Add(new Run(outerHtml) { Foreground = getBrush(foreColor) });
					return;
				case HtmlNodeType.Document:
				case HtmlNodeType.Element:
					break;
				case HtmlNodeType.Text:
					HtmlTextNode textNode = (HtmlTextNode)node;
					string text = textNode.Text;

					p.Inlines.Add(text);
					return;
				default:
					throw new InvalidOperationException();
			}

			if (node.NodeType == HtmlNodeType.Element)
			{
				p.Inlines.Add(new Run(string.Format("<{0}", node.Name)) { Foreground = getBrush(Colors.Blue) });

				HtmlAttributeCollection attributes = node.Attributes;

				foreach (HtmlAttribute attribute in attributes)
				{
					p.Inlines.Add(" ");
					p.Inlines.Add(new Run(attribute.Name) { Foreground = getBrush(Colors.Red) });
					p.Inlines.Add("=");
					p.Inlines.Add(new Run(string.Format("\"{0}\"", HttpUtility.HtmlEncode(attribute.Value))) { Foreground = getBrush(Colors.Purple) });
				}

				if (node.ChildNodes.Count == 0)
				{
					if (attributes.Count == 0) p.Inlines.Add(" ");
					p.Inlines.Add(new Run("/>") { Foreground = getBrush(Colors.Blue) });
				}
				else
					p.Inlines.Add(new Run(">") { Foreground = getBrush(Colors.Blue) });
			}

			foreach (HtmlNode childNode in node.ChildNodes)
				this.load_SORTEDInternal(childNode, p);

			if (node.NodeType == HtmlNodeType.Element && node.ChildNodes.Count != 0)
				p.Inlines.Add(new Run(string.Format("</{0}>", node.Name)) { Foreground = getBrush(Colors.Blue) });
		}
		#endregion

		#region HTML_SOURCE
		private void load_SOURCE()
		{
			if (this._SOURCE_loaded) return;

			this.rtbHTML_SOURCE.BeginInit();

			FlowDocument doc = new FlowDocument();
			this.rtbHTML_SOURCE.Document = doc;

			this.rtbHTML_SOURCE.AppendText(this._source);

			this.rtbHTML_SOURCE.EndInit();

			this._SOURCE_loaded = true;
		}

		private void load_SOURCEInternal()
		{

		}
		#endregion

		#region BrushDic
		internal static readonly Dictionary<Color, SolidColorBrush> brushDic = new Dictionary<Color, SolidColorBrush>();
		static SolidColorBrush getBrush(Color color)
		{
			if (brushDic.ContainsKey(color))
				return brushDic[color];
			else
			{
				SolidColorBrush brush = new SolidColorBrush(color);
				brushDic.Add(color, brush);

				return brush;
			}
		}
		static SolidColorBrush getBrush(byte a, byte r, byte g, byte b)
		{
			return getBrush(Color.FromArgb(a, r, g, b));
		}
		static SolidColorBrush getBrush(byte r, byte g, byte b)
		{
			return getBrush(0xFF, r, g, b);
		}
		#endregion
		
		private void addField(string field, Type type, string value)
		{
			this.lvFields.Items.Add(new
			{
				Field = field,
				Type = type,
				Value = value
			});
        }
	}
}
