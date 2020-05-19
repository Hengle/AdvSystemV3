using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FungusExt;

namespace Fungus
{
    public class SayParas
    {
        public string say;
        public AdvDataCharacter content;
        public Sprite portrait;
        public AdvUpdateOption option;
    }
    /// <summary>
    /// Writes text in a dialog box.
    /// </summary>
    [CommandInfo("Narrative", 
                 "Say (Extend)", 
                 "Writes text in a dialog box.")]
    [AddComponentMenu("")]
    public class SayExtend : Say , ICommand
    {
        [SerializeField] protected int csvLine;
        [SerializeField] protected string csvCommandKey;
        public int CSVLine { get { return csvLine; } set { csvLine = value; }}
        public string CSVCommandKey { get { return csvCommandKey; } set { csvCommandKey = value; }}
        
        //parent use : character, portrait , saytext
        [SerializeField] protected bool showIcon = true;
        [SerializeField] protected string overrideTerm;             //Term is a string define from manager
        [SerializeField] protected Color overrideNameColor;
        [SerializeField] protected Sprite overridePortrait;
        [SerializeField] protected AdvDataCharacter dataCharacter;
        [SerializeField] protected List<string> portraitEquip = new List<string>();
        
        [Tooltip("這句話是不是已經被看過了 , 僅限制於 Runtime, 須對外來儲存結果")] protected bool hasRead = false;

        public virtual bool ShowIcon { get { return showIcon; } set { showIcon = value; }}
        public virtual bool HasRead => hasRead;
        
