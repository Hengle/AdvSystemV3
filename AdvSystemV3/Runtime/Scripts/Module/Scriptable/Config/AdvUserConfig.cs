using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu (menuName = "Adv Config/User Config")]
public class AdvUserConfig : ReleaseConfig<AdvUserConfig>
{
    [Header("文字顯示速度")]
    [Range(0,1)]
    public float NextTextShowTime = 0.5f;

    [Header("自動播放模式的顯示速度")]
    [Range(0,1)]
    public float EveryLineEndWaitTime = 0.5f;
    
    [LabelText("對話框透明度"),Range(0,1)]
    public float SayDialogAlpha = 1.0f;
    
    [Header("Skip Mode的省略方式")]
    public AdvDialogSkipMode advDialogSkipMode;

    [Header("用Ctrl鍵進行Skip")]
    public AdvDialogUseCtrlToSkip advDialogUseCtrlToSkip;

    [Header("點擊時是否繼續自動播放")]
    public AdvStillAutoWhenClick advStillAutoWhenClick;

    [Header("點擊時進行語音Skip")]
    public AdvVoiceSkipWhenClick advVoiceSkipWhenClick;

    [Header("初始系統音量設定")]
    [LabelText("總音量大小"), Range(-40,0)]
    public float MasterValue = 0.0f;
    [LabelText("背景音量大小"), Range(-40,0)]
    public float BgmValue = 0.0f;

    [LabelText("語音音量大小"), Range(-40,0)]
    public float VoiceValue = 0.0f;

    [LabelText("效果音量大小"), Range(-40,0)]
    public float EffectValue = 0.0f;
    
    public float[] GetVolumeSetting()
    {
        float[] volumes = new float[4];
        volumes[0] = MasterValue;
        volumes[1] = EffectValue;
        volumes[2] = VoiceValue;
        volumes[3] = BgmValue;
        return volumes;
    }
}

//////////////
/// Adv 自定義 Class， Enum
//////////////

public enum AdvDialogSkipMode
{
    HaveRead,
    All,
}

public enum AdvDialogUseCtrlToSkip
{
    HaveRead,
    All,
}

public enum AdvStillAutoWhenClick
{
    Yes,
    No,
}

public enum AdvVoiceSkipWhenClick
{
    Yes,
    No,
}
