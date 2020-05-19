using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteDicing;

namespace Fungus
{
    [CommandInfo("立繪", 
                 "BillBoard", 
                 "控制立繪相關")]
    [AddComponentMenu("")]
    public class BillBoard : Command , ICommand
    {
        [SerializeField] protected int csvLine;
        [SerializeField] protected string csvCommandKey;
        public int CSVLine { get { return csvLine; } set { csvLine = value; }}
        public string CSVCommandKey { get { return csvCommandKey; } set { csvCommandKey = value; }}


        [Tooltip("Display type")]
        [SerializeField] protected DisplayType display;

        [Tooltip("Billboar to show")]
        [SerializeField] protected Sprite spriteBillboard;

        [SerializeField] protected DicedSpriteAtlas spriteAtlas;

        [Tooltip("主要角色使用 Dicing")]
        [SerializeField] protected DicedSprite spriteDynamicBillboard;
        
        [Tooltip("Flip billboard")]
        [SerializeField] protected bool flipFace;

        [Tooltip("Move portrait into new position")]
        [SerializeField] protected bool move;

        [Tooltip("Move the portrait from this position")]
        [SerializeField] protected BillboardPosition fromPosition;

        [Tooltip("Move the portrait to this position")]
        [SerializeField] protected BillboardPosition toPosition;

        [Tooltip("位置至目標的座標偏移")]
        [SerializeField] protected ToPositionShift positionShift;

        [Tooltip("位置至目標的座標偏移數量")]
        [SerializeField] protected int positionShiftAmount;

        [Tooltip("Distance between camera (equal Portrait size)")]
        [SerializeField] protected BillboardDistance spriteDistance;

        [SerializeField] protected BillboardHideFlag hideWhich;

        [Tooltip("Use Default Settings (Fade & Move duration , Offset)")]
        [SerializeField] protected bool useDefaultSettings = true;

        [Tooltip("Start from offset position")]
        [SerializeField] protected bool shiftIntoPlace;
        [Tooltip("Shift Offset")]
        [SerializeField] protected Vector2 shiftOffset;

        [Tooltip("Fade Duration")]
        [SerializeField] protected float fadeDuration = 0.5f;

        [Tooltip("Movement Duration")]
        [SerializeField] protected float moveDuration = 0.5f;

        [Tooltip("Wait until the tween has finished before executing the next command")]
        [SerializeField] protected bool waitUntilFinished = true;

        public virtual DisplayType _Display { get { return display; } set { display = value; } }
        public virtual Sprite _SpriteBillboard { get { return spriteBillboard; } set { spriteBillboard = value; } }
        public virtual bool _FlipFace { get { return flipFace; } set { flipFace = value; } }
        public virtual bool _Move { get { return move; } set { move = value; } }
        public virtual BillboardPosition _FromPosition { get { return fromPosition; } set { fromPosition = value; } }
        public virtual BillboardPosition _ToPosition { get { return toPosition; } set { toPosition = value; } }
        public virtual BillboardHideFlag _HideWhich { get { return hideWhich; } set { hideWhich = value; } }
        public virtual bool _UseDefaultSettings { get { return useDefaultSettings; } set { useDefaultSettings = value; } }
        public virtual bool _ShiftIntoPlace { get { return shiftIntoPlace; } set { shiftIntoPlace = value; } }
        public virtual Vector2 _ShiftOffset { get { return shiftOffset; } set { shiftOffset = value; } }
        public virtual float _FadeDuration { get { return fadeDuration; } set { fadeDuration = value; } }
        public virtual float _MoveDuration { get { return moveDuration; } set { moveDuration = value; } }
        public virtual bool _WaitUntilFinished { get { return waitUntilFinished; } set { waitUntilFinished = value; } }

        private StageExtend targetStage = null;
        public override void OnEnter()
        {
            BillboardOptions options = new BillboardOptions();
            
            options.billboardSprite = spriteBillboard;
            options.billboardDiceAtlas = spriteAtlas;
            if(spriteDynamicBillboard == null && spriteAtlas != null){
                spriteDynamicBillboard = spriteAtlas.GetSpriteContainName("Normal");
            }
            options.billboardDiceSprite = spriteDynamicBillboard;
            options.display = display;
            options.fromPosition = fromPosition;
            options.toPosition = toPosition;
            options.toPositionShift = positionShift;
            options.shiftAmount = positionShiftAmount;
            options.toDistance = spriteDistance;
            options.hideWhich = hideWhich;
            options.flipFace = flipFace;
            options.useDefaultSettings = useDefaultSettings;
            options.fadeDuration = fadeDuration;
            options.moveDuration = moveDuration;
            options.shiftOffset = shiftOffset;
            options.move = move;
            options.shiftIntoPlace = shiftIntoPlace;
            options.waitUntilFinished = waitUntilFinished;

            targetStage = AdvManager.Instance.advStage;
            targetStage.BillboardLayout.RunBillboardCommand(options, Continue); // 已在 callback 加上 Continue, 因此此指令執行完時直接 Continue
            //targetStage.RunBillboardCommand(options, Continue);
        }