        public override void OnEnter(){
            if (!showAlways && executionCount >= showCount)
            {
                Continue();
                return;
            }

            executionCount++;

            // Override the active say dialog if needed
            if (dataCharacter != null && dataCharacter.SetSayDialog != null)
            {
                SayDialog.ActiveSayDialog = character.SetSayDialog;
            }

            if (setSayDialog != null)
            {
                SayDialog.ActiveSayDialog = setSayDialog;
            }

            // Default say dialog is AdvManager's
            var sayDialog = AdvManager.Instance.advSayDialog;
            if (sayDialog == null)
            {
                Continue();
                return;
            }

            var flowchart = GetComponent<FlowchartExtend>();

            bool isRead = false;
            if(AdvManager.Instance.HasReadTable.TryGetValue($"{gameObject.name}.{csvCommandKey}", out isRead)){
                hasRead = true;
            }

            sayDialog.SetActive(true);

            //1. Set name , color , portait
            string nameTerm = overrideTerm;
            string stringName = "";
            if(!string.IsNullOrEmpty(overrideTerm)){
                stringName = LocalizeManager.GetLocalizeName(overrideTerm);
                stringName = string.IsNullOrEmpty(stringName) ? $"no key:{overrideTerm}" : stringName;
            }

            Color nameColor;
            Color contentColor;
            nameColor = sayDialog.ColorName.TryGetValue(overrideTerm, out nameColor) ? nameColor : AdvProjectConfig.Instance.SaySpeakerColor;
            contentColor = sayDialog.ColorText.TryGetValue(overrideTerm, out contentColor) ? contentColor : AdvProjectConfig.Instance.SayContentColor;

            sayDialog.SetCharacter(stringName, nameColor, overridePortrait);
            sayDialog.SetStoryColor(contentColor);

            //2. Set Content text
            string displayText = storyText;
            string textTerm = $"{flowchart.GoogleSheetID}.{flowchart.GooglePageID}.{CSVCommandKey}";
            string localText = LocalizeManager.GetLocalizeText(textTerm);

            if(!string.IsNullOrEmpty(localText))
                displayText = localText;

            var activeCustomTags = CustomTag.activeCustomTags;
            for (int i = 0; i < activeCustomTags.Count; i++)
            {
                var ct = activeCustomTags[i];
                displayText = displayText.Replace(ct.TagStartSymbol, ct.ReplaceTagStartWith);
                if (ct.TagEndSymbol != "" && ct.ReplaceTagEndWith != "")
                {
                    displayText = displayText.Replace(ct.TagEndSymbol, ct.ReplaceTagEndWith);
                }
            }
            string subbedText = flowchart.SubstituteVariables(displayText); //displayText is origin Tag string , subbedText is Replace by Variables Text

            //3. Set Icon show / hide
            sayDialog.SetIconDisplay(showIcon);

            // //如果立繪中有存在這個 SpeakerTerm , 啟用立繪相關功能
            // BillboardController billboardCtrl = null;
            // if(!string.IsNullOrEmpty(speakerTerm.ToString())){
            //     billboardCtrl = AdvManager.Instance.mainStage.BillboardLayout.FindBillboardWithTerm(speakerTerm);
            // }
            // if(billboardCtrl != null){
            //     //關閉頭相框
            //     sayDialog.SayIconObject.SetActive(false);

            //     //從 atlas 取得說話表情
            //     billboardCtrl.SetRendererSprite(portrait);
            //     AdvManager.Instance.mainStage.BillboardLayout.PopUpBillboard(billboardCtrl);
            // }
            // else 
            // {
            //     AdvManager.Instance.mainStage.BillboardLayout.ResumeBillboardPop();
            // }

            //4. Set billboard on Stage
            UIBillboardController uiCtrl = null;
            if(!string.IsNullOrEmpty(overrideTerm)){
                uiCtrl = AdvManager.Instance.advStage.BillboardGUILayout.FindBillboardWithTerm(overrideTerm);
            }
            if(uiCtrl != null){
                //關閉頭相框
                if(AdvProjectConfig.Instance.AlwaysHideIconWhenOnBillboard)
                    sayDialog.SayIconObject.SetActive(false);

                if(overridePortrait){
                    uiCtrl.RuntimeSetEmoji(overridePortrait.name);
                }

                    uiCtrl.RuntimeUnequip();
                    uiCtrl.RuntimeSetEquip(portraitEquip);
            }
            else 
            {

            }


            sayDialog.SetLogInfo(nameTerm, textTerm, voiceOverClip);

            sayDialog.Say(subbedText, !extendPrevious, waitForClick, fadeWhenDone, stopVoiceover, waitForVO, voiceOverClip, hasRead, delegate {
                
                //已經確定讀完這句話了
                AdvManager.Instance.OnSayRead?.Invoke(gameObject.name, csvCommandKey);
                hasRead = true;

                Continue();
            });
        }
        public override string GetSummary()
        {
            string namePrefix = "";
            if (!string.IsNullOrEmpty(overrideTerm))
            {
                namePrefix = overrideTerm + " ";
            }
            if (extendPrevious)
            {
                namePrefix = "EXTEND" + ": ";
            }
            return namePrefix + "\"" + storyText + "\"";
        }

        void Reset(){
            #if UNITY_EDITOR
            overrideNameColor = AdvEditorConfig.Instance ? AdvEditorConfig.Instance.EditorSaySpeakerNameColor : Color.white;
            #endif
        }

