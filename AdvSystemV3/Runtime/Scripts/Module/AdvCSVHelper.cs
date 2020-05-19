using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ideafixxxer.CsvParser;

public static class AdvCSVHelper
{
    public static bool VerifyKey(string content){

        CsvParser csvParser = new CsvParser();
        string[][] csvTable = csvParser.Parse(content);

         //判斷CSV 檔案 有無內容
        if (csvTable.Length <= 1)
        {
            AdvUtility.Log("資料不完整或是格是不正確");
            return false;
        }

        //搜尋開頭字串正不正確
        int id_key = Array.IndexOf(csvTable[0], AdvUtility.TitleKeys);
        int id_command = Array.IndexOf(csvTable[0], AdvUtility.TitleCommand);
        int id_target = Array.IndexOf(csvTable[0], AdvUtility.TitleTarget);
        int id_arg1 = Array.IndexOf(csvTable[0], AdvUtility.TitleArg1);
        int id_arg2 = Array.IndexOf(csvTable[0], AdvUtility.TitleArg2);
        int id_image = Array.IndexOf(csvTable[0], AdvUtility.TitleImage);
        int id_name = Array.IndexOf(csvTable[0], AdvUtility.TitleName);

        //判斷CSV 檔案格式正不正確
        if (id_command == -1)
        {
            AdvUtility.LogWarning("資料不完整或是格是不正確，確認 CSV 是否為逗號分隔 (?");
            return false;
        }
        
        if(id_command == -1 || id_target == -1 || id_arg1 == -1 || id_arg2 == -1 || 
            id_image == -1 || id_name == -1)
        {
            AdvUtility.LogWarning("CSV 檔案沒有正確的開頭資訊 (Keys, Command, Target, Arg1, Arg2, Image, Name, Content_zh-tw)");
            return false;
        }

        int rowLength = csvTable.Length;
        int columnLength = csvTable[0].Length;

        //檢查是否有重複的值
        int lineid = 1;
        List<string> Allkeys = new List<string>();
        foreach (var csvLine in csvTable)
        {
            if(csvLine[id_key] == ""){
                AdvUtility.Log("匯入中止，CSV中有空白的Key，將Key值填充好後再次匯入 行數(" + lineid + ")");
                return false;
            }

            if(Allkeys.Contains(csvLine[id_key])){
                AdvUtility.Log("匯入中止，CSV中有相同的Key，將重複的Key刪除留空後再次匯入 key(" + csvLine[id_key] + ")");
                return false;
            }
            Allkeys.Add(csvLine[id_key]);
            lineid++;
        }

        return true;
    }

    public static string AssignKey(string content){

        CsvParser csvParser = new CsvParser();
        string[][] csvTable = csvParser.Parse(content);

         //判斷CSV 檔案 有無內容
        if (csvTable.Length <= 1)
        {
            AdvUtility.Log("資料不完整或是格是不正確");
            return null;
        }

        int id_key = Array.IndexOf(csvTable[0], AdvUtility.TitleKeys);
        int rowLength = csvTable.Length;
        int columnLength = csvTable[0].Length;

        //檢查是否有重複的值
        int lineid = 1;
        List<string> Allkeys = new List<string>();
        foreach (var csvLine in csvTable)
        {
            if(csvLine[id_key] == ""){
                AdvUtility.Log("匯入中止，CSV中有空白的Key，將Key值填充好後再次匯入 行數(" + lineid + ")");
                return null;
            }

            if(Allkeys.Contains(csvLine[id_key])){
                AdvUtility.Log("匯入中止，CSV中有相同的Key，將重複的Key刪除留空後再次匯入 key(" + csvLine[id_key] + ")");
                return null;
            }
            Allkeys.Add(csvLine[id_key]);
            lineid++;
        }

        //至此確保所有的 Key 是唯一, 而且有部分是空白
        int randId = 0;
        for(int i = 1; i< rowLength ; i++){
            if(csvTable[i][id_key] == ""){
                //為空白處給值
                csvTable[i][id_key] = "cmd" + randId;
                randId++;
                while(Allkeys.Contains(csvTable[i][id_key])){
                    csvTable[i][id_key] = "cmd" + randId;
                    randId++;
                }
            }
        }

        // Rebuild output
        string output = "";
        for(int i = 0 ; i < rowLength ; i++){
            for(int j = 0; j < columnLength ; j++){
                if(j != 0)
                    output += ",";
                output += csvTable[i][j];
            }
            output += "\n";
        }

        return output;
    }
}
