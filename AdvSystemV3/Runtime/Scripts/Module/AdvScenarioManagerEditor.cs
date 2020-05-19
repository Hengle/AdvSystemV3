#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using Ideafixxxer.CsvParser;

public class AdvScenarioManagerEditor
{
    const string requestDataMethod = "GetRawCSV";
    const string requestDescription = "GetSheetName";
    const string requestPagesInfo = "GetPagesInfo";
    string webServiceURL;
    string spreadsheetId;
    string sheet_gid;
    UnityWebRequest mConnection_WWW;
    AdvPagePrefab pageRef;
    Fungus.AdvUpdateOption option;
    List<AdvPagePrefab> pagesInfo;
    ImportType importType;
    bool autoRemove;
    bool popupDetal;

    enum ImportType {
        Data,
        Description,
        Create,
        DownloadPagesInfo,
        JustCheck
    }

    public static void AdvCreateNew(string sURL, string sID, string gID,  AdvPagePrefab page){
        if(page == null || string.IsNullOrEmpty(page.Page_gid))
            return;

        AdvScenarioManagerEditor instance = new AdvScenarioManagerEditor();

        instance.webServiceURL = sURL;
        instance.spreadsheetId = sID;
        instance.sheet_gid = gID;
        instance.pageRef = page;
        instance.importType = ImportType.Create;
        instance.Import_Google(false);
    }

    public static void AdvUpdatePrefab(string sURL, string sID, string gID, AdvPagePrefab page, Fungus.AdvUpdateOption opt, bool popup, bool autoRm = false)
    {
        if(page == null || page.prefab == null)
            return;

        AdvScenarioManagerEditor instance = new AdvScenarioManagerEditor();

        instance.webServiceURL = sURL;
        instance.spreadsheetId = sID;
        instance.sheet_gid = gID;
        instance.pageRef = page;
        instance.option = opt;
        instance.importType = ImportType.Data;
        instance.autoRemove = autoRm;
        instance.popupDetal = popup;
        instance.Import_Google(false);
    }

    public static void AdvGetDescription(string sURL, string sID, string gID, AdvPagePrefab page){
        if(page == null || page.prefab == null)
            return;

        if(!string.IsNullOrEmpty(page.description))
            return;

        AdvScenarioManagerEditor instance = new AdvScenarioManagerEditor();

        instance.webServiceURL = sURL;
        instance.spreadsheetId = sID;
        instance.sheet_gid = gID;
        instance.pageRef = page;
        instance.importType = ImportType.Description;
        instance.Import_Google(false);
    }

    public static void AdvGetPagesInfo(string sURL, string sID, List<AdvPagePrefab> pages){

        AdvScenarioManagerEditor instance = new AdvScenarioManagerEditor();

        instance.webServiceURL = sURL;
        instance.spreadsheetId = sID;
        instance.sheet_gid = "dummy";
        instance.pagesInfo = pages;
        instance.importType = ImportType.DownloadPagesInfo;
        instance.Import_Google(false);
    }

    void Import_Google(bool JustCheck)
    {
        UnityWebRequest www = Import_Google_CreateWWWcall(JustCheck);
        if (www == null)
            Debug.LogError("Unable to import from google");
        else
        {
            mConnection_WWW = www;
            EditorApplication.update += CheckForConnection;
            Debug.Log("Downloading spreadsheet ...");
        }
    }

    UnityWebRequest Import_Google_CreateWWWcall(bool JustCheck)
    {
        if (!HasGoogleSpreadsheet())
            return null;
        
        string request = requestDataMethod;
        
        switch (importType)
        {
            case ImportType.Data:
                request = requestDataMethod;
                break;
            case ImportType.Description:
                request = requestDescription;
                break;
            case ImportType.DownloadPagesInfo:
                request = requestPagesInfo;
                break;
        }

        string query = string.Format("{0}?key={1}&gid={2}&action={3}",
                                        webServiceURL,
                                        spreadsheetId,
                                        sheet_gid,
                                        request);

        if (JustCheck)
        {
            query += "&justcheck=true";
        }

        UnityWebRequest www = UnityWebRequest.Get(query);
        //I2.Loc.I2Utils.SendWebRequest(www);

#if UNITY_2017_2_OR_NEWER
        www.SendWebRequest();
#else
        www.Send();
#endif

        return www;
    }

    void CheckForConnection()
    {
        if (mConnection_WWW != null && mConnection_WWW.isDone)
        {
            string Result = string.Empty;
            string Error = mConnection_WWW.error;

            if (string.IsNullOrEmpty(Error))
            {
                Result = System.Text.Encoding.UTF8.GetString(mConnection_WWW.downloadHandler.data); //mConnection_WWW.text;
            }

            StopConnectionWWW();
            Imported_Google(Result, Error);
        }
    }

    void StopConnectionWWW()
    {
        EditorApplication.update -= CheckForConnection;				
        mConnection_WWW = null;
    }

    void Imported_Google( string Result, string Error)
    {
        if (!string.IsNullOrEmpty(Error))
        {
            Debug.Log("Unable to access google : " + Error);
            return;
        }

        bool isEmpty = string.IsNullOrEmpty(Result) || Result == "\"\"";

        if (!isEmpty)
        {
            if(importType == ImportType.Data)
                UpdateData(Result);
            else if(importType == ImportType.Create)
                CreateData(Result);
            else if (importType == ImportType.DownloadPagesInfo)
                DownloadPagesInfo(Result);
            else
                UpdateDescription(Result);

        } else {
            Debug.LogError("No data in downloaded csv");
        }

        Debug.Log("Language Source was up-to-date with Google Spreadsheet");
    }

