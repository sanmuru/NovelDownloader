using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml;

namespace NovelDownloader.Tool.OnlineNovelDownloaderPluginCreater
{
	partial class MainWindow
	{
		private string projectName;
		private string projectLocation;
		PropertyNodeItem item = new PropertyNodeItem();
		private string language_extension = ".cs";
		PropertyNodeItem tokens_item;

		private void Init(string name, string location)
		{
			this.projectName = name;
			this.projectLocation = Path.Combine(location, name);

			this.pluginName = name;



			this.gdMain.IsEnabled = true;
			this.gdMain.Visibility = Visibility.Visible;

			PropertyNodeItem project_item = new PropertyNodeItem();
			item.Add(project_item);
			project_item.DisplayName = this.projectName;
			project_item.Icon = new BitmapImage(new Uri("images/project_icon.png", UriKind.Relative));
			project_item.ToolTip = Path.Combine(this.projectLocation, this.projectName + this.language_extension);

			//PropertyNodeItem properties_item = new PropertyNodeItem();
			//project_item.Add(properties_item);
			//properties_item.DisplayName = "Properties";
			//properties_item.ToolTip = Path.Combine(this.projectLocation, "Properties");

			//PropertyNodeItem assemblyinfo_item = new PropertyNodeItem();
			//properties_item.Add(assemblyinfo_item);
			//assemblyinfo_item.DisplayName = "AssemblyInfo";
			//assemblyinfo_item.Icon = new BitmapImage(new Uri("images/item_icon.png", UriKind.Relative));
			//assemblyinfo_item.ToolTip = Path.Combine(this.projectLocation, "AssemblyInfo" + language_extension);

			PropertyNodeItem references_item = new PropertyNodeItem();
			project_item.Add(references_item);
			references_item.DisplayName = "引用";
			references_item.ToolTip = "References";

			tokens_item = new PropertyNodeItem();
			project_item.Add(tokens_item);
			tokens_item.DisplayName = "Tokens";
			tokens_item.Icon = new BitmapImage(new Uri("images/token_icon.png", UriKind.Relative));
			tokens_item.ToolTip = "Tokens";



			this.tvTokenManager.ItemsSource = item;
		}

		private void generateFiles()
		{
			if (!Directory.Exists(this.projectLocation)) Directory.CreateDirectory(this.projectLocation);

			this.generateProjectFile();
			this.generateAssemblyInfo();
			this.generateTokens();
		}

		private void generateProjectFile()
		{
			string path = Path.Combine(this.projectLocation, string.Format("{0}{1}proj", this.projectName, this.language_extension));
#if !DEBUG
			if (File.Exists(path))
			{
				if (MessageBox.Show(string.Format("文件 \"{0}\" 已存在，是否覆盖它？", path)) == MessageBoxResult.No) return;
			}
#endif

			XmlDocument doc = new XmlDocument();
			doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));

			XmlElement project_element = doc.CreateElement("Project");
			doc.AppendChild(project_element);
			project_element.SetAttribute("ToolsVersion", "14.0");
			project_element.SetAttribute("DefaultTargets", "Build");
			project_element.SetAttribute("xmlns", @"http://schemas.microsoft.com/developer/msbuild/2003");

			#region Import1
			XmlElement import_element1 = doc.CreateElement("Import");
			project_element.AppendChild(import_element1);
			import_element1.SetAttribute("Project",
				string.Join(Path.DirectorySeparatorChar.ToString(), new[]
				{
					GetMSBuildMacro("MSBuildExtensionsPath"), GetMSBuildMacro("MSBuildToolsVersion"), "Microsoft.Common.props"
				})
			);
			import_element1.SetAttribute("Condition",
				string.Format("Exists('{0}')",
					string.Join(Path.DirectorySeparatorChar.ToString(), new[]
					{
						GetMSBuildMacro("MSBuildExtensionsPath"), GetMSBuildMacro("MSBuildToolsVersion"), "Microsoft.Common.props"
					})
				)
			);
			#endregion

