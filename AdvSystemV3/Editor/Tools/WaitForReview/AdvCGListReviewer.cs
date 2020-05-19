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

    public class AdvCGListReviewer : OdinEditorWindow
    {
        [MenuItem("Tools/Fungus/Adv CG Names List")]
        public static AdvCGListReviewer OpenWindow()
        {
            var window = GetWindow<AdvCGListReviewer>();

            // Nifty little trick to quickly position the window in the middle of the editor.
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);

            return window;
        }

        [Title("Adv CG Names List"), InfoBox("拖曳要檢查的Prefab 丟入列表, 將輸出CG 指令中用到的圖檔")]
        public List<GameObject> objectList;
        [LabelText("無用"), HorizontalGroup("Head")] public int nothing = 3;

        [InfoBox("Result")]
        [ReadOnly, Multiline(300)] public string result;


        [Button(ButtonSizes.Large), HorizontalGroup("Head")]
        public void StartCheck(){
            if(objectList.Count <= 0)
                return;

            result = "";
            List<string> names = new List<string>();

            foreach (GameObject sourceObject in objectList)
            {
                if(sourceObject == null)
                    continue;
                
                ControlCG [] comps = sourceObject.GetComponents<ControlCG>();

                foreach (ControlCG comp in comps)
                {
                    if(comp.SpriteCG != null){
                        string CGName = comp.SpriteCG.name;
                        if(!names.Contains(CGName)){
                            names.Add (CGName);
                            result += string.Format("{0}\n", CGName);
                        }
                        continue;
                    }
                }
            }
            result += string.Format("check finished. total number : {0} \n", names.Count);
            names.Sort();
            string outputLog = "";
            foreach (var item in names)
            {
                outputLog += item + "\n";
            }
            Debug.Log(outputLog);
        }
    }
}
#endif
