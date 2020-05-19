using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class AdvUtility
{
    //CSV Titles Keys	Command	Target	Arg1	Arg2    Image	ScenePreiveiw(無用欄位)	HeadPreview(無用欄位)	Name	Content_zh-tw
    public static string TitleKeys = "Keys";
    public static string TitleCommand = "Command";
    public static string TitleTarget = "Target";
    public static string TitleArg1 = "Arg1";
    public static string TitleArg2 = "Arg2";
    public static string TitleImage = "Image";
    public static string TitleName = "Name";

    //////////////////////////
    //Command Strings , [0] is CSV string , [1] is Fungus command Class name
    //////////////////////////
    
    //Special CSV command
    public static string CSVCommandPrefix = "*";
    public static string [] CSVCommandSay = {"Say","SayExtend"};

    // //Normal CSV command , 目前支援18個 CSV 指令 , old Map
    // public static string [,] CSVCommandMapping = new string[18, 2] {
    //     {"Bg",                  "ControlBackground"},       //0
    //     {"BgOff",               "ControlBackground"},
    //     {"Cg",                  "ControlCG"},               //2
    //     {"CgOff",               "ControlCG"},
    //     {"Billboard",           "BillBoard"},               //4
    //     {"BillboardOff",        "BillBoard"},
    //     {"BillboardPfb",        "BillboardPrefab"},         //6
    //     {"BillboardPfbOff",     "BillboardPrefab"},
    //     {"Selection",           "Menu"},                    //8
    //     {"Jump",                "Call"},                    //9
    //     {"Timeline",            "PlayTimelineAsset"},       //10
    //     {"BGM",                 "PlayMusic"},               //11
    //     {"Wait",                "Wait"},                    //12
    //     {"vpun",                "PunchPosition"},
    //     {"hpun",                "PunchPosition"},
    //     {"rotpun",              "PunchRotation"},
    //     {"scalpun",             "PunchScale"},              //16
    //     {"Entrance",            "ChangeEntrance"},          //17
    // };

    //Normal CSV command , 目前支援18個 CSV 指令
    public static string [,] CSVCommandMapping = new string[18, 2] {
        {"Bg",                  "ControlBackground"},       //0
        {"BgOff",               "ControlBackground"},
        {"Cg",                  "ControlCG"},               //2
        {"CgOff",               "ControlCG"},
        {"Billboard",           "BillBoard"},               //4
        {"BillboardOff",        "BillBoard"},
        {"BillboardPfb",        "BillboardPrefab"},         //6
        {"BillboardPfbOff",     "BillboardPrefab"},
        {"Selection",           "MenuExtend"},                    //8
        {"Jump",                "CallExtend"},                    //9
        {"Timeline",            "PlayTimelineAsset"},       //10
        {"BGM",                 "PlayMusicExtend"},               //11
        {"Wait",                "WaitExtend"},                    //12
        {"vpun",                "PunchPositionExtend"},
        {"hpun",                "PunchPositionExtend"},
        {"rotpun",              "PunchRotationExtend"},
        {"scalpun",             "PunchScaleExtend"},              //16
        {"Entrance",            "ChangeEntrance"},          //17
    };

    //Mapping adv option and CSVCommandMapping index
    public static bool CheckOptionInIndex(Fungus.AdvUpdateOption option, int index){
        if(index == 0 || index == 1){
            if(option.background)
                return true;
        }
        if(index == 2 || index == 3)
            if(option.CG)
                return true;
        if(index == 4 || index == 5 || index == 6 || index == 7)
            if(option.billboard)
                return true;
        if(index == 8)
            if(option.selection || option.selectionText)
                return true;
        if(index == 9)
            if(option.jump)
                return true;
        if(index == 10)
            if(option.timeline)
                return true;
        if(index == 11)
            if(option.BGM)
                return true;
        if(index == 12)
            if(option.wait)
                return true;
        if(index == 13 || index == 14 || index == 15 || index == 16)
            if(option.punch)
                return true;
        if(index == 17)
            if(option.entrance)
                return true;

        return false;
    }

    // public static string TitleContent_zhtw = "Content_zh-tw";
    // public static string TitleContent_zhcn = "Content_zh-cn";
    // public static string TitleContent_en = "Content_english";
    // public static string TitleContent_jp = "Content_jp";
    // public static string TitleContent_kr = "Content_kr";
    // public static string [] CSVCommandTimeline = {"Timeline","PlayTimelineAsset"};
    // public static string [] CSVCommandBGM = {"BGM","PlayMusic"};
    // public static string [] CSVCommandWait = {"Wait","Wait"};
    // public static string [] CSVCommandVPunch = {"vpun","PunchPosition"};
    // public static string [] CSVCommandHPunch = {"hpun","PunchPosition"};
    // public static string [] CSVCommandRotPunch = {"rotpun","PunchRotation"};
    // public static string [] CSVCommandScalPunch = {"scalpun","PunchScale"};
    // public static string [] CSVCommandBackground = {"Bg","ControlBackground"};
    // public static string [] CSVCommandBackgroundOff = {"BgOff","ControlBackground"};
    // public static string [] CSVCommandCG = {"Cg","ControlCG"};
    // public static string [] CSVCommandCGOff = {"CgOff","ControlCG"};
    // public static string [] CSVCommandBillboard = {"Billboard","BillBoard"};
    // public static string [] CSVCommandBillboardOff = {"BillboardOff","BillBoard"};
    // public static string [] CSVCommandBillboardPfb = {"BillboardPfb","BillboardPrefab"};
    // public static string [] CSVCommandBillboardPfbOff = {"BillboardPfbOff","BillboardPrefab"};
    // public static string [] CSVCommandSelection = {"Selection","Menu"};
    // public static string [] CSVCommandJump = {"Jump","Call"};
    // public static string [] CSVCommandEnter = {"Entrance","ChangeEntrance"};

}
