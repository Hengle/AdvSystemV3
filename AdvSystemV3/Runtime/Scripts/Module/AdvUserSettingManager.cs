using UnityEngine;

public class AdvUserSettingManager
{
    static AdvUserSettingManager instance;
    public static AdvUserSettingManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AdvUserSettingManager();
                return instance;
            }
            return instance;
        }
    }

    public AdvUserSettingData DefaultValue;
    public AdvUserSettingManager()
    {
        DefaultValue = new AdvUserSettingData(){
            dataNextTextShowTime = AdvUserConfig.Instance.NextTextShowTime,
            dataEveryLineEndWaitTime = AdvUserConfig.Instance.EveryLineEndWaitTime,
            dataSayDialogAlpha = AdvUserConfig.Instance.SayDialogAlpha,
            dataDialogSkipMode = AdvUserConfig.Instance.advDialogSkipMode,
            dataDialogUseCtrlToSkip = AdvUserConfig.Instance.advDialogUseCtrlToSkip,
            dataStillAutoWhenClick = AdvUserConfig.Instance.advStillAutoWhenClick,
            dataVoiceSkipWhenClick = AdvUserConfig.Instance.advVoiceSkipWhenClick,
            dataAdvLanguage = (int) FungusExt.LocalizeManager.GetExistLanguage(),
        };
        
        Debug.Log("Get Default Adv language is :" + ((SystemLanguage)DefaultValue.dataAdvLanguage).ToString());

        SetUserSettingDefault();
    }

    //AdvUserConfig is the asset of Default set
    public void SetUserSettingDefault()
    {
        SetAdvUserSetting(DefaultValue);
    }

    [SerializeField] private float nextTextShowTime;
    [SerializeField] private float everyLineEndWaitTime;
    [SerializeField] private float sayDialogAlpha;
    [SerializeField] private AdvDialogSkipMode dialogSkipMode;
    [SerializeField] private AdvDialogUseCtrlToSkip dialogUseCtrlToSkip;
    [SerializeField] private AdvStillAutoWhenClick stillAutoWhenClick;
    [SerializeField] private AdvVoiceSkipWhenClick voiceSkipWhenClick;
    [SerializeField] private int advLanguage;
    

    public int AdvLanguage {
        get { return advLanguage; }
        set {
            advLanguage = value;
            OnAdvLanguageSet?.Invoke(value);
        }
    }

    public float NextTextShowTime
    {
        get { return nextTextShowTime; }
        set
        {
            nextTextShowTime = value;
            OnNextTextShowTimeChange?.Invoke(nextTextShowTime);
        }
    }
    public float EveryLineEndWaitTime
    {
        get { return everyLineEndWaitTime; }
        set
        {
            everyLineEndWaitTime = value;
            OnEveryLineEndWaitTimeChange?.Invoke(everyLineEndWaitTime);
        }
    }
    public float SayDialogAlpha
    {
        get { return sayDialogAlpha; }
        set
        {
            sayDialogAlpha = value;
            OnAdvDialogAlphaSet?.Invoke(sayDialogAlpha);
        }
    }
    public AdvDialogSkipMode DialogSkipMode
    {
        get { return dialogSkipMode; }
        set
        {
            dialogSkipMode = value;
            OnAdvDialogSkipModeChange?.Invoke(dialogSkipMode);
        }
    }
    public AdvDialogUseCtrlToSkip DialogUseCtrlToSkip
    {
        get { return dialogUseCtrlToSkip; }
        set
        {
            dialogUseCtrlToSkip = value;
            OnAdvDialogUseCtrlToSkipChange?.Invoke(dialogUseCtrlToSkip);
        }
    }
    public AdvStillAutoWhenClick StillAutoWhenClick
    {
        get { return stillAutoWhenClick; }
        set
        {
            stillAutoWhenClick = value;
            OnAdvStillAutoWhenClickChange?.Invoke(stillAutoWhenClick);
        }
    }
    public AdvVoiceSkipWhenClick VoiceSkipWhenClick
    {
        get { return voiceSkipWhenClick; }
        set
        {
            voiceSkipWhenClick = value;
            OnAdvVoiceSkipWhenClickChange?.Invoke(voiceSkipWhenClick);
        }
    }

    public AdvUserSettingData GetAdvDefaultSetting()
    {
        return DefaultValue;
    }

    //取得與設置 Adv User Setting 窗口
    public AdvUserSettingData GetAdvUserSetting()
    {
        AdvUserSettingData data = new AdvUserSettingData()
        {
            dataNextTextShowTime = nextTextShowTime,
            dataEveryLineEndWaitTime = everyLineEndWaitTime,
            dataSayDialogAlpha = sayDialogAlpha,
            dataDialogSkipMode = dialogSkipMode,
            dataDialogUseCtrlToSkip = dialogUseCtrlToSkip,
            dataStillAutoWhenClick = stillAutoWhenClick,
            dataVoiceSkipWhenClick = voiceSkipWhenClick,
            dataAdvLanguage = advLanguage,
        };

        return data;
    }
    public void SetAdvUserSetting(AdvUserSettingData data)
    {
        NextTextShowTime = data.dataNextTextShowTime;
        EveryLineEndWaitTime = data.dataEveryLineEndWaitTime;
        SayDialogAlpha = data.dataSayDialogAlpha;
        DialogSkipMode = data.dataDialogSkipMode;
        DialogUseCtrlToSkip = data.dataDialogUseCtrlToSkip;
        StillAutoWhenClick = data.dataStillAutoWhenClick;
        VoiceSkipWhenClick = data.dataVoiceSkipWhenClick;
        AdvLanguage = data.dataAdvLanguage;
    }

    // Delegate for subscription of AdvManager , Writer
    public event NextTextShowTimeHandler OnNextTextShowTimeChange;              //Writer subscription
    public event EveryLineEndWaitTimeHandler OnEveryLineEndWaitTimeChange;      //Write subscription
    public event AdvSayDialogAlphaSetHandler OnAdvDialogAlphaSet;               //AdvManager subscription
    public event AdvDialogSkipModeHandler OnAdvDialogSkipModeChange;            //No use , Because DialogInput only check Manager's Value
    public event AdvDialogUseCtrlToSkipHandler OnAdvDialogUseCtrlToSkipChange;  //No use , Because DialogInput only check Manager's Value
    public event AdvStillAutoWhenClickHandler OnAdvStillAutoWhenClickChange;    //No use , Because DialogInput only check Manager's Value
    public event AdvVoiceSkipWhenClickHandler OnAdvVoiceSkipWhenClickChange;    //No use , Because DialogInput only check Manager's Value
    public event AdvLanguageSetHandler OnAdvLanguageSet;                        //AdvManager subscription
    public delegate void NextTextShowTimeHandler(float toValue);
    public delegate void EveryLineEndWaitTimeHandler(float toValue);
    public delegate void AdvSayDialogAlphaSetHandler(float iAlpha);
    public delegate void AdvDialogSkipModeHandler(AdvDialogSkipMode toValue);
    public delegate void AdvDialogUseCtrlToSkipHandler(AdvDialogUseCtrlToSkip toValue);
    public delegate void AdvStillAutoWhenClickHandler(AdvStillAutoWhenClick toValue);
    public delegate void AdvVoiceSkipWhenClickHandler(AdvVoiceSkipWhenClick toValue);
    public delegate void AdvLanguageSetHandler(int language);
    
}

public class AdvUserSettingData {
    public float dataNextTextShowTime;
    public float dataEveryLineEndWaitTime;
    public float dataSayDialogAlpha = 1.0f;
    public AdvDialogSkipMode dataDialogSkipMode;
    public AdvDialogUseCtrlToSkip dataDialogUseCtrlToSkip;
    public AdvStillAutoWhenClick dataStillAutoWhenClick;
    public AdvVoiceSkipWhenClick dataVoiceSkipWhenClick;
    public int dataAdvLanguage;
}
