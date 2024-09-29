using System;
using System.IO;
using System.Text;
using Cysharp.Text;
using Framework;
using UnityEditor;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

public struct CellInfo
{
    public string Type;
    public string Name;
    public string Desc;
}

public class ConfigBuilder
{
    
    [MenuItem("FunnyTools/Config/BuildNormalConfig")]
    static void BuildNormalConfig()
    {

        ExportAll(EditorPath.EditorConfigRoot);
        EditorUtility.ClearProgressBar();
      
        
        EditorUtility.DisplayDialog("Finish", "Build Success!", "ok");
    }
    
    [MenuItem("FunnyTools/Config/BuildConfigCode")]
    static void BuildConfigCode()
    {
        ExportAllClass(EditorPath.EditorConfigCodeRoot);
        EditorUtility.ClearProgressBar();
      
        
        EditorUtility.DisplayDialog("Finish", "Build Success!", "ok");
        
    }

    #region Excel Function

    private  static  float _progress = 0.0f;
    private static void ExportAll(string exportDir)
    {
        var allFiles = Directory.GetFiles(EditorPath.EditorConfigSourceRoot);
        int fileCount = allFiles.Length;
        int exported = 0;

        foreach (string filePath in allFiles)
        {
            if (Path.GetExtension(filePath) != ".xlsx")
            {
                continue;
            }
            if (Path.GetFileName(filePath).StartsWith("~"))
            {
                continue;
            }
            string fileName = Path.GetFileName(filePath);
 

            Export(filePath, exportDir);
            exported++;
            _progress = (float)exported / fileCount;
            EditorUtility.DisplayProgressBar("Processing...","Make cup of tea....",_progress);
        }
        

        CustomLogger.Log(LoggerLevel.Log  ,"所有表导表完成");
        AssetDatabase.Refresh();
    }

