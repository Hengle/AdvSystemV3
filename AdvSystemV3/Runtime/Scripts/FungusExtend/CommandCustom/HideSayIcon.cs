using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Narrative", 
                 "Say Avatar display", 
                 "是否顯示大頭像")]
    [AddComponentMenu("")]
    public class HideSayIcon : Command
    {
        [SerializeField] protected bool IconDisplayBelow;

        public override void OnEnter()
        {
           AdvManager.Instance.advSayDialog.IconDisplay = IconDisplayBelow;
           Continue();
        }

        public override string GetSummary()
        {
            return IconDisplayBelow ? "Set Show Avatar" : "Set Hide Avatar";
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 100, 255);
        }
    }
}