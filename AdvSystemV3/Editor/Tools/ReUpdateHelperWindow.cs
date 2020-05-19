#if UNITY_EDITOR
namespace FungusExt
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector;
    using Fungus;

    public class ReUpdateHelperWindow : OdinEditorWindow
    {
        [MenuItem("Tools/Fungus/Deprecated/Adv Prefab Auto Re-Update Tool")]
        public static ReUpdateHelperWindow OpenWindow()
        {
            var window = GetWindow<ReUpdateHelperWindow>(false, "Re-Update Prefab (Require Odin plugin)");

            const int width = 700;
            const int height = 700;
            var x = (Screen.currentResolution.width - width) / 2;
            var y = (Screen.currentResolution.height - height) / 2;
            window.position = new Rect(x, y, width, height);

            return window;
        }

        protected override void OnGUI()
        {
            EditorGUILayout.HelpBox("用途: 可一次指定多個prefab進行 Re-Update Last CSV (Require Odin plugin)", MessageType.Info);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            base.OnGUI();
        }
        
        [Title("Adv Prefabs list to Re-Update"), InfoBox("拖曳要更新的Prefab 進入列表, <color=#FF0000>Update 完成後需要按一次 Ctrl + S 來儲存prefab變更</color>")]
        public List<FlowchartExtend> objectList;

        [Button(ButtonSizes.Large)]
        public void ReUpdateLastCSV(){
            if(objectList.Count <= 0)
                return;

            AdvUpdateOption _option = new AdvUpdateOption();

            foreach (FlowchartExtend sourceObject in objectList)
            {
                if(sourceObject == null)
                    continue;

                if(string.IsNullOrEmpty(sourceObject.csvBackup)){
                    AdvUtility.LogError("沒有Last CSV");
                    return;
                }

                List<AdvCSVLine> willRemove = AdvUtility.UpdateBlockByCSV(sourceObject, sourceObject.csvBackup, _option, true);
                if(willRemove.Count > 0){
                    Debug.LogWarning("Will Remove greater 0 , may be some error... (" + sourceObject.gameObject.name + ")");
                }

                UnityEditor.EditorUtility.SetDirty(sourceObject.gameObject);
                PrefabUtility.SavePrefabAsset(sourceObject.gameObject);
            }
        }
    }
}
#endif
