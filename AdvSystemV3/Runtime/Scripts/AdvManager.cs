using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using Fungus;

public partial class AdvManager : SingletonTool.SingletonMono<AdvManager>
{
    public static AdvManager instance => Instance;

    public UnityEngine.UI.Image sayDialogImage;
    public UnityEngine.UI.Image sayDialogNameImage;

    //Adv Manager Data
    [Header("adv 功能子物件 (Runtime 自動抓取)")]
    public StageExtend advStage;
    public SayDialogExtend advSayDialog;
    public MenuDialogExtend advMenuDialog;
    public WriterExtend advWriter;
    public DialogInputExtend advDialogInput;
    public AdvNarrativeLog advNarrativeLog;

    public NarrativeLogExtend fungusNarrativeLog;   //與Fungus NarrativeLog 分開獨立運作

    [Header("adv 關聯子物件")]
    public AdvCanvas CanvasAdvFungus;
    public AdvCanvas CanvasAdvStageObject;
    public AdvCanvas CanvasAdvDialog;
    public AdvCanvas CanvasAdvMenu;
    public AdvCanvas CanvasAdvSystem;

    [Header("Adv 常數參數")]
    [SerializeField] protected float fadeDuration = 0.25f;
    [SerializeField] protected int MaxFlowchartStore = 30;
    [SerializeField] protected bool useColorChangeForAutoButtons = false;
    [SerializeField] public bool useDebugMsg = false;

    [Header("Adv Runtime 參數")]
    public bool IsEndToStopAdvBGM = true;

    [Header("暫存數值")]
    //Adv Manager Template
    public FlowchartExtend templateFlowchart;
    public Block focusBlock;
    public AdvLogLayout lastLogLayout = null;
    public Dictionary<string, bool> HasReadTable;
    Dictionary<string, FlowchartExtend> loadedFlowchartList = new Dictionary<string, FlowchartExtend>();
    Queue<string> loadedFlowchartQueue = new Queue<string>();
    float advTargetAlpha = 0;
    bool isUIShow = true;
    bool isClearPreviousStage = true;
    string submitButton;

    [Header("對外窗口 event")]
    public System.Action<FlowchartExtend, string> OnLoadAdvContnet; //Excuting Flowchart, Block Name
    public System.Action<Block> OnBlockExcuted;  //Excuting Block
    public System.Action<string, string> OnSayRead; //return: prefab name, cmd key
    public bool isAdvPlaying => advTargetAlpha > 0;

    protected override void Awake()
    {
        base.Awake();
        advStage = GetComponentInChildren<StageExtend>();
        advSayDialog = GetComponentInChildren<SayDialogExtend>();
        advMenuDialog = GetComponentInChildren<MenuDialogExtend>();
        advWriter = GetComponentInChildren<WriterExtend>();
        advDialogInput = GetComponentInChildren<DialogInputExtend>();
        advNarrativeLog = GetComponentInChildren<AdvNarrativeLog>();

        fungusNarrativeLog = GetComponentInChildren<NarrativeLogExtend>();

        HasReadTable = new Dictionary<string, bool>();
    }
    IEnumerator Start()
    {
        submitButton = EventSystem.current.GetComponent<StandaloneInputModule>().submitButton;

        base.transform.position = new Vector3(0, 0, -1); //Make sure adv object's position
        Camera.main.orthographic = true;
        Debug.Log("Auto Set Main Camera to Orthographic");

        BlockSignals.OnBlockEnd += (block) => { AdvSignals.DoAdvCheckFlowchartEnd(); };

        if(CanvasAdvStageObject == null){
            Debug.LogError("Adv Prefab not initialize");
            yield break;
        }

        StopAdvScene();
        ExitAllCanvas();
        
        yield return null;  //Wait for other plugin setup
        OnLanguageChange(AdvUserSettingManager.Instance.AdvLanguage);
    }



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //控制ESC 按鍵的行為
            ExcuteESCButton();
        }
        if (Input.GetMouseButtonDown(0) && !isUIShow)
        {
            //當場上UI隱藏時, 左鍵可以喚醒UI
            SetAdvUI_Active();
        }
        if (Input.GetButtonDown(submitButton) && !isUIShow)
        {
            //當場上UI隱藏時, 左鍵可以喚醒UI
            SetAdvUI_Active();
        }
        if (Input.GetMouseButtonDown(1))
        {
            //SwitchAdvUIShow();  //以滑鼠右鍵控制UI顯示
            //已交由 DialogMouseDetector 控制, 避免影響所有UI, ex: Log UI
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CompleteStopAdvSystem();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            AdvUserSettingManager.Instance.AdvLanguage = (int)SystemLanguage.English;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            AdvUserSettingManager.Instance.AdvLanguage = (int)SystemLanguage.ChineseTraditional;
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            AdvUserSettingManager.Instance.AdvLanguage = (int)SystemLanguage.Japanese;
        }
