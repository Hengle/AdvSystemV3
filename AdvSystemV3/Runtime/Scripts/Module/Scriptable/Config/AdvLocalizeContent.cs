using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace FungusExt
{
    [CreateAssetMenu(menuName = "Adv Manager/Localize Manager")]
    public class AdvLocalizeContent : ReleaseConfig<AdvLocalizeContent>
    {
        [Header("Web Server URL")] public string webServices;
        [Header("Localize Config")] public List<SupportLanguages> SupportLanguages;
        public int CSVTextContentStartAtColumn = 8;
        [Header("Total Spreadsheet & gid")] public List<SheetData> spreadsheets;
        [Header("Names Spreadsheet page")] public NamesSheetPage namesSheetPage;

        [Header("Localize Content")]
        public List<LocalizeActorName> ActorNames;
        public List<NarrativeSheet> LocalizeNarratives;

        public List<string> GetActorNamesList(){
            if(ActorNames == null)
                return null;
            if(ActorNames.Count == 0)
                return null;

            List<string> temp = new List<string>();
            foreach (var item in ActorNames)
            {
                temp.Add(item.key);
            }
            return temp;
        }
    }


    [System.Serializable]
    public class SheetData
    {
        public string sheet_id;
        public string description;
        public List<PageData> infos;
    }

    [System.Serializable]
    public class PageData
    {
        [HorizontalGroup("Info", 0.4f, LabelWidth = 80)] public string gid;
        [HorizontalGroup("Info")] public string description;
    }

    [System.Serializable]
    public class NamesSheetPage
    {
        public string sheet_id;
        public string page_gid;
    }
}