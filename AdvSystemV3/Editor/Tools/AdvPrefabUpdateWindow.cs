#if UNITY_EDITOR
namespace FungusExt
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector;
    using Fungus;

    public class AdvPrefabUpdateWindow : OdinEditorWindow
    {
        [MenuItem("Tools/Fungus/Adv Prefab Updater")]
        public static AdvPrefabUpdateWindow OpenWindow()
        {
            var window = GetWindow<AdvPrefabUpdateWindow>(false, "Prefab Update (Require Odin plugin)");

            const int width = 700;
            const int height = 700;
            var x = (Screen.currentResolution.width - width) / 2;
            var y = (Screen.currentResolution.height - height) / 2;
            window.position = new Rect(x, y, width, height);

            return window;
        }

        protected override void OnGUI()
        {
            EditorGUILayout.HelpBox("用途: 單一指定prefab更新csv內容, 能夠檢查哪些key需要移除 (Require Odin plugin)", MessageType.Info);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            base.OnGUI();
        }
        
        [Title("Adv Prefab Update")]
        [InfoBox("要更新的Prefab")]
        public FlowchartExtend sourceObject;

        [Title("Adv CSV Source")]
        [InfoBox("更新來源CSV")]
        [AssetsOnly]
        public TextAsset sourceCSV;


        [Title("Update Config")]
        [InfoBox("需要更新的類型")]
        [ToggleLeft] public bool _sayContentText = true;
        [ToggleLeft] public bool _SelectionContentText = true;
        [ToggleLeft] public bool _saySpeakerTerm = false;
        [ToggleLeft] public bool _sayAvatarSprite = false;
        [ToggleLeft] public bool _sayVoice = false;
        [ToggleLeft] public bool _blockName = false;
        [ToggleLeft] public bool _background = false;
        [ToggleLeft] public bool _CG = false;
        [ToggleLeft] public bool _billboard = false;
        [ToggleLeft] public bool _bgm = false;


        [Button(ButtonSizes.Large)]
        public void StartProcess() {
            AdvUpdateOption _option = new AdvUpdateOption(){
                sayText = _sayContentText,
                selectionText = _SelectionContentText,
                sayTerm = _saySpeakerTerm,
                saySprite = _sayAvatarSprite,
                sayVoice = _sayVoice,
                blockName = _blockName,
                background = _background,
                CG = _CG,
                billboard = _billboard,
                BGM = _bgm,
            };

            if(sourceCSV == null){
                AdvUtility.LogError("沒有選擇 CSV檔案");
                return;
            }
            if(WifeEditorUtility.isInPrefabStage() == false){
                Debug.LogError("必須要進入 prefab 編輯模式裡才能夠使用此功能");
                return;
            }

            List<AdvCSVLine> outdate = AdvUtility.UpdateBlockByCSV(sourceObject, sourceCSV.text, _option, true);
            willRemove = outdate;

            UnityEditor.EditorUtility.SetDirty(sourceObject.gameObject);
        }
        [Title("Will Remove")]
        [InfoBox("CSV 中已移除的Cmd，會存在於此清單，點擊 StartRemove 來確認移除")]
        [TableList(DrawScrollView = true, HideToolbar = false)]
        public List<AdvCSVLine> willRemove;

        [Button(ButtonSizes.Large)]
        public void StartRemove() {
            if(willRemove == null)
                return;
            if(sourceObject == null)
                return;

            foreach (var item in willRemove)
            {
                sourceObject.csvLines.Remove(item);
                Command cmd = item.generatedCommand;
                if(cmd != null){
                    AdvUtility.FindParentBlock(cmd).CommandList.Remove(cmd);
                    DestroyImmediate(cmd);
                }
                Block blk = item.generateBlock;
                if(blk != null){
                    var commandList = blk.CommandList;
                    for (int j = 0; j < commandList.Count; ++j)
                    {
                        DestroyImmediate(commandList[j]);
                    }
                    if (blk._EventHandler != null)
                    {
                        DestroyImmediate(blk._EventHandler);
                    }
                    sourceObject.ClearSelectedCommands();

                    DestroyImmediate(blk);
                }
            }

            willRemove.Clear();
        }

        [Button(ButtonSizes.Large)]
        public void ReUpdateLastCSV(){
            AdvUpdateOption _option = new AdvUpdateOption(){
                sayText = _sayContentText,
                selectionText = _SelectionContentText,
                sayTerm = _saySpeakerTerm,
                saySprite = _sayAvatarSprite,
                sayVoice = _sayVoice,
                blockName = _blockName,
                background = _background,
                CG = _CG,
                billboard = _billboard,
                BGM = _bgm,
            };

            if(string.IsNullOrEmpty(sourceObject.csvBackup)){
                AdvUtility.LogError("沒有Last CSV");
                return;
            }
            if(WifeEditorUtility.isInPrefabStage() == false){
                Debug.LogError("必須要進入 prefab 編輯模式裡才能夠使用此功能");
                return;
            }

            List<AdvCSVLine> outdate = AdvUtility.UpdateBlockByCSV(sourceObject, sourceObject.csvBackup, _option, true);
            willRemove = outdate;

            UnityEditor.EditorUtility.SetDirty(sourceObject.gameObject);
        }
    }
}
#endif
