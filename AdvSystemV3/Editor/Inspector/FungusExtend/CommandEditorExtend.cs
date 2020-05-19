using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;

namespace Fungus.EditorUtils
{
    public class CommandEditorExtend : CommandEditor
    {
        public override void OnInspectorGUI(){
            if(target.hideFlags == 0)
                target.hideFlags = HideFlags.HideInInspector;
        }

        public override void DrawCommandInspectorGUI()
        {
            Command t = target as Command;
            if (t == null)
            {
                return;
            }

            var flowchart = (Flowchart)t.GetFlowchart();
            if (flowchart == null)
            {
                return;
            }

            CommandInfoAttribute commandInfoAttr = CommandEditor.GetCommandInfo(t.GetType());
            if (commandInfoAttr == null)
            {
                return;
            }

            GUILayout.BeginVertical(GUI.skin.box);

            if (t.enabled)
            {
                if (flowchart.ColorCommands)
                {
                    GUI.backgroundColor = t.GetButtonColor();
                }
                else
                {
                    GUI.backgroundColor = Color.white;
                }
            }
            else
            {
                GUI.backgroundColor = Color.grey;
            }
            GUILayout.BeginHorizontal(GUI.skin.button);

            string commandName = commandInfoAttr.CommandName;
            GUILayout.Label(commandName, GUILayout.MinWidth(80), GUILayout.ExpandWidth(true));

            GUILayout.FlexibleSpace();

            ICommand it = t as ICommand;
            if(it != null){
                //GUILayout.Label(new GUIContent("(CSV Line:" + it.CSVLine + ")"));
                GUILayout.Label(new GUIContent("(" + it.CSVCommandKey + ")"));
            }

            GUILayout.Label(new GUIContent("(" + t.ItemId + ")"));

            GUILayout.Space(10);

            GUI.backgroundColor = Color.white;
            bool enabled = t.enabled;
            enabled = GUILayout.Toggle(enabled, new GUIContent());

            if (t.enabled != enabled)
            {
                Undo.RecordObject(t, "Set Enabled");
                t.enabled = enabled;
            }

            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;

            EditorGUILayout.Separator();

            EditorGUI.BeginChangeCheck();
            DrawCommandGUI();
            if(EditorGUI.EndChangeCheck())
            {
                SelectedCommandDataStale = true;
            }

            EditorGUILayout.Separator();

            if (t.ErrorMessage.Length > 0)
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.normal.textColor = new Color(1,0,0);
                EditorGUILayout.LabelField(new GUIContent("Error: " + t.ErrorMessage), style);
            }

            GUILayout.EndVertical();

            // Display help text
            CommandInfoAttribute infoAttr = CommandEditor.GetCommandInfo(t.GetType());
            if (infoAttr != null)
            {
                EditorGUILayout.HelpBox(infoAttr.HelpText, MessageType.Info, true);
            }
        }

        public static void StringField(SerializedProperty property, GUIContent label, GUIContent nullLabel, List<string> objectList)
        {
            if (property == null)
            {
                return;
            }

            List<GUIContent> objectNames = new List<GUIContent>();

            //string selectedObject = property.objectReferenceValue as string;
            string selectedObject = property.stringValue;

            int selectedIndex = -1; // Invalid index

            // First option in list is <None>
            objectNames.Add(nullLabel);
            if (string.IsNullOrEmpty(selectedObject))
            {
                selectedIndex = 0;
            }

            for (int i = 0; i < objectList.Count; ++i)
            {
                if (objectList[i] == null) continue;
                objectNames.Add(new GUIContent(objectList[i]));

                if (selectedObject == objectList[i])
                {
                    selectedIndex = i + 1;
                }
            }

            string result;
            
            selectedIndex = EditorGUILayout.Popup(label, selectedIndex, objectNames.ToArray());

            if (selectedIndex == -1)
            {
                // Currently selected object is not in list, but nothing else was selected so no change.
                return;
            }
            else if (selectedIndex == 0)
            {
                result = ""; // Null option
            }
            else
            {
                result = objectList[selectedIndex - 1];
            }

            property.stringValue = result;
        }
    }
}