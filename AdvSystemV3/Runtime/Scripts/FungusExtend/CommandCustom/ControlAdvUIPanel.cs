using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Adv 面板", 
                 "Display Adv Panel", 
                 "隱藏或顯示 Adv 面板")]
    [AddComponentMenu("")]
    public class ControlAdvUIPanel : Command
    {
        [SerializeField] public bool IsMenuShow;
        public override void OnEnter()
        {
            AdvManager.Instance.SetAdvUIVisible(IsMenuShow);

            Continue();
        }

        public override string GetSummary()
        {
            string result = "";
            if(IsMenuShow)
                result += "Show";
            else
                result += "Hide";

            return result;
        }

        public override Color GetButtonColor()
        {
            return new Color32(175, 225, 225, 255);
        }
    }

}