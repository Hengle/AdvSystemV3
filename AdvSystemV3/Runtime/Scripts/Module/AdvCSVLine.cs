using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using FungusExt;
using Sirenix.OdinInspector;

[System.Serializable]
public class AdvCSVLine
{
    public string keys;
    public string Command;

    [VerticalGroup("Target"),LabelWidth(30)] 
    public string Target;

    [VerticalGroup("Target"),LabelWidth(30),LabelText("Arg1")] 
    public string Arg1;

    [VerticalGroup("Target"),LabelWidth(30),LabelText("Arg2")] 
    public string Arg2;

    public string Image;
    public string Name;
    [VerticalGroup("Content"),LabelWidth(30)] public List<LocalizeText> localizeText;
    [VerticalGroup("Link"),LabelWidth(30),LabelText("cmd")] public Command generatedCommand;
    [VerticalGroup("Link"),LabelWidth(30),LabelText("blk")] public Block generateBlock;

    [HideInInspector]
    public bool beUpdate;

    public AdvCSVLine(){}

    public AdvCSVLine(string[] titleLabel, string[] lineData){
        //搜尋開頭字串正不正確
        int id_key = Array.IndexOf(titleLabel, AdvUtility.TitleKeys);
        int id_command = Array.IndexOf(titleLabel, AdvUtility.TitleCommand);
        int id_target = Array.IndexOf(titleLabel, AdvUtility.TitleTarget);
        int id_arg1 = Array.IndexOf(titleLabel, AdvUtility.TitleArg1);
        int id_arg2 = Array.IndexOf(titleLabel, AdvUtility.TitleArg2);
        int id_image = Array.IndexOf(titleLabel, AdvUtility.TitleImage);
        int id_name = Array.IndexOf(titleLabel, AdvUtility.TitleName);

        if(id_key == -1 || id_command == -1 || id_target == -1 || titleLabel.Length <= AdvUtility.CSVLanguageDataStart)
        {
            AdvUtility.LogWarning("CSV 檔案沒有正確的開頭資訊 (Keys, Command, Target, Arg1, Arg2, Text)");
            return;
        }

        keys = lineData[id_key];
        Command = lineData[id_command];
        Target = lineData[id_target];
        Arg1 = lineData[id_arg1];
        Arg2 = lineData[id_arg2];
        Image = lineData[id_image];
        Name = lineData[id_name];

        localizeText = new List<LocalizeText>();
        for (int i = AdvUtility.CSVLanguageDataStart; i < titleLabel.Length; i++) {
            localizeText.Add(new LocalizeText(){tag = titleLabel[i], content = lineData[i]});
        }

        beUpdate = true;
    }
}