using dmExcelLoader;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentsBuilder.Template
{
	public class ManagerClassTemplate : Template
	{
		public static string Base = @"using System;
using System.Collections.Generic;
using System.Reflection;

using dmExcelLoader;
using dmExcelLoader.FormatParser;
using dmExcelLoader.Resource;

#namespacedataclass

namespace #namespace
{
	public class ResourceDatabase
	{
		EnumParser enumParser;

		#membervalues

		public void Load(ExcelLoader excelLoader)
		{
			enumParser = excelLoader.enumParser;

			var sheetDic = new Dictionary<string, FormatSheet>();

			excelLoader.formatSheetList
					   .ForEach(x => sheetDic.Add(x.SheetName, x));

			#loader
		}

		T InnerLoad<T>(List<HeaderType> headerTypeList, Row row) where T : class, new()
		{
			T t = new T();

			FieldInfo[] fieldinfos = t.GetType().GetFields();

			foreach (FieldInfo fi in fieldinfos)
			{
				HeaderType ht = headerTypeList.Find(x => x.ValueName.Equals(fi.Name, System.StringComparison.OrdinalIgnoreCase));

				if (ht == null)
					continue;

				Cell cell = Array.Find(row.FilledCells, x => x.ReferenceIndex == ht.ReferenceIndex);
				if (cell == null ||
					cell.Text == null)
				{
					fi.SetValue(t, TypeInfos.GetDefaultValue(ht.ValueRealType));
					continue;
				}

				if (ht.ValueRealType == typeof(Enum))
				{
					foreach (var enumSheet in enumParser.enumSheetDic.Values)
					{
						if (enumSheet.enumInfoDic.ContainsKey(ht.ValueType))
						{
							var enumInfo = enumSheet.enumInfoDic[ht.ValueType].Find(x => x.name == ht.ValueName);
							if (enumInfo == null)
								continue;

							fi.SetValue(t, Convert.ToInt32(enumInfo.value));

							break;
						}
					}
				}
				else
				{
					object ret = cell.Text;

					if (ht.ValueRealType == typeof(int[]))
					{
						ret = Array.ConvertAll(cell.Text.Split(';'), s => int.Parse(s));
					}
					else if (ht.ValueRealType == typeof(float[]))
					{
						ret = Array.ConvertAll(cell.Text.Split(';'), s => float.Parse(s));
					}
					else if (ht.ValueRealType == typeof(string[]))
					{
						ret = cell.Text.Split(';');
					}

					fi.SetValue(t, Convert.ChangeType(ret, ht.ValueRealType));
				}
			}

			return t;
		}

		#loadfunction
	}
}";

		public static string BaseMemberValue = "List<#classname> resource#sheetname = new List<#classname>();";
		public static string BaseMemberDicName = "resource#sheetname_Dic";
		public static string BaseMemberDicValue = "Dictionary<#type, #classname> #dicname = new Dictionary<#type, #classname>();";
		public static string BaseMemberKeyPairValue = "KeyPairDictionary<#type1, #type2, #classname> resource#sheetname_KeyPair_#name1_#name2 = new KeyPairDictionary<#type1, #type2, #classname>();";
		public static string BaseLoad = "Load_#sheetname(sheetDic[\"#sheetname\"]);";
		public static string BaseLoadFunction = @"private void Load_#sheetname(FormatSheet sheet)
		{
			foreach (var row in sheet.rowList)
			{
				#classname data = InnerLoad<#classname>(sheet.HeaderTypeList, row);

				#containerlist.Add(data);
				#containerdic
			}
		}";

		public static string BaseLoadContainerDic = "#dicname.Add(data.#valuename, data);";

		public void Save(List<ClassTemplate> classTemplateList)
		{
			string savePath = Configuration.AddProjectPath + Configuration.PathClassResourceManager;
			string fileName = Configuration.ResourceManagerFileName;
			string fullPath = savePath + "/" + fileName;

			List<string> memberValueList = new List<string>();
			List<string> memberLoadList = new List<string>();
			List<string> memberLoaderList = new List<string>();

			foreach(var classTemplate in classTemplateList)
			{
				string classname = Configuration.PrefixDataClass + classTemplate.formatSheet.SheetName;
				string sheetname = classTemplate.formatSheet.SheetName;

				string member = BaseMemberValue;
				member = member.Replace("#classname", classname);
				member = member.Replace("#sheetname", sheetname);

				memberValueList.Add(member);

				string memberDicLoad = "";

				foreach (var ht in classTemplate.formatSheet
												.HeaderTypeList
												.Where(x => x.IsKey == true))
				{
					string dicName = BaseMemberDicName;
					dicName = dicName.Replace("#sheetname", sheetname);

					string memberDic;
					memberDic = BaseMemberDicValue;
					memberDic = memberDic.Replace("#type", ht.ValueRealType.ToString());
					memberDic = memberDic.Replace("#classname", classname);
					memberDic = memberDic.Replace("#dicname", dicName);

					memberValueList.Add(memberDic);

					memberDicLoad = BaseLoadContainerDic;
					memberDicLoad = memberDicLoad.Replace("#dicname", dicName);
					memberDicLoad = memberDicLoad.Replace("#valuename", ht.ValueName);
				}
				
				string loader = BaseLoad;
				loader = loader.Replace("#sheetname", sheetname);

				memberLoadList.Add(loader);

				string loadFunction = BaseLoadFunction;

				loadFunction = loadFunction.Replace("#classname", classname);
				loadFunction = loadFunction.Replace("#sheetname", sheetname);
				loadFunction = loadFunction.Replace("#containerlist", "resource" + sheetname);
				loadFunction = loadFunction.Replace("#containerdic", memberDicLoad);
				
				memberLoaderList.Add(loadFunction);
			}

			string code = Base;

			if (Configuration.NamespaceDataClass != 
				Configuration.NamespaceResourceManager)
				code = code.Replace("#namespacedataclass", $"using {Configuration.NamespaceDataClass};");
			else
				code = code.Replace("#namespacedataclass", "");

			code = code.Replace("#namespace", Configuration.NamespaceResourceManager);
			code = code.Replace("#loader", string.Join("\r\n\t\t\t", memberLoadList));
			code = code.Replace("#membervalues", string.Join("\r\n\t\t", memberValueList));
			code = code.Replace("#loadfunction", string.Join("\r\n\t\t", memberLoaderList));

			Directory.CreateDirectory(savePath);

			File.WriteAllText(fullPath, code);
		}
	}
}