			#region PropertyGroup1
			XmlElement propertygroup_element1 = doc.CreateElement("PropertyGroup");
			project_element.AppendChild(propertygroup_element1);

			XmlElement configuration_element = doc.CreateElement("Configuration");
			propertygroup_element1.AppendChild(configuration_element);
			configuration_element.SetAttribute("Condition", string.Format(" '{0}' == '{1}' ", GetMSBuildMacro("Configuration"), string.Empty));
			configuration_element.AppendChild(doc.CreateTextNode("Debug"));

			XmlElement platform_element = doc.CreateElement("Platform");
			propertygroup_element1.AppendChild(platform_element);
			platform_element.SetAttribute("Condition", string.Format(" '{0}' == '{1}' ", GetMSBuildMacro("Platform"), string.Empty));
			platform_element.AppendChild(doc.CreateTextNode("AnyCPU"));

			XmlElement projectguid_element = doc.CreateElement("ProjectGuid");
			propertygroup_element1.AppendChild(projectguid_element);
			projectguid_element.AppendChild(doc.CreateTextNode(Guid.NewGuid().ToString("B")));

			XmlElement outputtype_element = doc.CreateElement("OutputType");
			propertygroup_element1.AppendChild(outputtype_element);
			outputtype_element.AppendChild(doc.CreateTextNode("Library"));

			XmlElement appdesignerfolder_element = doc.CreateElement("AppDesignerFolder");
			propertygroup_element1.AppendChild(appdesignerfolder_element);
			appdesignerfolder_element.AppendChild(doc.CreateTextNode("Properties"));

			XmlElement rootnamespace_element = doc.CreateElement("RootNamespace");
			propertygroup_element1.AppendChild(rootnamespace_element);
			rootnamespace_element.AppendChild(doc.CreateTextNode("NovelDownloader.Plugin." + this.pluginName));

			XmlElement assemblyname_element = doc.CreateElement("AssemblyName");
			propertygroup_element1.AppendChild(assemblyname_element);
			assemblyname_element.AppendChild(doc.CreateTextNode(this.projectName));

			XmlElement targetframeworkversion_element = doc.CreateElement("TargetFrameworkVersion");
			propertygroup_element1.AppendChild(targetframeworkversion_element);
			targetframeworkversion_element.AppendChild(doc.CreateTextNode("v3.5"));

			XmlElement filealignment_element = doc.CreateElement("FileAlignment");
			propertygroup_element1.AppendChild(filealignment_element);
			filealignment_element.AppendChild(doc.CreateTextNode("512"));
			#endregion

			#region PropertyGroup2
			XmlElement propertygroup_element2 = doc.CreateElement("PropertyGroup");
			project_element.AppendChild(propertygroup_element2);
			propertygroup_element2.SetAttribute("Condition", string.Format(" '{0}|{1}' == '{2}|{3}' ", GetMSBuildMacro("Configuration"), GetMSBuildMacro("Platform"), "Debug", "AnyCPU"));

			XmlElement debugsymbols_element1 = doc.CreateElement("DebugSymbols");
			propertygroup_element2.AppendChild(debugsymbols_element1);
			debugsymbols_element1.AppendChild(doc.CreateTextNode("true"));

			XmlElement debugtype_element1 = doc.CreateElement("DebugType");
			propertygroup_element2.AppendChild(debugtype_element1);
			debugtype_element1.AppendChild(doc.CreateTextNode("full"));

			XmlElement optimize_element1 = doc.CreateElement("Optimize");
			propertygroup_element2.AppendChild(optimize_element1);
			optimize_element1.AppendChild(doc.CreateTextNode("False"));

			XmlElement outputpath_element1 = doc.CreateElement("OutputPath");
			propertygroup_element2.AppendChild(outputpath_element1);
			outputpath_element1.AppendChild(doc.CreateTextNode(
				string.Join(Path.DirectorySeparatorChar.ToString(), new[] { "bin", "Debug", string.Empty })
			));

