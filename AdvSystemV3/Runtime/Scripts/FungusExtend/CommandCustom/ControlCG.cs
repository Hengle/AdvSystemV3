using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using SpriteDicing;

namespace Fungus
{
    [CommandInfo("Background", 
                 "Control CG", 
                 "控制 CG 可切分背景相關")]
    [AddComponentMenu("")]
    public class ControlCG : Command , ICommand
    {
        [SerializeField] protected int csvLine;
        [SerializeField] protected string csvCommandKey;
        public int CSVLine { get { return csvLine; } set { csvLine = value; }}
        public string CSVCommandKey { get { return csvCommandKey; } set { csvCommandKey = value; }}


        [SerializeField] protected DicedSpriteAtlas atlasCG;
        [SerializeField] protected DicedSprite spriteCG;

        [Tooltip("顏色, 如果是黑幕可以設置為全黑")]
        [SerializeField] protected Color sprtieColor = new Color(1, 1, 1, 1);

        [Tooltip("Display type")]
        [SerializeField] protected BackgroundDisplayType display = BackgroundDisplayType.Show;

        [Tooltip("Fading Time")]
        [SerializeField] protected float duration = 0.5f;

        [Tooltip("Sprite Ease Type")]
        [SerializeField] protected Ease FadeEaseType = Ease.Linear;

        [Tooltip("Go to Next Command when Fade Finished")]
        [SerializeField] protected bool waitUntilFinished = true;

        public DicedSpriteAtlas AtlasCG {get {return atlasCG; } set {atlasCG = value; }}
        public DicedSprite SpriteCG {get {return spriteCG; } set {spriteCG = value; }}
        public BackgroundDisplayType Display {get {return display; }}

        public override void OnEnter()
        {
            var cgFront = AdvManager.Instance.advStage.BackgoundLayout.DS_CG_Front;
            var cgBehide = AdvManager.Instance.advStage.BackgoundLayout.DS_CG_Behide;


            if(display == BackgroundDisplayType.Show){
                // Fade in the new sprite image
                if(cgFront.DicedSprite != null)
                    cgBehide.SetDicedSprite(cgFront.DicedSprite);
                cgBehide.Color = cgFront.Color;

                cgFront.Color = new Color(sprtieColor.r, sprtieColor.g, sprtieColor.b, 0f);
                DicedSprite targetDS = AdvVariantManager.Instance.GetDiceSprite($"{spriteCG.AtlasAsset.name}.{spriteCG.name}");
                if(targetDS != null){
                    cgFront.SetDicedSprite(targetDS);
                    AdvManager.Instance.advStage.BackgoundLayout.OnReadCG?.Invoke(targetDS);
                } else {
                    Debug.LogError($">> Can't load CG key: {spriteCG.AtlasAsset.name}.{spriteCG.name}");
                }
                
                DOTween.To(() => cgFront.Color, x => cgFront.Color = x, sprtieColor, duration).SetEase(FadeEaseType).OnComplete(() => {
                    if(waitUntilFinished){
                        Continue();
                    }
                });

                //如果要蓋上去的背景具有半透明，則淡出上一張背景
                if(sprtieColor.a < 0.99f){
                    DOTween.To(() => cgBehide.Color, x => cgBehide.Color = x, new Color(0, 0, 0, 0), duration).SetEase(FadeEaseType);
                }

                if(!waitUntilFinished){
                    Continue();
                }
            }
            else if (display == BackgroundDisplayType.HideAll) 
            {
                cgBehide.Color = new Color(sprtieColor.r, sprtieColor.g, sprtieColor.b, 0f);
                DOTween.To(() => cgFront.Color, x => cgFront.Color = x, new Color(0, 0, 0, 0), duration).SetEase(FadeEaseType).OnComplete(() => {
                    if(waitUntilFinished){
                        Continue();
                    }
                });

                if(!waitUntilFinished){
                    Continue();
                }
            }
            else
            {
                Continue();
            }
        }

        public List<DicedSprite> GetCGInAtlas(){
            if(atlasCG == null)
                return new List<DicedSprite>();

            return atlasCG.GetDicedSpriteList();
        }

        public override string GetSummary()
        {
            string namePrefix = "\"";
            if (spriteCG != null) 
            {
                namePrefix += spriteCG.name + "\"";
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
            if(string.Equals(data.command, "CgOff", System.StringComparison.OrdinalIgnoreCase)){
                display = BackgroundDisplayType.HideAll;
                return;
            }
            if(bgKeys != null){
                spriteCG = bgKeys.GetDiceCGByKey(data.image);
                if(spriteCG == null){
                    if(!string.IsNullOrEmpty(data.image))
                        AdvUtility.LogWarning("找不到CG檔:" + data.image + " , 於 行數 " + (this.itemId - 3));
                } else {
                    atlasCG = spriteCG.AtlasAsset;
                }
            }
        }
    }
}