using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SamLu.NovelDownloader
{
	/// <summary>
	/// 表示产品的版本号。
	/// </summary>
	public class Version : IComparable<Version>, IEquatable<Version>
	{
		/// <summary>
        /// Base 阶段版本。
        /// </summary>
        public const string BaseVersion = "base";
        /// <summary>
        /// Alpha 阶段版本。
        /// </summary>
        public const string AlphaVersion = "alpha";
        /// <summary>
        /// Beta 阶段版本。
        /// </summary>
        public const string BetaVersion = "beta";
        /// <summary>
        /// RC 阶段版本。
        /// </summary>
        public const string RCVersion = "rc";
        /// <summary>
        /// Release 阶段版本。
        /// </summary>
        public const string ReleaseVersion = "release";

		/// <summary>
        /// 默认的最小版本（v0.0.0）。
        /// </summary>
        public static readonly Version MinVersion = new Version(0, 0, 0, null, null);
		
		/// <summary>
		/// 主版本号。
		/// </summary>
		public uint Major { get; set; }
		/// <summary>
		/// 次版本号。
		/// </summary>
		public uint Minor { get; set; }
		/// <summary>
		/// 修订版本号。
		/// </summary>
		public uint Revison { get; set; }
		/// <summary>
		/// 日期版本号。
		/// </summary>
		public string Date { get; set; }
		/// <summary>
		/// 阶段版本号。
		/// </summary>
		public string Period { get; set; }

		/// <summary>
        /// 初始化 <see cref="Version"/> 类的新实例。
        /// </summary>
        public Version() : this(0, 0) { }

		/// <summary>
        /// 初始化 <see cref="Version"/> 类的新实例。此实例指定主、次版本号。
        /// </summary>
        /// <param name="major">主版本号。</param>
        /// <param name="minor">次版本号。</param>
        public Version(uint major, uint minor) : this(major, minor, 0) { }

        /// <summary>
        /// 初始化 <see cref="Version"/> 类的新实例。此实例指定主、次、修订、日期、阶段版本号。
        /// </summary>
        /// <param name="major">主版本号。</param>
        /// <param name="minor">次版本号。</param>
        /// <param name="revison">修订版本号。</param>
        /// <param name="date">日期版本号。</param>
        /// <param name="period">阶段版本号。</param>
        public Version(uint major, uint minor, uint revison, string date = null, string period= null)
		{
			this.Major = major;
			this.Minor = minor;
			this.Revison = revison;
			this.Date = date;
			this.Period = period;
		}

		/// <summary>
		/// 比较两个版本号。
		/// </summary>
		/// <param name="other">另一个版本号。</param>
		/// <returns>两个版本号的先后顺序。</returns>
		public int CompareTo(Version other)
		{
			if (this.Major == other.Major)
			{
				if (this.Minor == other.Minor)
				{
					return this.Revison.CompareTo(other.Revison);
				}
				return this.Minor.CompareTo(other.Minor);
			}
			return this.Major.CompareTo(other.Major);
		}

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        public override bool Equals(object obj)
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
        {
			return ((obj is Version) && this.Equals((Version)obj));
		}

		/// <summary>
		/// 判断两个版本号是否相等。
		/// </summary>
		/// <param name="other">另一个版本号。</param>
		/// <returns>两个版本号是否相等。</returns>
		public bool Equals(Version other)
		{
			return (
				this.Major == other.Major &&
				this.Minor == other.Major &&
				this.Revison == other.Revison
			);
		}

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        public override int GetHashCode()
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
        {
			return this.ToString().GetHashCode();
		}

		/// <summary>
		/// 获取版本号的字符串表示。
		/// </summary>
		/// <returns>版本号的字符串表示。</returns>
		public override string ToString()
		{
			return string.Format("{0}.{1}.{2}{3}",
				this.Major, this.Minor, this.Revison,
				(string.IsNullOrEmpty(this.Date) ?
					string.Empty :
					string.Format(".{0}{1}",
						this.Date,
						(string.IsNullOrEmpty(this.Period) ? string.Empty :
							string.Format("_{0}", this.Period)
						)
					)
				)
			);
		}
	}
}