    private static void Export(string fileName, string exportDir)
    {
        XSSFWorkbook xssfWorkbook;
        using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            xssfWorkbook = new XSSFWorkbook(file);
        }
        string protoName = Path.GetFileNameWithoutExtension(fileName);
        string exportPath = Path.Combine(exportDir, $"{protoName}.txt");
        using (FileStream txt = new FileStream(exportPath, FileMode.Create))
        using (StreamWriter sw = new StreamWriter(txt))
        {
            for (int i = 0; i < xssfWorkbook.NumberOfSheets; ++i)
            {
                ISheet sheet = xssfWorkbook.GetSheetAt(i);
                ExportSheet(sheet, sw);
            }
        }

    }

      
    private static void ExportSheet(ISheet sheet, StreamWriter sw)
    {
        int cellCount = sheet.GetRow(3).LastCellNum;

        CellInfo[] cellInfos = new CellInfo[cellCount];

        for (int i = 2; i < cellCount; i++)
        {
            string fieldDesc = GetCellString(sheet, 2, i);
            string fieldName = GetCellString(sheet, 3, i);
            string fieldType = GetCellString(sheet, 4, i);
            cellInfos[i] = new CellInfo() { Name = fieldName, Type = fieldType, Desc = fieldDesc };
        }
       
        for (int i = 5; i <= sheet.LastRowNum; ++i)
        {
            if (GetCellString(sheet, i, 2) == "")
            {
                continue;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            IRow row = sheet.GetRow(i);
            for (int j = 2; j < cellCount; ++j)
            {
                string desc = cellInfos[j].Desc.ToLower();
                if (desc.StartsWith("#"))
                {
                    continue;
                }


                string fieldValue = GetCellString(row, j);
                if (fieldValue == "")
                {
                    throw new Exception($"sheet: {sheet.SheetName} 中有空白字段 {i},{j}");
                }

                if (j > 2)
                {
                    sb.Append(",");
                }

                string fieldName = cellInfos[j].Name;
                
                string fieldType = cellInfos[j].Type;
                sb.Append($"\"{fieldName}\":{Convert(fieldType, fieldValue)}");
            }
            sb.Append("}");
         
            sw.WriteLine(sb.ToString());
        }
    }
    
    private static string Convert(string type, string value)
    {
        switch (type)
        {
            case "int[]":
            case "int32[]":
            case "long[]":
                return $"[{value}]";
            case "string[]":
                var split = value.Split(',');
                
                
                if (split.Length <= 1)
                {
                    split = value.Split(';');
                }
                StringBuilder sb = new StringBuilder();
                sb.Append("[");
                for (int i = 0; i < split.Length; i++)
                {
                    sb.Append($"\"{split[i]}\"");
                    if (i < split.Length -1) sb.Append(",");

                }

                sb.Append("]");

                return sb.ToString();
            case "int":
            case "int32":
            case "int64":
            case "long":
            case "float":
            case "double":
                return value;
            case "string":
                return $"\"{value}\"";
            //fix number extension
            case "wint[]":
                return $"[{value}]";
            case "wint":
                return value;
            
            default:
                EditorUtility.ClearProgressBar();
                throw new Exception($"不支持此类型: {type}");
            
        }
    }

    private static string GetCellString(ISheet sheet, int i, int j)
    {
        var row = sheet.GetRow(i);
        if (row == null)
            return "";
        var cell = row?.GetCell(j);
        if (cell != null)
        {
            if (cell.CellType == CellType.Formula)
            {
                cell.SetCellType(CellType.String);
                return cell.StringCellValue;
            }
            else
            {
                return cell.ToString();
            }
        }

        return "";
    }

    private static string GetCellString(IRow row, int i)
    {
        var cell = row?.GetCell(i);
        if (cell != null)
        {
            if (cell.CellType == CellType.Formula)
            {
                cell.SetCellType(CellType.String);
                return cell.StringCellValue;
            }
            else
            {
                return cell.ToString();
            }
        }
        
        return "";
    }

    

    #endregion

    #region  Code Generator

    private static void ExportAllClass(string exportDir)
    {
        var allFiles = Directory.GetFiles(EditorPath.EditorConfigSourceRoot);
        int fileCount = allFiles.Length;
        int exported = 0;

        foreach (string filePath in allFiles)
        {
            if (Path.GetExtension(filePath) != ".xlsx")
            {
                continue;
            }
            if (Path.GetFileName(filePath).StartsWith("~"))
            {
                continue;
            }
            string fileName = Path.GetFileName(filePath);
 

            ExportClass(filePath,exportDir, GenerateCodeHead());
            exported++;
            _progress = (float)exported / fileCount;
            EditorUtility.DisplayProgressBar("Processing...","Make cup of tea....",_progress);
        }
        

        CustomLogger.Log(LoggerLevel.Log  ,"所有code生成完成");
        AssetDatabase.Refresh();
    }

    private static void ExportClass(string fileName, string exportDir, string csHead)
    {
        XSSFWorkbook xssfWorkbook;
        using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            xssfWorkbook = new XSSFWorkbook(file);
        }

        string protoName = Path.GetFileNameWithoutExtension(fileName);
		
        string exportPath = Path.Combine(exportDir, $"{protoName}.cs");
        using (FileStream txt = new FileStream(exportPath, FileMode.Create))
        using (StreamWriter sw = new StreamWriter(txt))
        {
            using (var sb = ZString.CreateStringBuilder())
            {
                ISheet sheet = xssfWorkbook.GetSheetAt(0);
                sb.Append(csHead);

                sb.Append($"\t[Config(\"{protoName}\")]\n");
                sb.Append($"\tpublic partial class {protoName}Category : ACategory<{protoName}>\n");
                sb.Append("\t{\n");
                sb.Append("\t}\n\n");

                sb.Append($"\tpublic class {protoName}: IConfig\n");
                sb.Append("\t{\n");
       
                int cellCount = sheet.GetRow(3).LastCellNum;
			
                for (int i = 2; i < cellCount; i++)
                {
                    string fieldDesc = GetCellString(sheet, 2, i);

                    if (fieldDesc.StartsWith("#"))
                    {
                        continue;
                    }
                
				
                    string fieldName = GetCellString(sheet, 3, i);

                    if (fieldName == "Id" || fieldName == "_id")
                    {
                        continue;
                    }

                    string fieldType = GetCellString(sheet, 4, i);
                    if (fieldType == "" || fieldName == "")
                    {
                        continue;
                    }
                    //fix number convert
                    string realFieldType = ConvertToRealFieldType(fieldType);

                    sb.Append($"\t\tpublic {realFieldType} {fieldName};\n");
                }

                sb.Append("\t}\n");
                sb.Append("}\n");

                sw.Write(sb.ToString());
            }

        }
    }


    private static string GenerateCodeHead()
    {
        using (var head = ZString.CreateStringBuilder())
        {
            head.Append("/*-----------------------------------------------------------------\n");
            head.Append("    以下代码为自动生成的代码，任何对以下代码的修改将在下次自动生成后被覆\n");
            head.Append("    盖，不要对以下代码进行修改\n");
            head.Append("-----------------------------------------------------------------*/\n");
            head.Append("\n");

            head.Append("using Framework;\nusing System.Collections.Generic;\n\nnamespace Savery\n{\n");

            return head.ToString();
        }
        
    }

    private static string ConvertToRealFieldType(string fieldType)
    {
        string realFieldType = "";

        switch (fieldType)
        {
            case "wint":
                realFieldType = "sfloat";
                break;
            case "wint[]":
                realFieldType = "List<sfloat>";
                break;
            case "int[]":
                realFieldType = "List<int>";
                break;
            case "int32[]":
                realFieldType = "List<int32>";
                break;
            case "long[]":
                realFieldType = "List<long>";
                break;
            case "string[]":
                realFieldType = "List<string>";
                break;
            default:
                realFieldType = fieldType;
                break;
            

        }

        return realFieldType;

    }

    #endregion
  
}


public static class EditorPath
{
    
    public static readonly string EditorConfigRoot = "Assets/GameRes/Export/Config";
    public static readonly string EditorConfigCodeRoot = "Assets/Script/Savery/Config/ConfigGenerated";
    public static readonly string EditorConfigSourceRoot = "../ClientConfig";
    
    
}