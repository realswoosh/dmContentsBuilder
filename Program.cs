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
			
			// excel data to binary
			// because if u have read many data, binary read fast more then read raw excel data

			excelLoader.Transform();
						
			BinaryLoader binaryLoader = new BinaryLoader();
			binaryLoader.Configuration = config;
			binaryLoader.Load();

			// 3. if execute 1,2 step you will find classfile setting folder in config
			// 4. down code remove comment mark
			// 

			//dmGameData.ResourceManager.ResourceDatabase resourceDatabase = new dmGameData.ResourceManager.ResourceDatabase();
			//resourceDatabase.Load(excelLoader);


			// 1. first this code execute
			// 2. after chech config path (ex. PathOutputClass, PathOutputClassResourceManager)

			FormatGenerator generator = new FormatGenerator();
			generator.Configuration = config;
			generator.GenerateClassFile(excelLoader);

		}
	}
}
