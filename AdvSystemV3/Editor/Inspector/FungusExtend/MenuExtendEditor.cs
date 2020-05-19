// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(MenuExtend))]
    public class MenuExtendEditor : CommandEditorExtend
    {
        protected SerializedProperty csvCommandKeyProp;
        protected SerializedProperty HasConditionProp;
        protected SerializedProperty requireValueProp;
        //NEW EXTEND

        protected SerializedProperty textProp;
        protected SerializedProperty descriptionProp;
        protected SerializedProperty targetBlockProp;
        protected SerializedProperty hideIfVisitedProp;
        protected SerializedProperty interactableProp;
        protected SerializedProperty setMenuDialogProp;
        protected SerializedProperty hideThisOptionProp;

        FlowchartExtend flowchart;

        public override void OnEnable()
        {
            base.OnEnable();

            textProp = serializedObject.FindProperty("text");
            descriptionProp = serializedObject.FindProperty("description");
            targetBlockProp = serializedObject.FindProperty("targetBlock");
            hideIfVisitedProp = serializedObject.FindProperty("hideIfVisited");
            interactableProp = serializedObject.FindProperty("interactable");
            setMenuDialogProp = serializedObject.FindProperty("setMenuDialog");
            hideThisOptionProp = serializedObject.FindProperty("hideThisOption");

            csvCommandKeyProp = serializedObject.FindProperty("csvCommandKey");
            HasConditionProp = serializedObject.FindProperty("HasCondition");
            requireValueProp = serializedObject.FindProperty("RequireValue");

            flowchart = (target as MenuExtend).GetComponent<FlowchartExtend>();
            CatchLocalizeText();
            //NEW EXTEND
        }

        protected virtual void CatchLocalizeText(){
            //Catch Localize Data when focus
            
            if(flowchart == null)
                return;
            MenuExtend t = target as MenuExtend;
            string textTerm = $"{flowchart.GoogleSheetID}.{flowchart.GooglePageID}.{csvCommandKeyProp.stringValue}";
            string localText = FungusExt.LocalizeManager.GetLocalizeText(textTerm);
            if(string.IsNullOrEmpty(localText)){
                string blockName = AdvUtility.FindParentBlock(flowchart, t).BlockName;
                Debug.LogError($"At : {t.gameObject.name} / {blockName} / {t.ItemId}");
            }
            textProp.stringValue = string.IsNullOrEmpty(localText) ? $"(key not found : {textTerm})" : localText;
            serializedObject.ApplyModifiedProperties();
        }
        
        public override void DrawCommandGUI()
        {
            if (flowchart == null)
            {
                return;
            }
            
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(csvCommandKeyProp);
            EditorGUILayout.PropertyField(textProp);

            EditorGUILayout.PropertyField(descriptionProp);
            
            BlockEditor.BlockField(targetBlockProp,
                                   new GUIContent("Target Block", "Block to call when option is selected"), 
                                   new GUIContent("<None>"), 
                                   flowchart);
            
            EditorGUILayout.PropertyField(hideIfVisitedProp);
            EditorGUILayout.PropertyField(interactableProp);
            EditorGUILayout.PropertyField(setMenuDialogProp);
            EditorGUILayout.PropertyField(hideThisOptionProp);

            MenuExtend t = target as MenuExtend;
            EditorGUILayout.PropertyField(HasConditionProp);

            if(t.HasCondition == true){
                EditorGUILayout.PropertyField(requireValueProp, new GUIContent("Require Gold"));
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }    
}
