using dmExcelLoader.FormatParser;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentsBuilder.Template
{
	public class ClassTemplate : Template
	{
		public static string Base = @"namespace #namespace
{
	public class #classname
	{
#classmemberfield
	}
}";
		public static string BaseIsKey = $"\t\t[dmExcelLoader.Key(typeof(#typename))]";

		public FormatSheet formatSheet { get; set; }

		public void Save()
		{
			string className = Configuration.PrefixDataClass + formatSheet.SheetName;

			string savePath = Configuration.AddProjectPath + Configuration.PathClass;
			string fileName = className + ".cs";
			string fullPath = savePath + "/" + fileName;

			Directory.CreateDirectory(savePath);

			string code = Base;
			code = code.Replace("#namespace", Configuration.NamespaceDataClass);
			code = code.Replace("#classname", className);

			code = MemberField(code);

			File.WriteAllText(fullPath, code);
		}

		string MemberField(string code)
		{
			List<string> memberField = new List<string>();

			foreach (var headerType in formatSheet.HeaderTypeList)
			{
				string valueType = "";
				string valueName = headerType.ValueName;

				if (headerType.ValueRealType == typeof(Enum))
					valueType = $"{Configuration.NamespaceDataClass}.{headerType.ValueType}";
				else
					valueType = headerType.ValueRealType.ToString();
					
				string member = $"\t\tpublic {valueType} {valueName};";

				if (headerType.IsKey)
				{
					string isKey = BaseIsKey.Replace("#typename", headerType.ValueRealType.ToString());
					member = isKey + "\r\n" + member;
				}

				memberField.Add(member);
			}

			string tmpFields = string.Join("\r\n", memberField);
			code = code.Replace("#classmemberfield", tmpFields);
			return code;
		}
	}
}
