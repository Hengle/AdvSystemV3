using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Object = UnityEngine.Object;

namespace Fungus.EditorUtils
{
    public class FlowchartWindowExtend : FlowchartWindow
    {
        [MenuItem("Tools/Fungus/Flowchart Window (Extend)")]
        static void Init()
        {
            GetWindow(typeof(FlowchartWindowExtend), false, "Flowchart (Extend)");
        }

        protected TextAsset selectedTextAsset;

        private void OnFocus() {
            if(GetFlowchart() == null)
                return;
                
            List<string> storeBlockName = new List<string>();
            foreach (var item in GetFlowchart().GetComponents<Block>())
            {
                if(!storeBlockName.Contains(item.BlockName)){
                    storeBlockName.Add(item.BlockName);
                } else {
                    Debug.LogError("目前的Flowchart : " + GetFlowchart().name + " 具有相同的BlockName!, 請拖動block位置檢查有無重疊 : " + item.BlockName);
                }
            }
        }

        protected override void DrawOverlay(Event e){
            DrawSaveMenu(e);
            base.DrawOverlay(e);
            DrawTextAssetImporter();
        }

        protected virtual void DrawSaveMenu(Event e){
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(" Save As... ", EditorStyles.toolbarButton))
            {
                SaveAsNew();
            }
            GUILayout.Space(30);
            if (GUILayout.Button($" Save ", EditorStyles.toolbarButton))
            {
                SaveFlowchart();
            }
            GUILayout.EndHorizontal();
        }

        protected virtual void DrawTextAssetImporter(){

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                //選擇生成Blocks所需的TextAsset並保存到成員變量selectedText
                selectedTextAsset = (TextAsset)EditorGUILayout.ObjectField("Select CSV Asset: ", selectedTextAsset, typeof(TextAsset), true);

                //點擊Create Blocks調用CreateBlocksByTxt創建Blocks
                if (GUILayout.Button("Create Blocks", EditorStyles.toolbarButton))
                {
                    string origintxt = selectedTextAsset.text;
                    if (origintxt == null || origintxt == "")
                    {
                        Debug.LogError("Can't read text from file or the text is blank , please check the file");
                        return;
                    }
                    // 系統會 "登" 一聲
                    EditorApplication.Beep();
                                    
                    // 顯示對話框功能(帶有 OK 和 Cancel 兩個按鈕)
                    if (EditorUtility.DisplayDialog("Really?", "Do you really want to clear all and create new ?", "Yes", "No") == true)
                    {
                        bool result = AdvCSVHelper.VerifyKey(origintxt);
                        if(result){
                            Debug.Log("Keys 檢查完畢, 開始執行");
                            CreateBlocksByCsv(origintxt); 
                        }
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        void CreateBlocksByCsv(string originTxt){
            FlowchartExtend flowchart = GetFlowchart() as FlowchartExtend;
            if(flowchart == null) {
                Debug.Log("This is not a FlowchartExtend");
                return;
            }
            AdvUtility.CreateBlockByCSV(flowchart, originTxt, true);

            flowchart.ClearSelectedCommands();
            DeselectAll();
            UpdateBlockCollection();
        }

        protected virtual void SaveAsNew(){
            string path = AssetDatabase.GenerateUniqueAssetPath($"{AdvEditorConfig.Instance.AdvPrefabFolderPath}{flowchart.gameObject.name}.prefab");
            StartSave(path);
        }

        protected virtual void SaveFlowchart(){
            var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                Debug.LogError("in prefab edit mode, use unity's save method");
                return;
            }
            string path = $"{AdvEditorConfig.Instance.AdvPrefabFolderPath}{flowchart.gameObject.name}.prefab";
            StartSave(path);
        }

        protected void StartSave(string path){
            try {
                GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(flowchart.gameObject, path);
                Debug.Log($"Save prefab to {path}");
            } catch (Exception e) {
                Debug.Log("Save >> " + e.Message.ToString());
            }
        }


        public static string GetCurrentFolder()
        {
            var path = "";
            var obj = Selection.activeObject;
            if (obj == null) path = "Assets";
            else path = AssetDatabase.GetAssetPath(obj.GetInstanceID());

            return path;
        }

        // Some bug fix?
        //Back Script in FlowchartWindow
        // protected void UpdateFilteredBlocks()
        // {
        //     if (filterStale)
        //     {
        //         filterStale = false;
        //         //reset all
        //         foreach (var item in filteredBlocks)
        //         {
        //             if(item == null){
        //                 //UpdateBlockCollection();
        //             } else{
        //                 item.IsFiltered = false;
        //             }
        //         }
                
        //         //gather new
        //         filteredBlocks = blocks.Where(block => block.BlockName.ToLower().Contains(searchString.ToLower())).ToArray();
                
        //         //update filteredness
        //         foreach (var item in filteredBlocks)
        //         {
        //             item.IsFiltered = true;
        //         }

        //         blockPopupSelection = Mathf.Clamp(blockPopupSelection, 0, filteredBlocks.Length - 1);
        //     }
        // }
    }
}