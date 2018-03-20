using dmExcelLoader.FormatParser;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentsBuilder.Template
{

	public class EnumTemplate : Template
	{
		public static string Base = @"namespace #namespace
{
#enumbody
}";
		public static string BaseEnum = @"public enum #enumname : int
	{
#enumfield
	}";
		public string FileName { get; set; }

		public Dictionary<string, List<EnumInfo>> enumInfoDic { get; set; }

		public void Save()
		{
			string savePath = Configuration.AddProjectPath + Configuration.PathOutputClass;
			string fileName = FileName;
			string fullPath = savePath + "/" + fileName;

			Directory.CreateDirectory(savePath);

			string code = Base;
			code = code.Replace("#namespace", Configuration.NamespaceDataClass);

			List<string> enumBody = new List<string>();

			foreach (var kvp in enumInfoDic)
			{
				List<string> enumMember = new List<string>();
				kvp.Value.ForEach(x => enumMember.Add($"\t\t{x.name} = {x.value},"));

				string codeEnum = "\t" + BaseEnum;

				codeEnum = codeEnum.Replace("#enumname", kvp.Key);
				codeEnum = codeEnum.Replace("#enumfield", string.Join("\r\n", enumMember));

				enumBody.Add(codeEnum);
			}

			code = code.Replace("#enumbody", string.Join("\r\n", enumBody));

			File.WriteAllText(fullPath, code);
		}
	}
}
