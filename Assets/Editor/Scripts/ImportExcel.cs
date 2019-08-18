using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Data;
using Excel;
using System.CodeDom;
using System.CodeDom.Compiler;
using UnityEditor;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public class ImportExcel {
    public static string path = Application.dataPath + "/Excel";  // Excel表位置
    public static string classPath = Application.dataPath + "/Editor/Config";  // 数据类文件存放位置
    public static string bytesFilePath = Application.dataPath + "/Editor/Bytes";  // 二进制文件存放位置

    [MenuItem("Tool/CreateClassFile")]
    public static void Get() {
        GetAllData(path);
    }

    [MenuItem("Tool/ImportExcelData")]
    public static void GetData() {
        AssetDatabase.Refresh();
        DirectoryInfo theFolder = new DirectoryInfo(path);
        DirectoryInfo[] dirInfo = theFolder.GetDirectories();
        //遍历文件夹
        FileInfo[] fileInfo = theFolder.GetFiles();
        foreach (FileInfo NextFile in fileInfo) {
            if (NextFile.Extension != ".xlsx" && NextFile.Extension != ".xls") continue;
            DataSet result = ReadExcel(NextFile.DirectoryName + "/" + NextFile.Name);
            for (int i = 0; i < result.Tables.Count; i++) {
                Dictionary<int, ConfigItem[]> dic = GetData(result.Tables[i].Rows, result.Tables[i].TableName);

                // 序列化
                BinaryFormatter bf = new BinaryFormatter();
                if (Directory.Exists(bytesFilePath) == false) {
                    Directory.CreateDirectory(bytesFilePath);
                }
                FileStream fs = new FileStream(bytesFilePath + "/" + result.Tables[i].TableName + ".bytes", FileMode.OpenOrCreate);
                bf.Serialize(fs, dic);
                fs.Close();

                //bf = new BinaryFormatter();
                //FileStream stream = File.Open(bytesFilePath + "/" + result.Tables[i].TableName + ".bytes", FileMode.Open, FileAccess.Read, FileShare.Read);
                //object obj = bf.Deserialize(stream);
                //Dictionary<int, object>  cfgList = Convert.ChangeType(obj, obj.GetType()) as Dictionary<int, object>;
                //stream.Close();
            }
        }
        FolderMove(classPath, Application.dataPath + "/Scripts/Config");
        FolderMove(bytesFilePath, Application.dataPath + "/Scripts/Bytes");
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 获取所有表数据
    /// </summary>
    /// <param name="path"></param>
    /// <param name="sheet"></param>
    public static void GetAllData(string path) {
        DirectoryInfo theFolder = new DirectoryInfo(path);
        DirectoryInfo[] dirInfo = theFolder.GetDirectories();
        //遍历文件夹
        FileInfo[] fileInfo = theFolder.GetFiles();
        foreach (FileInfo NextFile in fileInfo) {
            if (NextFile.Extension != ".xlsx" && NextFile.Extension != ".xls") continue;
            DataSet result = ReadExcel(NextFile.DirectoryName +"/"+ NextFile.Name);
            for (int i = 0; i < result.Tables.Count; i++) {
                string className = CeaterConfigFile(result.Tables[i].TableName, result.Tables[i].Rows);
                AssetDatabase.Refresh();
            }
        }
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("title", "content", "确认", "取消");
    }

    /// <summary>
    /// 获取对应表中的数据
    /// </summary>
    /// <param name="filePath">文件完整路径</param>
    /// <returns></returns>
    public static DataSet ReadExcel(string filePath) {
        FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        DataSet result = excelReader.AsDataSet();
        return result;
        //if (result.Tables[sheet] == null) {
        //    Debug.Log(filePath + "中的sheet:" + sheet + "不存在。");
        //    return null;
        //}
        //return result.Tables[sheet].Rows;
    }

    /// <summary>
    /// 通过Excel表头创建对应结构体文件
    /// </summary>
    /// <param name="fileName">Sheet</param>
    /// <param name="collect">表数据</param>
    /// <returns></returns>
    public static string CeaterConfigFile(string sheet, DataRowCollection collect) {

        CodeCompileUnit compunit = new CodeCompileUnit();
        CodeNamespace sample = new CodeNamespace("Config");
        compunit.Namespaces.Add(sample);

        //引用命名空间
        sample.Imports.Add(new CodeNamespaceImport("System"));
        sample.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));

        //在命名空间下添加一个类
        CodeTypeDeclaration wrapProxyStruct = new CodeTypeDeclaration(sheet);
        wrapProxyStruct.IsClass = true;
        wrapProxyStruct.IsEnum = false;
        wrapProxyStruct.IsInterface = false;
        wrapProxyStruct.IsPartial = false;
        wrapProxyStruct.IsStruct = false;
        wrapProxyStruct.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializableAttribute))));

        //把这个类添加到命名空间 
        sample.Types.Add(wrapProxyStruct);


        // 生成字段
        string[][] items = new string[collect[0].ItemArray.Length][];

        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < collect[i].ItemArray.Length; j++) {
                if (items[j] == null)
                    items[j] = new string[4];
                items[j][i] = collect[i][j].ToString();
            }
        }
        foreach (var item in items) {
            CodeSnippetTypeMember member = new CodeSnippetTypeMember();
            string memberContent = @"        public [type] [Name] {get;set;}";
            member.Comments.Add(new CodeCommentStatement(item[3]));
            member.Text = memberContent.Replace("[type]", item[1]).Replace("[Name]", item[0]);
            wrapProxyStruct.Members.Add(member);
        }

        //生成代码       
        CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
        CodeGeneratorOptions options = new CodeGeneratorOptions();
        options.BracingStyle = "C";
        options.BlankLinesBetweenMembers = true;

        //输出目录控制
        string outputFile = Path.Combine(Application.dataPath, classPath);
        if (Directory.Exists(outputFile) == false) {
            Directory.CreateDirectory(outputFile);
        }

        //输出目录
        outputFile = Path.Combine(outputFile, sheet+".cs");

        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile)) {
            provider.GenerateCodeFromCompileUnit(compunit, sw, options);
        }

        return sheet;
    }

    /// <summary>
    /// 获取Excel表中的数据并转换为字典
    /// </summary>
    /// <param name="collect">表数据</param>
    /// <param name="type">数据类型</param>
    /// <returns></returns>
    public static Dictionary<int, ConfigItem[]> GetData(DataRowCollection collect,string className) {

        string[][] items = new string[collect.Count-4][];
        //Type type = Type.GetType("Config." + className);
        for (int i = 0; i < collect.Count-4; i++) {
            if (items[i] == null)
                items[i] = new string[collect[i + 4].ItemArray.Length];
            for (int j = 0; j < collect[i+4].ItemArray.Length; j++) {

                items[i][j] = collect[i+4][j].ToString();
            }
        }
        Dictionary<int, ConfigItem[]> dic = new Dictionary<int, ConfigItem[]>();
        foreach (var item in items) {
            ConfigItem[] cfgItems = new ConfigItem[collect[0].ItemArray.Length]; 
            //var tempClass = Activator.CreateInstance(type);
            for (int i = 0; i < collect[0].ItemArray.Length; i++) {
                ConfigItem dataItem = new ConfigItem() {
                    name = collect[0][i].ToString(),
                    type = collect[1][i].ToString(),
                    data = item[i]
                };
                cfgItems[i] = dataItem;
                //string typeStr = collect[1][i].ToString();
                //Type t = GetTypeByString(typeStr);
                //var q = Convert.ChangeType(item[i], t);
                ////var q = Expression.Parameter(t, item[i]);
                //PropertyInfo info = type.GetProperty(collect[0][i].ToString());
                //info.SetValue(tempClass, q);
            }

            BinaryFormatter bf = new BinaryFormatter();
            if (Directory.Exists(bytesFilePath) == false) {
                Directory.CreateDirectory(bytesFilePath);
            }
            dic.Add(int.Parse(item[0]), cfgItems);
        }
        return dic;
    }

    /// <summary>
    /// 文件夹移动
    /// </summary>
    /// <param name="fromPath"></param>
    /// <param name="toPath"></param>
    public static void FolderMove(string fromPath, string toPath) {
        //判断目标目录是否存在，如果不在则创建
        if (Directory.Exists(toPath))
            Directory.Delete(toPath,true);
        Directory.CreateDirectory(toPath);
        string[] fileList = Directory.GetFileSystemEntries(fromPath);
        foreach (string file in fileList) {
            if (Directory.Exists(file)) {
                FolderMove(file, toPath + "/"+Path.GetFileName(file));
            } else
                File.Move(file, toPath + "/" + Path.GetFileName(file));
        }
        //Directory.Delete(fromPath);
    }

