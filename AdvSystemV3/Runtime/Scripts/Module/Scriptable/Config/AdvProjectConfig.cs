using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Adv Config/Project Config")]
public class AdvProjectConfig : ReleaseConfig<AdvProjectConfig>
{
    [Adv.HelpBox] public string tip = "此專案的Adv樣式設定";

    [Header("預設 Say 顏色")]
    [Tooltip("預設 Say 名稱顏色")] public Color SaySpeakerColor = Color.white;
    [Tooltip("預設 Say 內文顏色")] public Color SayContentColor = Color.white;
    [Tooltip("獨立設定特定名字的顏色與內文色"), Space(5)] public List<NameColor> nameAndColor;

    [Header("系統規則")]
    [Tooltip("立繪出現時, 一律隱藏對話大頭照")] public bool AlwaysHideIconWhenOnBillboard = true;
    [Tooltip("進入新的Flowchart時, 重置Avatar Display 設置")] public bool SetShowAvatarInNewFlowchart = true;

    [System.Serializable]
    public class NameColor {
        [ValueDropdown ("useNamePopup")] public string useName;
        public Color useColor = Color.white;
        public Color useColorStory = Color.black;

        public List<string> useNamePopup(){
            return FungusExt.AdvLocalizeContent.Instance.GetActorNamesList();
        }
    }
}