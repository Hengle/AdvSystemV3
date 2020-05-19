using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;

public class DownloadManager
{
    const string requestPageCSV = "GetRawCSV";          //input: SheetID, PageID , return: pageCSV
    const string requestPageName = "GetSheetName";      //input: SheetID, PageID , return: pageName
    const string requestPagesInfos = "GetPagesInfo";    //input: SheetID ,         return: pageNames, pageIDs

    string webServiceURL;
    string spreadsheetId;
    string sheet_gid;
    int mRow;
    int mColumn;

    string webAction;
    Action<string> mCallback;

    /// <summary>
    /// 以HTTP GET 方式連接EA設計的 Google App Scripts
    /// 並取得回傳的CSV檔案
    /// </summary>
    /// <param name="service">App Script Service 網址</param>
    /// <param name="ssid">來源Spreadsheet的ID</param>
    /// <param name="gid">來源Spreadsheet的分頁gid</param>
    /// <param name="action">想進行的行為, 預設為GetRawCSV</param>
    public static void GoogleGetCSV(Action<string> callback, string service, string ssid, string gid, int row = 0, int column = 0){
        GoogleSheetConnect(callback, service, ssid, gid, row, column, requestPageCSV);
    }

    public static void GoogleGetPageName(Action<string> callback, string service, string ssid, string gid){
        GoogleSheetConnect(callback, service, ssid, gid, 0, 0, requestPageName);
    }

    public static void GoogleGetPagesInfos(Action<string> callback, string service, string ssid){
        GoogleSheetConnect(callback, service, ssid, "dummy", 0, 0, requestPagesInfos);
    }

    public static void GoogleSheetConnect(Action<string> callback, string service, string ssid, string gid, int row = 0, int column = 0, string action = requestPageCSV){
        DownloadManager instance = new DownloadManager();

        instance.webServiceURL = service;
        instance.spreadsheetId = ssid;
        instance.sheet_gid = gid;
        instance.mRow = row;
        instance.mColumn = column;
        instance.webAction = action;
        instance.mCallback = callback;
        instance.Start();
    }

    public static void GoogleOpenURL(string service, string ssid, string gid, int row = 0, int column = 0, string action = requestPageCSV)
    {
        string query = string.Format("{0}?key={1}&gid={2}&action={3}",
                                        service,
                                        ssid,
                                        gid,
                                        action);
        if(row > 0 && column > 0)
            query += string.Format("&row={0}&column={1}", row, column);

        Application.OpenURL(query);
    }

    async void Start(){
        string result = await Import_Google();
        mCallback?.Invoke(result);
    }

    async Task<string> Import_Google()
    {
        UnityWebRequest www = Import_Google_CreateWWWcall();
        string result = string.Empty;
        if (www == null)
            Debug.LogError("Unable to import from google");
        else
        {
            Debug.Log("Downloading spreadsheet ...");
            result = await WaitForWWW(www);
        }
        return await Task.FromResult<string>(result);
    }

    async Task<string> WaitForWWW(UnityWebRequest www)
    {
        string result = string.Empty;

        while (true)
        {
            if (www == null)
                break;

            if (www.isDone)
            {
                string Error = www.error;

                if (string.IsNullOrEmpty(Error))
                {
                    result = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    bool isEmpty = string.IsNullOrEmpty(result) || result == "\"\"";

                    if (isEmpty){
                        Debug.LogError("No data in downloaded csv");
                        result = string.Empty;
                    }

                    Debug.Log("CSV was up-to-date with Google Spreadsheet");
                }
                else
                {
                    Debug.Log("Unable to access google : " + Error);
                }

                break;
            }
            await Task.Yield();
        }
        return await Task.FromResult<string>(result);
    }

    UnityWebRequest Import_Google_CreateWWWcall()
    {
        if (string.IsNullOrEmpty(webServiceURL) || string.IsNullOrEmpty(spreadsheetId) || string.IsNullOrEmpty(sheet_gid) || string.IsNullOrEmpty(webAction))
            return null;

        string query = string.Format("{0}?key={1}&gid={2}&action={3}",
                                        webServiceURL,
                                        spreadsheetId,
                                        sheet_gid,
                                        webAction);
        
        if(mRow > 0 && mColumn > 0)
            query += string.Format("&row={0}&column={1}", mRow, mColumn);

        UnityWebRequest www = UnityWebRequest.Get(query);

#if UNITY_2017_2_OR_NEWER
        www.SendWebRequest();
#else
        www.Send();
#endif

        return www;
    }
}
