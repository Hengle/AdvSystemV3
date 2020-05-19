using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using FungusExt;

namespace Fungus
{
    public class MenuDialogExtend : MenuDialog
    {
        [Header("Extend Variable")]
        protected int OptionResultIndex = 0;
        protected int localizeIndex = 0;
        protected string[] _onScreenTextTerms;
        protected FlowchartExtend _onScreenFlowchart;
        protected CanvasGroup canvasGroup;
        public System.Func<int, bool> OnRequireValue;

        public override void SetActive(bool state)
        {
            gameObject.SetActive(state);

            //TODO: some Fade Effect
            if (canvasGroup == null)
                return;

            if (state)
            {
                canvasGroup.alpha = 1;
                canvasGroup.blocksRaycasts = true;
            }
            else
            {
                canvasGroup.alpha = 0;
                canvasGroup.blocksRaycasts = false;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            canvasGroup = GetComponent<CanvasGroup>();
            _onScreenTextTerms = new string[CachedButtons.Length];
        }

        /// <summary>
        /// Clear 將會重置 Buttons 的顯示, 通常在選完一個選項後呼叫, 擴充了重置 選擇結果記錄
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            OptionResultIndex = 0;
            localizeIndex = 0;
        }

        /// <summary>
        /// 增加對話項目, 擴充了 "條件選擇項目"
        /// </summary>
        public virtual bool AddOption(string displayTerm, bool interactable, bool hideOption, Block targetBlock, FlowchartExtend srcFlowChart, bool hasCondition, int requireValue)
        {
            if (localizeIndex < _onScreenTextTerms.Length)
            {
                _onScreenTextTerms[localizeIndex] = displayTerm;
                localizeIndex++;
            }
            _onScreenFlowchart = srcFlowChart;

            string localText = LocalizeManager.GetLocalizeText(displayTerm);
            string displayText = string.IsNullOrEmpty(localText)? $"no key:{displayTerm}" : srcFlowChart.SubstituteVariables(localText);

            if (hasCondition)
            {
                if (OnRequireValue != null)
                {
                    //OnRequireValue 自行添加判斷式, Menu將帶入 requireValue 來跑判斷式
                    interactable = OnRequireValue(requireValue);
                }
            }

            return AddOption(displayText, interactable, hideOption, targetBlock);
        }

        /// <summary>
        /// override AddOption, 增加了檢查 flowchart 是否完工
        /// </summary>
        public override bool AddOption(string text, bool interactable, bool hideOption, Block targetBlock)
        {
            int thisResultIndex = OptionResultIndex;
            OptionResultIndex++;
            _onScreenFlowchart.IsMenuOpen = true;

            var block = targetBlock;
            UnityEngine.Events.UnityAction action = delegate
            {
                EventSystem.current.SetSelectedGameObject(null);
                StopAllCoroutines();
                // Stop timeout
                Clear();  //Fire MenuEnd
                HideSayDialog();

                //Save the result in this running flowchart
                _onScreenFlowchart.SaveUserSelectedOption(thisResultIndex);
                _onScreenFlowchart.IsMenuOpen = false;

                if (block != null)
                {
                    var flowchart = block.GetFlowchart();
#if UNITY_EDITOR
                    // Select the new target block in the Flowchart window
                    flowchart.SelectedBlock = block;
#endif
                    SetActive(false);
                    // Use a coroutine to call the block on the next frame
                    // Have to use the Flowchart gameobject as the MenuDialog is now inactive
                    flowchart.StartCoroutine(CallBlock(block));
                }
                else
                {
                    SetActive(false);
                    _onScreenFlowchart.CheckFlowChartIdle();
                }
            };

            //Modify the origin code below from "private" to "protected virtual"
            return AddOption(text, interactable, hideOption, action);
        }


        public void UpdateCurrentLangContent(SystemLanguage language)
        {
            //initialize will call it , and in the time it's null
            if (cachedButtons == null)
                return;

            for (int i = 0; i < cachedButtons.Length; i++)
            {
                TextAdapter textAdapter = new TextAdapter();
                textAdapter.InitFromGameObject(cachedButtons[i].gameObject, true);
                if (textAdapter.HasTextObject() && !string.IsNullOrEmpty(_onScreenTextTerms[i]))
                {
                    string localText = LocalizeManager.GetLocalizeText(_onScreenTextTerms[i]);
                    
                    if(string.IsNullOrEmpty(localText)){
                        localText = $"no key:{_onScreenTextTerms[i]}";
                    }
                    else
                    {
                        //trim flowchart's token
                        if (_onScreenFlowchart != null)
                            localText = _onScreenFlowchart.SubstituteVariables(localText);

                        //trim global TextVariation's token
                        localText = TextVariationHandler.SelectVariations(localText);
                    }

                    textAdapter.Text = localText;
                }
            }
        }

        public void CloseMenuDialog(){
            EventSystem.current.SetSelectedGameObject(null);
            StopAllCoroutines();
            // Stop timeout
            Clear();  //Fire MenuEnd
            HideSayDialog();
            SetActive(false);
        }

    }
}