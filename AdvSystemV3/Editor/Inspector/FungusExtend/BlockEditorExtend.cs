// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

namespace Fungus.EditorUtils
{
    [CustomEditor(typeof(Block))]
    public class BlockEditorExtend : BlockEditor
    {
        private SerializedProperty commandListPropertyExtend;
        protected override void OnEnable()
        {
            base.OnEnable();

            //this appears to happen when leaving playmode
            try
            {
                if (serializedObject == null)
                    return;
            }
            catch (Exception)
            {
                return;
            }

            commandListPropertyExtend = serializedObject.FindProperty("commandList");

            //Sora.add
            //Debug.Log("Editor on enable " + (target as Block).BlockName + " , c:" + (target as Block).CommandList.Count);
            // Remove any null entries in the command list.
            // It happens when a command component is deleted or renamed.
            for (int i = commandListPropertyExtend.arraySize - 1; i >= 0; --i)
            {
                SerializedProperty commandProperty = commandListPropertyExtend.GetArrayElementAtIndex(i);
                if (commandProperty != null && commandProperty.objectReferenceValue == null)
                {
                    commandListPropertyExtend.DeleteArrayElementAtIndex(i);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        public override void DrawBlockName(Flowchart flowchart){
            base.DrawBlockName(flowchart);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Tools", "Tools for command"))){
                GenericMenu commandMenu = new GenericMenu();
                commandMenu.AddItem(new GUIContent("Say (Extend)/Tag Show Icon"), false, TagSayShowIcon);
                commandMenu.AddItem(new GUIContent("Say (Extend)/Untag Show Icon"), false, UntagSayShowIcon);
                commandMenu.ShowAsContext();
            }
            EditorGUILayout.EndHorizontal();
        }
        
        protected override void PlayCommand()
        {
            //Modify method to "protected virtual void PlayCommand()"
            base.PlayCommand();
            AdvManager.Instance.StartAdvSceneEditor();
        }

        protected void TagSayShowIcon(){
            SetSayShowIcon(true, "Tag Say Icon");
        }

        protected void UntagSayShowIcon(){
            SetSayShowIcon(false, "Untag Say Icon");
        }

        protected void SetSayShowIcon(bool val, string undoName){
            var block = target as Block;
            var flowchart = (Flowchart)block.GetFlowchart();

            if (flowchart == null ||
                flowchart.SelectedBlock == null)
            {
                return;
            }

            foreach (Command selectedCommand in flowchart.SelectedCommands)
            {
                if (selectedCommand.GetType() == typeof(SayExtend))
                {
                    Undo.RecordObject(selectedCommand, undoName);
                    (selectedCommand as SayExtend).ShowIcon = val;
                }
            }

            Repaint();
        }
    }
}
