#if UNITY_EDITOR && ODIN_INSPECTOR
namespace AdvSystemV2
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector;
    using Sirenix.Utilities.Editor;
    using Sirenix.Utilities;
    using Fungus;

    public class AdvCGRemap : OdinEditorWindow
    {
        [MenuItem("Tools/Fungus/CG Recover")]
        public static AdvCGRemap OpenWindow()
        {
            var window = GetWindow<AdvCGRemap>();

            // Nifty little trick to quickly position the window in the middle of the editor.
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);

            return window;
        }
        
        [Title("CG Reference Recover")]
        [InfoBox("Prefab infomation provider")]
        public GameObject sourceObject;

        [InfoBox("Prefab want to recover")]
        public GameObject targetObject;

        [InfoBox("Result")]
        [ReadOnly, Multiline(30)] public string result;


        [Button(ButtonSizes.Large)]
        public void ReoverCG(){
            if(sourceObject == null || targetObject == null)
                return;

            result = "";
            ControlCG [] srcComp = sourceObject.GetComponents<ControlCG>();
            ControlCG [] tarComp = targetObject.GetComponents<ControlCG>();
            List<ControlCG> remainTarList = new List<ControlCG>(tarComp);
            int index = 0;
            foreach (var item in srcComp)
            {
                if(item.Display == BackgroundDisplayType.HideAll || item.Display == BackgroundDisplayType.None)
                    continue;

                string CGName = item.SpriteCG ? item.SpriteCG.name : "(Null)";

                foreach (var target in tarComp)
                {
                    if(item.ItemId == target.ItemId){
                        target.SpriteCG = item.SpriteCG;
                        remainTarList.Remove(target);

                        result += string.Format("id:{0} / index:{1} : CG Name {2} apply \n", index, item.ItemId, CGName);
                    }
                }
                
                index++;
            }

            result += "\n";
            foreach (var item in remainTarList)
            {
                if(item.Display == BackgroundDisplayType.Show)
                    result += string.Format("Not Update CG index:{0} , Block Name : {1} \n", item.ItemId , AdvUtility.FindParentBlock(item.GetComponent<FlowchartExtend>(), item).BlockName);
            }

            UnityEditor.EditorUtility.SetDirty(targetObject);
            PrefabUtility.SavePrefabAsset(targetObject);
        }
    }
}
#endif
