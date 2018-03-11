using dmExcelLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentsBuilder
{
	class Program
	{
		static void Main(string[] args)
		{
			string currentPath = Directory.GetCurrentDirectory();

			ExcelLoader.Configuration = LoaderConfiguration.Defaultconfiguration;

			ExcelLoader excelLoader = new ExcelLoader();
			excelLoader.Load(currentPath + "/../../ExcelData");
		}
	}
}