        public static Dictionary<string, Sprite> spriteHistoryEditor = new Dictionary<string, Sprite>();
        public void InitializeByParams(object[] param)
        {
            CommandParam data = param[0] as CommandParam;
            bool isContentSet = true;
            bool isTermSet = true;
            bool isSpriteSet = true;
            bool isVoiceSet = true;
            if(data.option != null){
                isContentSet = data.option.sayText;
                isTermSet = data.option.sayTerm;
                isSpriteSet = data.option.saySprite;
                isVoiceSet = data.option.sayVoice;
            }

            if(isTermSet){  //if(data.iconDisplay.StartsWith("Hide"))
                this.overrideTerm = data.name;
                if(string.IsNullOrEmpty(data.name))
                    showIcon = false;
            }
            if(isContentSet){
                if(data.locText.Count > 0)
                    this.storyText = data.locText[0].content;

                csvCommandKey = data.keys;
            }

            if(isVoiceSet){
                AdvKeyContent ADVKeys = AdvKeyContent.GetCurrentInstance();
                if(ADVKeys != null){
                    if(!string.IsNullOrEmpty(data.arg2)){
                        voiceOverClip = ADVKeys.GetVoiceByKey(data.arg2);
                        if(voiceOverClip == null){
                            Debug.Log("找不到Voice檔:" + data.arg2 + " , 於 行數 " + (this.itemId - 3));
                        }
                    }
                }
            }

            if(isSpriteSet){
                AdvKeyContent ADVKeys = AdvKeyContent.GetCurrentInstance();
                if(ADVKeys != null){
                    if(!string.IsNullOrEmpty(data.image)){

                        string[] tempSplit = data.image.Split('&');
                        data.image = tempSplit[0];
                        
                        portraitEquip.Clear();
                        if(tempSplit.Length > 1){
                            for (int i = 1; i < tempSplit.Length; i++)
                            {
                                portraitEquip.Add(tempSplit[i]);
                            }
                        }

                        //從AdvKey 取得 Sprite 資源
                        portrait = ADVKeys.GetAvatarByKey(data.image);
                        //如果有填Key 但又找不到的話
                        if(portrait == null && !string.IsNullOrEmpty(data.image)){
                            Debug.Log("找不到Avatar檔:" + data.image + " , 於 行數 " + (this.itemId - 3));
                            #if UNITY_EDITOR
                            portrait = AdvEditorConfig.Instance.DefaultSprite;
                            #endif

                        } else {
                            //順利找到圖的話, 記錄這張圖進字典
                            spriteHistoryEditor[data.name] = portrait;
                        }
                    }
                    //沒填 Portrait Name，可能是沿用圖檔, 需確認是否有填姓名
                    else {
                        if(!string.IsNullOrEmpty(data.name)){
                            if(spriteHistoryEditor.ContainsKey(data.name)){
                                portrait = spriteHistoryEditor[data.name];
                            }
                        }
                    }
                }
            }



            /* Reading Text File
            this.storyText  = param[0].ToString();
            if(param[1].ToString() != ""){
                Character _character = GameObject.Find(param[1].ToString()).GetComponent<Character>();
                if(_character != null){
                    character = _character;
                }
            }
            if(param[2].ToString() != ""){
                if(character != null){
                    portrait = character.GetPortrait(param[2].ToString());
                }
            }
            */

            //舊版本, 需設置Character來驅動 Say
            /*
            Character targetCharacter = null;
            //this area is for Simple Talk System , it can pass data.content
            if(data.content != null){
                //Find Character by Content 
                foreach (var item in Character.ActiveCharacters)
                {
                    if(item.charactorContent == data.content){
                        targetCharacter = item;
                    }
                }
                //Otherwise, Create Character GameObject
                if(targetCharacter == null){
                    targetCharacter = new GameObject(data.content.nameText).AddComponent<Character>();
                    targetCharacter.charactorContent = data.content;
                    targetCharacter.SetDataByContent();
                }
            } else if(data.characterName != ""){
                //Find Character by name
                foreach (var item in Character.ActiveCharacters)
                {
                    if(item.charactorContent != null){
                        if(item.charactorContent.nameText == data.characterName){
                            targetCharacter = item;
                        }
                    } else if(item.NameText == data.characterName){
                        targetCharacter = item;
                    }
                }
                //Otherwise, Create Character GameObject
                if(targetCharacter == null){
                    targetCharacter = new GameObject(data.characterName).AddComponent<Character>();
                    targetCharacter.SetStandardText(data.characterName);
                    Debug.LogWarning("Character Data is Created by default, need to Assign Character Content");
                }
            }
            //Assign Character Data
            character = targetCharacter;

            //驅動 Portrait 內容
            if(data.portrait != null){
                portrait = data.portrait;
            } else if(data.portraitName != "" && character != null) {
                portrait = character.GetPortrait(data.portraitName);
            }
            */
        }
    }
}