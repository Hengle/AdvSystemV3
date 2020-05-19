using System;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using System.Linq;

public static partial class AdvUtility
{

    //////////////
    /// Adv 自訂 Debug 顯示窗口
    //////////////
    public static void Log(string msg)
    {
        Debug.Log(msg);
        AdvDebugMsg.AddMessage(msg);
    }

    public static void LogWarning(string msg)
    {
        Debug.LogWarning(msg);
        AdvDebugMsg.AddMessage(msg);
    }

    public static void LogError(string msg)
    {
        Debug.LogError(msg);
        AdvDebugMsg.AddMessage(msg);
    }

    public static void LogBox(string msg)
    {
        AdvDebugMessageBox.AddMessage(msg);
    }

    public static T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        var dst = destination.GetComponent(type) as T;
        if (!dst) dst = destination.AddComponent(type) as T;
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            if (field.IsStatic) continue;
            field.SetValue(dst, field.GetValue(original));
        }
        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
            prop.SetValue(dst, prop.GetValue(original, null), null);
        }
        return dst as T;
    }

    public static bool IsInPrefabStage(){
        #if UNITY_EDITOR
        var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage != null)
        {
            return true;
        }
        #endif
        return false;
    }

    public static bool GetIsPrefab(this GameObject a_Object)
    {
        return a_Object.scene.rootCount == 0;
    }

    public static A GetDomainName<T, A>() where A : Attribute
    {
        var dnAttribute = typeof(T).GetCustomAttributes(
            typeof(A), true
        ).FirstOrDefault() as A;
        if (dnAttribute != null)
        {
            return dnAttribute;
        }
        return null;
    }

    public static void RebuildCSVLine(Fungus.FlowchartExtend sourceObject){
        List<AdvCSVLine> lines = new List<AdvCSVLine>();
            
        //Commands
        List<Fungus.Command> comps = new List<Fungus.Command>(sourceObject.GetComponents<Fungus.Command>());
        for (int i = comps.Count - 1; i >= 0; i--)
        {
            ICommand icmd = comps[i] as ICommand;
            if(icmd != null && !string.IsNullOrEmpty(icmd.CSVCommandKey)){
                AdvCSVLine newLine = new AdvCSVLine(){keys = icmd.CSVCommandKey, Command = ParseClassToCSVCMD(comps[i])};
                lines.Add(newLine);
            }
        }

        sourceObject.csvLines.RemoveAll((x) => !x.Command.StartsWith("*"));
        sourceObject.csvLines.AddRange(lines);
    }

    public static void AutoLinkCSVLine(Fungus.FlowchartExtend sourceObject){
        if(sourceObject == null)
            return;

        List<AdvCSVLine> csvLineData = sourceObject.csvLines;
        List<AdvCSVLine> csvLineBlocks = new List<AdvCSVLine>();

        //Get base CSV info
        foreach (var item in csvLineData)
        {
            if(item.Command.StartsWith("*")){
                csvLineBlocks.Add(item);
            }
        }

        //Start auto link
        List<Fungus.Command> comps = new List<Fungus.Command>(sourceObject.GetComponents<Fungus.Command>());
        for (int i = 0; i < csvLineData.Count; i++)
        {
            for (int j = comps.Count - 1; j >= 0; j--)
            {
                ICommand icmd = comps[j] as ICommand;
                if(icmd!= null && csvLineData[i].keys == icmd.CSVCommandKey){
                    //if(csvLineData[i].generatedCommand == null) Debug.Log($"Find unlink CSVLine ,will be linked :{csvLineData[i].keys}");
                    csvLineData[i].generatedCommand = comps[j];
                    comps.RemoveAt(j);
                    break;
                }
            }
        }

        foreach (var item in csvLineBlocks)
        {
            if(item.generateBlock == null){
                Fungus.Block _block = sourceObject.FindBlock(item.Command);
                if(_block != null){
                    item.generateBlock = _block;
                }
            }
        }
    }

    public static string ParseClassToCSVCMD(Fungus.Command cmd){
        if(cmd.GetType().Name == "Say")
            return "";

        for (int i = 0; i < AdvUtility.CSVCommandMapping.GetLength(0); i++)
        {
            if(System.String.Equals(cmd.GetType().Name, AdvUtility.CSVCommandMapping[i, 1], System.StringComparison.OrdinalIgnoreCase)){
                return AdvUtility.CSVCommandMapping[i, 0];
            }
        }
        return "";
    }
}