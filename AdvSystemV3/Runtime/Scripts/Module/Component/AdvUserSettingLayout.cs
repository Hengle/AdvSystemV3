using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdvUserSettingLayout : MonoBehaviour
{
    public Slider SLDSetNextText;
    public Slider SLDSetLineEnd;
    public Button BTNSetSkipMode;
    public Button BTNSetCtrlSkip;
    public Button BTNSetStillAuto;
    public Button BTNSetVoiceSkip;
    [HideInInspector] public TextMeshProUGUI TextSetSkipMode;
    [HideInInspector] public TextMeshProUGUI TextSetCtrlSkip;
    [HideInInspector] public TextMeshProUGUI TextSetStillAuto;
    [HideInInspector] public TextMeshProUGUI TextSetVoiceSkip;
    public Dropdown DPDLanguage;
    public Button BTNResetOption;
    public Button BTNBack;

    CanvasGroup canvas;

    void Awake(){
        TextSetSkipMode = BTNSetSkipMode.GetComponentInChildren<TextMeshProUGUI>();
        TextSetCtrlSkip = BTNSetCtrlSkip.GetComponentInChildren<TextMeshProUGUI>();
        TextSetStillAuto = BTNSetStillAuto.GetComponentInChildren<TextMeshProUGUI>();
        TextSetVoiceSkip = BTNSetVoiceSkip.GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start(){
        CloseSelfPanel();
    }

    void OnEnable () {
        SLDSetNextText.onValueChanged.AddListener(UserSetNextTextShowTime);
        SLDSetLineEnd.onValueChanged.AddListener(UserSetEveryLineEndWaitTime);
        BTNSetSkipMode.onClick.AddListener (UserSetAdvDialogSkipMode);
        BTNSetCtrlSkip.onClick.AddListener (UserSetAdvDialogUseCtrlToSkip);
        BTNSetStillAuto.onClick.AddListener (UserSetAdvStillAutoWhenClick);
        BTNSetVoiceSkip.onClick.AddListener (UserSetAdvVoiceSkipWhenClick);
        DPDLanguage.onValueChanged.AddListener(UserSetAdvLanguage);
        BTNResetOption.onClick.AddListener (ResetAdvUserSetting);
        BTNBack.onClick.AddListener(CloseSelfPanel);

        UpdateContent();
    }

    void OnDisable () {
        SLDSetNextText.onValueChanged.RemoveAllListeners ();
        SLDSetLineEnd.onValueChanged.RemoveAllListeners ();
        BTNSetSkipMode.onClick.RemoveAllListeners ();
        BTNSetCtrlSkip.onClick.RemoveAllListeners ();
        BTNSetStillAuto.onClick.RemoveAllListeners ();
        BTNSetVoiceSkip.onClick.RemoveAllListeners ();
        DPDLanguage.onValueChanged.RemoveAllListeners();
        BTNResetOption.onClick.RemoveAllListeners ();
        BTNBack.onClick.RemoveAllListeners();
    }


    ////////////////////
    // Adv 使用者 自訂設置窗口
    ////////////////////

    public void OpenSelfPanel(){
        if(canvas == null)
            canvas = GetComponent<CanvasGroup>();
            
        canvas.alpha = 1;
        canvas.blocksRaycasts = true;
    }

    public void CloseSelfPanel(){
        if(canvas == null)
            canvas = GetComponent<CanvasGroup>();
            
        canvas.alpha = 0;
        canvas.blocksRaycasts = false;
    }

    public void ResetAdvUserSetting(){
        AdvUserSettingManager.Instance.SetUserSettingDefault();
        UpdateContent();
    }

    //用於更新UI內容, OnEnable 時會啟動, 或者幫這個涵式訂閱 SaveDataManager -> onSettingDataLoaded
    public void UpdateContent(){
        SLDSetNextText.value = AdvUserSettingManager.Instance.NextTextShowTime;
        SLDSetLineEnd.value = AdvUserSettingManager.Instance.EveryLineEndWaitTime;

        TextSetSkipMode.text = GetAdvOptionTerm(AdvUserSettingManager.Instance.DialogSkipMode);
        TextSetCtrlSkip.text = GetAdvOptionTerm(AdvUserSettingManager.Instance.DialogUseCtrlToSkip);
        TextSetStillAuto.text = GetAdvOptionTerm(AdvUserSettingManager.Instance.StillAutoWhenClick);
        TextSetVoiceSkip.text = GetAdvOptionTerm(AdvUserSettingManager.Instance.VoiceSkipWhenClick);
    }

    public void UserSetNextTextShowTime(float src){
        AdvUserSettingManager.Instance.NextTextShowTime = src;
        SLDSetNextText.value = src;
    }

    public void UserSetEveryLineEndWaitTime(float src){
        AdvUserSettingManager.Instance.EveryLineEndWaitTime = src;
        SLDSetLineEnd.value = src;
    }

    public void UserSetAdvDialogSkipMode(){
        if(AdvUserSettingManager.Instance.DialogSkipMode == AdvDialogSkipMode.HaveRead){
            AdvUserSettingManager.Instance.DialogSkipMode = AdvDialogSkipMode.All;
        }
        else {
            AdvUserSettingManager.Instance.DialogSkipMode = AdvDialogSkipMode.HaveRead;
        }

        TextSetSkipMode.text = GetAdvOptionTerm(AdvUserSettingManager.Instance.DialogSkipMode);
    }

    public void UserSetAdvDialogUseCtrlToSkip(){
        if(AdvUserSettingManager.Instance.DialogUseCtrlToSkip == AdvDialogUseCtrlToSkip.HaveRead){
            AdvUserSettingManager.Instance.DialogUseCtrlToSkip = AdvDialogUseCtrlToSkip.All;
        }
        else {
            AdvUserSettingManager.Instance.DialogUseCtrlToSkip = AdvDialogUseCtrlToSkip.HaveRead;
        }

        TextSetCtrlSkip.text = GetAdvOptionTerm(AdvUserSettingManager.Instance.DialogUseCtrlToSkip);
    }

    public void UserSetAdvStillAutoWhenClick(){      
        if(AdvUserSettingManager.Instance.StillAutoWhenClick == AdvStillAutoWhenClick.Yes){
            AdvUserSettingManager.Instance.StillAutoWhenClick = AdvStillAutoWhenClick.No;
        }
        else {
            AdvUserSettingManager.Instance.StillAutoWhenClick = AdvStillAutoWhenClick.Yes;
        }

        TextSetStillAuto.text = GetAdvOptionTerm(AdvUserSettingManager.Instance.StillAutoWhenClick);
    }

    public void UserSetAdvVoiceSkipWhenClick(){      
        if(AdvUserSettingManager.Instance.VoiceSkipWhenClick == AdvVoiceSkipWhenClick.Yes){
            AdvUserSettingManager.Instance.VoiceSkipWhenClick = AdvVoiceSkipWhenClick.No;
        }
        else {
            AdvUserSettingManager.Instance.VoiceSkipWhenClick = AdvVoiceSkipWhenClick.Yes;
        }

        TextSetVoiceSkip.text = GetAdvOptionTerm(AdvUserSettingManager.Instance.VoiceSkipWhenClick);
    }

    public void UserSetAdvLanguage(int id){
        SystemLanguage targetLanguage = ConvertIdToLanguage(id);
        AdvUserSettingManager.Instance.AdvLanguage =  (int) targetLanguage;
    }

    public SystemLanguage ConvertIdToLanguage(int id){
        switch (id)
        {
            case 0:
                return SystemLanguage.ChineseTraditional;
            case 1:
                return SystemLanguage.English;
            case 2:
                return SystemLanguage.Japanese;
            default:
                break;
        }
        return SystemLanguage.ChineseTraditional;
    }

    //////////////
    /// Adv Config 相關字串
    //////////////

    public static string GetAdvOptionTerm(AdvDialogSkipMode src){
        if(src == AdvDialogSkipMode.HaveRead)
            return "Read Text";
        return "All Text";
    }
    public static string GetAdvOptionTerm(AdvDialogUseCtrlToSkip src){
        if(src == AdvDialogUseCtrlToSkip.HaveRead)
            return "Read Text";
        return "All Text";
    }
    public static string GetAdvOptionTerm(AdvStillAutoWhenClick src){
        if(src == AdvStillAutoWhenClick.Yes)
            return "Yes";
        return "No";
    }
    public static string GetAdvOptionTerm(AdvVoiceSkipWhenClick src){
        if(src == AdvVoiceSkipWhenClick.Yes)
            return "Yes";
        return "No";
    }
}
