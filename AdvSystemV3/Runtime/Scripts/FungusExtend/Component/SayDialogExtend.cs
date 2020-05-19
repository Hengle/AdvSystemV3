using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FungusExt;
using UnityEngine.UI;

namespace Fungus
{
    public class SayDialogExtend : SayDialog
    {
        [Header("Extend Variable")]
        public GameObject SayDialogObject;
        public GameObject SayIconObject;

        //Save Sprite for Text trigger Say
        protected Dictionary<string,Sprite> SpriteCache = new Dictionary<string, Sprite>();
        public bool ThisSayHasRead { get; set; }
        [HideInInspector] public bool IconDisplay = true;

        public readonly Dictionary<string, Color> ColorName = new Dictionary<string, Color>();
        public readonly Dictionary<string, Color> ColorText = new Dictionary<string, Color>();

        protected override void Awake(){
            base.Awake();
        }

        protected override void Start(){
            // Dialog always starts invisible, will be faded in when writing starts
            GetCanvasGroup().alpha = 0f;

            // Add a raycaster if none already exists so we can handle dialog input
            GraphicRaycaster raycaster = GetComponentInParent<GraphicRaycaster>();
            if (raycaster == null)
            {
                gameObject.AddComponent<GraphicRaycaster>();    
            }

            // It's possible that SetCharacterImage() has already been called from the
            // Start method of another component, so check that no image has been set yet.
            // Same for nameText.

            if (NameText == "")
            {
                SetCharacterName("", Color.white);
            }
            if (currentCharacterImage == null)
            {                
                // Character image is hidden by default.
                SetCharacterImage(null);
            }
            

            foreach (var item in AdvProjectConfig.Instance.nameAndColor)
            {
                ColorName.Add(item.useName, item.useColor);
                ColorText.Add(item.useName, item.useColorStory);
            }

            IconDisplay = true;
        }


        private string _onScreenNameTerm;
        private string _onScreenTextTerm;
        private Sprite _onScreenSpeakerSprite;
        private AudioClip _onScreenSpeakerVoice;
        
        
        //The on screen variable will be used to save log content
        public string OnScreenNameTerm => _onScreenNameTerm;
        public string OnScreenTextTerm => _onScreenTextTerm;
        public Sprite OnScreenSpeakerSprite => _onScreenSpeakerSprite;
        public AudioClip OnScreenSpeakerVoice => _onScreenSpeakerVoice;

        
        /// <summary>
        /// 紀錄該句Say對話內容
        /// </summary>
        public void SetLogInfo(string nameTerm, string textTerm, AudioClip voice){
            _onScreenSpeakerVoice = voice;
            _onScreenNameTerm = nameTerm;
            _onScreenTextTerm = textTerm;
        }

        /// <summary>
        /// 更新已顯示在畫面上的文字, 切換語言時呼叫, 如果正在顯示文字中, 立即中斷顯示, 替換上目標語言
        /// </summary>
        public void UpdateCurrentLangContent(SystemLanguage targetLanguage){
            AdvManager.Instance.advWriter.StopDoWord();

            if(string.IsNullOrEmpty(_onScreenNameTerm))
                NameText = "";
            else {
                string _name = LocalizeManager.GetLocalizeName(_onScreenNameTerm);
                NameText = string.IsNullOrEmpty(_name)? $"no key:{_onScreenNameTerm}" : _name;
            }

            //When initialize, the "_onScreenTextTerm" may be null
            if(string.IsNullOrEmpty(_onScreenTextTerm))
                StoryText = "";
            else {
                string _text = LocalizeManager.GetLocalizeText(_onScreenTextTerm);
                StoryText = string.IsNullOrEmpty(_text)? $"no key:{_onScreenTextTerm}" : _text;
            }
        }

        public virtual void Say(string text, bool clearPrevious, bool waitForInput, bool fadeWhenDone, bool stopVoiceover, bool waitForVO, AudioClip voiceOverClip, bool hasRead, Action onComplete)
        {
            ThisSayHasRead = hasRead;

            StartCoroutine(DoSay(text, clearPrevious, waitForInput, fadeWhenDone, stopVoiceover, waitForVO, voiceOverClip, onComplete));
        }

        public virtual void SetCharacter(string speakerName, Color speakerColor, Sprite image)
        {
            if (string.IsNullOrEmpty(speakerName))
            {
                if (NameText != null)
                {
                    NameText = "";
                }
            }
            else
            {
                SetCharacterName(speakerName, speakerColor);

                //Set Sprite Cache , let null sprite can use cache sprite by name
                if(image != null){
                    SpriteCache[speakerName] = image;
                } else {

                    if(SpriteCache.ContainsKey(speakerName)){
                        image = SpriteCache[speakerName];
                    }
                }
            }

            _onScreenSpeakerSprite = image;
            SetCharacterImage(image);
        }


        public void SetStoryColor(Color storyColor){
            storyTextAdapter.SetTextColor(storyColor);
        }

        public void SetIconDisplay(bool display){
            SayIconObject.SetActive(true);

            if(display == false || !IconDisplay){
                SayIconObject.SetActive(false);
            }
        }
    }
}