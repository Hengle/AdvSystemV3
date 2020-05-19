using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


namespace Fungus
{
    [CommandInfo("Background", 
                 "Base Color", 
                 "控制背景相關")]
    [AddComponentMenu("")]
    public class ControlBackgroundBase : Command
    {
        [Tooltip("顏色, 如果是黑幕可以設置為全黑")]
        [SerializeField] protected Color baseColor = new Color(1, 1, 1, 1);

        public override void OnEnter()
        {
            if(AdvManager.Instance.advStage.BackgoundLayout != null)
                AdvManager.Instance.advStage.BackgoundLayout.BackgroundColor.color = baseColor;

            Continue();
        }

        public override string GetSummary()
        {
            string namePrefix = "\"";
            namePrefix += baseColor.ToString() + "\"";
            return namePrefix;
        }

        public override Color GetButtonColor()
        {
            return new Color32(100, 184, 200, 255);
        }
    }
}
