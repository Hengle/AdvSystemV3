using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("立繪", 
                 "BillBoard Prefab", 
                 "控制Prefab立繪相關")]
    [AddComponentMenu("")]
    public class BillboardPrefab : Command , ICommand
    {
        [SerializeField] protected int csvLine;
        [SerializeField] protected string csvCommandKey;
        public int CSVLine { get { return csvLine; } set { csvLine = value; }}
        public string CSVCommandKey { get { return csvCommandKey; } set { csvCommandKey = value; }}


        [Tooltip("Display type")]
        [SerializeField] protected DisplayType display = DisplayType.Show;

        [Tooltip("Billboard to show"), SerializeField] protected UIBillboardController targetPrefab;
        [Tooltip("Sprite of Emoji"), SerializeField] protected string useEmoji;
        [Tooltip("Sprite of Body"), SerializeField] protected string useBody;
        [Tooltip("Sprite of Body"), SerializeField] protected List<string> useEquips;
        
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
        public virtual UIBillboardController _TargetPrefab { get { return targetPrefab; } set { targetPrefab = value; } }
        public virtual List<string> _UseEquips { get { return useEquips; } set { useEquips = value; } }
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

        public override void OnEnter()
        {
            System.Action<GameObject> onSpawn = (obj) => {
                UIBillboardController copyController = obj.GetComponent<UIBillboardController>();
                if(copyController != null)
                    copyController.SetInitDisplay(useEmoji, useBody, useEquips);
            };

            BillboardOptions options = new BillboardOptions();
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

            StageExtend targetStage = AdvManager.Instance.advStage;
            RectTransform prefabRect = null;

            if(targetPrefab != null)
                prefabRect = targetPrefab.GetComponent<RectTransform>();

            targetStage.BillboardGUILayout.RunBillboardGUIObjCommand(prefabRect, options, onSpawn, Continue); // 已在 callback 加上 Continue, 因此此指令執行完時直接 Continue
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
                if (targetPrefab != null) 
                {
                    namePrefix += targetPrefab.name + "\"";
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

            UIBillboardController _prefab = null;

            AdvKeyContent ADVKeys = AdvKeyContent.GetCurrentInstance();
            if(ADVKeys != null){
                
                _prefab = ADVKeys.GetBillboardPrefabByKey(data.image);
                if(_prefab != null){

                    if(!string.IsNullOrEmpty(data.name))
                        useBody = data.name;

                    if(!string.IsNullOrEmpty(data.arg2)){
                        string[] splite = data.arg2.Split('|');
                        useEquips = new List<string>();
                        foreach (var item in splite)
                        {
                            useEquips.Add(item);
                        }
                    }
                } else if (!string.IsNullOrEmpty(data.image)){
                    AdvUtility.LogWarning("找不到Billboard prefab檔:" + data.image + " , 於 行數 " + (this.itemId - 3));
                    if (Application.isPlaying)
                    {
                        //_diceSprite = AdvManager.Instance.DefaultDiceSprite;
                    }
                }

            }

            targetPrefab = _prefab;
            display = StageExtend.GetDisplayTypeByCommand(data.command);
            spriteDistance = StageExtend.GetBbDistanceByString(data.arg1);
            toPosition = StageExtend.GetBbPositionByString(data.target);
            hideWhich = StageExtend.GetBbHideByString(data.target);
            useDefaultSettings = true;
        }

    }
}