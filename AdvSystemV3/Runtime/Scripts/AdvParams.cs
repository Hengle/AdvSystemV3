using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    public delegate void MenuSelectCallback(List<int> UserSelectedOption);
    public delegate void HasReadCallback(string csvKey, int commandId);

    #region RuntimeTarget
    public enum AdvTargetObject
    {
        SelectedGameObject,
        SayIcon,
        Dialog,
        BillboardLeftLeft,
        BillboardLeft,
        BillboardMiddle,
        BillboardRight,
        BillboardRightRight,
        Background,
        CG,
    }

    #endregion

    #region AdvUpdate

    public class SearchBlockHandler
    {
        public System.Action<Block> createBlockEvent;
    }

    public class CommandParam
    {
        public string keys;
        public string command;
        public string target;
        public string arg1;
        public string arg2;
        public string image;
        public string name;
        public List<FungusExt.LocalizeText> locText;
        public AdvUpdateOption option;
    }

    public class AdvUpdateOption
    {
        public bool sayText;
        public bool selectionText;
        public bool sayTerm;
        public bool saySprite;
        public bool sayVoice;
        public bool blockName;
        public bool background;
        public bool CG;
        public bool billboard;
        public bool BGM;
        public bool timeline;
        public bool wait;
        public bool punch;
        public bool selection;
        public bool jump;
        public bool entrance;

        public AdvUpdateOption()
        {
            sayText = true;
            selectionText = true;

            sayTerm = false;
            saySprite = false;
            blockName = false;
            background = false;
            billboard = false;
            BGM = false;
            timeline = false;
            wait = false;
            punch = false;
            selection = false;
            jump = false;
            entrance = false;
        }
    }

    #endregion
}