#endif
    }

    void LateUpdate()
    {
        UpdateUI();
    }

    private void OnEnable()
    {
        AdvUserSettingManager.Instance.OnAdvLanguageSet += OnLanguageChange;
        AdvUserSettingManager.Instance.OnAdvDialogAlphaSet += OnSayDialogAlphaChange;
        BlockSignals.OnBlockStart += OnFungusBlockExcuted;
    }

    private void OnDisable()
    {
        AdvUserSettingManager.Instance.OnAdvLanguageSet -= OnLanguageChange;
        AdvUserSettingManager.Instance.OnAdvDialogAlphaSet -= OnSayDialogAlphaChange;
        BlockSignals.OnBlockStart -= OnFungusBlockExcuted;
    }

    public void LoadContent(FlowchartExtend flowchart)
    {
        LoadContent(flowchart, null);
    }

    public void LoadContent(FlowchartExtend flowchart, MenuSelectCallback callback)
    {
        LoadContent(flowchart, flowchart.EntranceBlockName, callback);
    }

    /// <summary>
    /// 主要接口，讀取 Flowchart Prefab檔，第二參數 Adv 結束對話時執行
    /// </summary>
    public void LoadContent(FlowchartExtend flowchart, string blockName, MenuSelectCallback callback)
    {
        Debug.Log("<color=lime>trying Start Flowhart</color>");
        InstantiateAndManage(flowchart);
        StartAdvScene();
        if(AdvProjectConfig.Instance.SetShowAvatarInNewFlowchart)
            advSayDialog.IconDisplay = true;
            
        templateFlowchart.StartFlowChart(blockName, callback);
        OnLoadAdvContnet?.Invoke(flowchart, blockName);
        Debug.Log("<color=lime>Flowhart Started</color>");
    }

    /// <summary>
    /// 隱藏Adv系統所有的可見項目，並且停止Flowchart的執行
    /// </summary>
    public void CompleteStopAdvSystem()
    {
        if (templateFlowchart != null)
            templateFlowchart.StopAllBlocks();
        advMenuDialog.CloseMenuDialog();
        StopAdvScene();
    }

    #region Adv 運行中功能選項

    /// <summary>
    /// UI Button : 當UI Hide Button 按下此鈕時呼叫
    /// </summary>
    public void AdvMethod_Hide()
    {
        SetAdvUI_Inactive();
        StopAutoWrite();
        StopAutoSkip();
    }

    /// <summary>
    /// UI Button : 當UI Auto Button 按下此鈕時呼叫
    /// </summary>
    public void AdvMethod_Auto(ButtonOnOffSprite btn = null)
    {
        bool _result = advWriter.SwitchAutoWrite();

        if (btn == null)
            return;

        if (useColorChangeForAutoButtons)
        {
            if (_result)
                btn.color = new Color(1, 0, 0, 1);
            else
                btn.color = new Color(1, 1, 1, 1);
        }
        else
        {
            if (_result)
                btn.SetSpriteOnOff(true);
            else
                btn.SetSpriteOnOff(false);
        }
    }

    /// <summary>
    /// UI Button : 當UI Skip Button 按下此鈕時呼叫
    /// </summary>
    public void AdvMethod_Skip(ButtonOnOffSprite btn = null)
    {
        bool _result = advDialogInput.SwitchAutoSkip();

        if (btn == null)
            return;

        if (useColorChangeForAutoButtons)
        {
            if (_result)
                btn.color = new Color(1, 0, 0, 1);
            else
                btn.color = new Color(1, 1, 1, 1);
        }
        else
        {
            if (_result)
                btn.SetSpriteOnOff(true);
            else
                btn.SetSpriteOnOff(false);
        }
    }

    /// <summary>
    /// UI Button : 當UI Log Button 按下此鈕時呼叫
    /// </summary>
    public void AdvMethod_Log(AdvLogLayout targetLogPanel)
    {
        if (targetLogPanel == null)
            return;

        lastLogLayout = targetLogPanel;
        targetLogPanel.OpenSelf();
        targetLogPanel.ScrollToBottom();
        StopAutoSkip();
    }

    #endregion

    #region Scene Control

    /// <summary>
    /// 初始化、顯示Adv系統所有的可見項目
    /// </summary>
    public void StartAdvScene()
    {
        isUIShow = true;
        advTargetAlpha = 1.0f;

        ActiveAllCanvas();
        if (isClearPreviousStage)
            advStage.NewStage();
    }

    /// <summary>
    /// 初始化、顯示Adv系統所有的可見項目 (Editor)
    /// </summary>
    public void StartAdvSceneEditor()
    {
        advTargetAlpha = 1.0f;

        ActiveAllCanvas();
        advStage.NewStageEditor();
    }

    /// <summary>
    /// 隱藏Adv系統所有的可見項目，需另外注意隱藏後還必須釋放使用者輸入
    /// </summary>
    public void StopAdvScene()
    {
        //因為Adv系統 Dialog Fadeout後 自動Inactive
        advTargetAlpha = 0;

        if (CanvasAdvStageObject.CanContrl)
        {
            advStage.CloseStage();
        }

        if (IsEndToStopAdvBGM)
        {
            Fungus.FungusManager.Instance.MusicManager.StopMusic();
            //PluginBridge.ResumeMainBgm();
        }

        AdvSignals.DoAdvStopping();
    }

    /// <summary>
    /// 隱藏與顯示皆由一個 Alpha參數控制，因此以Update監控Alpha參數
    /// </summary>
    void UpdateUI()
    {
        if(CanvasAdvStageObject == null){
            Debug.LogError("Adv Prefab not initialize");
            return;
        }

        if (fadeDuration <= 0f)
        {
            //No Fade Animation , so assign value immediately
            AssignAllCanvasAlpha(advTargetAlpha);
        }
        else
        {
            float delta = (1f / fadeDuration) * Time.deltaTime;
            float alpha = Mathf.MoveTowards(CanvasAdvStageObject.Alpha, advTargetAlpha, delta);

            AssignAllCanvasAlpha(alpha);

            if (alpha <= 0f)
            {
                // once invisible , set canvas uninteractivable
                ExitAllCanvas();

                AdvSignals.DoAdvStopped();
            }
        }

        if (!isUIShow)
        {
            //ESC Buttom to Display UI
            CanvasAdvMenu.Alpha = 0;
            CanvasAdvSystem.Alpha = 0;
            CanvasAdvDialog.Alpha = 0;
        }
    }

    #endregion

    #region Canvas Control

    public void ActiveAllCanvas()
    {
        CanvasAdvFungus.ActiveCanvas();
        CanvasAdvStageObject.ActiveCanvas();
        CanvasAdvDialog.ActiveCanvas();
        CanvasAdvMenu.ActiveCanvas();
        CanvasAdvSystem.ActiveCanvas();
    }

    public void ExitAllCanvas()
    {
        CanvasAdvFungus.ExitCanvas();
        CanvasAdvStageObject.ExitCanvas();
        CanvasAdvDialog.ExitCanvas();
        CanvasAdvMenu.ExitCanvas();
        CanvasAdvSystem.ExitCanvas();
    }

    public void AssignAllCanvasAlpha(float val)
    {
        CanvasAdvFungus.Alpha = val;
        CanvasAdvStageObject.Alpha = val;
        CanvasAdvDialog.Alpha = val;
        CanvasAdvMenu.Alpha = val;
        CanvasAdvSystem.Alpha = val;
    }

    #endregion

    #region System Working Method

    /// <summary>
    /// 給 ESC 鍵使用，主要是切換 Adv 對話與設定區的隱藏顯示，以便玩家觀賞立繪
    /// </summary>
    public void SwitchAdvUIShow()
    {
        if (advDialogInput.IsAutoSkip)
            StopAutoSkip();

        if (advWriter.IsAutoWrite)
            StopAutoWrite();

        if (isUIShow)
            SetAdvUI_Inactive();
        else
            SetAdvUI_Active();
    }

    /// <summary>
    /// 隱藏對話框, 選項, 系統按鈕,僅保留背景以及立繪, 並關閉IO
    /// </summary>
    public void SetAdvUI_Inactive()
    {
        isUIShow = false;

        SetUI_Interactable(false);
    }

    /// <summary>
    /// 顯示對話框, 選項, 系統按鈕, 並啟用IO
    /// </summary>
    public void SetAdvUI_Active()
    {
        isUIShow = true;

        //延遲數秒後再開啟IO, 避免交錯輸入Bug
        base.StartCoroutine(DelaySetUI_Active());
    }

    /// <summary>
    /// 停止adv系統的 Auto Write
    /// </summary>
    public void StopAutoWrite()
    {
        advWriter.IsAutoWrite = false;
    }

    /// <summary>
    /// 停止adv系統的 Auto Skip
    /// </summary>
    public void StopAutoSkip()
    {
        advDialogInput.IsAutoSkip = false;
    }

    /// <summary>
    /// 立即呼叫已存在的 Adv Log
    /// </summary>
    public void AdvMethod_Log()
    {
        AdvMethod_Log(lastLogLayout);
    }

    /// <summary>
    /// 顯示對話框, 選項, 系統按鈕(On/Off)
    /// </summary>
    public void SetAdvUIVisible(bool val)
    {
        isUIShow = val;
    }

    #endregion

    #region Other Option Method

    /// <summary>
    /// 停止目前的Flowchart
    /// </summary>
    public void StopAllRunningFlowchart()
    {
        if (templateFlowchart != null)
            templateFlowchart.StopAllBlocks();
    }

    /// <summary>
    /// Adv開始時, 清除上一個場景殘留的畫面(On/Off)
    /// </summary>
    public void SetStageViewClearWhenEnter(bool val)
    {
        isClearPreviousStage = val;
    }

    /// <summary>
    /// Adv結束時, 將最後的adv畫面保留於畫面中(On/Off)
    /// </summary>
    public void SetStageViewStay(bool val)
    {
        CanvasAdvStageObject.CanContrl = !val;
    }

    #endregion

    #region Private Helper

    void InstantiateAndManage(FlowchartExtend flowchart)
    {
        string flowchartName = flowchart.GetName();
        if (loadedFlowchartList.ContainsKey(flowchartName))
        {
            templateFlowchart = loadedFlowchartList[flowchartName];
        }
        else
        {
            templateFlowchart = Instantiate(flowchart, Vector3.zero, Quaternion.identity, base.transform);
            templateFlowchart.name = flowchartName;
            loadedFlowchartList.Add(flowchartName, templateFlowchart);
            templateFlowchart.onEndSystem += StopAdvScene;

            //以Queue 限制最大Flowchart 數量
            loadedFlowchartQueue.Enqueue(flowchartName);
            if (loadedFlowchartQueue.Count > MaxFlowchartStore)
            {
                string tempName = loadedFlowchartQueue.Dequeue();

                if (loadedFlowchartList.ContainsKey(tempName))
                {
                    Destroy(loadedFlowchartList[tempName].gameObject);
                    loadedFlowchartList.Remove(tempName);
                }
            }
        }
    }

    /// <summary>
    /// ESC 鍵相關，不同狀態時提供不同功能 , blocksRaycasts 為True就是正在運作中
    /// </summary>
    void ExcuteESCButton()
    {
        if (lastLogLayout != null && lastLogLayout.CanvasGroupUsed.blocksRaycasts)
            lastLogLayout.CloseSelf();
        else
        {
            SwitchAdvUIShow();
        }
    }

    IEnumerator DelaySetUI_Active()
    {
        yield return null;
        SetUI_Interactable(true);
    }
    void SetUI_Interactable(bool val)
    {
        //取消使用者輸入影響對話
        advDialogInput.IsLockInput = !val;  //advDialogInput.enabled = val;

        //取消Option按鈕可按的部分
        CanvasAdvMenu.Interactable = val;
        CanvasAdvSystem.Interactable = val;
    }

    /// <summary>
    /// 切換語言時呼叫的callback函數，須將當前的所有物件替換語言
    /// </summary>
    void OnLanguageChange(int lang)
    {
        SystemLanguage targetLanguage = (SystemLanguage)lang;
        if (advNarrativeLog) advNarrativeLog.RefreshLogContentUI(targetLanguage);
        if (advSayDialog) advSayDialog.UpdateCurrentLangContent(targetLanguage);
        if (advMenuDialog) advMenuDialog.UpdateCurrentLangContent(targetLanguage);
        Debug.Log(">> Adv: Change ADV Language to " + targetLanguage);
    }

    /// <summary>
    // Adv 調整對話框透明度的callback函數
    /// </summary>
    public void OnSayDialogAlphaChange(float alpha)
    {
        sayDialogImage.color     = new Color(sayDialogImage.color.r    , sayDialogImage.color.g    , sayDialogImage.color.b    , alpha);
        sayDialogNameImage.color = new Color(sayDialogNameImage.color.r, sayDialogNameImage.color.g, sayDialogNameImage.color.b, alpha );
    }

    void OnFungusBlockExcuted(Block block)
    {
        OnBlockExcuted?.Invoke(block);
    }

    #endregion

    #region IO
    public static class IO
    {
        public static GameObject GetAdvTargetObject(AdvTargetObject objectLabel)
        {
            switch (objectLabel)
            {
                case AdvTargetObject.SayIcon:
                    return Instance.advSayDialog.SayIconObject;

                case AdvTargetObject.Dialog:
                    return Instance.advSayDialog.SayDialogObject;

                case AdvTargetObject.Background:
                    return Instance.advStage.BackgoundLayout.BackgroundTexBehide.gameObject;

                case AdvTargetObject.CG:
                    return Instance.advStage.BackgoundLayout.BackgroundDiceSpriteBehide.gameObject;

                case AdvTargetObject.BillboardLeftLeft:
                    return GetAdvBillboardObject(BillboardPosition.LeftLeft);

                case AdvTargetObject.BillboardLeft:
                    return GetAdvBillboardObject(BillboardPosition.Left);

                case AdvTargetObject.BillboardMiddle:
                    return GetAdvBillboardObject(BillboardPosition.Middle);

                case AdvTargetObject.BillboardRight:
                    return GetAdvBillboardObject(BillboardPosition.Right);

                case AdvTargetObject.BillboardRightRight:
                    return GetAdvBillboardObject(BillboardPosition.RightRight);

                default:
                    break;
            }
            return null;
        }

        public static GameObject GetAdvBillboardObject(BillboardPosition billPos)
        {
            //Deprecate
            // GameObject tempObj = Instance.advStage.BillboardLayout.GetBillboardOnStage(billPos);
            // if (tempObj != null)
            //     return tempObj;

            RectTransform tempRect = Instance.advStage.BillboardGUILayout.GetBillboardOnStage(billPos);
            if (tempRect != null)
                return tempRect.gameObject;

            return null;
        }

        public static readonly Dictionary<string, AdvTargetObject> MapAdvTargetObject = new Dictionary<string, AdvTargetObject>(){
            { "LL", AdvTargetObject.BillboardLeftLeft },
            { "L", AdvTargetObject.BillboardLeft },
            { "M", AdvTargetObject.BillboardMiddle },
            { "R", AdvTargetObject.BillboardRight },
            { "RR", AdvTargetObject.BillboardRightRight },
            { "Icon", AdvTargetObject.SayIcon },
            { "Dialog", AdvTargetObject.Dialog },
        };

        public static AdvTargetObject GetAdvTargetObjectByString(string str)
        {
            AdvTargetObject result;
            if(!MapAdvTargetObject.TryGetValue(str, out result))
                result = AdvTargetObject.Dialog;

            return result;
        }
    }
    #endregion

}
