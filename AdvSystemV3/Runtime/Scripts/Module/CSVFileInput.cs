using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using Fungus;

public class CSVFileInput : MonoBehaviour
{
    public GameObject flowchartPrefab;
    public Canvas CSVOption;
    public Image ResultPanel;
    public InputField commandInput;
    public InputField BlockInput;

    // Working Template
    public FlowchartExtend workFlowchart;
    CallExtend firstCall;

    public void OpenFileDialog(){
        //SimpleFileBrowser.FileBrowser.ShowLoadDialog( (path) => { ReadFile(path); }, null,
        //false, null, "選擇 CSV 檔案", "Select" );
    }

    public void OpenFileDialogToReplace(){
        //SimpleFileBrowser.FileBrowser.ShowLoadDialog( (path) => { ReadFileReplace(path); }, null,
        //false, null, "選擇 CSV 檔案", "Select" );
    }

    public void ReadFileReplace(string filePath){
        var sr = new StreamReader(File.Open(filePath, FileMode.Open));
        string rsult = sr.ReadToEnd();
        sr.Dispose();
        AdvUtility.UpdateBlockByCSV(workFlowchart, rsult, null, false);
    }


    public void ReadFile(string filePath){
        string rsult = string.Empty;
        AdvManager.Instance.useDebugMsg = true;
        try {
            var sr = new StreamReader(File.Open(filePath, FileMode.Open));
            rsult = sr.ReadToEnd();
            sr.Dispose();
        } catch (Exception e) {
            AdvUtility.Log("CSV 無法讀取， 確認是否有其他程式使用該檔案中 (" + e.Message + ")");
            return;
        }

        if(workFlowchart != null){
            Destroy(workFlowchart.gameObject);
        }
        GameObject copyFlowchart = Instantiate(flowchartPrefab);
        workFlowchart = copyFlowchart.GetComponent<FlowchartExtend>();
        //workFlowchart = flowchartPrefab.GetComponent<Flowchart>();
        firstCall = workFlowchart.GetComponent<CallExtend>();

        //1. 開始 ADV系統
        //2. 清除舊的除錯訊息
        AdvManager.Instance.StartAdvScene();
        AdvDebugMsg.ClearMessage();
        AdvDebugMessageBox.ClearMessage();

        //顯示讀取結果面板
        ResultPanel.gameObject.SetActive(true);

        //開始讀取
        AdvUtility.CreateBlockByCSV(workFlowchart, rsult, false);
    }

    
    

    protected IEnumerator RunBlock(Flowchart flowchart, Block targetBlock, int commandIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        flowchart.ExecuteBlock(targetBlock, commandIndex);
        
        AdvManager.Instance.StartAdvScene();
        // if(bg != null){
        //     AdvUtility.Log("> 強制設置背景");
        //     bg.DirectSetBackground();
        // }
    }
    void PlayCommand(int commandId)
    {
        //if(this.workFlowchart.csvCmdList[commandId] == null)
        if(AdvUtility.FindCommandByCSVLine(workFlowchart, commandId) == null)
        {
            AdvUtility.LogWarning("找不到該行數 Command !");
            return;
        }
        Command targetCmd = AdvUtility.FindCommandByCSVLine(workFlowchart, commandId);
        var targetBlock = targetCmd.ParentBlock;
        //ControlBackground bg = targetCmd.CSVBackgroundInfo;
        if (targetBlock.IsExecuting())
        {
            // The Block is already executing.
            // Tell the Block to stop, wait a little while so the executing command has a 
            // chance to stop, and then start execution again from the new command. 
            targetBlock.Stop();

            workFlowchart.StartCoroutine(RunBlock(workFlowchart, targetBlock, targetCmd.CommandIndex, 0.2f));
        }
        else
        {
            // Block isn't executing yet so can start it now.
            workFlowchart.ExecuteBlock(targetBlock, targetCmd.CommandIndex);

            AdvManager.Instance.StartAdvScene();
        }
        
    }

    public void RunFlowchart(){
        StartCoroutine(CoRunFlowChart());
    }

    IEnumerator CoRunFlowChart(){
        if(workFlowchart.AutoGenerateBlock == null)
            yield break;
        
        workFlowchart.StopAllBlocks();
        firstCall.TargetBlock = workFlowchart.AutoGenerateBlock;
        workFlowchart.onEndSystem += OnFlowchartEnd;
        yield return new WaitForSeconds(0.5f);
        
        //flowchart.ExecuteBlock(mainBlock);
        workFlowchart.gameObject.SetActive(false);
        workFlowchart.gameObject.SetActive(true);
        
        //yield return new WaitForSeconds(1.0f);
        //DebugMsg.LogWarning(AdvManager.Instance.sayDialog.ToString());
        //DebugMsg.LogWarning("is :" + AdvManager.Instance.sayDialog.isActiveAndEnabled);
        //DebugMsg.LogWarning(AdvManager.Instance.sayDialog);
        //AdvManager.Instance.sayDialog.SetActive(true);
        //DebugMsg.LogWarning("is :" + AdvManager.Instance.sayDialog.isActiveAndEnabled);
        //yield return new WaitForSeconds(1.0f);
        //DebugMsg.LogWarning("is :" + AdvManager.Instance.sayDialog.isActiveAndEnabled);
    }

    void OnFlowchartEnd(){
        AdvUtility.Log("> Adv 系統結束");
        AdvManager.Instance.StopAdvScene();
    }

    
    private void Update() {
        if(Input.GetKeyDown(KeyCode.F1)){
            CSVOption.gameObject.SetActive(!CSVOption.gameObject.activeSelf);
            AdvDebugMsg.instance.gameObject.SetActive(!AdvDebugMsg.instance.gameObject.activeSelf);
        }
        if(Input.GetKeyDown(KeyCode.F12)){
            Application.Quit();
        }
    }

    //直接執行CSV行數
    public void RunCommand(){
        int commandId;
        int.TryParse(commandInput.text , out commandId);
        PlayCommand(commandId);
    }

    //從哪個Block 進入
    public void IntoBlock(){

        Block targetBlock = workFlowchart.FindBlock(BlockInput.text);

        if(targetBlock != null) {
            AdvUtility.Log("尋找到 Block :" + targetBlock.name);
            workFlowchart.StopAllBlocks();
            AdvManager.Instance.StartAdvScene();
            workFlowchart.ExecuteBlock(targetBlock);
        } else {
            AdvUtility.Log("尋找 Block 失敗 (" + BlockInput.text + ")");
        }
    }

    public void SetLangTw(){
        //AdvManager.Instance.SetAdvDisplayLanguage(SystemLanguage.ChineseTraditional);
    }
    public void SetLangEn(){
        //AdvManager.Instance.SetAdvDisplayLanguage(SystemLanguage.English);
    }
    public void SetLangJp(){
        //AdvManager.Instance.SetAdvDisplayLanguage(SystemLanguage.Japanese);
    }
}