/// <summary>
/// 获取类型对应的字符串
/// </summary>
/// <param name="type"></param>
/// <returns></returns>
public static Type GetTypeByString(string type) {
        switch (type.ToLower()) {
            case "bool":
                return Type.GetType("System.Boolean", true, true);
            case "byte":
                return Type.GetType("System.Byte", true, true);
            case "sbyte":
                return Type.GetType("System.SByte", true, true);
            case "char":
                return Type.GetType("System.Char", true, true);
            case "decimal":
                return Type.GetType("System.Decimal", true, true);
            case "double":
                return Type.GetType("System.Double", true, true);
            case "float":
                return Type.GetType("System.Single", true, true);
            case "int":
                return Type.GetType("System.Int32", true, true);
            case "uint":
                return Type.GetType("System.UInt32", true, true);
            case "long":
                return Type.GetType("System.Int64", true, true);
            case "ulong":
                return Type.GetType("System.UInt64", true, true);
            case "object":
                return Type.GetType("System.Object", true, true);
            case "short":
                return Type.GetType("System.Int16", true, true);
            case "ushort":
                return Type.GetType("System.UInt16", true, true);
            case "string":
                return Type.GetType("System.String", true, true);
            case "date":
            case "datetime":
                return Type.GetType("System.DateTime", true, true);
            case "guid":
                return Type.GetType("System.Guid", true, true);
            default:
                return Type.GetType(type, true, true);
        }
    }
}

