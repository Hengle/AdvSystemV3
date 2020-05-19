using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Flow", 
                 "Call (Extend)", 
                 "Execute another block in the same Flowchart as the command, or in a different Flowchart.")]
    [AddComponentMenu("")]
    public class CallExtend : Call, ICommand
    {
        [SerializeField] protected int csvLine;
        [SerializeField] protected string csvCommandKey;
        public int CSVLine { get { return csvLine; } set { csvLine = value; }}
        public string CSVCommandKey { get { return csvCommandKey; } set { csvCommandKey = value; }}


        protected string targetBlockName;
        SearchBlockHandler searchHandler;
        public void InitializeByParams(object[] param)
        {
            CommandParam data = param[0] as CommandParam;
            SearchBlockHandler _sHandler = param[1] as SearchBlockHandler;

            targetBlockName = data.target;
            //Search Block Name
            targetBlock = GetFlowchart().FindBlock(targetBlockName);

            //If not exist , add event to listen future block 
            if (targetBlock == null)
            {
                searchHandler = _sHandler;
                searchHandler.createBlockEvent += FindBlock;
            }
        }

        void FindBlock(Block block)
        {
            if (block.BlockName == targetBlockName)
            {
                targetBlock = block;
                searchHandler.createBlockEvent -= FindBlock;
            }
        }

        public Block TargetBlock
        {
            set { targetBlock = value; }
            get { return targetBlock; }
        }

        public override void OnEnter()
        {
            if(targetBlock == null){
                Continue();
                return;
            } else {
                base.OnEnter();
            }
        }
    }
}