			XmlElement defineconstants_element1 = doc.CreateElement("DefineConstants");
			propertygroup_element2.AppendChild(defineconstants_element1);
			defineconstants_element1.AppendChild(doc.CreateTextNode(
				string.Join(";", new[] { "DEBUG", "TRACE" })
			));

			XmlElement errorreport_element1 = doc.CreateElement("ErrorReport");
			propertygroup_element2.AppendChild(errorreport_element1);
			errorreport_element1.AppendChild(doc.CreateTextNode("prompt"));

			XmlElement warninglevel_element1 = doc.CreateElement("WarningLevel");
			propertygroup_element2.AppendChild(warninglevel_element1);
			warninglevel_element1.AppendChild(doc.CreateTextNode("4"));

			XmlElement documentationfile_element1 = doc.CreateElement("DocumentationFile");
			propertygroup_element2.AppendChild(documentationfile_element1);
			documentationfile_element1.AppendChild(doc.CreateTextNode(
				string.Join(Path.DirectorySeparatorChar.ToString(), new[] { "bin", "Debug", this.projectName + ".XML" })
			));
			#endregion

			#region PropertyGroup3
			XmlElement propertygroup_element3 = doc.CreateElement("PropertyGroup");
			project_element.AppendChild(propertygroup_element3);
			propertygroup_element3.SetAttribute("Condition", string.Format(" '{0}|{1}' == '{2}|{3}' ", GetMSBuildMacro("Configuration"), GetMSBuildMacro("Platform"), "Release", "AnyCPU"));

			XmlElement debugtype_element2 = doc.CreateElement("DebugType");
			propertygroup_element3.AppendChild(debugtype_element2);
			debugtype_element2.AppendChild(doc.CreateTextNode("pdbonly"));

			XmlElement optimize_element2 = doc.CreateElement("Optimize");
			propertygroup_element3.AppendChild(optimize_element2);
			optimize_element2.AppendChild(doc.CreateTextNode("true"));

			XmlElement outputpath_element2 = doc.CreateElement("OutputPath");
			propertygroup_element3.AppendChild(outputpath_element2);
			outputpath_element2.AppendChild(doc.CreateTextNode(
				string.Join(Path.DirectorySeparatorChar.ToString(), new[] { "bin", "Release", string.Empty })
			));

			XmlElement defineconstants_element2 = doc.CreateElement("DefineConstants");
			propertygroup_element3.AppendChild(defineconstants_element2);
			defineconstants_element2.AppendChild(doc.CreateTextNode(
				string.Join(";", new[] { "TRACE" })
			));

			XmlElement errorreport_element2 = doc.CreateElement("ErrorReport");
			propertygroup_element3.AppendChild(errorreport_element2);
			errorreport_element2.AppendChild(doc.CreateTextNode("prompt"));

			XmlElement warninglevel_element2 = doc.CreateElement("WarningLevel");
			propertygroup_element3.AppendChild(warninglevel_element2);
			warninglevel_element2.AppendChild(doc.CreateTextNode("4"));

			XmlElement documentationfile_element2 = doc.CreateElement("DocumentationFile");
			propertygroup_element3.AppendChild(documentationfile_element2);
			documentationfile_element2.AppendChild(doc.CreateTextNode(
				string.Join(Path.DirectorySeparatorChar.ToString(), new[] { "bin", "Release", this.projectName + ".XML" })));
			#endregion

			#region ItemGroup1
			XmlElement itemgroup_element1 = doc.CreateElement("ItemGroup");
			project_element.AppendChild(itemgroup_element1);

			var assemblies = new[]
			{
				typeof(SamLu.Web.HTML).Assembly
			};
			var references = assemblies.Select(assembly => assembly.FullName)
			.Concat(new[]
			{
				"System",
				"System.Core",
				"System.Web",
				"System.Xml.Linq",
				"System.Data.DataSetExtensions",
				"System.Data",
				"System.Xml"
			}).Select(reference =>
			{
				XmlElement reference_item = doc.CreateElement("Reference");
				itemgroup_element1.AppendChild(reference_item);
				reference_item.SetAttribute("Include", reference);

				return reference_item;
			}).ToArray();

