﻿using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(CallExtend))]
    public class CallExtendEditor : CommandEditorExtend 
    {
        protected SerializedProperty targetFlowchartProp;
        protected SerializedProperty targetBlockProp;
        protected SerializedProperty startLabelProp;
        protected SerializedProperty startIndexProp;
        protected SerializedProperty callModeProp;

        public override void OnEnable()
        {
            base.OnEnable();

            targetFlowchartProp = serializedObject.FindProperty("targetFlowchart");
            targetBlockProp = serializedObject.FindProperty("targetBlock");
            startLabelProp = serializedObject.FindProperty("startLabel");
            startIndexProp = serializedObject.FindProperty("startIndex");
            callModeProp = serializedObject.FindProperty("callMode");
        }

        public override void DrawCommandGUI()
        {
            serializedObject.Update();

            Call t = target as Call;

            Flowchart flowchart = null;
            if (targetFlowchartProp.objectReferenceValue == null)
            {
                flowchart = (Flowchart)t.GetFlowchart();
            }
            else
            {
                flowchart = targetFlowchartProp.objectReferenceValue as Flowchart;
            }

            EditorGUILayout.PropertyField(targetFlowchartProp);

            if (flowchart != null)
            {
                BlockEditor.BlockField(targetBlockProp,
                                       new GUIContent("Target Block", "Block to call"), 
                                       new GUIContent("<None>"), 
                                       flowchart);

                EditorGUILayout.PropertyField(startLabelProp);

                EditorGUILayout.PropertyField(startIndexProp);
            }

            EditorGUILayout.PropertyField(callModeProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
