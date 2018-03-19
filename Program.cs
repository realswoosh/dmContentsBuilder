using System.IO;

using dmExcelLoader;

namespace ContentsBuilder
{
	class Program
	{
		static void Main(string[] args)
		{
			string currentPath = Directory.GetCurrentDirectory();
			
			var config = LoaderConfiguration.Defaultconfiguration;

			config.Path = $"{currentPath}/../../ExcelData";
						
			ExcelLoader excelLoader = new ExcelLoader();
			excelLoader.Configuration = config;
			excelLoader.Load();
			excelLoader.FormatParse();

			//dmGameData.ResourceManager.ResourceDatabase resourceDatabase = new dmGameData.ResourceManager.ResourceDatabase();
			//resourceDatabase.Load(excelLoader);

			//FormatGenerator generator = new FormatGenerator();
			//generator.Configuration = config;
			//generator.GenerateClassFile(excelLoader);

		}
	}
}
