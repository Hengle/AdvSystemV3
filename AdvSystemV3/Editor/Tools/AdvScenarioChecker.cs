#if UNITY_EDITOR
namespace FungusExt
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector;
    using Fungus;

    public class AdvScenarioChecker : OdinEditorWindow
    {
        [MenuItem("Tools/Fungus/Adv Scenario Checker")]
        public static AdvScenarioChecker OpenWindow()
        {
            var window = GetWindow<AdvScenarioChecker>(false, "Scenario Checker (Require Odin plugin)");

            const int width = 700;
            const int height = 700;
            var x = (Screen.currentResolution.width - width) / 2;
            var y = (Screen.currentResolution.height - height) / 2;
            window.position = new Rect(x, y, width, height);

            return window;
        }

        protected override void OnGUI()
        {
            EditorGUILayout.HelpBox("用途: 檢查prefab文本中, 是否有空白(漏翻譯)的文本 (Require Odin plugin)", MessageType.Info);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            base.OnGUI();
        }

        [Title("Adv Scenario Checker"), InfoBox("拖曳要檢查的Prefab 丟入列表, 將會檢查劇本是否有遺漏翻譯或文句空白")]
        public List<GameObject> objectList;
        [LabelText("檢查語言數量"), HorizontalGroup("Head")] public int LocalizeNumber = 3;

        [InfoBox("Result")]
        [ReadOnly, Multiline(300)] public string result;


        [Button(ButtonSizes.Large), HorizontalGroup("Head")]
        public void StartCheck(){
            if(objectList.Count <= 0)
                return;

            result = "";

            foreach (GameObject sourceObject in objectList)
            {
                if(sourceObject == null)
                    continue;
                
                SayExtend [] comps = sourceObject.GetComponents<SayExtend>();

                foreach (SayExtend comp in comps)
                {
                    // if(comp.LocalizeText == null || comp.LocalizeText.Count == 0){
                    //     result += string.Format("prefab:{0} 's Say({1})({2}) has no LocalizeText ! \n", sourceObject.name, comp.ItemId, AdvUtility.FindParentBlock(comp.GetFlowchart(),comp).BlockName);
                    //     continue;
                    // }

                    // if(comp.LocalizeText.Count != LocalizeNumber){
                    //     result += string.Format("prefab:{0} 's Say({1})({2}) , localizeText number not equal Settings !\n", sourceObject.name, comp.ItemId, AdvUtility.FindParentBlock(comp.GetFlowchart(),comp).BlockName);
                    // }
                    
                    // foreach (var lText in comp.LocalizeText)
                    // {
                    //     if(string.IsNullOrEmpty(lText.content)){
                    //         result += string.Format("prefab:{0} 's Say({1})({2}) , ({3})'s content is null ! \n", sourceObject.name, comp.ItemId, AdvUtility.FindParentBlock(comp.GetFlowchart(),comp).BlockName, lText.tag);
                    //     }
                    // }
                }

                MenuExtend [] m_comps = sourceObject.GetComponents<MenuExtend>();

                foreach (MenuExtend comp in m_comps)
                {
                    // if(comp.LocalizeText == null || comp.LocalizeText.Count == 0){
                    //     result += string.Format("prefab:{0} 's Menu({1})({2}) has no LocalizeText ! \n", sourceObject.name, comp.ItemId, AdvUtility.FindParentBlock(comp.GetFlowchart(),comp).BlockName);
                    //     continue;
                    // }

                    // if(comp.LocalizeText.Count != LocalizeNumber){
                    //     result += string.Format("prefab:{0} 's Menu({1})({2}) , localizeText number not equal Settings !\n", sourceObject.name, comp.ItemId, AdvUtility.FindParentBlock(comp.GetFlowchart(),comp).BlockName);
                    // }
                    
                    // foreach (var lText in comp.LocalizeText)
                    // {
                    //     if(string.IsNullOrEmpty(lText.content)){
                    //         result += string.Format("prefab:{0} 's Menu({1})({2}) , ({3})'s content is null ! \n", sourceObject.name, comp.ItemId, AdvUtility.FindParentBlock(comp.GetFlowchart(),comp).BlockName, lText.tag);
                    //     }
                    // }
                }

                result += string.Format("{0} check finished. \n", sourceObject.name);
            }
        }
    }
}
#endif
