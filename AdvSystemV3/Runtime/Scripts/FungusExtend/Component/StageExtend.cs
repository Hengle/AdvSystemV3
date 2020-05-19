using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    public class StageExtend : Stage
    {
        [Header("多立繪時, 正在說話者popup 距離與時間")]
        public float DimYValue = 1.0f;
        public float DimYDuration = 0.15f;

        [Header("場景 : BC BG, CG, UI立繪")] public UIBackgroundLayout BackgoundLayout;
        [Header("DS立繪(Dicing Sprite), NS立繪(Normal Sprite)")] public UIBillboardLayout BillboardLayout;
        [Header("UI立繪")] public UIBillboardGUILayout BillboardGUILayout;
        [Header("前景/換場特效")] public UIForegroundLayout ForegroundLayout;

        public void NewStage()
        {
            BillboardGUILayout.ImmediateClearAllBillboard();
            //BillboardLayout.ImmediateClearAllBillboard();
            BackgoundLayout.BackgroundTexBehide.color = new Color(1f, 1f, 1f, 0f);
            BackgoundLayout.BackgroundTexFront.color = new Color(1f, 1f, 1f, 0f);
        }

        public void NewStageEditor()
        {
            BillboardGUILayout.ImmediateClearAllBillboard();
            //BillboardLayout.ImmediateClearAllBillboard();
        }

        public void CloseStage()
        {
            //BillboardLayout.FadeOutAllBillboard();
            BackgoundLayout.FadeOutAllBackground();
            BackgoundLayout.FadeOutAllCG();
        }

        #region Param Define

        public static readonly Dictionary<string, BillboardPosition> MapPosition = new Dictionary<string, BillboardPosition>(){
            { "M", BillboardPosition.Middle },
            { "L", BillboardPosition.Left },
            { "R", BillboardPosition.Right },
            { "OverLeft", BillboardPosition.OffscreenLeft },
            { "OverRight", BillboardPosition.OffscreenRight },
            { "LL", BillboardPosition.LeftLeft },
            { "RR", BillboardPosition.RightRight },
        };
        public static BillboardPosition GetBbPositionByString(string str)
        {
            BillboardPosition result;
            MapPosition.TryGetValue(str, out result);
            return result;
        }

        public static readonly Dictionary<string, BillboardDistance> MapDistance = new Dictionary<string, BillboardDistance>{
            { "Middle", BillboardDistance.Middle },
            { "Near", BillboardDistance.Near },
            { "Far", BillboardDistance.Far },
        };
        public static BillboardDistance GetBbDistanceByString(string str)
        {
            BillboardDistance result;
            MapDistance.TryGetValue(str, out result);
            return result;
        }

        public static readonly Dictionary<string, BillboardHideFlag> MapHideFlag = new Dictionary<string, BillboardHideFlag>{
            { "M", BillboardHideFlag.Middle },
            { "L", BillboardHideFlag.Left },
            { "R", BillboardHideFlag.Right },
            { "OverLeft", BillboardHideFlag.OffscreenLeft },
            { "OverRight", BillboardHideFlag.OffscreenRight },
            { "LL", BillboardHideFlag.LeftLeft },
            { "RR", BillboardHideFlag.RightRight },
            { "All", BillboardHideFlag.All },
        };
        public static BillboardHideFlag GetBbHideByString(string str)
        {
            BillboardHideFlag result;
            MapHideFlag.TryGetValue(str, out result);
            return result;
        }

        public static readonly Dictionary<string, DisplayType> MapDisplayType = new Dictionary<string, DisplayType>{
            { "Show", DisplayType.Show },
            { "Hide", DisplayType.Hide },
            { "Replace", DisplayType.Replace },
            { "MoveToFront", DisplayType.MoveToFront },
        };
        public static DisplayType GetDisplayTypeByString(string str)
        {
            DisplayType result;
            if(!MapDisplayType.TryGetValue(str, out result))
                result = DisplayType.None;

            return result;
        }

        public static readonly Dictionary<string, DisplayType> MapDisplayTypeByCommand = new Dictionary<string, DisplayType>{
            { "Billboard", DisplayType.Show },
            { "BillboardOff", DisplayType.Hide },
            { "BillboardPfb", DisplayType.Show },
            { "BillboardPfbOff", DisplayType.Hide },
        };
        public static DisplayType GetDisplayTypeByCommand(string str)
        {
            DisplayType result;
            if(!MapDisplayTypeByCommand.TryGetValue(str, out result))
                result = DisplayType.None;

            return DisplayType.None;
        }

        #endregion
    }
    
    public class BillboardOptions
    {
        public DisplayType display;
        public Sprite billboardSprite;
        public SpriteDicing.DicedSpriteAtlas billboardDiceAtlas;
        public SpriteDicing.DicedSprite billboardDiceSprite;
        public bool flipFace;
        public BillboardPosition fromPosition;
        public BillboardPosition toPosition;
        public ToPositionShift toPositionShift;
        public int shiftAmount;
        public BillboardDistance toDistance;
        public BillboardHideFlag hideWhich;
        public RectTransform fromRect;
        public RectTransform toRect;
        public bool useDefaultSettings;
        public float fadeDuration;
        public float moveDuration;
        public bool move;
        public bool shiftIntoPlace;
        public Vector2 shiftOffset;
        public bool waitUntilFinished;
        public System.Action onComplete;
    }

    public enum BillboardPosition
    {
        Middle,
        OffscreenLeft,
        LeftLeft,
        Left,
        Right,
        RightRight,
        OffscreenRight,
    }

    public enum BillboardHideFlag
    {
        Middle,
        OffscreenLeft,
        LeftLeft,
        Left,
        Right,
        RightRight,
        OffscreenRight,
        All,
    }

    public enum BillboardDistance
    {
        Middle,
        Near,
        Far,
    }

    public enum ToPositionShift
    {
        None,
        Up,
        Down,
        Left,
        Right,
    }

}