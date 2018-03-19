using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Xml.XPath;
using System.IO;

using dmExcelLoader.Resource;
using dmExcelLoader;
using dmExcelLoader.FormatParser;

using ContentsBuilder.Template;


namespace ContentsBuilder
{
	public class FormatGenerator
	{
		public IHeader HeaderParser { get; set; }

		public LoaderConfiguration Configuration { get; set; }
				
		List<EnumTemplate> enumTemplateList = new List<EnumTemplate>();
		
		public void GenerateClassFile(ExcelLoader excelLoader)
		{
			var enumParser = excelLoader.enumParser;

			if (Configuration.IsEnumCombine)
			{
				EnumTemplate enumTemplate = new EnumTemplate();
				enumTemplate.Configuration = Configuration;
				enumTemplate.FileName = "Enum.cs";
				enumTemplate.enumInfoDic = enumParser.enumSheetDic.Values
													 .SelectMany(x => x.enumInfoDic)
													 .ToDictionary(x => x.Key, x => x.Value);

				enumTemplate.Save();
			}
			else
			{

			}

			List<ClassTemplate> classTemplateList = new List<ClassTemplate>();

			foreach (var formatSheet in excelLoader.formatSheetList)
			{
				classTemplateList.Add(new ClassTemplate()
				{
					Configuration = Configuration,
					formatSheet = formatSheet
				});
			}

			classTemplateList.ForEach(x => x.Save());

			ManagerClassTemplate manageTemplate = new ManagerClassTemplate();
			manageTemplate.Configuration = Configuration;
			manageTemplate.Save(classTemplateList);

			if (Configuration.IsAutoAdd)
			{
				ProjectAdd();
			}
		}

		public void ProjectAdd()
		{
			string projectPath = Configuration.AddProjectPath + Configuration.AddProjectName;

			XNamespace defaultNs = "http://schemas.microsoft.com/developer/msbuild/2003";
			XmlNamespaceManager r = new XmlNamespaceManager(new NameTable());
			r.AddNamespace("p", defaultNs.NamespaceName);

			XDocument doc = XDocument.Load(projectPath);
			var itemGroup = doc.XPathSelectElement("//p:ItemGroup/p:Compile", r).Parent;
			var remove1 = doc.XPathSelectElements($"//p:ItemGroup/p:Compile[contains(@Include, '{Configuration.PathClass}')]", r);
			var remove2 = doc.XPathSelectElements($"//p:ItemGroup/p:Compile[contains(@Include, '{Configuration.PathClassResourceManager}')]", r);
			remove1.Remove();
			remove2.Remove();

			var files = Directory.GetFiles(Configuration.AddProjectPath + "/" + Configuration.PathClass, "*.cs");

			foreach (string file in files)
			{
				string resourceFilePath = Configuration.PathClass + "\\" + Path.GetFileName(file);
				itemGroup.Add(new XElement(defaultNs + "Compile", new XAttribute("Include", resourceFilePath)));
			}

			itemGroup.Add(new XElement(defaultNs + "Compile", new XAttribute("Include", $"{Configuration.PathClassResourceManager}\\{Configuration.ResourceManagerFileName}")));

			doc.Save(projectPath);
		}
	}
}
