using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NovelDownloader.Token;

namespace NovelDownloader
{
	/// <summary>
	/// 表示所有标志的基类。
	/// </summary>
	public abstract class NDToken
	{
		/// <summary>
		/// 标志的父<see cref="NDToken"/>对象。
		/// </summary>
		public virtual NDToken Parent { get; protected set; }

		/// <summary>
		/// 标志的子<see cref="NDToken"/>对象集合。
		/// </summary>
		public virtual ICollection<NDToken> Children { get; protected set; } = new List<NDToken>();

		/// <summary>
		/// <see cref="NDToken"/>对象的类型。
		/// </summary>
		public virtual string Type { get; protected set; }
		
		/// <summary>
		/// <see cref="NDToken"/>对象的标题。
		/// </summary>
		public virtual string Title { get; set; }

		/// <summary>
		/// <see cref="NDToken"/>对象的描述。
		/// </summary>
		public virtual string Description { get; set; }

		/// <summary>
		/// <see cref="NDToken"/>对象的统一资源标识符。
		/// </summary>
		public virtual Uri Uri { get; protected set; }

		/// <summary>
		/// 当爬虫启动时发生。
		/// </summary>
		public event EventHandler CreepStarted = (sender, e) => { };
		/// <summary>
		/// 当爬虫捕捉到数据时发生。
		/// </summary>
		public event EventHandler<DataEventArgs<object>> CreepFetched = (sender, e) => { };
		/// <summary>
		/// 当爬虫终止时发生。
		/// </summary>
		public event EventHandler CreepFinished = (sender, e) => { };

		/// <summary>
		/// 当爬虫遇到内部错误时发生。
		/// </summary>
		public event EventHandler<DataEventArgs<Exception>> CreepErrored = (sender, e) => { };

		/// <summary>
		/// 初始化<see cref="NDToken"/>对象。
		/// </summary>
		protected NDToken() { }

		/// <summary>
		/// 使用指定的标志类型、标题和说明初始化<see cref="NDToken"/>对象。
		/// </summary>
		/// <param name="type">指定的标志类型。</param>
		/// <param name="title">指定的标题。</param>
		/// <param name="description">指定的说明。</param>
		protected NDToken(string type, string title, string description)
		{
			this.Type = type;
			this.Title = title;
			this.Description = description;
		}

		/// <summary>
		/// 使用指定的统一资源标识符初始化<see cref="NDToken"/>对象。
		/// </summary>
		/// <param name="uri"></param>
		protected NDToken(Uri uri)
		{
			this.Uri = uri;
		}

		/// <summary>
		/// 向子元素集合中添加一个标志。
		/// </summary>
		/// <param name="token"></param>
		/// <exception cref="ArgumentNullException">
		/// 参数<paramref name="token"/>为<see langword="null"/>。
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// 参数<paramref name="token"/>指定的标志的<see cref="Parent"/>父元素已经赋值了另一个<see cref="NDToken"/>对象，且其不等于此<see cref="NDToken"/>对象。
		/// </exception>
		public void Add(NDToken token)
		{
			if (token == null) throw new ArgumentNullException(nameof(token));
			
			if (token.Parent != null && token.Parent != this) throw new InvalidOperationException("标志已经有父元素，无法覆盖。");

			token.Parent = this;
			this.Children.Add(token);
		}

		/// <summary>
		/// 向子元素集合中添加一系列标志。
		/// </summary>
		/// <param name="tokens">一系列标志。</param>
		/// <exception cref="ArgumentNullException">
		/// 参数<paramref name="tokens"/>为<see langword="null"/>。
		/// </exception>
		public void AddRange(IEnumerable<NDToken> tokens)
		{
			if (tokens == null) throw new ArgumentNullException(nameof(tokens));

			foreach (NDToken token in tokens)
				this.Add(token);
		}

		/// <summary>
		/// 从子元素集合中移除指定标志的第一个匹配项。
		/// </summary>
		/// <param name="token">指定标志。</param>
		/// <returns>操作是否成功</returns>
		/// <exception cref="ArgumentNullException">
		/// 参数<paramref name="token"/>为<see langword="null"/>。
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// 参数<paramref name="token"/>指定的标志不存在于<see cref="Children"/>中。
		/// </exception>
		public bool Remove(NDToken token)
		{
			if (token == null) throw new ArgumentNullException(nameof(token));

			if (!this.Children.Contains(token)) throw new InvalidOperationException("子元素集合中不存在对象。");

			return this.Children.Remove(token);
		}
		
		protected bool creepStarted = false;
		protected object syncObj = new object();

		#region StartCreep
		/// <summary>
		/// 可由<see cref="NDTBook"/>的子类型重载的内部调用的检测是否可以启动爬虫的方法。
		/// </summary>
		/// <returns>
		/// <para>是否可以启动爬虫。</para>
		/// <para>为<see langword="true"/>时，表示爬虫可以爬向下一个标志。</para>
		/// <para>为<see langword="false"/>时，表示爬虫不可以爬向下一个标志。</para>
		/// </returns>
		protected virtual bool CanStartCreep()
		{
			return false;
		}

		/// <summary>
		/// 启动爬虫。
		/// </summary>
		public virtual void StartCreep()
		{
			lock (this.syncObj)
			{
				if (!this.creepStarted)
				{
					this.creepStarted = true;
					this.OnCreepStarted(this, new EventArgs());
				}
			}

			bool canStartCreep = false;
			lock (this.syncObj)
			{
				canStartCreep = this.CanStartCreep();
#if DEBUG
				Console.WriteLine("canStartCreep = {0}", canStartCreep);
#endif
			}

			if (canStartCreep)
			{
				this.StartCreepInternal();

				while (true)
				{
					lock (this.syncObj)
					{
						try
						{
							if (!this.CreepInternal()) break;
						}
						catch (Exception)
						{
#if DEBUG
							throw;
#endif
						}
					}
				}
			}

			this.OnCreepFinished(this, new EventArgs());
		}

