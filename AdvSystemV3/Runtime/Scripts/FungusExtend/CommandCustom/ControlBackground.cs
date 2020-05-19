using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Fungus
{
    /// <summary>
    /// Supported display operations for Background.
    /// </summary>
    public enum BackgroundDisplayType
    {
        /// <summary> No operation </summary>
        None,
        /// <summary> Show the Background. </summary>
        Show,
        /// <summary> Hide the Background. </summary>
        HideAll,
    }

    [CommandInfo("Background", 
                 "Control BG", 
                 "控制背景相關")]
    [AddComponentMenu("")]
    public class ControlBackground : Command , ICommand
    {
        [SerializeField] protected int csvLine;
        [SerializeField] protected string csvCommandKey;
        public int CSVLine { get { return csvLine; } set { csvLine = value; }}
        public string CSVCommandKey { get { return csvCommandKey; } set { csvCommandKey = value; }}


        [Tooltip("Background Sprite")]
        [SerializeField] protected Sprite spriteBackground;

        [Tooltip("顏色, 如果是黑幕可以設置為全黑")]
        [SerializeField] protected Color sprtieColor = new Color(1, 1, 1, 1);

        [Tooltip("Display type")]
        [SerializeField] protected BackgroundDisplayType display = BackgroundDisplayType.Show;

        [Tooltip("Fading Time")]
        [SerializeField] protected float duration = 0.5f;

        [Tooltip("Sprite Ease Type")]
        [SerializeField] protected LeanTweenType FadeEaseType = LeanTweenType.linear;

        [Tooltip("Go to Next Command when Fade Finished")]
        [SerializeField] protected bool waitUntilFinished = true;


        private Image targetBackgroundFront;
        private Image targetBackgroundBehide;

        public void DirectSetBackground(){
            targetBackgroundFront = AdvManager.Instance.advStage.BackgoundLayout.BackgroundTexFront;
            targetBackgroundBehide = AdvManager.Instance.advStage.BackgoundLayout.BackgroundTexBehide;

            targetBackgroundBehide.sprite = targetBackgroundFront.sprite;
            targetBackgroundBehide.color = targetBackgroundFront.color;

            targetBackgroundFront.color = new Color(sprtieColor.r, sprtieColor.g, sprtieColor.b, 1f);
            targetBackgroundFront.sprite = spriteBackground;
        }

        public override void OnEnter()
        {
            targetBackgroundFront = AdvManager.Instance.advStage.BackgoundLayout.BackgroundTexFront;
            targetBackgroundBehide = AdvManager.Instance.advStage.BackgoundLayout.BackgroundTexBehide;


            if(display == BackgroundDisplayType.Show){
                // Fade in the new sprite image
                targetBackgroundBehide.sprite = targetBackgroundFront.sprite;
                targetBackgroundBehide.color = targetBackgroundFront.color;

                targetBackgroundFront.color = new Color(sprtieColor.r, sprtieColor.g, sprtieColor.b, 0f);
                targetBackgroundFront.sprite = spriteBackground;
                LeanTween.alpha(targetBackgroundFront.rectTransform, sprtieColor.a, duration).setEase(FadeEaseType).setRecursive(false).setOnComplete(() => {
                        if(waitUntilFinished){
                            Continue();
                        }
                    });
                //如果要蓋上去的背景具有半透明，則淡出上一張背景
                if(sprtieColor.a < 0.99f){
                    LeanTween.alpha(targetBackgroundBehide.rectTransform, 0f, duration).setEase(FadeEaseType).setRecursive(false);
                }

                if(!waitUntilFinished){
                    Continue();
                }
            } else if (display == BackgroundDisplayType.HideAll) {
                targetBackgroundBehide.color = new Color(sprtieColor.r, sprtieColor.g, sprtieColor.b, 0f);
                LeanTween.alpha(targetBackgroundFront.rectTransform, 0f, duration).setEase(FadeEaseType).setRecursive(false).setOnComplete(() => {
                        if(waitUntilFinished){
                            Continue();
                        }
                    });
                if(!waitUntilFinished){
                    Continue();
                }
            } else {
                Continue();
            }
        }

        public override string GetSummary()
        {
            string namePrefix = "\"";
            if (spriteBackground != null) 
            {
                namePrefix += spriteBackground.name + "\"";
            }
            if(display == BackgroundDisplayType.HideAll)
            {
                namePrefix += "Hide All" + "\"";
            }
            return namePrefix;
        }

        public override Color GetButtonColor()
        {
            return new Color32(221, 184, 169, 255);
        }

        public void InitializeByParams(object[] param){
            CommandParam data = param[0] as CommandParam;
            
            AdvKeyContent bgKeys = AdvKeyContent.GetCurrentInstance();
            if(bgKeys != null){
                spriteBackground = bgKeys.GetBackgroundByKey(data.image);
                if(spriteBackground == null && !string.IsNullOrEmpty(data.image)){
                    AdvUtility.LogWarning("找不到BG檔:" + data.image + " , 於 行數 " + (this.itemId - 3));
                }
            }
            if(data.command == "BgOff"){
                display = BackgroundDisplayType.HideAll;
            }
        }
    }
}
