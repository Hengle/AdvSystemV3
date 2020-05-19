using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Threading.Tasks;

[CreateAssetMenu (menuName = "Adv Content/Adv Scenario Manager")]
[GlobalConfig ("ScriptableObjects/")]
public class AdvScenarioManager : GlobalConfig<AdvScenarioManager>
{
    [HorizontalGroup("URL"), PropertySpace(SpaceBefore = 0, SpaceAfter = 10)] public string webServiceURL;
    [PropertySpace(SpaceBefore = 0, SpaceAfter = 10)] public string DefaultPrefabFolder = "Assets/ScriptableObjects/AdvContents";

    [FoldoutGroup("Update Config"), ToggleLeft] public bool _sayContentText = true;
    [FoldoutGroup("Update Config"), ToggleLeft] public bool _SelectionContentText = true;
    [FoldoutGroup("Update Config"), ToggleLeft] public bool _saySpeakerTerm = false;
    [FoldoutGroup("Update Config"), ToggleLeft] public bool _sayAvatarSprite = false;
    [FoldoutGroup("Update Config"), ToggleLeft] public bool _sayVoice = false;
    [FoldoutGroup("Update Config"), ToggleLeft] public bool _blockName = false;
    [FoldoutGroup("Update Config"), ToggleLeft] public bool _background = false;
    [FoldoutGroup("Update Config"), ToggleLeft] public bool _CG = false;
    [FoldoutGroup("Update Config"), ToggleLeft] public bool _billboard = false;
    [FoldoutGroup("Update Config"), ToggleLeft] public bool _bgm = false;

    [FoldoutGroup("Tools"), ToggleLeft] public bool showDownloadPagesInfo = false;
    [FoldoutGroup("Tools"), ToggleLeft] public bool showClearAndCreateButton = false;
    [FoldoutGroup("Tools"), ToggleLeft] public bool showRebuildCSVInfo = false;
    [FoldoutGroup("Tools"), ToggleLeft] public bool autoRemoveUnlinkCmd = false;

    [Space(10)] public List<AdvSpreadsheet> advSpreadsheets;

    [HorizontalGroup("URL"), Button(ButtonSizes.Small)]
    public void Instal(){
        Application.OpenURL("https://script.google.com/d/1jxv5tqqhMOIoWTds7DAeKRaGRZuI4ZVTklHmr09-iz157hMDzLBw95V-/newcopy");
    }

    [Button(ButtonSizes.Medium)]
    public void UpdateAllData(){
        Fungus.AdvUpdateOption _option = GetOption();

        #if UNITY_EDITOR
        int callOrder = 0;
        foreach (AdvSpreadsheet spreadsheet in advSpreadsheets)
        {
            foreach (AdvPagePrefab item in spreadsheet.Pages)
            {
                DelayCall(() => AdvScenarioManagerEditor.AdvUpdatePrefab(webServiceURL, spreadsheet.SpreadsheetID, item.Page_gid, item, _option, false, autoRemoveUnlinkCmd), callOrder);
                callOrder++;
            }
        }
        #endif
    }

    [Button(ButtonSizes.Medium), FoldoutGroup("Tools")]
    public void GetSheetPageDescription(){
        #if UNITY_EDITOR
        int callOrder = 0;
        foreach (AdvSpreadsheet spreadsheet in advSpreadsheets)
        {
            foreach (AdvPagePrefab item in spreadsheet.Pages)
            {
                DelayCall(() => AdvScenarioManagerEditor.AdvGetDescription(webServiceURL, spreadsheet.SpreadsheetID, item.Page_gid, item), callOrder);
                callOrder++;
            }
        }
        #endif
    }

    async void DelayCall(System.Action callback, int delay){
        await Task.Delay(System.TimeSpan.FromSeconds(delay * 2));
        Debug.Log("<color=green>Start Update prefab :" + delay + "</color>");
        callback();
    }

    public AdvSpreadsheet FindSpredsheetByPage(AdvPagePrefab page){
        foreach (var item in advSpreadsheets)
        {
            if(item.Pages.Contains(page))
                return item;
        }
        return null;
    }

    public Fungus.AdvUpdateOption GetOption(){
        Fungus.AdvUpdateOption _option = new Fungus.AdvUpdateOption(){
            sayText = _sayContentText,
            selectionText = _SelectionContentText,
            sayTerm = _saySpeakerTerm,
            saySprite = _sayAvatarSprite,
            sayVoice = _sayVoice,
            blockName = _blockName,
            background = _background,
            CG = _CG,
            billboard = _billboard,
            BGM = _bgm,
        };
        return _option;
    }
}

[System.Serializable]
public class AdvSpreadsheet {
    [HorizontalGroup("SheetID")] public string SpreadsheetID;
    public string description;
    public List<AdvPagePrefab> Pages;

    #if UNITY_EDITOR
    [HorizontalGroup("SheetID"), Button(ButtonSizes.Small), ShowIf("isDownloadShow")]
    void DownloadAllPages(){
        Pages = new List<AdvPagePrefab>();
        AdvScenarioManagerEditor.AdvGetPagesInfo(AdvScenarioManager.Instance.webServiceURL, SpreadsheetID, Pages);
    }

    bool isDownloadShow(){
        return AdvScenarioManager.Instance.showDownloadPagesInfo;
    }
    #endif
}


[System.Serializable]
public class AdvPagePrefab
{
    [HorizontalGroup("Asset")] public Fungus.FlowchartExtend prefab;
    public string Page_gid;
    [HorizontalGroup("Info"), ReadOnly] public string description;

    #if UNITY_EDITOR
    [HorizontalGroup("Cmd"), Button(ButtonSizes.Small)]
    public void OpenURL(){
        string url = string.Format("{0}?key={1}&gid={2}&action={3}",
                                        AdvScenarioManager.Instance.webServiceURL,
                                        AdvScenarioManager.Instance.FindSpredsheetByPage(this)?.SpreadsheetID,
                                        Page_gid,
                                        "GetRawCSV");
        Application.OpenURL(url);
    }

    [HorizontalGroup("Cmd"), Button(ButtonSizes.Small)]
    public void Update(){
        AdvScenarioManagerEditor.AdvUpdatePrefab(AdvScenarioManager.Instance.webServiceURL, 
                                                 AdvScenarioManager.Instance.FindSpredsheetByPage(this)?.SpreadsheetID, 
                                                 Page_gid, 
                                                 this, 
                                                 AdvScenarioManager.Instance.GetOption(),
                                                 true,
                                                 false);
    }

    [HorizontalGroup("Cmd"), Button(ButtonSizes.Small), ShowIf("isRebuildShow")]
    public void RebuildInfos(){
        if(prefab == null)
            return;
        AdvScenarioManagerEditor.RebuildCSVLine(prefab);
    }

    [HorizontalGroup("Asset"), Button(ButtonSizes.Small), ShowIf("isCreateShow")]
    public void ClearAndCreate(){
        AdvScenarioManagerEditor.AdvCreateNew(AdvScenarioManager.Instance.webServiceURL, 
                                                 AdvScenarioManager.Instance.FindSpredsheetByPage(this)?.SpreadsheetID, 
                                                 Page_gid, 
                                                 this);
    }

    bool isCreateShow(){
        return AdvScenarioManager.Instance.showClearAndCreateButton;
    }

    bool isRebuildShow(){
        return AdvScenarioManager.Instance.showRebuildCSVInfo;
    }
    #endif
}