    bool HasGoogleSpreadsheet()
    {
        return !string.IsNullOrEmpty(webServiceURL) && !string.IsNullOrEmpty(spreadsheetId) && !string.IsNullOrEmpty(sheet_gid);
    }

    void CreateData(string Result){
        string sourceCSV = Result;
        Fungus.FlowchartExtend sourceObject = pageRef.prefab;
        if(sourceObject == null)
        {
            string path = string.Format("{0}/{1}.prefab", AdvScenarioManager.Instance.DefaultPrefabFolder, string.IsNullOrEmpty(pageRef.description) ? pageRef.Page_gid : pageRef.description);
            
            Fungus.FlowchartExtend tempObj = new GameObject().AddComponent<Fungus.FlowchartExtend>();
            sourceObject = PrefabUtility.SaveAsPrefabAsset(tempObj.gameObject, path).GetComponent<Fungus.FlowchartExtend>();
            pageRef.prefab = sourceObject;
            MonoBehaviour.DestroyImmediate(tempObj.gameObject);
        }
        sourceObject.GoogleSheetID = spreadsheetId;
        sourceObject.GooglePageID = sheet_gid;

        AdvUtility.CreateBlockByCSV(sourceObject, sourceCSV, true);
    }

    void UpdateData(string Result){
        string sourceCSV = Result;
        Fungus.FlowchartExtend sourceObject = pageRef.prefab;

        sourceObject.GoogleSheetID = spreadsheetId;
        sourceObject.GooglePageID = sheet_gid;
        List<AdvCSVLine> outdate = AdvUtility.UpdateBlockByCSV(sourceObject, sourceCSV, option, true);
        List<AdvCSVLine> willRemove = outdate;

        UnityEditor.EditorUtility.SetDirty(sourceObject.gameObject);
        PrefabUtility.SavePrefabAsset(sourceObject.gameObject);

        if(willRemove == null)
            return;

        if(popupDetal){
            AdvPrefabUpdateResult.OpenWindow(sourceObject, willRemove);
            return;
        }

        if(!autoRemove)
            return;

        foreach (var item in willRemove)
        {
            sourceObject.csvLines.Remove(item);
            Fungus.Command cmd = item.generatedCommand;
            if(cmd != null){
                string msg = string.Format("Auto remove update lost cmd ... {0} -> {1} -> {2}&cmd{3}", cmd.gameObject.name, AdvUtility.FindParentBlock(cmd).BlockName, cmd.ItemId, item.keys);
                Debug.Log(msg);
                AdvUtility.FindParentBlock(cmd).CommandList.Remove(cmd);
                Object.DestroyImmediate(cmd);
            }
        }

        willRemove.Clear();
    }

    void UpdateDescription(string Result){
        pageRef.description = Result;
    }

    void DownloadPagesInfo(string Result){

        CsvParser csvParser = new CsvParser();
        string[][] csvTable = csvParser.Parse(Result);

        for (int i = 0; i < csvTable.Length; i++)
        {
            pagesInfo.Add(new AdvPagePrefab(){description = csvTable[i][0], Page_gid = csvTable[i][1]});
        }
    }

    public static void RebuildCSVLine(Fungus.FlowchartExtend sourceObject){
        AdvUtility.RebuildCSVLine(sourceObject);
        AdvUtility.AutoLinkCSVLine(sourceObject);

        UnityEditor.EditorUtility.SetDirty(sourceObject.gameObject);
        Debug.Log("Rebuild CSVLine & Link Block Success");
    }

}

public class AdvPrefabUpdateResult : Sirenix.OdinInspector.Editor.OdinEditorWindow
{
    public static AdvPrefabUpdateResult OpenWindow(Fungus.FlowchartExtend flowchart, List<AdvCSVLine> result){
        var window = GetWindow<AdvPrefabUpdateResult>();
        window.sourceObject = flowchart;
        window.willRemove = result;
        return window;
    }

    [Sirenix.OdinInspector.Title("Adv Prefab Update")]
    [Sirenix.OdinInspector.InfoBox("要更新的Prefab")]
    public Fungus.FlowchartExtend sourceObject;

    [Sirenix.OdinInspector.Title("Will Remove")]
    [Sirenix.OdinInspector.InfoBox("CSV 中已移除的Cmd，會存在於此清單，點擊 StartRemove 來確認移除")]
    [Sirenix.OdinInspector.TableList(DrawScrollView = true, HideToolbar = false)]
    public List<AdvCSVLine> willRemove;

    [Sirenix.OdinInspector.Button(Sirenix.OdinInspector.ButtonSizes.Large)]
    public void StartRemove() {
        foreach (var item in willRemove)
        {
            sourceObject.csvLines.Remove(item);
            Fungus.Command cmd = item.generatedCommand;
            if(cmd != null){
                string msg = string.Format("Remove deleted cmd : {0} -> {1} -> {2} & {3}", cmd.gameObject.name, AdvUtility.FindParentBlock(cmd).BlockName, cmd.ItemId, item.keys);
                Debug.Log(msg);
                AdvUtility.FindParentBlock(cmd).CommandList.Remove(cmd);
                Object.DestroyImmediate(cmd, true);
            }
        }
        willRemove.Clear();
    }
}


#endif