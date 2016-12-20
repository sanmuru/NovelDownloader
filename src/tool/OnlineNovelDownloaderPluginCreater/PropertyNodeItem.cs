using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace NovelDownloader.Tool.OnlineNovelDownloaderPluginCreater
{
	internal class PropertyNodeItem : ICollection<PropertyNodeItem>
	{
		private ICollection<PropertyNodeItem> children = new List<PropertyNodeItem>();
		public ICollection<PropertyNodeItem> Children
		{
			get
			{
				return this.children;
			}
		}
		public string DisplayName { get; set; }
		public string ToolTip { get; set; }
		public ImageSource Icon { get; set; }

		public int Count
		{
			get
			{
				return this.children.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.children.IsReadOnly;
			}
		}

		public void Add(PropertyNodeItem item)
		{
			this.children.Add(item);
		}

		public void Clear()
		{
			this.children.Clear();
		}

		public bool Contains(PropertyNodeItem item)
		{
			return this.children.Contains(item);
		}

		public void CopyTo(PropertyNodeItem[] array, int arrayIndex)
		{
			this.children.CopyTo(array, arrayIndex);
		}

		public IEnumerator<PropertyNodeItem> GetEnumerator()
		{
			return this.children.GetEnumerator();
		}

		public bool Remove(PropertyNodeItem item)
		{
			return this.children.Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.children.GetEnumerator();
		}
	}
}
