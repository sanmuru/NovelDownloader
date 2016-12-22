using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SamLu.Collections.Generic;

namespace NovelDownloader.Tool.OnlineNovelDownloaderPluginCreater.PropertyNodeItem
{
	internal abstract class PropertyNodeItem : ICollection<PropertyNodeItem>
	{
		public virtual ICollection<PropertyNodeItem> Children { get; } = new ObservableCollection<PropertyNodeItem>();

		public virtual int Count
		{
			get
			{
				return this.Children.Count;
			}
		}

		public virtual bool IsReadOnly
		{
			get
			{
				return this.Children.IsReadOnly;
			}
		}

		public virtual void Add(PropertyNodeItem item)
		{
			this.Children.Add(item);
		}

		public virtual void Clear()
		{
			this.Children.Clear();
		}

		public virtual bool Contains(PropertyNodeItem item)
		{
			return this.Children.Contains(item);
		}

		public virtual void CopyTo(PropertyNodeItem[] array, int arrayIndex)
		{
			this.Children.CopyTo(array, arrayIndex);
		}

		public virtual IEnumerator<PropertyNodeItem> GetEnumerator()
		{
			return this.Children.GetEnumerator();
		}

		public virtual bool Remove(PropertyNodeItem item)
		{
			return this.Children.Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.Children.GetEnumerator();
		}
	}
}
