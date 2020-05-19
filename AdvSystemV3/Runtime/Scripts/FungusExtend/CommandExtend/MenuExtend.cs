using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Narrative", 
                 "Menu (Extend)", 
                 "Displays a button in a multiple choice menu")]
    [AddComponentMenu("")]
    public class MenuExtend : Menu , ICommand
    {
        [SerializeField] protected int csvLine;
        [SerializeField] protected string csvCommandKey;
        public int CSVLine { get { return csvLine; } set { csvLine = value; }}
        public string CSVCommandKey { get { return csvCommandKey; } set { csvCommandKey = value; }}


        [SerializeField] public bool HasCondition = false;
        [SerializeField] public int RequireValue = 0;

        private string targetBlockName;

        public override void OnEnter()
        {
            bool hideOption = (hideIfVisited && targetBlock != null && targetBlock.GetExecutionCount() > 0) || hideThisOption.Value;

            // Default Menu dialog is AdvManager's
            MenuDialogExtend menuDialog = AdvManager.Instance.advMenuDialog;

            if (menuDialog != null)
            {
                menuDialog.SetActive(true);

                var flowchartExt = GetComponent<FlowchartExtend>(); //TODO: use better method
                string textTerm = $"{flowchartExt.GoogleSheetID}.{flowchartExt.GooglePageID}.{CSVCommandKey}";
                
                menuDialog.AddOption(textTerm, interactable, hideOption, targetBlock, flowchartExt, HasCondition, RequireValue);
            }
            
            Continue();
        }

        public void InitializeByParams(object[] param){
            CommandParam data = param[0] as CommandParam;
            SearchBlockHandler _sHandler = param[1] as SearchBlockHandler;

            bool isUpdateText = true;
            bool isUpdateLink = true;

            if(data.option != null){
                isUpdateText = data.option.selectionText;
                isUpdateLink = data.option.selection;
            }

            if(isUpdateText){
                if(data.locText.Count > 0)
                    this.text = data.locText[0].content;
            }

            if(isUpdateLink){

                targetBlockName = data.target;
                //Search Block Name
                targetBlock = GetFlowchart().FindBlock(targetBlockName);

                //If not exist , add event to listen future block 
                if(targetBlock == null){
                    searchHandler = _sHandler;
                    searchHandler.createBlockEvent += FindBlock;
                }
            }
        }

        SearchBlockHandler searchHandler;

        void FindBlock(Block block){
            //Debug.Log("Callback search " + block.BlockName + " whether is ? : " + targetBlockName);
            if(block.BlockName == targetBlockName){
                targetBlock = block;
                searchHandler.createBlockEvent -= FindBlock;
            }
        }
    }
}