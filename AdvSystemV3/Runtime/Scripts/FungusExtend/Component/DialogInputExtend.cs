using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Fungus
{
    public class DialogInputExtend : DialogInput
    {
        [Header("Extend Variable")]
        [SerializeField] protected bool isAutoSkip = false;
        [SerializeField] protected bool isLockInput = false;

        public bool SwitchAutoSkip() => isAutoSkip = !isAutoSkip;
        public bool IsAutoSkip
        {
            get { return isAutoSkip; }
            set
            {
                isAutoSkip = value;
                if (isAutoSkip == false)
                    AdvSignals.DoAdvStopAutoSkip();
            }
        }
        public bool IsLockInput { get { return isLockInput; } set { isLockInput = value; } }
        protected WriterExtend writerExtend;

        protected override void Awake()
        {
            base.Awake();
            writerExtend = writer as WriterExtend;
        }

        protected override void Update()
        {
            if (isLockInput)
                return;

            if (EventSystem.current == null)
            {
                return;
            }

            if (currentStandaloneInputModule == null)
            {
                currentStandaloneInputModule = EventSystem.current.GetComponent<StandaloneInputModule>();
            }

            if (writerExtend != null && writerExtend.IsWriting)
            {
                # region Extend Method

                if (Input.GetButtonDown(currentStandaloneInputModule.submitButton))
                    SetNextLineFlag();

                if (cancelEnabled)
                {
                    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                    {
                        if (AdvUserSettingManager.Instance.DialogUseCtrlToSkip == AdvDialogUseCtrlToSkip.HaveRead)
                        {
                            if (AdvManager.Instance.advSayDialog.ThisSayHasRead) SetNextLineFlag();
                        }
                        else SetNextLineFlag();
                    }
                }

                if (isAutoSkip)
                {
                    if (AdvUserSettingManager.Instance.DialogSkipMode == AdvDialogSkipMode.HaveRead)
                    {
                        if (AdvManager.Instance.advSayDialog.ThisSayHasRead) SetNextLineFlag();
                    }
                    else SetNextLineFlag();
                }
                # endregion
            }

            switch (clickMode)
            {
                case ClickMode.Disabled:
                    break;
                case ClickMode.ClickAnywhere:
                    if (Input.GetMouseButtonDown(0))
                    {
                        SetNextLineFlag();
                    }
                    break;
                case ClickMode.ClickOnDialog:
                    if (dialogClickedFlag)
                    {
                        SetNextLineFlag();
                        dialogClickedFlag = false;
                    }
                    break;
            }

            if (ignoreClickTimer > 0f)
            {
                ignoreClickTimer = Mathf.Max(ignoreClickTimer - Time.deltaTime, 0f);
            }

            if (ignoreMenuClicks)
            {
                // Ignore input events if a Menu is being displayed
                if (MenuDialog.ActiveMenuDialog != null &&
                    MenuDialog.ActiveMenuDialog.IsActive() &&
                    MenuDialog.ActiveMenuDialog.DisplayedOptionsCount > 0)
                {
                    dialogClickedFlag = false;
                    nextLineInputFlag = false;
                }
            }

            // Tell any listeners to move to the next line
            if (nextLineInputFlag)
            {
                var inputListeners = gameObject.GetComponentsInChildren<IDialogInputListener>();
                for (int i = 0; i < inputListeners.Length; i++)
                {
                    var inputListener = inputListeners[i];
                    inputListener.OnNextLineEvent();
                }
                nextLineInputFlag = false;
            }
        }

        /// <summary>
        // SayDialog 面板按下事件, Fungus 系統中, 以 inspector 面板來連結呼叫, 因此 reference 為 0
        /// </summary>
        public override void SetDialogClickedFlag()
        {
            // Lock input when Dialog is hide
            if (isLockInput)
                return;

            if (AdvUserSettingManager.Instance.StillAutoWhenClick == AdvStillAutoWhenClick.No)
            {
                writerExtend.IsAutoWrite = false;
            }

            base.SetDialogClickedFlag();
        }

        /// <summary>
        // Continue 按鈕用, Fungus 系統中, 以 inspector 面板來連結呼叫, 因此 reference 為 0
        /// </summary>
        public override void SetButtonClickedFlag()
        {
            if (isLockInput)
                return;

            base.SetButtonClickedFlag();
        }

    }
}