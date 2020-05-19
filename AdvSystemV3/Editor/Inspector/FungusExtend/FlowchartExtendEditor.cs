// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fungus.EditorUtils
{
    [CustomEditor(typeof(FlowchartExtend))]
    public class FlowchartExtendEditor : FlowchartEditor
    {
        protected SerializedProperty entranceBlockNameProp;
        protected SerializedProperty googleSheetIdProp;
        protected SerializedProperty googlePageIdProp;
        protected SerializedProperty csvBackupProp;

        protected override void OnEnable()
        {
            if (NullTargetCheck()) // Check for an orphaned editor instance
                return;

            entranceBlockNameProp = serializedObject.FindProperty("EntranceBlockName");
            googleSheetIdProp = serializedObject.FindProperty("GoogleSheetID");
            googlePageIdProp = serializedObject.FindProperty("GooglePageID");
            csvBackupProp = serializedObject.FindProperty("csvBackup");

            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var flowchart = target as FlowchartExtend;

            //accouding hideCommands var, set component's hide flag to HIDE
            flowchart.UpdateHideFlags();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(descriptionProp);
            EditorGUILayout.PropertyField(colorCommandsProp);
            EditorGUILayout.PropertyField(hideComponentsProp);
            EditorGUILayout.PropertyField(stepPauseProp);
            EditorGUILayout.PropertyField(saveSelectionProp);
            EditorGUILayout.PropertyField(localizationIdProp);
            EditorGUILayout.PropertyField(showLineNumbersProp);
            EditorGUILayout.PropertyField(luaEnvironmentProp);
            EditorGUILayout.PropertyField(luaBindingNameProp);

            // Show list of commands to hide in Add Command menu
            //ReorderableListGUI.Title(new GUIContent(hideCommandsProp.displayName, hideCommandsProp.tooltip));
            //ReorderableListGUI.ListField(hideCommandsProp);
            EditorGUILayout.PropertyField(hideCommandsProp, new GUIContent(hideCommandsProp.displayName, hideCommandsProp.tooltip), true);

            GUILayout.Space(20);
            EditorGUILayout.LabelField("Extend Variable", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(entranceBlockNameProp);
            EditorGUILayout.PropertyField(googleSheetIdProp);
            EditorGUILayout.PropertyField(googlePageIdProp);
            EditorGUILayout.PropertyField(csvBackupProp);
            GUILayout.Space(20);

            if (EditorGUI.EndChangeCheck())
            {
                FlowchartDataStale = true;
            }

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
            float buttonWidth = 250;

            InsertCenterContent(delegate
            {
                if (GUILayout.Button(new GUIContent("Open Flowchart Window (Extend)", "Opens the Flowchart Window Extend"), GUILayout.Width(buttonWidth)))
                {
                    EditorWindow.GetWindow(typeof(FlowchartWindowExtend), false, "Flowchart (Extend)");
                }
            });

            GUILayout.Space(20);
            EditorGUILayout.LabelField("Flowchart Update Tool", labelStyle, GUILayout.ExpandWidth(true));

            InsertCenterContent(delegate
            {
                if (GUILayout.Button(new GUIContent("Open Updater Window", "Update Flowchart By CSV"), GUILayout.Width(buttonWidth)))
                {
                    var window = FungusExt.AdvPrefabUpdateWindow.OpenWindow();
                    window.sourceObject = flowchart;
                }
            });
            InsertCenterContent(delegate
            {
                if (GUILayout.Button(new GUIContent("Re-Update Last CSV", "Re-Update Prefab by last update or created CSV"), GUILayout.Width(buttonWidth)))
                {
                    if(string.IsNullOrEmpty(flowchart.csvBackup)){
                        Debug.LogError("沒有 Last CSV (last csv not fount)");
                        return;
                    }
                    if(WifeEditorUtility.isInPrefabStage() == false){
                        Debug.LogError("必須要進入 prefab 編輯模式裡才能夠使用此功能");
                        return;
                    }
                    List<AdvCSVLine> willRemove = AdvUtility.UpdateBlockByCSV(flowchart, flowchart.csvBackup, new AdvUpdateOption(), true);
                    if(willRemove.Count > 0){
                        Debug.LogWarning("Will Remove greater 0 , may be some error, use AdvCSVLine Editor to check");
                    }
                    UnityEditor.EditorUtility.SetDirty(flowchart.gameObject);
                }
            });

            GUILayout.Space(20);
            EditorGUILayout.LabelField("AdvCSVLine Fix tool", labelStyle, GUILayout.ExpandWidth(true));

            InsertCenterContent(delegate
            {
                if (GUILayout.Button(new GUIContent("Open CSV Line Editor", "CSV Line Data Viewer And Editor"), GUILayout.Width(buttonWidth)))
                {
                    var window = FungusExt.AdvCSVLineEditorWindow.OpenWindow();
                    window.ViewCSVLineData(flowchart);
                }
            });


            serializedObject.ApplyModifiedProperties();

            //Show the variables in the flowchart inspector
            GUILayout.Space(20);

            DrawVariablesGUI(false, Mathf.FloorToInt(EditorGUIUtility.currentViewWidth) - VariableListAdaptor.ReorderListSkirts);
        }

        protected void InsertCenterContent(System.Action contentCode)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            contentCode();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}