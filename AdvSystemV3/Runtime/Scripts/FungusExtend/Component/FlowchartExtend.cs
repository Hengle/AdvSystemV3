using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    public class FlowchartExtend : Flowchart
    {
        //Extend Variables
        [Tooltip("不加參數時, 執行的Block名稱")] public string EntranceBlockName = "Main";
        [Tooltip("連結的 Google Sheet ID")] public string GoogleSheetID;
        [Tooltip("連結的 Google Page ID")] public string GooglePageID;

        //flowchart API extend
        [Tooltip("A callback for user assign")] public MenuSelectCallback onFlowchartFinished;
        [Tooltip("A callback for system event")] public Action onEndSystem;

        //for Menu system use
        protected List<int> UserSelectedOption = new List<int>();
        protected bool isMenuOpen = false;

        //for Update System
        [Tooltip("When create flowchart by csv, it will generate a entrance block")]
        public Block AutoGenerateBlock = null;

        [Tooltip("csv line text will be parse to AdvCsvLine. it stores keys info and linked block/command, so we can use it to update flowchart")][HideInInspector]
        public List<AdvCSVLine> csvLines = new List<AdvCSVLine>();

        [Tooltip("When download/update flowchart by csv, the csv will copy to it")]
        public string csvBackup;

        public bool IsMenuOpen
        {
            set { isMenuOpen = value; }
            get { return isMenuOpen; }
        }

        public Block MainBlock
        {
            get
            {
                if (AutoGenerateBlock == null)
                {
                    return FindBlock(EntranceBlockName);
                }
                return AutoGenerateBlock;
            }
        }

        /// <summary>
        /// DO NOT CALL THIS ON LOOP, 不要在迴圈呼叫此函式, 用於檢查flowchart 是否執行中
        /// </summary>
        public void CheckFlowChartIdle()
        {
            bool resultFlag = true;

            var blocks = GetComponents<Block>();
            for (int i = 0; i < blocks.Length; i++)
            {
                var block = blocks[i];
                if (block.IsExecuting())
                {
                    resultFlag = false;
                    break;
                }
            }
            if (isMenuOpen) resultFlag = false;

            if (resultFlag)
            {
                Debug.Log("<color=cyan>Flowhart Finished</color>");
                onFlowchartFinished?.Invoke(UserSelectedOption);
                onEndSystem?.Invoke();
                AdvSignals.AdvCheckFlowchartEnd -= CheckFlowChartIdle;
            }
        }

        public void SaveUserSelectedOption(int selectedIndex)
        {
            UserSelectedOption.Add(selectedIndex);
        }

        public void StartFlowChart(MenuSelectCallback callback)
        {
            StartFlowChart(EntranceBlockName, callback);
        }

        public void StartFlowChart(string blockName, MenuSelectCallback callback)
        {
            onFlowchartFinished = callback;
            UserSelectedOption.Clear();

            Block _targetBlock = FindBlock(blockName);

            if (_targetBlock == null)
            {
                Debug.LogError("找不到指定 Block ! (" + blockName + ")");
            }
            else
            {
                // Ensure CommandList is not null at "runtime"
                _targetBlock.CommandList.RemoveAll(item => item == null);
                AdvSignals.AdvCheckFlowchartEnd = null;
                AdvSignals.AdvCheckFlowchartEnd += CheckFlowChartIdle;
                ExecuteBlock(_targetBlock, 0, null);
            }
        }

        [ContextMenu("Excute This Flowchart")]
        public void ExcuteSelfEditor()
        {
            if (Application.isPlaying)
            {
                if (AdvManager.Instance != null)
                    AdvManager.Instance.LoadContent(this, null);
            }
            else
                Debug.Log("僅限播放模式中使用此功能");
        }

        [ContextMenu("Get All Block Names")]
        public void GetBlockNames()
        {
            var blocks = GetComponents<Block>();
            string result = "";
            foreach (var item in blocks)
            {
                result += item.BlockName + "\n";
            }
            Debug.Log(result);
        }
    }
}