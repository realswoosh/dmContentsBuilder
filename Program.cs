using dmExcelLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentsBuilder
{
	class Program
	{
		static void Main(string[] args)
		{
			ExcelLoader excelLoader = new ExcelLoader();

			excelLoader.Load("/Data");
		}
	}
}
