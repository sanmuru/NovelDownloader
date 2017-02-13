using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace NovelDownloader.Tool.OnlineNovelDownloaderPluginCreater.PropertyNodeItem
{
	internal class ProjectPropertyNodeItem : PropertyNodeItem
	{
		public string DisplayName { get; set; }
		public string ToolTip { get; set; }
		public ImageSource Icon { get; set; }
		public ICollection<string> ContextMenu { get; } = new ObservableCollection<string>();
	}
}
