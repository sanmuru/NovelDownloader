using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NovelDownloader.Tool.OnlineNovelDownloaderPluginCreater.PropertyNodeItem
{
	internal class TokenTemplateInfoPropertyNodeItem : PropertyNodeItem
	{
		internal static readonly TokenTemplateInfoPropertyNodeItem BookTokenTemplate = new TokenTemplateInfoPropertyNodeItem()
		{
			DisplayName = "书籍节点",
			Type = TokenType.Book,
			Icon = new BitmapImage(new Uri("images/item_icon.png", UriKind.Relative)),
			Description = "一个空的书籍节点。",
			ToolTip = "书籍节点",
			DefaultName = "BookToken{0}"
		};

		internal static readonly TokenTemplateInfoPropertyNodeItem VolumeTokenTemplate = new TokenTemplateInfoPropertyNodeItem()
		{
			DisplayName = "卷节点",
			Type = TokenType.Volume,
			Icon = new BitmapImage(new Uri("images/item_icon.png", UriKind.Relative)),
			Description = "一个空的卷节点。",
			ToolTip = "卷节点",
			DefaultName = "VolumeToken{0}"
		};

		internal static readonly TokenTemplateInfoPropertyNodeItem ChapterTokenTemplate = new TokenTemplateInfoPropertyNodeItem()
		{
			DisplayName = "章节点",
			Type = TokenType.Chapter,
			Icon = new BitmapImage(new Uri("images/item_icon.png", UriKind.Relative)),
			Description = "一个空的章节节点。",
			ToolTip = "章节节点",
			DefaultName = "ChapterToken{0}"
		};

		internal static readonly TokenTemplateInfoPropertyNodeItem TextTokenTemplate = new TokenTemplateInfoPropertyNodeItem()
		{
			DisplayName = "文本节点",
			Type = TokenType.Text,
			Icon = new BitmapImage(new Uri("images/item_icon.png", UriKind.Relative)),
			Description = "一个空的文本节点。",
			ToolTip = "文本节点",
			DefaultName = "TextToken{0}"
		};

		internal static readonly TokenTemplateInfoPropertyNodeItem ImageTokenTemplate = new TokenTemplateInfoPropertyNodeItem()
		{
			DisplayName = "图片节点",
			Type = TokenType.Image,
			Icon = new BitmapImage(new Uri("images/item_icon.png", UriKind.Relative)),
			Description = "一个空的图片节点。",
			ToolTip = "图片节点",
			DefaultName = "ImageToken{0}"
		};

		public string DisplayName { get; set; }
		public TokenType Type { get; set; }
		public ImageSource Icon { get; set; }
		public string Description { get; set; }
		public string ToolTip { get; set; }
		internal string DefaultName { get; set; }

		internal static PropertyNodeItem GetTemplateItemsSource()
		{
			PropertyNodeItem item = new TokenTemplateInfoPropertyNodeItem();
			item.Add(TokenTemplateInfoPropertyNodeItem.BookTokenTemplate);
			item.Add(TokenTemplateInfoPropertyNodeItem.VolumeTokenTemplate);
			item.Add(TokenTemplateInfoPropertyNodeItem.ChapterTokenTemplate);
			item.Add(TokenTemplateInfoPropertyNodeItem.TextTokenTemplate);
			item.Add(TokenTemplateInfoPropertyNodeItem.ImageTokenTemplate);

			return item;
		}
	}
}
