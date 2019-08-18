using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Config;
using System.Reflection;

public class ConfigManager<T>: MonoBehaviour where T : class , new() {
    public string bytesFilePath = Application.dataPath + "/Scripts/Bytes";  // 二进制文件存放位置
    private Dictionary<int, T> cfgList = new Dictionary<int, T>();

    public ConfigManager() {
        //导入二进制数据
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = File.Open(bytesFilePath + "/" + typeof(T).Name + ".bytes", FileMode.Open, FileAccess.Read, FileShare.Read);
        object obj = bf.Deserialize(stream);
        Dictionary<int, ConfigItem[]> items = Convert.ChangeType(obj, obj.GetType()) as Dictionary<int, ConfigItem[]>;
        foreach (var item in items) {
            ConfigItem[] cfgItems = item.Value;
            T t = new T();
            for (int i = 0; i < cfgItems.Length; i++) {
                
                t.GetType().GetProperty(cfgItems[i].name).SetValue(t, Convert.ChangeType(cfgItems[i].data, GetTypeByString(cfgItems[i].type)));
            }
            cfgList[item.Key] = t;
        }
        stream.Close();
    }

    /// <summary>
    /// 获取对应id的数据
    /// </summary>
    /// <param name="key">数据id</param>
    /// <returns></returns>
    public T GetConfig(int key) {
        if (!cfgList.ContainsKey(key)) return null;
        return cfgList[key];
    }

    /// <summary>
    /// 获取类型对应的字符串
    /// TODO 字符串未处理
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