		/// <summary>
		/// 可由<see cref="NDTBook"/>的子类型重载的内部调用的启动爬虫的方法。
		/// </summary>
		protected virtual void StartCreepInternal() { }
		#endregion

		#region Creep
		/// <summary>
		/// 检测爬虫是否可以爬向下一个标志。
		/// </summary>
		/// <typeparam name="TData">标志数据的类型。</typeparam>
		/// <param name="data">标志数据。</param>
		/// <returns>
		/// <para>爬虫是否可以爬向下一个标志。</para>
		/// <para>为<see langword="true"/>时，表示爬虫可以爬向下一个标志。</para>
		/// <para>为<see langword="false"/>时，表示爬虫不可以爬向下一个标志。</para>
		/// </returns>
		protected virtual bool CanCreep<TData>(TData data)
		{
			return false;
		}

		/// <summary>
		/// 爬虫向下一个标志爬行。并获取采集到的数据。
		/// </summary>
		/// <typeparam name="TData">标志数据的类型。</typeparam>
		/// <typeparam name="TFetch">采集到的数据的类型。</typeparam>
		/// <param name="data">标志数据。</param>
		/// <returns>采集到的数据。</returns>
		public virtual TFetch Creep<TData, TFetch>(TData data)
		{
			this.CreepFetched.Invoke(this, new DataEventArgs<object>(data));
			return default(TFetch);
		}

		/// <summary>
		/// 可由<see cref="NDTBook"/>的子类型重载的内部调用的爬虫爬行的方法。
		/// </summary>
		protected virtual bool CreepInternal()
		{
			if (!this.CanCreep(this)) return false;

			object data = this.Creep<NDToken, object>(this);

			this.OnCreepFetched(this, new DataEventArgs<object>(data));
			return true;
		}
		#endregion

		/// <summary>
		/// 供<see cref="NDToken"/>的派生类型引发<see cref="CreepStarted"/>事件的调用方法。
		/// </summary>
		/// <param name="sender">事件的引发者。</param>
		/// <param name="e">事件的参数。</param>
		protected virtual void OnCreepStarted(object sender, EventArgs e)
		{
			this.CreepStarted.Invoke(sender, e);
		}

		/// <summary>
		/// 供<see cref="NDToken"/>的派生类型引发<see cref="CreepFetched"/>事件的调用方法。
		/// </summary>
		/// <param name="sender">事件的引发者。</param>
		/// <param name="e">事件的参数。</param>
		protected virtual void OnCreepFetched(object sender, DataEventArgs<object> e)
		{
			this.CreepFetched.Invoke(sender, e);
		}

		/// <summary>
		/// 供<see cref="NDToken"/>的派生类型引发<see cref="CreepFetched"/>事件的调用方法。
		/// </summary>
		/// <typeparam name="TData">封装在事件参数中的数据的类型。</typeparam>
		/// <param name="sender">事件的引发者。</param>
		/// <param name="data">封装在事件参数中的数据。</param>
		protected virtual void OnCreepFetched<TData>(object sender, TData data)
		{
			this.OnCreepFetched(sender, (object)data);
		}

		/// <summary>
		/// 供<see cref="NDToken"/>的派生类型引发<see cref="CreepFetched"/>事件的调用方法。
		/// </summary>
		/// <param name="sender">事件的引发者。</param>
		/// <param name="data">封装在事件参数中的数据。</param>
		protected virtual void OnCreepFetched(object sender, object data)
		{
			this.OnCreepFetched(sender, new DataEventArgs<object>(data));
		}

		/// <summary>
		/// 供<see cref="NDToken"/>的派生类型引发<see cref="CreepFinished"/>事件的调用方法。
		/// </summary>
		/// <param name="sender">事件的引发者。</param>
		/// <param name="e">事件的参数。</param>
		protected virtual void OnCreepFinished(object sender, EventArgs e)
		{
			this.CreepFinished.Invoke(sender, e);
		}

		/// <summary>
		/// 供<see cref="NDToken"/>的派生类型引发<see cref="CreepErrored"/>事件的调用方法。
		/// </summary>
		/// <param name="sender">事件的引发者。</param>
		/// <param name="e">事件的参数。</param>
		protected virtual void OnCreepErrored(object sender, DataEventArgs<Exception> e)
		{
			this.CreepErrored.Invoke(sender, e);
		}

		/// <summary>
		/// 供<see cref="NDToken"/>的派生类型引发<see cref="CreepErrored"/>事件的调用方法。
		/// </summary>
		/// <typeparam name="TException">封装在事件参数中的异常的类型。</typeparam>
		/// <param name="sender">事件的引发者。</param>
		/// <param name="exception">封装在事件参数中的异常。</param>
		protected virtual void OnCreepErrored<TException>(object sender, TException exception) where TException : Exception
		{
			this.OnCreepErrored(sender, (Exception)exception);
		}

		/// <summary>
		/// 供<see cref="NDToken"/>的派生类型引发<see cref="CreepErrored"/>事件的调用方法。
		/// </summary>
		/// <param name="sender">事件的引发者。</param>
		/// <param name="exception">封装在事件参数中的异常。</param>
		protected virtual void OnCreepErrored(object sender, Exception exception)
		{
			this.OnCreepErrored(sender, new DataEventArgs<Exception>(exception));
		}
	}
}