			for (int i = 0; i < assemblies.Length; i++)
			{
				XmlElement assembly_element = references.ElementAt(i);

				XmlElement specificversion_element = doc.CreateElement("SpecificVersion");
				assembly_element.AppendChild(specificversion_element);
				specificversion_element.AppendChild(doc.CreateTextNode("false"));

				XmlElement hintpath_element = doc.CreateElement("HintPath");
				assembly_element.AppendChild(hintpath_element);
				hintpath_element.AppendChild(doc.CreateTextNode(Path.GetDirectoryName(assemblies[i].EscapedCodeBase)));
			}
			#endregion

			#region ItemGroup2
			XmlElement itemgroup_element2 = doc.CreateElement("ItemGroup");
			project_element.AppendChild(itemgroup_element2);

			var compile_includes = new[]
			{
				Path.Combine("Properties", "AssemblyInfo" + this.language_extension)
			}.Concat(
				this.tokens_item.Select(token => token.DisplayName + this.language_extension)
			).ToList();
			compile_includes.ForEach(compile_include =>
			{
				XmlElement compile_item = doc.CreateElement("Compile");
				itemgroup_element2.AppendChild(compile_item);
				compile_item.SetAttribute("Include", compile_include);
			});
			#endregion

			#region Import2
			XmlElement import_element2 = doc.CreateElement("Import");
			project_element.AppendChild(import_element2);
			import_element2.SetAttribute("Project", Path.Combine(GetMSBuildMacro("MSBuildToolsPath"), "Microsoft.CSharp.targets"));
			#endregion

