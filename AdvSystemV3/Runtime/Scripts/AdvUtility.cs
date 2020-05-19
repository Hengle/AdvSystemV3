using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using FungusExt;
using Ideafixxxer.CsvParser;

public static partial class AdvUtility
{
    public static string SystemRandIntegerName = "Auto_Random";

    public static string CSVImportedBlockName = "Main";
    public static string DefaultCharacterSpriteKey = "Normal";
    public static string DefaultOutputFolder = "Assets/Content";
    public static int CSVLanguageDataStart = 8;

    //////////////
    //CSV 讀取相關功能
    //////////////

    enum CmdResult {
        Success = 0,
        Ignore = 1,
        Error = -1,
    }

    static Type GetTypeOfCommand(string typeName){
        Type commandType = null;
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            commandType = asm.GetType("Fungus." + typeName);
            if(commandType != null)
            {
                break;
            }
        }
        if(commandType == null)
        {
            LogError("the commandType(" + typeName + ") cannot find, please check!");
        }
        return commandType;
    }

    static Command AddCommandToBlock(FlowchartExtend srcFlowchart, Block srcBlock , Type srcCommand){
        //1.建立 Command
        //2.設置 Command 屬於哪個Block
        //3.給予 Command 專屬Index , 用於差段執行指令
        Command resultCommand = srcBlock.gameObject.AddComponent(srcCommand) as Command;
        //resultCommand.ParentBlock = srcBlock;
        resultCommand.ItemId = srcFlowchart.NextItemId();

        // Let command know it has just been added to the block
        resultCommand.OnCommandAdded(srcBlock);
        srcBlock.CommandList.Add(resultCommand);
        return resultCommand;
    }

    static Variable AddVariableToFlowchart(FlowchartExtend srcFlowchart, string variableType){
        Variable newVariable = srcFlowchart.gameObject.AddComponent(GetTypeOfCommand(variableType)) as Variable;
        newVariable.Key = srcFlowchart.GetUniqueVariableKey(AdvUtility.SystemRandIntegerName);
        srcFlowchart.Variables.Add(newVariable);
        return newVariable;
    }

    static void SetupCommand(FlowchartExtend workFlowchart, Command workCmd, int lineIndex, string cmdKey){
        ICommand iCmd = workCmd as ICommand;
        if(iCmd == null)
            return;

        //設置Command 屬於的 Line值, Key 值, Backgound 值
        iCmd.CSVLine = lineIndex;
        iCmd.CSVCommandKey = cmdKey;
    }

    //CSV 讀取相關功能: 為Block 設立新的點
    static Vector2 GetNewBlockPosition (ref Vector2 oldPos , Vector2 bias)
    {
        oldPos = oldPos + bias;
        return oldPos;
    }

    //主要CSV 讀入部分
    public static void CreateBlockByCSV(FlowchartExtend srcFlowchart, string csvFile, bool inEditor)
    {
        //讀入 CSV 檔案，使其分為 string 二維陣列
        CsvParser csvParser = new CsvParser();
        string[][] csvTable = csvParser.Parse(csvFile);

        //判斷CSV 檔案 有無內容
        if (csvTable.Length <= 1)
        {
            AdvUtility.Log("No data rows in file");
            return;
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
            AdvUtility.LogWarning("確認 CSV 是否為逗號分隔 (?");
            LogBox("確認 CSV 檔案是否為逗號分隔 (? ");
            return;
        }
        if(id_key == -1 || id_command == -1 || id_target == -1)
        {
            AdvUtility.LogWarning("CSV 檔案沒有正確的開頭資訊 (Keys, Command, Target, Arg1, Arg2, Content_zh-tw)");
            return;
        }

        //至此 CSV檔案正常
        srcFlowchart.csvBackup = csvFile;

        if(AdvKeyContent.GetCurrentInstance() == null){
            AdvUtility.LogWarning("Adv Keys 尚未初始化! 資源讀取將會失敗!");
        }

        if(srcFlowchart.AutoGenerateBlock != null){
            AdvUtility.LogWarning("該Block 已經使用過Create指令, 將清除並重新Create!");
        }

        foreach (var comp in srcFlowchart.GetComponents<Component>())
        {
            if (!(comp is Transform) && !(comp is FlowchartExtend))
            {
                MonoBehaviour.DestroyImmediate(comp, true);
            }
        }

        //視覺化建立的 Block 的位置
        Vector2 newNodePosition = Vector2.zero;
        newNodePosition = new Vector2(
            50 / srcFlowchart.Zoom - srcFlowchart.ScrollPos.x + UnityEngine.Random.Range(-10.0f, 10.0f),
            50 / srcFlowchart.Zoom - srcFlowchart.ScrollPos.y + UnityEngine.Random.Range(-10.0f, 10.0f)
        );
        newNodePosition = GetNewBlockPosition(ref newNodePosition, new Vector2(50 , 50 ));

        //1. 儲存Block 資訊用於CSV架構 
        //2. SearchHandler為搜尋Block標籤用的事件，當新Block建立時，觸發事件去找該Block是不是該Command要找的Block
        Dictionary<string, Block> blockTree  = new Dictionary<string, Block>();
        SearchBlockHandler searchHandler = new SearchBlockHandler();

        //建立入口Block
        Block baseBlock = srcFlowchart.CreateBlock(newNodePosition);
        srcFlowchart.AutoGenerateBlock = baseBlock;                             //Main Block 設置完成
        baseBlock.BlockName = AdvUtility.CSVImportedBlockName;          //Main Block 名稱設置為 Main
        blockTree.Add(baseBlock.BlockName,baseBlock);

        //第一行指令設置為 建立入口用
        Call Entrance = AddCommandToBlock(srcFlowchart, baseBlock, GetTypeOfCommand("CallExtend")) as Call;

        //1. 正在添加的Block
        //2. 該Command 對應的背景, 用於跳場景用
        Block workBlock = baseBlock;
        ControlBackground workBackground = null;

        srcFlowchart.csvLines = new List<AdvCSVLine>();                 //重置 CSV Lines , 這樣的設置前提是每個 Flowchart只匯入一個 CSV

        LogBox("初始化完畢...");

        //針對每一行CSV做處理
        for(int i=1; i< csvTable.Length ; i++){

            //建立新的CSV Line, 讀入該Line資料進去
            AdvCSVLine thisLine = new AdvCSVLine(csvTable[0], csvTable[i]);
            //加入該CSV Line 進入 flowchart CSVLine 集合
            srcFlowchart.csvLines.Add(thisLine);

            string srcCommand = csvTable[i][id_command];
            bool comResult = false;

            //如果Command是*記錄點，建立Block，否則建立Command
            if (srcCommand.StartsWith("*")){
                comResult = true;
                string _name = srcCommand;
                workBlock = srcFlowchart.CreateBlock(GetNewBlockPosition(ref newNodePosition,new Vector2(150,50)));
                workBlock.BlockName = _name;
                blockTree.Add(workBlock.BlockName,workBlock);
                thisLine.generateBlock = workBlock;
                searchHandler.createBlockEvent?.Invoke(workBlock);
            } else {
                Command newCommand = CreateCommand(srcFlowchart, workBlock, csvTable[0], csvTable[i], searchHandler);
                if(newCommand != null){
                    comResult = true;
                    SetupCommand(srcFlowchart, newCommand, i, csvTable[i][id_key]);

                    //設置flowchart中,CSVLine表key值對應的Command , 用於更新資料
                    thisLine.generatedCommand = newCommand;

                    //如果指令是背景, 設置切入劇情時的背景
                    if (newCommand.GetType() == typeof(ControlBackground)) {
                        workBackground = newCommand as ControlBackground;
                    }
                }
            }
            
            if(comResult == false)
                LogBox("讀取第 " + i + " 行指令 失敗 , 指令碼(Command)錯誤");
            else
                LogBox("讀取第 " + i + " 行指令 成功");

        }
        LogBox("文本讀取完畢!");
    }


    public static List<AdvCSVLine> UpdateBlockByCSV(FlowchartExtend srcFlowchart, string csvFile, AdvUpdateOption advOption, bool inEditor)
    {
        //讀入 CSV 檔案，使其分為 string 二維陣列
        CsvParser csvParser = new CsvParser();
        string[][] csvTable = csvParser.Parse(csvFile);

        //判斷CSV 檔案 有無內容
        if (csvTable.Length <= 1)
        {
            AdvUtility.Log("No data rows in file");
            return null;
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
            AdvUtility.LogWarning("確認 CSV 是否為逗號分隔 (?");
            LogBox("確認 CSV 檔案是否為逗號分隔 (? ");
            return null;
        }
        if(id_key == -1 || id_command == -1 || id_target == -1 || csvTable[0].Length <= CSVLanguageDataStart)
        {
            AdvUtility.LogWarning("CSV 檔案沒有正確的開頭資訊 (Keys, Command, Target, Arg1, Arg2, Text)");
            return null;
        }
        //至此 CSV檔案正常
        srcFlowchart.csvBackup = csvFile;

        if(AdvKeyContent.GetCurrentInstance() == null){
            AdvUtility.LogWarning("Adv Keys 尚未初始化! 資源讀取將會失敗!");
        }

        if(advOption == null){
            advOption = new AdvUpdateOption();
        }

        SearchBlockHandler searchHandler = new SearchBlockHandler();
        ControlBackground workBackground = null;
        Block srcBlock = srcFlowchart.MainBlock;
        int srcCmdIndex = 0;

        SetCSVLineReadyToUpdate(srcFlowchart.csvLines);

        //針對每一行CSV做更新
        for(int i=1; i< csvTable.Length ; i++){
            //取出Command 以及 Key
            string srcCommandString = csvTable[i][id_command];
            string cmdKey = csvTable[i][id_key];

            //建立新的CSVLine
            AdvCSVLine newLine = new AdvCSVLine(csvTable[0], csvTable[i]);
            
            //尋找舊的CSVLine
            AdvCSVLine oldLine = SearchLineByKey(srcFlowchart.csvLines, cmdKey);

            //若CSV Line中具有該key
            if(oldLine != null){
                //從Key提取 Command , 設置該 Command 所在 Block
                //2019.7.25 有可能這個Command 已經被編劇砍掉了，所以回傳null，但CSVLine 還保留著這個 Reference
                Command srcCmd = oldLine.generatedCommand;

                //如果是*Block CSVLine, 則不會有 Command
                if(srcCmd != null){
                    //這邊的動作是為了追蹤已經到哪個Block,哪個Line, 使得新指令能夠依賴此資訊插入
                    //srcBlock = srcCmd.ParentBlock; // Fungus 的 ParentBlock 為 Property
                    srcBlock = FindParentBlock(srcFlowchart, srcCmd);
                    if(srcBlock == null){
                        //Fungus 的 ParentBlock 為 Property , 這是不可序列化的, 因此只限定於開啟prefab時, 此值才能運作
                        AdvUtility.LogError("更新失敗, 嘗試跳出Prefab 視窗後，重新進入Prefab");
                        throw new NullReferenceException("更新失敗, 嘗試跳出Prefab 視窗後，重新進入Prefab");
                    }
                    srcCmdIndex = srcBlock.CommandList.FindIndex(x => x == srcCmd);
                }

                //更新targetLine (AdvCSVLine)資料 , 但僅止於CSVLine資料更新, 實際Prefab 要不要動還是以參數為主
                newLine.generatedCommand = srcCmd;
                ReplaceLine(srcFlowchart.csvLines, oldLine, newLine);

                CmdResult cmdResult = CmdResult.Error;
                string ResultMessage = "";

                //在該Key中，如果Cmd值相同,則更新資料，(不同則新增資料 待製作)
                if(String.Equals(oldLine.Command, newLine.Command, StringComparison.OrdinalIgnoreCase)){
                    if (srcCommandString.StartsWith("*")){
                        //以新Block 為基準工作
                        if(oldLine.generateBlock != null){
                            srcBlock = oldLine.generateBlock;
                            srcCmdIndex = -1;
                        }
                        else
                        {
                            //以舊Key 來補救 舊Key 無 Cmd資訊
                            string _oldName = oldLine.Command;
                            Block workBlock = srcFlowchart.FindBlock(_oldName);
                            if(workBlock != null){
                                srcBlock = workBlock;
                                srcCmdIndex = -1;

                                newLine.generateBlock = srcBlock;
                                cmdResult = CmdResult.Success;
                                AdvUtility.Log("> 以舊Key 更新 GenerateBlock");
                            }
                        }
                        newLine.generateBlock = srcBlock;

                        if(advOption.blockName)
                            cmdResult = CmdResult.Success;
                        else
                            cmdResult = CmdResult.Ignore;
                    }
                    else
                    {
                        //確保這個 Command 沒被編劇砍掉 才執行
                        if(srcCmd != null){
                            cmdResult = SetCommandPara(srcCmd, csvTable[0], csvTable[i], advOption, searchHandler);
                            //更新command中的 CSVLine, Key
                            SetupCommand(srcFlowchart, srcCmd, i, cmdKey);
                            //如果指令是背景, 設置切入劇情時的背景
                            if (srcCmd.GetType() == typeof(ControlBackground)) {
                                workBackground = srcCmd as ControlBackground;
                            }
                        } else {
                            ResultMessage += $"該 command ({oldLine.Command}) 已於flowchart中刪除\n";
                        }
                    }
                } 
                else
                {
                    //不同Command 可能是 Block 不同
                    if (srcCommandString.StartsWith("*")){
                        Block oldBlock = oldLine.generateBlock;
                        if(oldBlock != null){
                            //以新Block 為基準工作
                            srcBlock = oldLine.generateBlock;
                            srcCmdIndex = -1;

                            newLine.generateBlock = srcBlock;

                            if(advOption.blockName){
                                srcBlock.BlockName = srcCommandString;
                                searchHandler.createBlockEvent?.Invoke(srcBlock);
                                cmdResult = CmdResult.Success;
                            } else
                                cmdResult = CmdResult.Ignore;

                        } else {
                            AdvUtility.Log("> 舊Key 不包含 Block 資訊 , 嘗試以舊 Name 找 Block");

                            //允許不同, 執行以下 Block 命名相關指令, 記錄點，建立Block
                            string _oldName = oldLine.Command;
                            string _newName = srcCommandString;
                            Block workBlock = srcFlowchart.FindBlock(_oldName);
                            //AdvUtility.Log("尋找Block:" + _oldName + " ,  結果 :" + workBlock);
                            if(workBlock != null){
                                //以新Block 為基準工作
                                srcBlock = workBlock;
                                srcCmdIndex = -1;

                                newLine.generateBlock = srcBlock;

                                if(advOption.blockName){
                                    workBlock.BlockName = _newName;
                                    searchHandler.createBlockEvent?.Invoke(workBlock);
                                    //AdvUtility.Log("啟動 Invoke");
                                    cmdResult = CmdResult.Success;
                                } else
                                    cmdResult = CmdResult.Ignore;
                            } else {
                                AdvUtility.Log("> 無法找到舊Block 名稱 : " + _oldName);
                            }
                        }
                    } else {
                        //不處理不同Cmd的情況
                        ResultMessage += $"來源與目標的 Command 類型不同 , 放棄更新 {oldLine.Command} != {newLine.Command}\n";
                    }
                }

                if(cmdResult == CmdResult.Error)
                    AdvUtility.Log("> 更新指令 from key:" + cmdKey + " >> <color=red>失敗</color> (" + srcFlowchart.GetName() + ") >> " + ResultMessage);
                //else if(cmdResult == CmdResult.Success)
                //    AdvUtility.Log("> 更新指令 from key:" + cmdKey + " 成功 (" + srcFlowchart.GetName() + ")");

            } else {
                //如果沒有該Key, 則新增資料
                AdvUtility.Log("> <color=magenta>找不到Key值:" + cmdKey + " , 因此建立新指令</color>");
                bool cmdResult = false;

                if (srcCommandString.StartsWith("*")){
                    cmdResult = true;
                    string _name = srcCommandString;
                    Vector2 newPosition = new Vector2(srcBlock._NodeRect.x, srcBlock._NodeRect.y);
                    srcBlock = srcFlowchart.CreateBlock(newPosition +  new Vector2(100, 75));
                    srcCmdIndex = -1;

                    srcBlock.BlockName = _name;

                    newLine.generateBlock = srcBlock;
                    srcFlowchart.csvLines.Add(newLine);
                    searchHandler.createBlockEvent?.Invoke(srcBlock);
                }
                else
                {
                    //## Command 會接在Block最後面，因此需要調換位置
                    Command newCommand = CreateCommand(srcFlowchart, srcBlock, csvTable[0], csvTable[i], searchHandler);
                    if(newCommand != null){
                        cmdResult = true;
                        SetupCommand(srcFlowchart, newCommand, i, cmdKey);

                        //## 需要調換Command位置
                        srcBlock.CommandList.RemoveAt(srcBlock.CommandList.Count - 1);
                        srcBlock.CommandList.Insert(srcCmdIndex + 1, newCommand);
                        srcCmdIndex = srcCmdIndex + 1;

                        //設置flowchart中,CSVLine表key值對應的Command , 用於更新資料
                        newLine.generatedCommand = newCommand;
                        srcFlowchart.csvLines.Add(newLine);

                        //如果指令是背景, 設置切入劇情時的背景
                        if (newCommand.GetType() == typeof(ControlBackground)) {
                            workBackground = newCommand as ControlBackground;
                        }
                    }
                }

                if(cmdResult == false)
                    AdvUtility.Log("> 更新指令 from key:" + cmdKey + " >> <color=red>失敗</color> (" + srcFlowchart.GetName() + ")");
                //else
                //    AdvUtility.Log("> 更新指令 from key:" + cmdKey + " 成功 (" + srcFlowchart.GetName() + ")");
            }
        }

        AdvUtility.Log("> 更新指令 順利結束");

        List<AdvCSVLine> outdate = GetCSVLineNeverUpdate(srcFlowchart.csvLines);
        return outdate;
    }

    static Command CreateCommand(FlowchartExtend workFlowchart, Block workBlock, string[] titleLabel, string[] lineData, SearchBlockHandler searchHandler){

        //搜尋開頭字串正不正確
        int id_key = Array.IndexOf(titleLabel, AdvUtility.TitleKeys);
        int id_command = Array.IndexOf(titleLabel, AdvUtility.TitleCommand);
        int id_target = Array.IndexOf(titleLabel, AdvUtility.TitleTarget);
        int id_arg1 = Array.IndexOf(titleLabel, AdvUtility.TitleArg1);
        int id_arg2 = Array.IndexOf(titleLabel, AdvUtility.TitleArg2);
        int id_image = Array.IndexOf(titleLabel, AdvUtility.TitleImage);
        int id_name = Array.IndexOf(titleLabel, AdvUtility.TitleName);

        if(id_key == -1 || id_command == -1 || id_target == -1)
        {
            AdvUtility.LogWarning("CSV 檔案沒有正確的開頭資訊 (Keys, Command, Target, Arg1, Arg2, Text)");
            return null;
        }

        List<LocalizeText> locTexts = new List<LocalizeText>();
        for (int i = CSVLanguageDataStart; i < titleLabel.Length; i++) {
            locTexts.Add(new LocalizeText(){tag = titleLabel[i], content = lineData[i]});
        }

        string srcCommand = lineData[id_command];

        Command commandCreated = null;

        //如果指令為 Say
        if (String.Equals(srcCommand, CSVCommandSay[0], StringComparison.OrdinalIgnoreCase) || srcCommand == ""){
            if(lineData[id_image] == "" && lineData[id_name] == "" && locTexts.Count == 0)
                return null;    // Prevent only "keys" CSVLine
            
            commandCreated = AddCommandToBlock(workFlowchart, workBlock, GetTypeOfCommand(CSVCommandSay[1]));
            ICommand iCmd = commandCreated as ICommand;
            iCmd?.InitializeByParams(ParamCreateCommand(
                lineData[id_key],
                lineData[id_command],
                lineData[id_target],
                lineData[id_arg1],
                lineData[id_arg2],
                lineData[id_image],
                lineData[id_name],
                locTexts,
                searchHandler));
        }
        //或者其他類型
        else for (int i = 0; i < CSVCommandMapping.GetLength(0); i++)
        {
            if(String.Equals(srcCommand, CSVCommandMapping[i, 0], StringComparison.OrdinalIgnoreCase)){
                commandCreated = AddCommandToBlock(workFlowchart, workBlock, GetTypeOfCommand(CSVCommandMapping[i, 1]));
                ICommand iCmd = commandCreated as ICommand;
                iCmd?.InitializeByParams(ParamCreateCommand(
                    lineData[id_key],
                    lineData[id_command],
                    lineData[id_target],
                    lineData[id_arg1],
                    lineData[id_arg2],
                    lineData[id_image],
                    lineData[id_name],
                    locTexts,
                    searchHandler));
                break;
            }
        }

        return commandCreated;
    }

    static CmdResult SetCommandPara(Command srcCmd, string[] titleLabel, string[] lineData, AdvUpdateOption advOption, SearchBlockHandler searchHandler){
        //搜尋開頭字串正不正確
        int id_key = Array.IndexOf(titleLabel, AdvUtility.TitleKeys);
        int id_command = Array.IndexOf(titleLabel, AdvUtility.TitleCommand);
        int id_target = Array.IndexOf(titleLabel, AdvUtility.TitleTarget);
        int id_arg1 = Array.IndexOf(titleLabel, AdvUtility.TitleArg1);
        int id_arg2 = Array.IndexOf(titleLabel, AdvUtility.TitleArg2);
        int id_image = Array.IndexOf(titleLabel, AdvUtility.TitleImage);
        int id_name = Array.IndexOf(titleLabel, AdvUtility.TitleName);


        if(id_key == -1 || id_command == -1 || id_target == -1 || titleLabel.Length <= CSVLanguageDataStart)
        {
            AdvUtility.LogWarning("CSV 檔案沒有正確的開頭資訊 (Keys, Command, Target, Arg1, Arg2, Text)");
            return CmdResult.Error;
        }

        ICommand iCmd = srcCmd as ICommand;
        if(iCmd == null)
            return CmdResult.Error;

        List<LocalizeText> locTexts = new List<LocalizeText>();
        for (int i = CSVLanguageDataStart; i < titleLabel.Length; i++) {
            locTexts.Add(new LocalizeText(){tag = titleLabel[i], content = lineData[i]});
        }

        CmdResult cmdResult = CmdResult.Error;
        string srcCommand = lineData[id_command];

        //如果指令為 Say
        if (String.Equals(srcCommand, CSVCommandSay[0], StringComparison.OrdinalIgnoreCase) || srcCommand == ""){
            iCmd.InitializeByParams(ParamCreateCommand(
                lineData[id_key],
                lineData[id_command],
                lineData[id_target],
                lineData[id_arg1],
                lineData[id_arg2],
                lineData[id_image],
                lineData[id_name],
                locTexts,
                searchHandler,
                advOption));

            cmdResult = (advOption.sayText || advOption.sayTerm || advOption.saySprite) ? CmdResult.Success : CmdResult.Ignore ;
        }
        else for (int i = 0; i < CSVCommandMapping.GetLength(0); i++)
        {
            if(String.Equals(srcCommand, CSVCommandMapping[i, 0], StringComparison.OrdinalIgnoreCase)){
                if(CheckOptionInIndex(advOption, i)){
                    iCmd.InitializeByParams(ParamCreateCommand(
                        lineData[id_key],
                        lineData[id_command],
                        lineData[id_target],
                        lineData[id_arg1],
                        lineData[id_arg2],
                        lineData[id_image],
                        lineData[id_name],
                        locTexts,
                        searchHandler,
                        advOption));

                        cmdResult = CmdResult.Success;
                } else {
                    cmdResult = CmdResult.Ignore;
                }
                break;
            }
        }

        if(cmdResult == CmdResult.Error)
            Debug.Log($"設置參數時失敗: {iCmd.CSVCommandKey}");
        
        return cmdResult;
    }

    /// <summary>
    /// 搜尋Flowchart Prefab中的 CSV Line List 裡，特定的Key 是否存在
    /// </summary>
    public static AdvCSVLine SearchLineByKey(List<AdvCSVLine> csvLines, string key){
        foreach (var item in csvLines)
        {
            if(item.keys == key){
                return item;
            }
        }
        return null;
    }

    /// <summary>
    /// 以一個CSVLine 替換掉 CSVLine List 中的某個 CSVLine
    /// </summary>
    static void ReplaceLine(List<AdvCSVLine> csvLines, AdvCSVLine srcLine, AdvCSVLine targetLine) {
        csvLines[csvLines.FindIndex(ind => ind.Equals(srcLine))] =  targetLine;
    }

    /// <summary>
    /// 從 Flowchart Prefab中的 CSVLine List裡，尋找該Line所對應的Command
    /// </summary>
    public static Command FindCommandByCSVLine(FlowchartExtend srcFlowchart, int line){
        foreach (var item in srcFlowchart.csvLines)
        {
            if(item.generatedCommand != null && item.generatedCommand as ICommand != null){

                if((item.generatedCommand as ICommand).CSVLine == line){
                    return item.generatedCommand;
                }
            }
        }
        return null;
    }

    static void SetCSVLineReadyToUpdate(List<AdvCSVLine> csvLines){
        //設置所有CSV Line 為準備更新中, 因此未被更新的CSV Line 將會選擇性移除
        foreach (var item in csvLines)
        {
            item.beUpdate = false;
        }
    }

    static List<AdvCSVLine> GetCSVLineNeverUpdate(List<AdvCSVLine> csvLines){
        List<AdvCSVLine> rst = new List<AdvCSVLine>();
        foreach (var item in csvLines)
        {
            if(item.beUpdate == false)
                rst.Add(item);
        }
        return rst;
    }

    public static Block FindParentBlock(FlowchartExtend srcFlowchart, Command cmd){
        var blocks = srcFlowchart.GetComponents<Block>();
        foreach (Block item in blocks)
        {
            if(item.CommandList.Contains(cmd)){
                return item;
            }
        }
        return null;
    }

    public static Block FindParentBlock(Command cmd){
        var blocks = cmd.GetFlowchart().GetComponents<Block>();
        foreach (Block item in blocks)
        {
            if(item.CommandList.Contains(cmd)){
                return item;
            }
        }
        return null;
    }

    //////////////
    /// 以CSV 指令 來建立 Fungus Command，參數設置
    //////////////
    static object[] ParamCreateCommand(string _keys, string _command, string _target, string _arg1, string _arg2, string _image, string _name, List<LocalizeText> _locText, SearchBlockHandler searchHandler, AdvUpdateOption _opt = null){
        return new object[] { new CommandParam(){
            keys = _keys,
            command = _command,
            target = _target,
            arg1 = _arg1,
            arg2 = _arg2,
            image = _image,
            name = _name,
            locText = _locText,
            option = _opt,
            } , 
            searchHandler   // 當找不到未來的Block，此Handler 可協助找到
        };
    }
}
