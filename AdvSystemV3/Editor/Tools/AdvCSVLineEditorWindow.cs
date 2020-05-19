#if UNITY_EDITOR
namespace FungusExt
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector;
    using Fungus;

    public class AdvCSVLineEditorWindow : OdinEditorWindow
    {
        public static AdvCSVLineEditorWindow OpenWindow()
        {
            var window = GetWindow<AdvCSVLineEditorWindow>(false, "AdvCSVLine (Require Odin plugin)");

            const int width = 700;
            const int height = 700;
            var x = (Screen.currentResolution.width - width) / 2;
            var y = (Screen.currentResolution.height - height) / 2;
            window.position = new Rect(x, y, width, height);

            return window;
        }

        protected override void OnGUI()
        {
            EditorGUILayout.HelpBox("用途: 一個除錯工具, 用於編輯prefab與csv之間的關聯資料 (Require Odin plugin)", MessageType.Info);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            base.OnGUI();
        }
        
        [Title("Adv Prefab Update")]
        [InfoBox("編輯中的Prefab")]
        public FlowchartExtend sourceObject;

        [Title("ADV CSV Line Data")]
        [InfoBox("保存於 Flowchart 中的 CSV Line")]
        [TableList(DrawScrollView = true)]
        public List<AdvCSVLine> csvLineData;

        [Title("ADV CSV Line Block Data")]
        [InfoBox("Block 功能的 Line")]
        [TableList(DrawScrollView = true)]
        public List<AdvCSVLine> csvLineBlocks;

        [Title("ADV CSV No Link Line")]
        [InfoBox("尚未 Link 的 Line")]
        [TableList(DrawScrollView = true)]
        public List<AdvCSVLine> csvLineNoLink;

        public void ViewCSVLineData(FlowchartExtend src){
            sourceObject = src;
            if(sourceObject == null)
                return;
            
            csvLineData = sourceObject.csvLines;

            csvLineBlocks = new List<AdvCSVLine>();
            csvLineNoLink = new List<AdvCSVLine>();
            foreach (var item in sourceObject.csvLines)
            {
                if(item.Command.StartsWith("*")){
                    csvLineBlocks.Add(item);
                }
                if(item.generateBlock == null && item.generatedCommand == null){
                    csvLineNoLink.Add(item);
                }
            }
        }

        [Title("Block Link Helper")]
        public string CmdKey;
        public string BlockName;
        
        [Button(ButtonSizes.Large)]
        public void LinkProcess(){
            if(sourceObject == null)
                return;
            if(string.IsNullOrEmpty(CmdKey) || string.IsNullOrEmpty(BlockName))
                return;
            if(csvLineData == null)
                return;
            Block _block = sourceObject.FindBlock(BlockName);
            if(_block == null)
                return;

            AdvCSVLine line = AdvUtility.SearchLineByKey(csvLineData, CmdKey);
            if(line != null)
                line.generateBlock = _block;

            ViewCSVLineData(sourceObject);
            AdvUtility.Log("Link Block Success");
        }

        [Button(ButtonSizes.Large)]
        public void AutoLink(){
            AdvUtility.AutoLinkCSVLine(sourceObject);

            UnityEditor.EditorUtility.SetDirty(sourceObject.gameObject);
            ViewCSVLineData(sourceObject);
            Debug.Log("Auto link success");
        }

        [Button(ButtonSizes.Large)]
        public void RebuildAdvCSVLine(){
            AdvUtility.RebuildCSVLine(sourceObject);

            UnityEditor.EditorUtility.SetDirty(sourceObject.gameObject);
            ViewCSVLineData(sourceObject);
            Debug.Log("Rebuild success");
        }

        [Button(ButtonSizes.Large)]
        public void RemoveUnlinkCSVLine(){
            if(!EditorUtility.DisplayDialog("Warning","Are you sure to remove unlink CSVLine?", "OK")){
                return;
            }
            foreach (var item in csvLineNoLink)
            {
                csvLineData.Remove(item);
            }
            UnityEditor.EditorUtility.SetDirty(sourceObject.gameObject);
            ViewCSVLineData(sourceObject);
            AdvUtility.Log("Remove Success");
        }
    }
}
#endif