			doc.Save(path);
		}

		private static string GetMSBuildMacro(string name)
		{
			return string.Format("$({0})", name);
		}

		string pluginName;
		string pluginDescription;
		string pluginCompany = "Sam Lu";
		string pluginCopyright = string.Format("Copyright © Sam Lu 1997 - 2016");
		string pluginTrademark = string.Format("Copyright ® Sam Lu 1997 - 2016");
		Version pluginVersion = new Version(0, 0, 0, 0);

		private void generateAssemblyInfo()
		{
			string Properties_dir = Path.Combine(this.projectLocation, "Properties");
			if (!Directory.Exists(Properties_dir)) Directory.CreateDirectory(Properties_dir);

			string path = Path.Combine(Properties_dir, string.Format("{0}{1}", "AssemblyInfo", this.language_extension));
#if !DEBUG
			if (File.Exists(path))
			{
				if (MessageBox.Show(string.Format("文件 \"{0}\" 已存在，是否覆盖它？", path)) == MessageBoxResult.No) return;
			}
#endif

			string pattern = @"using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

// 有关程序集的一般信息由以下
// 控制。更改这些特性值可修改
// 与程序集关联的信息。
[assembly: AssemblyTitle(""{0}"")]
[assembly: AssemblyDescription(""{1}"")]
[assembly: AssemblyConfiguration("""")]
[assembly: AssemblyCompany(""{2}"")]
[assembly: AssemblyProduct(""{0}"")]
[assembly: AssemblyCopyright(""{3}"")]
[assembly: AssemblyTrademark(""{4}"")]
[assembly: AssemblyCulture("""")]

//将 ComVisible 设置为 false 将使此程序集中的类型
//对 COM 组件不可见。  如果需要从 COM 访问此程序集中的类型，
//请将此类型的 ComVisible 特性设置为 true。
[assembly: ComVisible(false)]

// 程序集的版本信息由下列四个值组成: 
//
//      主版本
//      次版本
//      生成号
//      修订号
//
//可以指定所有这些值，也可以使用“生成号”和“修订号”的默认值，
// 方法是按如下所示使用“*”: :
// [assembly: AssemblyVersion(""1.0.*"")]
[assembly: AssemblyVersion(""{5}"")]
[assembly: AssemblyFileVersion(""{6}"")]
";
			string contents = string.Format(pattern,
				this.pluginName,
				this.pluginDescription,
				this.pluginCompany,
				this.pluginCopyright,
				this.pluginTrademark,
				this.pluginVersion.ToString(4),
				this.pluginVersion.ToString(4)
			);
			File.WriteAllText(path, contents);
		}

		private void generateTokens()
		{
			foreach (var item in this.tokens_item.OfType<TokenPropertyNodeItem>())
			{
				string path = Path.Combine(this.projectLocation, string.Format("{0}{1}", item.DisplayName, this.language_extension));
#if !DEBUG
				if (File.Exists(path))
				{
					if (MessageBox.Show(string.Format("文件 \"{0}\" 已存在，是否覆盖它？", path)) == MessageBoxResult.No) return;
				}
#endif

				string contents = string.Empty;
				File.WriteAllText(path, contents);
			}
		}


		private void addToken(TokenType type, string name, Dictionary<string, string> data)
		{
			switch (type)
			{
				case TokenType.Book:
					this.addBookToken(name);
					break;
				case TokenType.Volumn:
					this.addVolumeToken(name);
					break;
				case TokenType.Chapter:
					this.addChapterToken(name);
					break;
				case TokenType.Text:
					this.addTextToken(name);
					break;
				case TokenType.Image:
					this.addImageToken(name);
					break;
				case TokenType.Unknown:
					break;
				default:
					throw new InvalidOperationException();
			}
		}

		private void addBookToken(string name)
		{
			TokenPropertyNodeItem booktoken_item = new TokenPropertyNodeItem();
			booktoken_item.DisplayName = name;
			booktoken_item.Icon = new BitmapImage(new Uri("images/item_icon.png", UriKind.Relative));
			booktoken_item.ToolTip = Path.Combine(this.projectLocation, name + this.language_extension);
			booktoken_item.TokenType = TokenType.Book;
			this.tokens_item.Add(booktoken_item);
		}

		private void addVolumeToken(string name)
		{
			TokenPropertyNodeItem volumetoken_item = new TokenPropertyNodeItem();
			volumetoken_item.DisplayName = name;
			volumetoken_item.Icon = new BitmapImage(new Uri("images/item_icon.png", UriKind.Relative));
			volumetoken_item.ToolTip = Path.Combine(this.projectLocation, name + this.language_extension);
			volumetoken_item.TokenType = TokenType.Volumn;
			this.tokens_item.Add(volumetoken_item);
		}

		private void addChapterToken(string name)
		{
			TokenPropertyNodeItem chaptertoken_item = new TokenPropertyNodeItem();
			chaptertoken_item.DisplayName = name;
			chaptertoken_item.Icon = new BitmapImage(new Uri("images/item_icon.png", UriKind.Relative));
			chaptertoken_item.ToolTip = Path.Combine(this.projectLocation, name + this.language_extension);
			chaptertoken_item.TokenType = TokenType.Chapter;
			this.tokens_item.Add(chaptertoken_item);
		}

		private void addTextToken(string name)
		{
			TokenPropertyNodeItem texttoken_item = new TokenPropertyNodeItem();
			texttoken_item.DisplayName = name;
			texttoken_item.Icon = new BitmapImage(new Uri("images/item_icon.png", UriKind.Relative));
			texttoken_item.ToolTip = Path.Combine(this.projectLocation, name + this.language_extension);
			texttoken_item.TokenType = TokenType.Text;
			this.tokens_item.Add(texttoken_item);
		}

		private void addImageToken(string name)
		{
			TokenPropertyNodeItem imagetoken_item = new TokenPropertyNodeItem();
			imagetoken_item.DisplayName = name;
			imagetoken_item.Icon = new BitmapImage(new Uri("images/item_icon.png", UriKind.Relative));
			imagetoken_item.ToolTip = Path.Combine(this.projectLocation, name + this.language_extension);
			imagetoken_item.TokenType = TokenType.Image;
			this.tokens_item.Add(imagetoken_item);
		}
	}
}