        public override string GetSummary()
        {
            string namePrefix = "\"";
            if(display == DisplayType.Hide)
            {
                namePrefix += "Hide " + hideWhich.ToString() + "\"";
            }
            else
            {
                namePrefix += display.ToString() + " " + toPosition.ToString() + " " + spriteDistance.ToString() + " ";
                if (spriteBillboard != null) 
                {
                    namePrefix += spriteBillboard.name + "\"";
                }
                else if(spriteDynamicBillboard != null)
                {
                    namePrefix += spriteDynamicBillboard.name + "\"";
                }
                else if(spriteAtlas != null)
                {
                    namePrefix += spriteAtlas.name + "\"";
                }
                else
                {
                    namePrefix += "NULL \"";
                }
            }
            
            return namePrefix;
        }

        public override Color GetButtonColor()
        {
            return new Color32(230, 200, 250, 255);
        }

        public void InitializeByParams(object[] param)
        {
            CommandParam data = param[0] as CommandParam;

            Sprite _sprite = null;
            DicedSpriteAtlas _atlas;
            DicedSprite _diceSprite = null;

            AdvKeyContent ADVKeys = AdvKeyContent.GetCurrentInstance();
            if(ADVKeys != null){

                //尋找立繪圖區(DiceAtlas)
                _atlas = ADVKeys.GetDiceAtlasByKey(data.image);
                if(_atlas != null){
                    spriteAtlas = _atlas;
                    _diceSprite = ADVKeys.GetDiceBillboardByKeyContain("Normal", _atlas);

                    if(_diceSprite == null){
                        AdvUtility.LogWarning("找不到Billboard檔:" + data.image + " , 於 行數 " + (this.itemId - 3));
                        if (Application.isPlaying)
                        {
                            //_diceSprite = FungusExtendEditorConfig.Instance.DefaultDiceSprite;
                        }
                    }

                } else if (!string.IsNullOrEmpty(data.image)){
                    
                    //可能是使用怪物圖
                    _sprite = ADVKeys.GetEnemyByKey(data.image);

                    if(_sprite == null){
                        AdvUtility.LogWarning("找不到Billboard檔:" + data.image + " , 於 行數 " + (this.itemId - 3));
                        if (Application.isPlaying)
                        {
                            //_diceSprite = AdvManager.Instance.DefaultDiceSprite;
                        }
                    }

                }

                /*
                _diceSprite = ADVKeys.GetDiceBillboardByKey(data.billboardKey);  // ex: Lica_Normal
                if(_diceSprite == null && !string.IsNullOrEmpty(data.billboardKey)) {
                    AdvUtility.LogWarning("找不到Billboard檔:" + data.billboardKey + " , 於 行數 " + (this.itemId - 3));
                    _diceSprite = AdvManager.Instance.DefaultDiceSprite;
                }
                */

                /* // 舊版 UI 型 立繪
                //尋找立繪圖區
                _sprite = ADVKeys.GetBillboardByKey(data.billboardKey);
                if(_sprite == null){
                //可能使用怪物圖區，尋找怪物圖區
                    _sprite = ADVKeys.GetEnemyByKey(data.billboardKey);
                }
                //兩者皆沒有，替換為香菇
                if(_sprite == null && !string.IsNullOrEmpty(data.billboardKey)){

                    AdvUtility.LogWarning("找不到Billboard檔:" + data.billboardKey + " , 於 行數 " + (this.itemId - 3));
                    _sprite = AdvManager.Instance.DefaultSprite;
                }
                */
            }

            spriteBillboard = _sprite;
            spriteDynamicBillboard = _diceSprite;
            display = StageExtend.GetDisplayTypeByCommand(data.command);
            spriteDistance = StageExtend.GetBbDistanceByString(data.arg1);
            toPosition = StageExtend.GetBbPositionByString(data.target);
            hideWhich = StageExtend.GetBbHideByString(data.target);
            //flipFace = flipFace;
            useDefaultSettings = true;
            //fadeDuration = fadeDuration;
            //moveDuration = moveDuration;
            //shiftOffset = shiftOffset;
            //move = move;
            //shiftIntoPlace = shiftIntoPlace;
            //waitUntilFinished = waitUntilFinished;
        }
    }
}