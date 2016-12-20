using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace NovelDownloader.Tool.OnlineNovelDownloaderPluginCreater
{
	internal class PropertyNodeItem : ICollection<PropertyNodeItem>
	{
		public ICollection<PropertyNodeItem> Children { get; } = new ObservableCollection<PropertyNodeItem>();
		public string DisplayName { get; set; }
		public string ToolTip { get; set; }
		public ImageSource Icon { get; set; }
		public ICollection<string> ContextMenu { get; } = new ObservableCollection<string>();

		public int Count
		{
			get
			{
				return this.Children.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.Children.IsReadOnly;
			}
		}

		public void Add(PropertyNodeItem item)
		{
			this.Children.Add(item);
		}

		public void Clear()
		{
			this.Children.Clear();
		}

		public bool Contains(PropertyNodeItem item)
		{
			return this.Children.Contains(item);
		}

		public void CopyTo(PropertyNodeItem[] array, int arrayIndex)
		{
			this.Children.CopyTo(array, arrayIndex);
		}

		public IEnumerator<PropertyNodeItem> GetEnumerator()
		{
			return this.Children.GetEnumerator();
		}

		public bool Remove(PropertyNodeItem item)
		{
			return this.Children.Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.Children.GetEnumerator();
		}
	}
}
